using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Goober.Base.Extensions
{
    public static class CryptoExtensions
    {
        public const string EncryptedPasswordConfigKey = "Goober.BasicAuth.EncryptedPassword";

        public static string DefaultKey = "b14ca5898a4e4142aace2ea2143a2410";

        public static string EncryptString(this string plainText, string key = null)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key ?? DefaultKey);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(this string cipherText, string key = null)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key ?? DefaultKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string GetDecryptedString(this IConfiguration configuration, string configKey, string cryptoKey = null)
        {
            var value = configuration.GetValue<string>(configKey);
            if (string.IsNullOrEmpty(value) == true)
            {
                return value;
            }

            var res = DecryptString(value, cryptoKey);

            return res;
        }

        public static string GetDecryptedConnectionString(this IConfiguration configuration, string connectionName, string cryptoKey = null)
        {
            var connectionString = configuration.GetConnectionString(connectionName);
            if (string.IsNullOrEmpty(connectionString) == true)
            {
                return connectionString;
            }

            var res = connectionString.DecryptString(cryptoKey);

            return res;
        }
    }
}
