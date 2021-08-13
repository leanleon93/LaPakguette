using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LaPakguette.PakLib.Models
{
    public class PakFooter
    {
        internal PakFooter()
        {
        }

        public PakFooter(BinaryReader br)
        {
            var footerStart = br.BaseStream.Length - 205;
            br.BaseStream.Position = footerStart;
            Magic = br.ReadUInt32();
            Version = br.ReadUInt32();
            IndexOffset = br.ReadUInt64();
            IndexSize = br.ReadUInt64();
            Sha1Hash = br.ReadBytes(20);
            br.ReadByte();
            CompressionMethods = ReadCompressionMethods(br);
        }

        public string[] CompressionMethods { get; set; }
        public uint Magic { get; set; }
        public uint Version { get; set; }
        public ulong IndexOffset { get; set; }
        public ulong IndexSize { get; set; }
        public byte[] Sha1Hash { get; set; }

        private string[] ReadCompressionMethods(BinaryReader br)
        {
            var methods = new List<string>();
            while (true)
            {
                var method = ReadString(br);
                if (!string.IsNullOrEmpty(method))
                {
                    methods.Add(method);
                    br.BaseStream.Seek(27, SeekOrigin.Current);
                }
                else
                {
                    break;
                }
            }

            return methods.ToArray();
        }

        private string ReadString(BinaryReader br)
        {
            var list = new List<byte>();
            while (true)
            {
                var readChar = br.ReadByte();
                if (readChar != 0x00)
                    list.Add(readChar);
                else
                    break;
            }

            return Encoding.UTF8.GetString(list.ToArray());
        }

        internal void WriteToStream(BinaryWriter bw)
        {
            var pos = bw.BaseStream.Position;
            bw.Write(Magic);
            bw.Write(Version);
            bw.Write(IndexOffset);
            bw.Write(IndexSize);
            bw.Write(Sha1Hash);
            bw.Write((byte)0);
            foreach (var method in CompressionMethods)
            {
                bw.Write(Encoding.UTF8.GetBytes(method));
                bw.Write(new byte[28]);
            }

            var pos2 = bw.BaseStream.Position;
            //0xCD
            var padd = 0xCD - (pos2 - pos);
            bw.Write(new byte[padd]);
        }
    }
}