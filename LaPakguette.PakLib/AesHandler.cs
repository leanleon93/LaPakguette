using System;
using System.Security.Cryptography;

namespace LaPakguette.PakLib
{
    internal static class AesHandler
    {
        internal static byte[] DecryptAES(byte[] buffer, byte[] aesKey)
        {
            var origSize = buffer.Length;

            var padded = AddPadding(buffer, true);

            var output = new byte[padded.Length];

            var aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            var decrypt = aes.CreateDecryptor(aesKey, new byte[16]);
            decrypt.TransformBlock(padded, 0, padded.Length, output, 0);

            return RemovePadding(output, origSize);
        }

        internal static (byte[], byte[]) EncryptAES(byte[] buffer, byte[] aesKey)
        {
            var padded = AddPadding(buffer);

            var output = new byte[padded.Length];

            var aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            var encrypt = aes.CreateEncryptor(aesKey, new byte[16]);
            encrypt.TransformBlock(padded, 0, padded.Length, output, 0);

            return (output, SHA1Hash(padded));
        }

        internal static byte[] SHA1Hash(byte[] data)
        {
            using var sha1 = new SHA1CryptoServiceProvider();
            return sha1.ComputeHash(data);
        }

        private static byte[] RemovePadding(byte[] padded, int origSize)
        {
            if (origSize == padded.Length) return padded;
            var temp = padded;
            var output = new byte[origSize];
            Array.Copy(temp, 0, output, 0, origSize);
            return output;
        }

        internal static byte[] AddPadding(byte[] buffer, bool decrypt = false, byte[] padData = null)
        {
            if (padData == null) padData = buffer;
            var sizePadded = CalculatePaddedSize(buffer.Length, decrypt);
            if (sizePadded == buffer.Length) return buffer;
            var temp = new byte[sizePadded];
            buffer.CopyTo(temp, 0);
            var remainingLength = temp.Length - buffer.Length;
            var padOffset = buffer.Length;
            while (remainingLength != 0)
            {
                var copyLength = remainingLength > padData.Length ? padData.Length : remainingLength;
                Array.Copy(padData, 0, temp, padOffset, copyLength);
                remainingLength -= copyLength;
                padOffset += copyLength;
            }

            return temp;
        }

        internal static int CalculatePaddedSize(int size, bool decrypt = false, byte[] customAesKey = null)
        {
            var AES_BLOCK_SIZE = 16;
            if (!decrypt && size % AES_BLOCK_SIZE == 0) return size;
            return size + (AES_BLOCK_SIZE - size % AES_BLOCK_SIZE);
        }
    }
}