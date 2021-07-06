using System;
using System.Security.Cryptography;

namespace LaPakguette.PakLib
{
    public static class AesHandler
    {
        public static byte[] DecryptAES(byte[] buffer, byte[] aesKey)
        {
            var origSize = buffer.Length;

            var padded = AddPadding(buffer, true);

            var output = new byte[padded.Length];

            Rijndael aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            ICryptoTransform decrypt = aes.CreateDecryptor(aesKey, new byte[16]);
            decrypt.TransformBlock(padded, 0, padded.Length, output, 0);

            return RemovePadding(output, origSize);
        }

        public static byte[] EncryptAES(byte[] buffer, byte[] aesKey)
        {

            var padded = AddPadding(buffer);

            var output = new byte[padded.Length];

            Rijndael aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            ICryptoTransform encrypt = aes.CreateEncryptor(aesKey, new byte[16]);
            encrypt.TransformBlock(padded, 0, padded.Length, output, 0);

            return output;
        }

        private static byte[] RemovePadding(byte[] padded, int origSize)
        {
            if (origSize == padded.Length) return padded;
            var temp = padded;
            var output = new byte[origSize];
            Array.Copy(temp, 0, output, 0, origSize);
            return output;
        }

        private static byte[] AddPadding(byte[] buffer, bool decrypt = false)
        {
            var sizePadded = CalculatePaddedSize(buffer.Length, decrypt);
            if (sizePadded == buffer.Length) return buffer;
            byte[] temp = new byte[sizePadded];
            buffer.CopyTo(temp, 0);
            var remainingLength = temp.Length - buffer.Length;
            var padOffset = buffer.Length;
            while(remainingLength != 0)
            {
                var copyLength = remainingLength > buffer.Length ? buffer.Length : remainingLength;
                Array.Copy(buffer, 0, temp, padOffset, copyLength);
                remainingLength -= copyLength;
                padOffset += copyLength;
            }
            return temp;
        }
        internal static int CalculatePaddedSize(int size, bool decrypt = false, byte[] customAesKey = null)
        {
            var AES_BLOCK_SIZE = 16;
            if (!decrypt && size % AES_BLOCK_SIZE == 0) return size;
            return size + (AES_BLOCK_SIZE - (size % AES_BLOCK_SIZE));
        }
    }
}
