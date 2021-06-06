using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LaPakguette.PakLib.Models
{
    public class PakIndex
    {

        public PakIndex(BinaryReader br, long indexOffset, int indexSize, bool encrypted, byte[] AES_KEY)
        {
            br.BaseStream.Position = indexOffset;
            var buffer = br.ReadBytes(indexSize);
            if (encrypted)
            {
                if (AES_KEY == null) throw new Exception("No aes key provided. File is encrypted.");
                buffer = AesHandler.DecryptAES(buffer, AES_KEY);
            }
            using (var ms = new MemoryStream(buffer))
            {
                using (var br2 = new BinaryReader(ms))
                {
                    MountPointSize = br2.ReadUInt32();
                    MountPoint = Encoding.UTF8.GetString(br2.ReadBytes((int)MountPointSize - 1));
                    br2.ReadByte();
                    RecordCount = br2.ReadUInt32();
                    Records = new PakIndexRecord[RecordCount];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        Records[i] = new PakIndexRecord(br2);
                    }
                }
            }
        }
        public uint MountPointSize { get; set; }
        public string MountPoint { get; set; }
        public uint RecordCount { get; set; }
        public PakIndexRecord[] Records { get; set; }
    }
}
