using System.IO;

namespace LaPakguette.PakLib.Models
{
    public class PakFileMetadata
    {
        public PakFileMetadata(BinaryReader br)
        {
            DataRecordOffset = br.ReadUInt64();
            DataRecordSize = br.ReadUInt64();
            UncompressedSize = br.ReadUInt64();
            CompressionMethod = br.ReadUInt32();
            DataRecordSha1Hash = br.ReadBytes(20);
            if(CompressionMethod != 0x00)
            {
                CompressionBlockCount = br.ReadUInt32();
                CompressionBlocks = new CompressionBlock[CompressionBlockCount];
                for(int i = 0; i < CompressionBlockCount; i++)
                {
                    CompressionBlocks[i] = new CompressionBlock(br);
                }
            }
            IsEncrypted = br.ReadByte() == 1;
            UncompressedCompressionBlockSize = br.ReadUInt32();
        }

        public ulong DataRecordOffset { get; set; }
        public ulong DataRecordSize { get; set; }
        public ulong UncompressedSize { get; set; }
        public uint CompressionMethod { get; set; }
        public byte[] DataRecordSha1Hash { get; set; }
        public uint CompressionBlockCount { get; set; }
        public CompressionBlock[] CompressionBlocks { get; set; }
        public bool IsEncrypted { get; set; }
        public uint UncompressedCompressionBlockSize { get; set; }
    }
}