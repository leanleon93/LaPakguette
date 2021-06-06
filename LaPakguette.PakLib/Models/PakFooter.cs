using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LaPakguette.PakLib.Models
{
    public class PakFooter
    {
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

        private string[] ReadCompressionMethods(BinaryReader br)
        {
            var methods = new List<string>();
            while(true)
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
            List<byte> list = new List<byte>();
            while(true)
            {
                var readChar = br.ReadByte();
                if (readChar != 0x00)
                {
                    list.Add(readChar);
                }
                else
                {
                    break;
                }
            }
            
            return Encoding.UTF8.GetString(list.ToArray());
        }

        public string[] CompressionMethods { get; set; }
        public uint Magic { get; set; }
        public uint Version { get; set; }
        public ulong IndexOffset { get; set; }
        public ulong IndexSize { get; set; }
        public byte[] Sha1Hash { get; set; }
    }
}