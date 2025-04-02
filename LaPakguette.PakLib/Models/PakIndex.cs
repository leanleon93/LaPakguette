using System;
using System.IO;
using System.Text;

namespace LaPakguette.PakLib.Models
{
    public class PakIndex
    {
        public PakIndex()
        {
        }

        public PakIndex(string mountPoint)
        {
            MountPoint = mountPoint;
            Records = new PakIndexRecord[0];
        }

        public PakIndex(BinaryReader br, long indexOffset, int indexSize, bool encrypted, byte[] AES_KEY)
        {
            br.BaseStream.Position = indexOffset;
            var buffer = br.ReadBytes(indexSize);
            if (encrypted)
            {
                if (AES_KEY == null)
                    throw new Exception("No aes key provided. File is encrypted.");
                buffer = AesHandler.DecryptAES(buffer, AES_KEY);
            }

            using (var ms = new MemoryStream(buffer))
            {
                using (var br2 = new BinaryReader(ms))
                {
                    MountPointSize = br2.ReadInt32();
                    if (MountPointSize < 0)
                    {
                        var mountPointBytes = br2.ReadBytes(Math.Abs(MountPointSize) * 2 - 2);
                        MountPoint = Encoding.Unicode.GetString(mountPointBytes);
                        br2.ReadByte();
                        br2.ReadByte();
                    }
                    else
                    {
                        MountPoint = Encoding.UTF8.GetString(br2.ReadBytes(MountPointSize - 1));
                        br2.ReadByte();
                    }

                    var recordCount = br2.ReadUInt32();
                    Records = new PakIndexRecord[recordCount];
                    for (var i = 0; i < recordCount; i++)
                        Records[i] = new PakIndexRecord(br2);
                }
            }
        }

        public int MountPointSize { get; set; }
        public string MountPoint { get; set; }
        public int RecordCount => Records.Length;
        public PakIndexRecord[] Records { get; set; }

        internal (long, long, byte[]) WriteToStream(BinaryWriter bw, bool encryptIndex, byte[] AES_KEY)
        {
            var indexOffset = bw.BaseStream.Position;
            byte[] indexData;
            using (var ms = new MemoryStream())
            {
                using (var bw2 = new BinaryWriter(ms))
                {
                    if (NameHelper.CheckUnicodeString(MountPoint))
                    {
                        var mountPointBytes = Encoding.Unicode.GetBytes(MountPoint);
                        var length = (mountPointBytes.Length + 2) / 2 * -1;
                        bw2.Write(length);
                        bw2.Write(mountPointBytes);
                        bw2.Write((byte)0);
                        bw2.Write((byte)0);
                    }
                    else
                    {
                        var mountPointBytes = Encoding.UTF8.GetBytes(MountPoint);
                        bw2.Write(mountPointBytes.Length + 1);
                        bw2.Write(mountPointBytes);
                        bw2.Write((byte)0);
                    }

                    bw2.Write(Records.Length);
                    for (var i = 0; i < Records.Length; i++)
                        Records[i].WriteToStream(bw2);
                    indexData = ms.ToArray();
                }
            }

            byte[] hash;
            if (encryptIndex)
                (indexData, hash) = AesHandler.EncryptAES(indexData, AES_KEY);
            else
                hash = AesHandler.SHA1Hash(indexData);
            bw.Write(indexData);
            var indexSize = bw.BaseStream.Position - indexOffset;
            return (indexOffset, indexSize, hash);
        }
    }
}