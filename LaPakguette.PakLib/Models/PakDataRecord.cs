using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zlib;
using OodleCompressionLib;

namespace LaPakguette.PakLib.Models
{
    public class PakDataRecord
    {
        private const int MaxChunkSize = 65536;

        internal PakDataRecord(byte[] data)
        {
            Data = data;
            Metadata = new PakFileMetadata();
        }

        public PakDataRecord(BinaryReader br, string[] compressionMethods, byte[] aesKey)
        {
            var pos = br.BaseStream.Position;
            Metadata = new PakFileMetadata(br);
            var metadataSize = br.BaseStream.Position - pos;
            MetadataSize = metadataSize;
            Data = br.ReadBytes((int)Metadata.PaddedDataSize);
            br.BaseStream.Position = pos;
            //File.WriteAllBytes(@"C:\Users\leanw\Documents\pak.data", Data);
            DecryptData(aesKey);
            //File.WriteAllBytes(@"C:\Users\leanw\Documents\pak.data.dec", Data);
            DecompressData((int)metadataSize, compressionMethods);
            //File.WriteAllBytes(@"C:\Users\leanw\Documents\pak.data.deco", Data);
        }

        public PakFileMetadata Metadata { get; set; }
        public long MetadataSize { get; set; }
        public byte[] Data { get; set; }

        private void DecryptData(byte[] aesKey)
        {
            if (Metadata.IsEncrypted)
            {
                if (aesKey == null) throw new Exception("No aes key provided. File is encrypted.");
                var decrypted = AesHandler.DecryptAES(Data, aesKey);
                Data = new byte[Metadata.DataRecordSize];
                Array.Copy(decrypted, 0, Data, 0, Data.Length);
            }
        }

        private void EncryptData(bool encrypt, byte[] aesKey)
        {
            if (encrypt)
            {
                if (aesKey == null) throw new Exception("No aes key provided. File can not be encrypted.");
                (Data, _) = AesHandler.EncryptAES(Data, aesKey);
            }
        }

        private void CompressData(bool compress, string[] compressionMethods, bool encrypt)
        {
            if (compress)
            {
                if (compressionMethods == null || compressionMethods.Length == 0)
                    throw new Exception("No compression methods found.");
                var compressionMethod = compressionMethods[Metadata.CompressionMethod - 1];
                if (Data.Length > MaxChunkSize)
                {
                    var chunkCount = (Data.Length + MaxChunkSize - 1) / MaxChunkSize;
                    var chunks = new byte[chunkCount][];
                    Metadata.CompressionBlockCount = (uint)chunkCount;
                    Metadata.CompressionBlocks = new CompressionBlock[chunkCount];
                    ulong offset = 57 + Metadata.CompressionBlockCount * 16;
                    for (var i = 0; i < chunkCount; i++)
                    {
                        chunks[i] = new byte[Math.Min(MaxChunkSize, Data.Length - i * MaxChunkSize)];

                        Array.Copy(Data, i * MaxChunkSize, chunks[i], 0, chunks[i].Length);
                        var compressedData = new byte[0];
                        if (compressionMethod == "Oodle" || compressionMethod == "oodle")
                            compressedData = Oodle.Compress(chunks[i], OodleFormat.Kraken,
                                OodleCompressionLevel.Normal);
                        if (compressionMethod == "Zlib" || compressionMethod == "zlib")
                            compressedData = ZlibStream.CompressBuffer(chunks[i]);
                        var chunkSize = compressedData.Length;
                        if (encrypt)
                            compressedData = AesHandler.AddPadding(compressedData, false,
                                i == 0 ? compressedData : chunks[0]);
                        chunks[i] = compressedData;
                        Metadata.CompressionBlocks[i] = new CompressionBlock
                        {
                            CompressedDataStartOffset = offset,
                            CompressedDataEndOffset = offset + (ulong)chunkSize
                        };
                        offset += (ulong)chunks[i].Length;
                    }

                    Data = Combine(chunks);

                    Metadata.UncompressedCompressionBlockSize = MaxChunkSize;
                }
                else
                {
                    Metadata.CompressionBlockCount = 1;

                    if (compressionMethod == "Oodle" || compressionMethod == "oodle")
                        Data = Oodle.Compress(Data, OodleFormat.Kraken, OodleCompressionLevel.Normal);
                    if (compressionMethod == "Zlib" || compressionMethod == "zlib")
                        Data = ZlibStream.CompressBuffer(Data);
                }
            }
        }

