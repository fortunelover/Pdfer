using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Parameters;
using System.Xml;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;

namespace Cipher.Helper
{
    public static class RSAHelper
    {
        #region 加密解密
        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="PriStr"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecrypPri(string strPri, string data)
        {
            var RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(strPri);
            var bytes = Convert.FromBase64String(data);
            byte[] decryptedData;
            using (var plaiStream = new MemoryStream(bytes))
            {
                using (var decrypStream = new MemoryStream())
                {
                    plaiStream.Position = 0;
                    var offSet = 0;
                    var inputLen = bytes.Length;
                    int sizeByte = RSA.KeySize / 8;
                    for (var i = 0; inputLen - offSet > 0; offSet = i * sizeByte)
                    {
                        if (inputLen - offSet > sizeByte)
                        {
                            var buffer = new Byte[sizeByte];
                            plaiStream.Read(buffer, 0, sizeByte);
                            var decrypData = RSA.Decrypt(buffer, false);
                            decrypStream.Write(decrypData, 0, decrypData.Length);
                        }
                        else
                        {
                            var buffer = new Byte[inputLen - offSet];
                            plaiStream.Read(buffer, 0, inputLen - offSet);
                            var decrypData = RSA.Decrypt(buffer, false);
                            decrypStream.Write(decrypData, 0, decrypData.Length);
                        }
                        ++i;
                    }
                    decrypStream.Position = 0;
                    decryptedData = decrypStream.ToArray();
                    var dataStr = System.Text.Encoding.UTF8.GetString(decryptedData);
                    return dataStr;
                    //return Convert.ToBase64String(decryptedData);
                }
            }
        }

        /// <summary>
        /// 公钥解密（分段）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecrypPubBatch(string strPub, string data)
        {
            RSACryptoServiceProvider publicRsa = new RSACryptoServiceProvider();
            publicRsa.FromXmlString(strPub);

            AsymmetricKeyParameter keyPair = DotNetUtilities.GetRsaPublicKey(publicRsa);
            //转换密钥  
            // AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetRsaKeyPair(publicRsa);
            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");// 参数与Java中加密解密的参数一致       
            c.Init(false, keyPair); //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥 
            byte[] DataToEncrypt = Convert.FromBase64String(data);

            byte[] outBytes;
            using (var plaiStream = new MemoryStream(DataToEncrypt))
            {
                using (var decrypStream = new MemoryStream())
                {
                    plaiStream.Position = 0;
                    var offSet = 0;
                    var inputLen = DataToEncrypt.Length;
                    int sizeByte = publicRsa.KeySize / 8;
                    for (var i = 0; inputLen - offSet > 0; offSet = i * sizeByte)
                    {
                        if (inputLen - offSet > sizeByte)
                        {
                            var buffer = new Byte[sizeByte];
                            plaiStream.Read(buffer, 0, sizeByte);
                            var decrypData = c.DoFinal(buffer);
                            decrypStream.Write(decrypData, 0, decrypData.Length);
                        }
                        else
                        {
                            var buffer = new Byte[inputLen - offSet];
                            plaiStream.Read(buffer, 0, inputLen - offSet);
                            var decrypData = c.DoFinal(buffer);
                            decrypStream.Write(decrypData, 0, decrypData.Length);
                        }
                        ++i;
                    }
                    decrypStream.Position = 0;
                    outBytes = decrypStream.ToArray();
                    var dataStr = System.Text.Encoding.UTF8.GetString(outBytes);
                    return dataStr;
                    //return Convert.ToBase64String(decryptedData);
                }
            }
        }

        /// <summary>
        /// 公钥解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecrypPub(string strPub, string data)
        {
            RSACryptoServiceProvider publicRsa = new RSACryptoServiceProvider();
            publicRsa.FromXmlString(strPub);

            AsymmetricKeyParameter keyPair = DotNetUtilities.GetRsaPublicKey(publicRsa);
            //转换密钥  
            // AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetRsaKeyPair(publicRsa);
            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");// 参数与Java中加密解密的参数一致       
            c.Init(false, keyPair); //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥 
            byte[] DataToEncrypt = Convert.FromBase64String(data);
            byte[] outBytes = c.DoFinal(DataToEncrypt);//解密  
            var decryptdata = System.Text.Encoding.UTF8.GetString(outBytes);
            return decryptdata;
        }

