using System.IO;

namespace LaPakguette.PakLib.Models
{
    public class CompressionBlock
    {
        public CompressionBlock() { }
        public CompressionBlock(BinaryReader br)
        {
            CompressedDataStartOffset = br.ReadUInt64();
            CompressedDataEndOffset = br.ReadUInt64();
        }
        public ulong CompressedDataStartOffset { get; set; }
        public ulong CompressedDataEndOffset { get; set; }
    }
}