        private void DecompressData(int metadataBufferLength, string[] compressionMethods)
        {
            if (Metadata.CompressionMethod != 0x00)
            {
                if (compressionMethods == null || compressionMethods.Length == 0)
                    throw new Exception("No compression methods found.");
                var compressionMethod = compressionMethods[Metadata.CompressionMethod - 1];

                if (Metadata.CompressionBlockCount > 1)
                {
                    var fullData = new byte[metadataBufferLength + Data.Length];
                    var offset = 0;
                    Buffer.BlockCopy(new byte[metadataBufferLength], 0, fullData, offset, metadataBufferLength);
                    offset += metadataBufferLength;
                    Buffer.BlockCopy(Data, 0, fullData, offset, Data.Length);
                    var chunks = new List<byte[]>();
                    using (var ms = new MemoryStream(fullData))
                    using (var br2 = new BinaryReader(ms))

                    {
                        for (var i = 0; i < Metadata.CompressionBlocks.Length; i++)
                        {
                            var compressionBlock = Metadata.CompressionBlocks[i];
                            br2.BaseStream.Position = (long)compressionBlock.CompressedDataStartOffset;
                            var chunk = br2.ReadBytes((int)compressionBlock.CompressedDataEndOffset -
                                                      (int)compressionBlock.CompressedDataStartOffset);
                            if (compressionMethod == "Oodle" || compressionMethod == "oodle")
                            {
                                var uncompressedBlockSize = (int)Metadata.UncompressedCompressionBlockSize;
                                if (i + 1 == Metadata.CompressionBlocks.Length)
                                    uncompressedBlockSize = (int)Metadata.UncompressedSize -
                                                            (int)Metadata.UncompressedCompressionBlockSize * i;
                                chunk = Oodle.Decompress(chunk, uncompressedBlockSize);
                            }

                            if (compressionMethod == "Zlib" || compressionMethod == "zlib")
                                chunk = ZlibStream.UncompressBuffer(chunk);
                            chunks.Add(chunk);
                        }
                    }

                    Data = Combine(chunks.ToArray());
                }
                else
                {
                    if (compressionMethod == "Oodle" || compressionMethod == "oodle")
                        Data = Oodle.Decompress(Data, (int)Metadata.UncompressedSize);
                    if (compressionMethod == "Zlib" || compressionMethod == "zlib")
                        Data = ZlibStream.UncompressBuffer(Data);
                }
            }
        }

        internal PakFileMetadata WriteToStream(BinaryWriter bw, bool compress, bool encrypt,
            string[] compressionMethods, byte[] aesKey)
        {
            var dataOffset = (ulong)bw.BaseStream.Position;
            Metadata.DataRecordOffset = 0;
            Metadata.UncompressedSize = (ulong)Data.Length;
            compress = compress && Metadata.UncompressedSize > 200000;
            Metadata.CompressionMethod = compress ? 1 : (uint)0;
            Metadata.IsEncrypted = encrypt;
            CompressData(compress, compressionMethods, encrypt);
            Metadata.DataRecordSize = (ulong)Data.Length;
            EncryptData(encrypt, aesKey);
            Metadata.PaddedDataSize = (ulong)Data.Length;
            var hashWorthyData = new byte[Metadata.DataRecordSize];
            Array.Copy(Data, hashWorthyData, hashWorthyData.Length);
            Metadata.DataRecordSha1Hash = AesHandler.SHA1Hash(hashWorthyData);
            Metadata.WriteToStream(bw);
            bw.Write(Data);
            Metadata.DataRecordOffset = dataOffset;
            return Metadata;
        }

        private byte[] Combine(byte[][] arrays)
        {
            var bytes = new byte[arrays.Sum(a => a.Length)];
            var offset = 0;

            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, bytes, offset, array.Length);
                offset += array.Length;
            }

            return bytes;
        }
    }
}