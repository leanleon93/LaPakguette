using System;
using System.Security.Cryptography;

namespace LaPakguette.Lib
{
    internal static class AesHandler
    {
        internal static byte[] DecryptAES(byte[] buffer, byte[] aesKey)
        {
            // Padding buffer with zero
            var size = buffer.Length;
            var AES_BLOCK_SIZE = aesKey.Length;
            var sizePadded = AES_BLOCK_SIZE * (size / AES_BLOCK_SIZE + 1);
            var output = new byte[sizePadded];
            var tmp = new byte[sizePadded];
            buffer.CopyTo(tmp, 0);

            var aes = Rijndael.Create();
            aes.Mode = CipherMode.ECB;
            var decrypt = aes.CreateDecryptor(aesKey, new byte[16]);
            decrypt.TransformBlock(tmp, 0, sizePadded, output, 0);
            tmp = output;
            output = new byte[size];
            Array.Copy(tmp, 0, output, 0, size);

            return output;
        }
    }
}