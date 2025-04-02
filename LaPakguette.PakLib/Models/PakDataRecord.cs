using Ionic.Zlib;
using OodleCompressionLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            _aesKey = aesKey;
            _compressionMethods = compressionMethods;
            var pos = br.BaseStream.Position;
            Metadata = new PakFileMetadata(br);
            var metadataSize = br.BaseStream.Position - pos;
            MetadataSize = metadataSize;
            _data = br.ReadBytes((int)Metadata.PaddedDataSize);
            br.BaseStream.Position = pos;
        }
        private readonly string[] _compressionMethods;
        private readonly byte[] _aesKey;
        public PakFileMetadata Metadata { get; set; }
        public long MetadataSize { get; set; }
        private byte[] _data;
        private bool _accessed;
        public byte[] Data
        {
            get
            {
                if (_accessed)
                    return _data;
                DecryptData(_aesKey);
                DecompressData((int)MetadataSize, _compressionMethods);
                _accessed = true;
                return _data;
            }
            set { _data = value; _accessed = true; }
        }

        private void DecryptData(byte[] aesKey)
        {
            if (Metadata.IsEncrypted && !_accessed)
            {
                if (aesKey == null)
                    throw new Exception("No aes key provided. File is encrypted.");
                var decrypted = AesHandler.DecryptAES(_data, aesKey);
                _data = new byte[Metadata.DataRecordSize];
                Array.Copy(decrypted, 0, _data, 0, _data.Length);
            }
        }

        private void EncryptData(bool encrypt, byte[] aesKey)
        {
            if (encrypt)
            {
                if (aesKey == null)
                    throw new Exception("No aes key provided. File can not be encrypted.");
                (_data, _) = AesHandler.EncryptAES(_data, aesKey);
            }
        }

        private void CompressData(bool compress, string[] compressionMethods, bool encrypt)
        {
            if (compress)
            {
                if (compressionMethods == null || compressionMethods.Length == 0)
                    throw new Exception("No compression methods found.");
                var compressionMethod = compressionMethods[Metadata.CompressionMethod - 1];
                if (_data.Length > MaxChunkSize)
                {
                    var chunkCount = (_data.Length + MaxChunkSize - 1) / MaxChunkSize;
                    var chunks = new byte[chunkCount][];
                    Metadata.CompressionBlockCount = (uint)chunkCount;
                    Metadata.CompressionBlocks = new CompressionBlock[chunkCount];
                    ulong offset = 57 + Metadata.CompressionBlockCount * 16;
                    for (var i = 0; i < chunkCount; i++)
                    {
                        chunks[i] = new byte[Math.Min(MaxChunkSize, _data.Length - i * MaxChunkSize)];

                        Array.Copy(_data, i * MaxChunkSize, chunks[i], 0, chunks[i].Length);
                        var compressedData = new byte[0];
                        if (IsOodleCompression(compressionMethod))
                            compressedData = Oodle.Compress(chunks[i], OodleFormat.Kraken,
                                OodleCompressionLevel.Normal);
                        if (IsZlibCompression(compressionMethod))
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

                    _data = Combine(chunks);

                    Metadata.UncompressedCompressionBlockSize = MaxChunkSize;
                }
                else
                {
                    Metadata.CompressionBlockCount = 1;

                    if (IsOodleCompression(compressionMethod))
                        _data = Oodle.Compress(_data, OodleFormat.Kraken, OodleCompressionLevel.Normal);
                    if (IsZlibCompression(compressionMethod))
                        _data = ZlibStream.CompressBuffer(_data);
                }
            }
        }

        private void DecompressData(int metadataBufferLength, string[] compressionMethods)
        {
            if (Metadata.CompressionMethod != 0x00 && !_accessed)
            {
                if (compressionMethods == null || compressionMethods.Length == 0)
                    throw new Exception("No compression methods found.");
                var compressionMethod = compressionMethods[Metadata.CompressionMethod - 1];

                if (Metadata.CompressionBlockCount > 1)
                {
                    var fullData = new byte[metadataBufferLength + _data.Length];
                    var offset = 0;
                    Buffer.BlockCopy(new byte[metadataBufferLength], 0, fullData, offset, metadataBufferLength);
                    offset += metadataBufferLength;
                    Buffer.BlockCopy(_data, 0, fullData, offset, _data.Length);
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
                            if (IsOodleCompression(compressionMethod))
                            {
                                var uncompressedBlockSize = (int)Metadata.UncompressedCompressionBlockSize;
                                if (i + 1 == Metadata.CompressionBlocks.Length)
                                    uncompressedBlockSize = (int)Metadata.UncompressedSize -
                                                            (int)Metadata.UncompressedCompressionBlockSize * i;
                                chunk = Oodle.Decompress(chunk, uncompressedBlockSize);
                            }

                            if (IsZlibCompression(compressionMethod))
                                chunk = ZlibStream.UncompressBuffer(chunk);
                            chunks.Add(chunk);
                        }
                    }

                    _data = Combine(chunks.ToArray());
                }
                else
                {
                    if (IsOodleCompression(compressionMethod))
                        _data = Oodle.Decompress(_data, (int)Metadata.UncompressedSize);
                    if (IsZlibCompression(compressionMethod))
                        _data = ZlibStream.UncompressBuffer(_data);
                }
            }
        }

        private const string OODLE = "oodle", ZLIB = "zlib";

        private static bool IsOodleCompression(string compressionMethod)
        {
            return compressionMethod.ToLower() == OODLE;
        }

        private static bool IsZlibCompression(string compressionMethod)
        {
            return compressionMethod.ToLower() == ZLIB;
        }

        internal PakFileMetadata WriteToStream(BinaryWriter bw, bool compress, bool encrypt,
            string[] compressionMethods, byte[] aesKey)
        {
            var dataOffset = (ulong)bw.BaseStream.Position;
            compress = compress && Metadata.UncompressedSize > 200000;
            if (!_accessed && encrypt == Metadata.IsEncrypted && (compress && Metadata.CompressionMethod > 0 || !compress && Metadata.CompressionMethod == 0)) //if we repack with same settings just rewrite the packed buffer
            {
                Metadata.WriteToStream(bw);
                bw.Write(_data);
                Metadata.DataRecordOffset = dataOffset;
                return Metadata;
            }
            _ = Data; //force load
            Metadata.DataRecordOffset = 0;
            Metadata.UncompressedSize = (ulong)_data.Length;
            compress = compress && Metadata.UncompressedSize > 200000;
            Metadata.CompressionMethod = compress ? 1 : (uint)0;
            Metadata.IsEncrypted = encrypt;
            CompressData(compress, compressionMethods, encrypt);
            Metadata.DataRecordSize = (ulong)_data.Length;
            EncryptData(encrypt, aesKey);
            _accessed = false;
            Metadata.PaddedDataSize = (ulong)_data.Length;
            var hashWorthyData = new byte[Metadata.DataRecordSize];
            Array.Copy(_data, hashWorthyData, hashWorthyData.Length);
            Metadata.DataRecordSha1Hash = AesHandler.SHA1Hash(hashWorthyData);
            Metadata.WriteToStream(bw);
            bw.Write(_data);
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