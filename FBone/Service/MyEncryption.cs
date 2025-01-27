using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FBone.Service
{
    public static class MyEncryption
    {
        private static readonly string security_key = "D4-AB-32-A3-3A-1C-49-C6-FA-D2-8B-22-B5-FA-B0-36-52-0E-71-B3-73-12-C8-62-4D-75-5C-3B-9A-96-3D-AC";
        private static readonly string security_vector = "BD-52-21-C4-2F-8C-3A-F9-C0-4B-BE-56-C9-F6-B0-32";
        private static byte[] MyEncryptionHelper(this string val)
        {
            List<string> str_list = val.Split('-').ToList();
            List<byte> bytes_list = new List<byte>();
            foreach (string item in str_list)
            {
                bytes_list.Add(byte.Parse(item, NumberStyles.HexNumber));
            }
            return bytes_list.ToArray();
        }
        private static string MyEncryptionHelper(this byte[] val)
        {
            return BitConverter.ToString(val);
        }
        public static void KeyGeneration()
        {
            //Генерация ключей
            Aes crypt = Aes.Create();
            crypt.GenerateKey();
            string key = BitConverter.ToString(crypt.Key);
            string vector = BitConverter.ToString(crypt.IV);
        }

        public static string EncryptString(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            byte[] encrypted;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Aes rijAlg = Aes.Create())
            {
                rijAlg.Key = security_key.MyEncryptionHelper();
                rijAlg.IV = security_vector.MyEncryptionHelper();

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted string from the memory stream.
            return encrypted.MyEncryptionHelper();
        }

        public static string DecryptString(string encrText)
        {
            // Check arguments.
            if (encrText == null || encrText.Length <= 0)
                throw new ArgumentNullException("encrText");

            byte[] cipherText = encrText.MyEncryptionHelper();
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Aes rijAlg = Aes.Create())
            {
                rijAlg.Key = security_key.MyEncryptionHelper();
                rijAlg.IV = security_vector.MyEncryptionHelper();

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
