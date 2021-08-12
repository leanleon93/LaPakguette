using System;
using System.IO;
using System.Text;

namespace LaPakguette.PakLib.Models
{
    public class PakIndexRecord
    {

        internal PakIndexRecord(string filename)
        {
            FileName = filename;
        }

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

        internal void WriteToStream(BinaryWriter bw)
        {
            var filenameBytes = Encoding.UTF8.GetBytes(FileName);
            bw.Write((uint)(filenameBytes.Length + 1));
            bw.Write(filenameBytes);
            bw.Write((byte)0);
            Metadata.WriteToStream(bw);
        }
    }
}