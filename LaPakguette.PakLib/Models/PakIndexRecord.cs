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
            FileNameSize = br.ReadInt32();
            if (FileNameSize < 0)
            {
                FileName = Encoding.Unicode.GetString(br.ReadBytes(Math.Abs(FileNameSize) * 2 - 2));
                br.ReadByte();
                br.ReadByte();
            }
            else
            {
                FileName = Encoding.UTF8.GetString(br.ReadBytes(FileNameSize - 1));
                br.ReadByte();
            }

            Metadata = new PakFileMetadata(br);
        }

        public int FileNameSize { get; set; }
        public string FileName { get; set; }
        public PakFileMetadata Metadata { get; set; }

        internal void WriteToStream(BinaryWriter bw)
        {
            if (NameHelper.CheckUnicodeString(FileName))
            {
                var filenameBytes = Encoding.Unicode.GetBytes(FileName);
                var length = (filenameBytes.Length + 2) / 2 * -1;
                bw.Write(length);
                bw.Write(filenameBytes);
                bw.Write((byte)0);
                bw.Write((byte)0);
            }
            else
            {
                var filenameBytes = Encoding.UTF8.GetBytes(FileName);
                bw.Write(filenameBytes.Length + 1);
                bw.Write(filenameBytes);
                bw.Write((byte)0);
            }

            Metadata.WriteToStream(bw);
        }
    }
}