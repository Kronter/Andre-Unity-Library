using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Andre.Utils
{
    public static class FileReadWrite
    {
        private const string cryptoKey =
      "Q3JpcHRvZ3JhZmlhcyBjb20gUmluamRhZWwgLyBBRVM=";
        private const int keySize = 256;
        private const int ivSize = 16; // block size is 128-bit

        public static void WriteEncryptedToBinaryFile<T>(string filePath, T objectToWrite)
        {
            byte[] key = Convert.FromBase64String(cryptoKey);
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                using (CryptoStream cryptoStream = CreateEncryptionStream(key, stream))
                {
                    WriteToBinaryFile(objectToWrite, cryptoStream);
                }
            }
        }

        public static T ReadEncryptedFromBinaryFile<T>(string filePath)
        {
            byte[] key = Convert.FromBase64String(cryptoKey);
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                using (CryptoStream cryptoStream = CreateDecryptionStream(key, stream))
                {
                    return ReadFromBinaryFile<T>(cryptoStream);
                }
            }
        }

        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static void WriteToBinaryFile<T>(T objectToWrite, Stream stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }

        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        public static T ReadFromBinaryFile<T>(Stream stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            T objectToWrite = (T)binaryFormatter.Deserialize(stream);
            return objectToWrite;
        }

        public static CryptoStream CreateEncryptionStream(byte[] key, Stream outputStream)
        {
            byte[] iv = new byte[ivSize];

            using (var rng = new RNGCryptoServiceProvider())
            {
                // Using a cryptographic random number generator
                rng.GetNonZeroBytes(iv);
            }

            // Write IV to the start of the stream
            outputStream.Write(iv, 0, iv.Length);

            Aes rijndael = new AesManaged();
            rijndael.Padding = PaddingMode.ANSIX923;
            rijndael.KeySize = keySize;

            CryptoStream encryptor = new CryptoStream(
                outputStream,
                rijndael.CreateEncryptor(key, iv),
                CryptoStreamMode.Write);
            return encryptor;
        }

        public static CryptoStream CreateDecryptionStream(byte[] key, Stream inputStream)
        {
            byte[] iv = new byte[ivSize];

            if (inputStream.Read(iv, 0, iv.Length) != iv.Length)
            {
                throw new ApplicationException("Failed to read IV from stream.");
            }

            Aes rijndael = new AesManaged();
            rijndael.Padding = PaddingMode.ANSIX923;
            rijndael.KeySize = keySize;

            CryptoStream decryptor = new CryptoStream(
                inputStream,
                rijndael.CreateDecryptor(key, iv),
                CryptoStreamMode.Read);
            return decryptor;
        }
    }
}