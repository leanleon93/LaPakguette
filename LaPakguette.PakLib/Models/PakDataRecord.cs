using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OodleCompressionLib;

namespace LaPakguette.PakLib.Models
{
    public class PakDataRecord
    {
        public PakDataRecord(BinaryReader br, string[] compressionMethods, byte[] AES_KEY)
        {
            var pos = br.BaseStream.Position;
            Metadata = new PakFileMetadata(br);
            var metadataSize = br.BaseStream.Position - pos;
            Data = br.ReadBytes((int)Metadata.DataRecordSize);
            OrigData = Data;
            br.BaseStream.Position = pos;
            var metadataBuffer = br.ReadBytes((int)metadataSize);
            if (Metadata.IsEncrypted)
            {
                if (AES_KEY == null) throw new Exception("No aes key provided. File is encrypted.");
                Data = AesHandler.DecryptAES(Data, AES_KEY);
            }
            byte[] fullData = new byte[metadataBuffer.Length + Data.Length];
            int offset = 0;
            Buffer.BlockCopy(metadataBuffer, 0, fullData, offset, metadataBuffer.Length);
            offset += metadataBuffer.Length;
            Buffer.BlockCopy(Data, 0, fullData, offset, Data.Length);
            if (Metadata.CompressionMethod != 0x00)
            {
                if (compressionMethods == null || compressionMethods.Length == 0) throw new Exception("No compression methods found.");
                var compressionMethod = compressionMethods[Metadata.CompressionMethod - 1];
                
                if (Metadata.CompressionBlockCount > 1)
                {
                    var chunks = new List<byte[]>();
                    using (var ms = new MemoryStream(fullData))
                        using (var br2 = new BinaryReader(ms))

                        for (int i = 0; i < Metadata.CompressionBlocks.Length; i++)
                        {
                            CompressionBlock compressionBlock = (CompressionBlock)Metadata.CompressionBlocks[i];
                            br2.BaseStream.Position = (long)compressionBlock.CompressedDataStartOffset;
                            var chunk = br2.ReadBytes((int)compressionBlock.CompressedDataEndOffset - (int)compressionBlock.CompressedDataStartOffset);
                            if (compressionMethod == "Oodle" || compressionMethod == "oodle")
                            {
                                var uncompressedBlockSize = (int)Metadata.UncompressedCompressionBlockSize;
                                if(i + 1 == Metadata.CompressionBlocks.Length)
                                {
                                    uncompressedBlockSize = (int)Metadata.UncompressedSize - ((int)Metadata.UncompressedCompressionBlockSize * i);
                                }
                                chunk = Oodle.Decompress(chunk, uncompressedBlockSize);
                            }
                            if (compressionMethod == "Zlib" || compressionMethod == "zlib")
                            {
                                chunk = Ionic.Zlib.ZlibStream.UncompressBuffer(chunk);
                            }
                            chunks.Add(chunk);
                        }

                    Data = Combine(chunks.ToArray());
                }
                else
                {
                    if (compressionMethod == "Oodle" || compressionMethod == "oodle")
                    {
                        Data = Oodle.Decompress(Data, (int)Metadata.UncompressedSize);
                    }
                    if (compressionMethod == "Zlib" || compressionMethod == "zlib")
                    {
                        Data = Ionic.Zlib.ZlibStream.UncompressBuffer(Data);
                    }
                }
            }
        }

        private byte[] Combine(byte[][] arrays)
        {
            byte[] bytes = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;

            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, bytes, offset, array.Length);
                offset += array.Length;
            }

            return bytes;
        }

        public PakFileMetadata Metadata { get; set; }
        public byte[] Data { get; set; }
        public byte[] OrigData { get; set; }
    }
}