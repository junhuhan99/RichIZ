using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RichIZ.Services
{
    public class EncryptionService
    {
        private const string ENCRYPTION_KEY = "RichIZ_2025_Encryption_Key_32bit!";
        private const string SALT = "RichIZ_Salt_16b!";

        /// <summary>
        /// 문자열 암호화
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                using var aes = Aes.Create();
                aes.Key = DeriveKeyFromPassword(ENCRYPTION_KEY);
                aes.IV = Encoding.UTF8.GetBytes(SALT);

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using var msEncrypt = new MemoryStream();
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
            catch
            {
                return plainText; // 암호화 실패 시 원문 반환
            }
        }

        /// <summary>
        /// 문자열 복호화
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                using var aes = Aes.Create();
                aes.Key = DeriveKeyFromPassword(ENCRYPTION_KEY);
                aes.IV = Encoding.UTF8.GetBytes(SALT);

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);

                return srDecrypt.ReadToEnd();
            }
            catch
            {
                return cipherText; // 복호화 실패 시 원문 반환
            }
        }

        /// <summary>
        /// 파일 암호화
        /// </summary>
        public void EncryptFile(string inputFile, string outputFile)
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(ENCRYPTION_KEY);
            aes.IV = Encoding.UTF8.GetBytes(SALT);

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var fsInput = new FileStream(inputFile, FileMode.Open);
            using var fsOutput = new FileStream(outputFile, FileMode.Create);
            using var cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write);

            fsInput.CopyTo(cs);
        }

        /// <summary>
        /// 파일 복호화
        /// </summary>
        public void DecryptFile(string inputFile, string outputFile)
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(ENCRYPTION_KEY);
            aes.IV = Encoding.UTF8.GetBytes(SALT);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var fsInput = new FileStream(inputFile, FileMode.Open);
            using var cs = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read);
            using var fsOutput = new FileStream(outputFile, FileMode.Create);

            cs.CopyTo(fsOutput);
        }

        /// <summary>
        /// 비밀번호에서 키 생성
        /// </summary>
        private byte[] DeriveKeyFromPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        /// <summary>
        /// 해시 생성 (비밀번호 검증용)
        /// </summary>
        public string CreateHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 해시 검증
        /// </summary>
        public bool VerifyHash(string input, string hash)
        {
            var inputHash = CreateHash(input);
            return inputHash == hash;
        }
    }
}
