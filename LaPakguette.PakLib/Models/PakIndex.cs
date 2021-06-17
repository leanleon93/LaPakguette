using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LaPakguette.PakLib.Models
{
    public class PakIndex
    {
        public PakIndex() { }

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

        internal (long, long, byte[]) WriteToStream(BinaryWriter bw, bool encryptIndex, byte[] AES_KEY)
        {
            var indexOffset = bw.BaseStream.Position;
            byte[] indexData;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw2 = new BinaryWriter(ms))
                {
                    bw2.Write(MountPointSize);
                    bw2.Write(Encoding.UTF8.GetBytes(MountPoint));
                    bw2.Write((byte)0);
                    bw2.Write(RecordCount);
                    for (int i = 0; i < Records.Length; i++)
                    {
                        Records[i].WriteToStream(bw2);
                    }
                    indexData = ms.ToArray();
                }
            }
            var nonPaddedSize = indexData.Length;
            byte[] hashWorthyData = new byte[nonPaddedSize];
            if(encryptIndex)
            {
                indexData = AesHandler.EncryptAES(indexData, AES_KEY);
                Array.Copy(indexData, 0, hashWorthyData, 0, hashWorthyData.Length);
            }
            bw.Write(indexData);
            var indexSize = bw.BaseStream.Position - indexOffset;
            var hash = PakDataRecord.GetHashSHA1(encryptIndex ? hashWorthyData : indexData);
            return (indexOffset, indexSize, hash);
        }
    }
}
