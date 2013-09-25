using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Macro.Licenses.LicenseGenerator
{
    public class LicenseProvider
    {
        /// <summary>
        /// 解密
        /// </summary>
        public static string Decrypt(string @string)
        {
            if (String.IsNullOrEmpty(@string))
                return @string;

            string result;
            try
            {
                byte[] bytes = Convert.FromBase64String(@string);
                using (MemoryStream dataStream = new MemoryStream(bytes))
                {
                    RC2CryptoServiceProvider cryptoService = new RC2CryptoServiceProvider();
                    cryptoService.Key = Encoding.UTF8.GetBytes(Licenses.Default.Key);
                    cryptoService.IV = Encoding.UTF8.GetBytes(Licenses.Default.IV);
                    cryptoService.UseSalt = false;
                    using (CryptoStream cryptoStream = new CryptoStream(dataStream, cryptoService.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                            reader.Close();
                        }
                        cryptoStream.Close();
                    }
                    dataStream.Close();
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// 加密
        /// </summary>
        public static string Encrypt(string @string)
        {
            if (String.IsNullOrEmpty(@string))
                return @string;

            string result;
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(@string);
                using (MemoryStream dataStream = new MemoryStream())
                {
                    RC2CryptoServiceProvider cryptoService = new RC2CryptoServiceProvider();
                    cryptoService.Key = Encoding.UTF8.GetBytes(Licenses.Default.Key);
                    cryptoService.IV = Encoding.UTF8.GetBytes(Licenses.Default.IV);
                    cryptoService.UseSalt = false;
                    using (CryptoStream cryptoStream = new CryptoStream(dataStream, cryptoService.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] inputBytes = dataStream.ToArray();
                        result = Convert.ToBase64String(inputBytes);
                        cryptoStream.Close();
                    }
                    dataStream.Close();
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result;
        }
    }
}
