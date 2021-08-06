using System.IO;
using System.Security.Cryptography;
using System;
using ChipsMMO.Models.Misc;
using System.Text;

namespace ChipsMMO.Services
{
    public class EncryptionService
    {
        private readonly byte[] cryptoSecret;
        private readonly byte[] ivSecret;
        public EncryptionService()
        {
            var cryptoString = Environment.GetEnvironmentVariable("CHIPSMMO_PASSWORD_CRYPTO_SECRET");
            cryptoSecret = StringToByteArray(cryptoString);

            var ivString = Environment.GetEnvironmentVariable("CHIPSMMO_PASSWORD_IV_SECRET");
            ivSecret = StringToByteArray(ivString);
        }
        public EncryptedPassword Encrypt(string plainText)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = cryptoSecret;
                aesAlg.IV = ivSecret;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return new EncryptedPassword()
            {
                IV = ByteArrayToString(ivSecret),
                EncryptedData = ByteArrayToString(encrypted)
            };
        }
        public string Decrypt(string cipherText)
        {
            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = cryptoSecret;
                aesAlg.IV = ivSecret;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(StringToByteArray(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        private static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}