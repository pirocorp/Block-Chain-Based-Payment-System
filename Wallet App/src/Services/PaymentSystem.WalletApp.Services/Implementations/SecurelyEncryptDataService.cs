namespace PaymentSystem.WalletApp.Services.Implementations
{
    using System.IO;
    using System.Security.Cryptography;

    using PaymentSystem.Common.Utilities;

    public class SecurelyEncryptDataService : ISecurelyEncryptDataService
    {
        /// <summary>
        /// Securely encrypt object.
        /// </summary>
        /// <param name="data">Object.</param>
        /// <param name="keyBytes">Encryption key.</param>
        /// <param name="saltBytes">Salt.</param>
        /// <returns></returns>
        public byte[] EncryptData(object data, byte[] keyBytes, byte[] saltBytes)
        {
            var clearBytes = data.ToByteArray();
            byte[] encryptedBytes = null;

            // create an AES object
            using var aes = this.GetAES(keyBytes, saltBytes);

            using var ms = new MemoryStream();

            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
            }

            encryptedBytes = ms.ToArray();

            return encryptedBytes;
        }

        /// <summary>
        /// Securely encrypt object and store it in HEX.
        /// </summary>
        /// <param name="data">Object.</param>
        /// <param name="keyBytes">Encryption key.</param>
        /// <param name="saltBytes">Salt.</param>
        /// <returns></returns>
        public string EncryptDataHex(object data, byte[] keyBytes, byte[] saltBytes)
            => this.EncryptData(data, keyBytes, saltBytes).BytesToHex();

        /// <summary>
        /// Securely decrypt bytes.
        /// </summary>
        /// <param name="cryptBytes">Encrypted bytes.</param>
        /// <param name="keyBytes">Decryption key.</param>
        /// <param name="saltBytes">Salt.</param>
        /// <returns></returns>
        public byte[] DecryptBytes(byte[] cryptBytes, byte[] keyBytes, byte[] saltBytes)
        {
            byte[] clearBytes = null;

            using var aes = this.GetAES(keyBytes, saltBytes);

            using var ms = new MemoryStream();

            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cryptBytes, 0, cryptBytes.Length);
                cs.Close();
            }

            clearBytes = ms.ToArray();

            return clearBytes;
        }

        /// <summary>
        /// Securely decrypt object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="cryptBytes">Encrypted object.</param>
        /// <param name="keyBytes">Decryption key.</param>
        /// <param name="saltBytes">Salt.</param>
        /// <returns></returns>
        public T Decrypt<T>(byte[] cryptBytes, byte[] keyBytes, byte[] saltBytes)
            => this.DecryptBytes(cryptBytes, keyBytes, saltBytes).ToObject<T>();

        /// <summary>
        /// Creates AES object.
        /// </summary>
        /// <param name="keyBytes">Key for encrypting and decrypting data.</param>
        /// <param name="saltBytes">Salt.</param>
        /// <returns></returns>
        private AesManaged GetAES(byte[] keyBytes, byte[] saltBytes)
        {
            // create a key from the password and salt, use 32K iterations
            var key = new Rfc2898DeriveBytes(keyBytes, saltBytes, 32768);

            var aes = new AesManaged();

            // set the key size to 256
            aes.KeySize = 256;
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            return aes;
        }
    }
}
