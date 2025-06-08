using System.Security.Cryptography;

namespace Dermainsight.Services
{
    public class EncryptionService
    {

        private static string CreateKey()
        {
            return "V3A3eA4RzOa3aIu9H7pX3cF0UkRsbq0IjlxeuBh53iI=";
        }

        public static string Encrypt(string input)
        {
            string key = CreateKey();

            using (Aes asdAlg = Aes.Create())
            {
                asdAlg.Key = Convert.FromBase64String(key);
                asdAlg.Mode = CipherMode.CBC;
                asdAlg.Padding = PaddingMode.PKCS7;
                asdAlg.GenerateIV();

                using (MemoryStream msEncrypt = new MemoryStream())
                {

                    msEncrypt.Write(asdAlg.IV, 0, asdAlg.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, asdAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());

                }
            }
        }
        public static string Decrypt(string encrptedText)
        {
            string key = CreateKey();
            byte[] cipherText = Convert.FromBase64String(encrptedText);

            using(Aes asdAlg = Aes.Create())
            {
                asdAlg.Key = Convert.FromBase64String(key);
                asdAlg.Mode = CipherMode.CBC;
                asdAlg.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[16];
                Array.Copy(cipherText, iv, iv.Length);
                asdAlg.IV = iv;
                using (MemoryStream msDecrypt = new MemoryStream(cipherText, 16, cipherText.Length - 16))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, asdAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using(StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
