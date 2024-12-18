using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using System.Security.Cryptography;
using System.Text;

namespace Cipher.Helper
{
    internal static class HashHelper
    {
        public static string GetMD5(string source) 
        {
            byte[] result = Encoding.Default.GetBytes(source.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(result);
            string output = string.Empty;
            for (int i = 0; i < hash.Length; i++)
            {
                output = output + hash[i].ToString("x2").ToUpper();
            }
            return output;
        }

        public static string GetSHA1(string source)
        {
            byte[] result = Encoding.Default.GetBytes(source.Trim());
            SHA1 md5 = new SHA1CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(result);
            string output = string.Empty;
            for (int i = 0; i < hash.Length; i++)
            {
                output = output + hash[i].ToString("x2").ToUpper();
            }
            return output;
        }

        public static string GetSHA256(string source)
        {
            byte[] result = Encoding.Default.GetBytes(source.Trim());
            var md5 = new SHA256CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(result);
            string output = string.Empty;
            for (int i = 0; i < hash.Length; i++)
            {
                output = output + hash[i].ToString("x2").ToUpper();
            }
            return output;
        }

        /// <summary>
        /// 国产摘要算法
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetSM3(string source)
        {
            //加密
            byte[] msg = Encoding.Default.GetBytes(source);
            byte[] md = new byte[32];
            SM3Digest sm3 = new SM3Digest();
            sm3.BlockUpdate(msg, 0, msg.Length);
            sm3.DoFinal(md, 0);
            string PasswdDigest = new UTF8Encoding().GetString(Hex.Encode(md));
            return PasswdDigest.ToUpper();
        }


}
}
