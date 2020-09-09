using System;
using System.Security.Cryptography;
using System.Text;

namespace Helper
{
    public static partial class EncodeHelper
    {
        public static string MD5Hash(string str)
        {
            using (var md5 = MD5Cng.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }

        #region MD5加密方法

        /// <summary>
        /// MD5加密算法
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>MD5值</returns>
        public static string MD5(this string str)
        {
            return MD5(str, Encoding.UTF8);
        }

        /// <summary>
        /// MD5加密算法
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>MD5值</returns>
        public static string MD5(this string str, Encoding encoding)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encoding.GetBytes(str));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        #endregion

        #region SHA1加密方法

        /// <summary>
        /// SHA1加密算法
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>SHA1值</returns>
        public static string SHA1(this string str)
        {
            return SHA1(str, Encoding.UTF8);
        }

        /// <summary>
        /// SHA1加密算法
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>SHA1值</returns>
        public static string SHA1(this string str, Encoding encoding)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] result = sha1.ComputeHash(encoding.GetBytes(str));
            return BitConverter.ToString(result).Replace("-", ""); //encoding.GetString(result);
        }

        #endregion

        //解密
        private static string DES3Decrypt(string data, string key)
        {
            System.Security.Cryptography.TripleDESCryptoServiceProvider DES = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            DES.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            DES.Mode = System.Security.Cryptography.CipherMode.ECB;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            System.Security.Cryptography.ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            byte[] Buffer = Convert.FromBase64String(data);
            return System.Text.Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        //加密
        private static string DES3Encrypt(string data, string key)
        {
            System.Security.Cryptography.TripleDESCryptoServiceProvider DES = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            DES.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            DES.Mode = System.Security.Cryptography.CipherMode.ECB;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            System.Security.Cryptography.ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = System.Text.Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
    }
}
