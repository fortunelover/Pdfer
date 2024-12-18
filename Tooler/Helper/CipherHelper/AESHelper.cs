using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using Tooler.Common.Utils.Extensions;

namespace Cipher.Helper
{
    public static class AESHelper
    {
        public static string AesEncrypt(string str, string key, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7, string EncordingModeStr = "Base64", string IV = null)
        {
            if (string.IsNullOrEmpty(str))
                throw new Exception("待加密数据为空");
            byte[] t = null;
            switch (EncordingModeStr)
            {
                case "UTF-8":
                    t = Encoding.UTF8.GetBytes(key);
                    break;
                case "Base64":
                    t = Convert.FromBase64String(key);
                    break;
            }
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = t,
                Padding = paddingMode,
                Mode = cipherMode,

            };
            if (!IV.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                byte[] v = Convert.FromBase64String(IV);
                ICryptoTransform cTransform = rm.CreateEncryptor(t, v);
                rm.IV = v;
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray);
            }
            else
            {
                ICryptoTransform cTransform = rm.CreateEncryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray);
            }

        }




        public static string AesDecrypt(string str, string key, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7, string EncordingModeStr = "Base64", string IV = null)
        {
            if (string.IsNullOrEmpty(str))
                throw new Exception("待加密数据为空");
            byte[] t = null;
            switch (EncordingModeStr)
            {
                case "UTF-8":
                    t = Encoding.UTF8.GetBytes(key);
                    break;
                case "Base64":
                    t = Convert.FromBase64String(key);
                    break;
            }
            Byte[] toEncryptArray = Convert.FromBase64String(str);
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = t,
                Mode = cipherMode,
                Padding = paddingMode,
            };
            if (!IV.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                byte[] a = Convert.FromBase64String(IV);
                rm.IV = a;
                ICryptoTransform cTransform = rm.CreateDecryptor(t, a);
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
            else
            {
                ICryptoTransform cTransform = rm.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
        }




        public static void AesCreateKey(out string key, out string IV, string EncordingMode = "Base64")
        {
            switch (EncordingMode)
            {
                case "UTF-8":
                    {
                        Random random = new Random();
                        key = GetRandomString(16);
                        IV = GetRandomString(16);
                        break;
                    }
                default:
                    {
                        var aes = Aes.Create();
                        key = Convert.ToBase64String(aes.Key);
                        IV = Convert.ToBase64String(aes.IV);
                        break;
                    }

            }
        }

        /*public static string EncryptStringToBytes_Aes(string plainText, string str)
        {
            var Key=Encoding.UTF8.GetBytes(str);
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.Mode = CipherMode.OFB;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            string base64Encrypted = Convert.ToBase64String(encrypted);
            return base64Encrypted;
        }*/



        private static RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();

        public static string GetRandomString(int stringlength)
        {
            return GetRandomString(null, stringlength);
        }     //获得长度为stringLength的随机字符串，以key为字母表
        public static string GetRandomString(string key, int stringLength)
        {
            if (key == null || key.Length < 8)
            {
                key = "abcdefghijklmnopqrstuvwxyz1234567890";
            }

            int length = key.Length;
            StringBuilder randomString = new StringBuilder(length);
            for (int i = 0; i < stringLength; ++i)
            {
                randomString.Append(key[SetRandomSeeds(length)]);
            }

            return randomString.ToString();
        }

        private static int SetRandomSeeds(int length)
        {
            decimal maxValue = (decimal)long.MaxValue;
            byte[] array = new byte[8];
            _random.GetBytes(array);

            return (int)(Math.Abs(BitConverter.ToInt64(array, 0)) / maxValue * length);
        }
    }
}
