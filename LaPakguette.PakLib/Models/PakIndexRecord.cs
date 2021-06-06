using System.IO;
using System.Text;

namespace LaPakguette.PakLib.Models
{
    public class PakIndexRecord
    {

        public PakIndexRecord(BinaryReader br)
        {
            FileNameSize = br.ReadUInt32();
            FileName = Encoding.UTF8.GetString(br.ReadBytes((int)FileNameSize - 1));
            br.ReadByte();
            Metadata = new PakFileMetadata(br);
        }
        public uint FileNameSize { get; set; }
        public string FileName { get; set; }
        public PakFileMetadata Metadata { get; set; }
    }
}