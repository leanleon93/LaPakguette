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
