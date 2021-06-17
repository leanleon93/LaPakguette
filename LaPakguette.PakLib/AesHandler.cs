using System;
using System.Security.Cryptography;

namespace LaPakguette.PakLib
{
    public static class AesHandler
    {
        private static readonly byte[] AES_KEY = new byte[] { 0xd2, 0xe5, 0xf7, 0xf9, 0x4e, 0x62, 0x5e, 0xfe, 0x27, 0x26, 0xb5, 0x36, 0x0c, 0x10, 0x39, 0xce, 0x7c, 0xb9, 0xab, 0xb7, 0x60, 0xa9, 0x4f, 0x37, 0xbb, 0x15, 0xa6, 0xdc, 0x08, 0x74, 0x16, 0x56 };
        public static byte[] DecryptAES(byte[] buffer, byte[] customAesKey = null)
        {
            var aesKey = customAesKey ?? AES_KEY;
            var origSize = buffer.Length;

            var padded = AddPadding(buffer, true);

            var output = new byte[padded.Length];

            Rijndael aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            ICryptoTransform decrypt = aes.CreateDecryptor(aesKey, new byte[16]);
            decrypt.TransformBlock(padded, 0, padded.Length, output, 0);

            return RemovePadding(output, origSize);
        }

        public static byte[] EncryptAES(byte[] buffer, byte[] customAesKey = null)
        {
            var aesKey = customAesKey ?? AES_KEY;

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
