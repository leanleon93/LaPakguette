using System;
using System.Security.Cryptography;

namespace LaPakguette.Lib
{
    internal static class AesHandler
    {
        private static readonly byte[] AES_KEY = new byte[] { 0x38, 0x32, 0x46, 0xF1, 0xAC, 0x08, 0x11, 0xFC, 0x28, 0xCE, 0x17, 0x1F, 0x3E, 0x63, 0xC9, 0xC9, 0xAB, 0xCE, 0xE1, 0x37, 0xFF, 0x1C, 0xF1, 0xEF, 0x4A, 0x6B, 0x8A, 0x47, 0xBA, 0xB5, 0xAA, 0x7C };
        internal static byte[] DecryptAES(byte[] buffer, byte[] customAesKey = null)
        {
            // AES requires buffer to consist of blocks with 32 bytes (each)
            // expand last block by padding zeros if required...
            var aesKey = customAesKey ?? AES_KEY;
            var size = buffer.Length;
            int AES_BLOCK_SIZE = aesKey.Length;
            int sizePadded = AES_BLOCK_SIZE * ((size / AES_BLOCK_SIZE) + 1);
            byte[] output = new byte[sizePadded];
            byte[] tmp = new byte[sizePadded];
            buffer.CopyTo(tmp, 0);

            Rijndael aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            ICryptoTransform decrypt = aes.CreateDecryptor(aesKey, new byte[16]);
            decrypt.TransformBlock(tmp, 0, sizePadded, output, 0);
            tmp = output;
            output = new byte[size];
            Array.Copy(tmp, 0, output, 0, size);

            return output;
        }
    }
}
