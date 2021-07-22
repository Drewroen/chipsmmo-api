using System.IO;
using System.Security.Cryptography;
using System;
using ChipsMMO.Models.Misc;

namespace ChipsMMO.Services
{
    public class EncryptionService
    {
        private readonly byte[] cryptoSecret;
        private readonly byte[] ivSecret;
        public EncryptionService()
        {
            var cryptoString = Environment.GetEnvironmentVariable("CHIPSMMO_PASSWORD_CRYPTO_SECRET");
            cryptoSecret = Utility.StringToByteArray(cryptoString);

            var ivString = Environment.GetEnvironmentVariable("CHIPSMMO_PASSWORD_IV_SECRET");
            ivSecret = Utility.StringToByteArray(ivString);
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
                IV = Utility.ByteArrayToString(ivSecret),
                EncryptedData = Utility.ByteArrayToString(encrypted)
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

                using (MemoryStream msDecrypt = new MemoryStream(Utility.StringToByteArray(cipherText)))
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
    }
}