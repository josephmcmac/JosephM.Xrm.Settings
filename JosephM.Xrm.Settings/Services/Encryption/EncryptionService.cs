using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using JosephM.Xrm.Settings.Core;

namespace JosephM.Xrm.Settings.Services.Encryption
{
    public class EncryptionService
    {
        internal static string EncryptionSalt
        {
            get { return "JosephMEncryptionSalt0192837465"; }
        }

        public EncryptResponse Encrypt(string password, Guid id)
        {
            // Key = Hashed (setting Guid + Salt)
            using (var sha256 = SHA256.Create())
            {
                var enc = new UTF8Encoding();
                var keyBytes = sha256.ComputeHash(enc.GetBytes(id + EncryptionSalt));

                // Encrypt password
                using (var aes = new AesManaged())
                {
                    aes.GenerateIV();
                    aes.Key = keyBytes;
                    var encryptor = aes.CreateEncryptor();

                    using (var msEncrypt = new MemoryStream())
                    {
                        using (
                            var csEncrypt = new CryptoStream(msEncrypt, encryptor,
                                CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(password);
                            }
                            var encryptedPassword = Convert.ToBase64String(msEncrypt.ToArray());
                            if (encryptedPassword.IsNullOrWhiteSpace())
                            {
                                throw new NullReferenceException("Password encryption failed!");
                            }
                            return new EncryptResponse(encryptedPassword, Convert.ToBase64String(aes.IV));
                        }
                    }
                }
            }
        }

        public string Decrypt(string encrypted, string encryptedVi, Guid id)
        {

            var enc = new UTF8Encoding();
            var sha256 = SHA256.Create();
            var aesKeyBytes = sha256.ComputeHash(enc.GetBytes(id + EncryptionSalt));
            sha256.Dispose();

            return DecryptBase64_Aes(encrypted, aesKeyBytes, encryptedVi);
        }

        public string DecryptBase64_Aes(string base64ToDecrypt, byte[] aesKeyBytes, string aesIvBase64)
        {
            // Check arguments.
            if (base64ToDecrypt == null || base64ToDecrypt.Length <= 0)
                throw new ArgumentNullException("base64ToDecrypt");
            if (aesKeyBytes == null || aesKeyBytes.Length <= 0)
                throw new ArgumentNullException("aesKeyBytes");
            if (aesIvBase64 == null || aesIvBase64.Length <= 0)
                throw new ArgumentNullException("aesIvBase64");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            var encryptedBytes = Convert.FromBase64String(base64ToDecrypt);
            var ivBytes = Convert.FromBase64String(aesIvBase64);

            // Create an AesManaged object
            // with the specified key and IV.
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = aesKeyBytes;
                aesAlg.IV = ivBytes;

                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor();

                // Create the streams used for decryption.
                var msDecrypt = new MemoryStream(encryptedBytes);
                var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                var srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
                msDecrypt.Flush();
                csDecrypt.Flush();

                srDecrypt.Close();
                csDecrypt.Close();
                msDecrypt.Close();
                msDecrypt.Dispose();
            }
            return plaintext;
        }
    }
}
