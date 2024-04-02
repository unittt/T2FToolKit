using System;
using System.Security.Cryptography;
using System.Text;


namespace T2FToolKit
{
    internal static class AesUtil
    {
        private const CipherMode Mode = CipherMode.ECB;
        private const PaddingMode Padding = PaddingMode.PKCS7;
        
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string EncryptString(string key, string data)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                aes.Mode = Mode;
                aes.Padding = Padding;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((System.IO.Stream)memoryStream, encryptor,
                               CryptoStreamMode.Write))
                    {
                        using (System.IO.StreamWriter streamWriter =
                               new System.IO.StreamWriter((System.IO.Stream)cryptoStream))
                        {
                            streamWriter.Write(data);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DecryptString(string key, string data)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(data);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    aes.Mode = Mode;
                    aes.Padding = Padding;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((System.IO.Stream)memoryStream, decryptor,
                                   CryptoStreamMode.Read))
                        {
                            using (System.IO.StreamReader streamReader =
                                   new System.IO.StreamReader((System.IO.Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
               return string.Empty;
            }
        }
    }
}