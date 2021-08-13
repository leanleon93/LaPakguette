using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OodleCompressionLib;

namespace LaPakguette.PakLib.Models
{
    public class PakDataRecord
    {
        private const int maxChunkSize = 65536;

        internal PakDataRecord(byte[] data)
        {
            Data = data;
            Metadata = new PakFileMetadata();
        }

        public PakDataRecord(BinaryReader br, string[] compressionMethods, byte[] AES_KEY)
        {
            var pos = br.BaseStream.Position;
            Metadata = new PakFileMetadata(br);
            var metadataSize = br.BaseStream.Position - pos;
            MetadataSize = metadataSize;
            Data = br.ReadBytes((int)Metadata.PaddedDataSize);
            br.BaseStream.Position = pos;
            //File.WriteAllBytes(@"C:\Users\leanw\Documents\pak.data", Data);
            DecryptData(AES_KEY);
            //File.WriteAllBytes(@"C:\Users\leanw\Documents\pak.data.dec", Data);
            DecompressData((int)metadataSize, compressionMethods);
            //File.WriteAllBytes(@"C:\Users\leanw\Documents\pak.data.deco", Data);
        }

        private void DecryptData(byte[] AES_KEY)
        {
            if (Metadata.IsEncrypted)
            {
                if (AES_KEY == null) throw new Exception("No aes key provided. File is encrypted.");
                var decrypted = AesHandler.DecryptAES(Data, AES_KEY);
                Data = new byte[Metadata.DataRecordSize];
                Array.Copy(decrypted, 0, Data, 0, Data.Length);
            }
        }

        private void EncryptData(bool encrypt, byte[] AES_KEY)
        {
            if (encrypt)
            {
                if (AES_KEY == null) throw new Exception("No aes key provided. File can not be encrypted.");
                (Data, _) = AesHandler.EncryptAES(Data, AES_KEY);
            }
        }
        private void CompressData(bool compress, string[] compressionMethods, bool encrypt)
        {
            if(compress)
            {
                if (compressionMethods == null || compressionMethods.Length == 0) throw new Exception("No compression methods found.");
                var compressionMethod = compressionMethods[Metadata.CompressionMethod - 1];
                if (Data.Length > maxChunkSize)
                {
                    int chunkCount = (Data.Length + maxChunkSize - 1) / maxChunkSize;
                    var chunks = new byte[chunkCount][];
                    Metadata.CompressionBlockCount = (uint)chunkCount;
                    Metadata.CompressionBlocks = new CompressionBlock[chunkCount];
                    ulong offset = 57 + (Metadata.CompressionBlockCount * 16);
                    for (var i = 0; i < chunkCount; i++)
                    {
                        chunks[i] = new byte[Math.Min(maxChunkSize, Data.Length - i * maxChunkSize)];
                        
                        Array.Copy(Data, i * maxChunkSize, chunks[i], 0, chunks[i].Length);
                        byte[] compressedData = new byte[0];
                        if (compressionMethod == "Oodle" || compressionMethod == "oodle")
                        {
                            compressedData = Oodle.Compress(chunks[i], OodleFormat.Kraken, OodleCompressionLevel.Normal);
                        }
                        if (compressionMethod == "Zlib" || compressionMethod == "zlib")
                        {
                            compressedData = Ionic.Zlib.ZlibStream.CompressBuffer(chunks[i]);
                        }
                        var chunkSize = compressedData.Length;
                        if (encrypt)
                        {
                            compressedData = AesHandler.AddPadding(compressedData, false, i == 0 ? compressedData : chunks[0]);
                        }
                        chunks[i] = compressedData;
                        Metadata.CompressionBlocks[i] = new CompressionBlock
                        {
                            CompressedDataStartOffset = offset,
                            CompressedDataEndOffset = offset + (ulong)chunkSize
                        };
                        offset += (ulong)chunks[i].Length;
                    }
                    Data = Combine(chunks);

                    Metadata.UncompressedCompressionBlockSize = maxChunkSize;
                }
                else
                {
                    Metadata.CompressionBlockCount = 1;
                    
                    if (compressionMethod == "Oodle" || compressionMethod == "oodle")
                    {
                        Data = Oodle.Compress(Data, OodleFormat.Kraken, OodleCompressionLevel.Normal);
                    }
                    if (compressionMethod == "Zlib" || compressionMethod == "zlib")
                    {
                        Data = Ionic.Zlib.ZlibStream.CompressBuffer(Data);
                    }
                }
            }
        }
        private void DecompressData(int metadataBufferLength, string[] compressionMethods)
        {
            if (Metadata.CompressionMethod != 0x00)
            {
                if (compressionMethods == null || compressionMethods.Length == 0) throw new Exception("No compression methods found.");
                var compressionMethod = compressionMethods[Metadata.CompressionMethod - 1];

                if (Metadata.CompressionBlockCount > 1)
                {
                    byte[] fullData = new byte[metadataBufferLength + Data.Length];
                    int offset = 0;
                    Buffer.BlockCopy(new byte[metadataBufferLength], 0, fullData, offset, metadataBufferLength);
                    offset += metadataBufferLength;
                    Buffer.BlockCopy(Data, 0, fullData, offset, Data.Length);
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
                                if (i + 1 == Metadata.CompressionBlocks.Length)
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

        internal PakFileMetadata WriteToStream(BinaryWriter bw, bool compress, bool encrypt, string[] compressionMethods, byte[] AES_KEY)
        {
            var dataOffset = (ulong)bw.BaseStream.Position;
            Metadata.DataRecordOffset = (ulong)0;
            Metadata.UncompressedSize = (ulong)Data.Length;
            compress = compress && Metadata.UncompressedSize > 200000;
            Metadata.CompressionMethod = compress ? (uint)1 : (uint)0;
            Metadata.IsEncrypted = encrypt;
            CompressData(compress, compressionMethods, encrypt);
            Metadata.DataRecordSize = (ulong)Data.Length;
            EncryptData(encrypt, AES_KEY);
            Metadata.PaddedDataSize = (ulong)Data.Length;
            var hashWorthyData = new byte[Metadata.DataRecordSize];
            Array.Copy(Data, hashWorthyData, hashWorthyData.Length);
            Metadata.DataRecordSha1Hash = AesHandler.SHA1Hash(hashWorthyData);
            Metadata.WriteToStream(bw);
            bw.Write(Data);
            Metadata.DataRecordOffset = dataOffset;
            return this.Metadata;
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
        public long MetadataSize { get; set; }
        public byte[] Data { get; set; }
    }
}