        /// <summary>
        /// 公钥加密（分段）
        /// </summary>
        /// <param name="PubStr"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string EncrytPub(string strPub, string data)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(strPub);
            var dataByte = Encoding.UTF8.GetBytes(data);
            var dataByteBase64 = Convert.ToBase64String(dataByte);
            var bytes = Convert.FromBase64String(dataByteBase64);
            if (bytes == null || bytes.Length <= 0)
            {
                throw new NotSupportedException();
            }
            if (RSA == null)
            {
                throw new ArgumentNullException();
            }
            byte[] encryContent = null;
            #region 分段加密
            int bufferSize = (RSA.KeySize / 8) - 11;
            byte[] buffer = new byte[bufferSize];
            //分段加密
            using (MemoryStream input = new MemoryStream(bytes))
            using (MemoryStream ouput = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, bufferSize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] encrypt = RSA.Encrypt(temp, false);
                    ouput.Write(encrypt, 0, encrypt.Length);
                }
                encryContent = ouput.ToArray();
            }
            #endregion
            return Convert.ToBase64String(encryContent);
        }

        /// <summary>
        /// 私钥加密（分段）
        /// </summary>
        /// <param name="strPri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string EncrytPriBatch(string strPri, string data)
        {
            //加载私钥  
            RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider();
            privateRsa.FromXmlString(strPri);
            //转换密钥  
            AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(privateRsa);
            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");// 参数与Java中加密解密的参数一致       
            c.Init(true, keyPair.Private); //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥 
            byte[] DataToEncrypt = Encoding.UTF8.GetBytes(data);

            //分段加密
            byte[] outBytes = null;//最终加密的byte数据
            int bufferSize = (privateRsa.KeySize / 8) - 11;
            byte[] buffer = new byte[bufferSize];
            using (MemoryStream input = new MemoryStream(DataToEncrypt))
            using (MemoryStream ouput = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, bufferSize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] encrypt = c.DoFinal(temp);
                    ouput.Write(encrypt, 0, encrypt.Length);
                }
                outBytes = ouput.ToArray();
            }
            var outBytesBase64 = Convert.ToBase64String((byte[])outBytes);
            return outBytesBase64;
        }

        /// <summary>
        /// 私钥加密
        /// </summary>
        /// <param name="strPri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string EncrytPri(string strPri, string data)
        {
            //加载私钥  
            RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider();
            privateRsa.FromXmlString(strPri);
            //转换密钥  
            AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(privateRsa);
            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");// 参数与Java中加密解密的参数一致       
            c.Init(true, keyPair.Private); //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥 
            byte[] DataToEncrypt = Encoding.UTF8.GetBytes(data);
            byte[] outBytes = c.DoFinal(DataToEncrypt);//加密  
            var outBytesBase64 = Convert.ToBase64String((byte[])outBytes);
            return outBytesBase64;
        }

        #endregion

        #region 密钥格式转换

        //net->Java 公钥
        public static string RSAPublicKeyDotNet2Java(string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(publicKey);
            RSAParameters p = rsa.ExportParameters(false);
            var m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            var exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            RsaKeyParameters pub = new RsaKeyParameters(false, m, exp);
            Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo publicKeyInfo = Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPublicBytes);
        }

        //net->Java 私钥
        public static string RSAPrivateKeyDotNet2Java(string privateKey)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(privateKey);
            BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            BigInteger d = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
            BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
            BigInteger dp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
            BigInteger dq = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
            BigInteger qinv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));

            RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(m, exp, d, p, q, dp, dq, qinv);

            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
            return Convert.ToBase64String(serializedPrivateBytes);
        }

        //Java->net 公钥
        public static string RSAPublicKeyJava2DotNet(string pubKeyStr)
        {
            byte[] pubKeyByte = Convert.FromBase64String(pubKeyStr);
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(pubKeyByte);
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        //Java->net 私钥
        public static string RSAPrivateKeyJava2DotNet(string priKeyStr)
        {
            byte[] priKeyByte = Convert.FromBase64String(priKeyStr);
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(priKeyByte);
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        public static string ToDotNetPubKey(this string pubKeyStr)
        {
            byte[] pubKeyByte = Convert.FromBase64String(pubKeyStr);
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(pubKeyByte);
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        #endregion

        #region 密钥生成
        public static void CreateKey(out string publicKey, out string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }
        }

        public static string CreatePubKeyByPriKey(string privateKey)
        {
            var RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKey);
            string publicKey = RSA.ToXmlString(false);
            return publicKey;
        }

        #endregion

        #region 签名
        public static string SignData(string data, string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                var bytes = Encoding.UTF8.GetBytes(data);
                var signData = rsa.SignData(bytes, "SHA1");
                return Convert.ToBase64String(signData);
            }
        }


        public static bool VerifyData(string data, string publicKey, string signData)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                var bytes = Encoding.UTF8.GetBytes(data);
                return rsa.VerifyData(bytes, "SHA1", Convert.FromBase64String(signData));
            }
        }

        #endregion

    }
}
