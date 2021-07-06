using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LaPakguette.PakLib.Models
{
    public class Pak
    {
        private string _pakPath;
        internal const string mountpointFileName = "mp.txt";
        private Pak() { }
        public Pak(string pakFilePath, byte[] AES_KEY)
        {
            _pakPath = pakFilePath;
            using(var fs = new FileStream(pakFilePath, FileMode.Open))
            {
                using (var br = new BinaryReader(fs))
                {
                    br.BaseStream.Position = br.BaseStream.Length - 206;
                    IndexEncrypted = br.ReadByte() == 1;
                    Footer = new PakFooter(br);
                    Index = new PakIndex(br, (long)Footer.IndexOffset, (int)Footer.IndexSize, IndexEncrypted, AES_KEY);
                    DataRecords = new PakDataRecord[Index.RecordCount];
                    for(int i = 0; i < Index.RecordCount; i++)
                    {
                        br.BaseStream.Position = (long)Index.Records[i].Metadata.DataRecordOffset;
                        DataRecords[i] = new PakDataRecord(br, Footer.CompressionMethods, AES_KEY);
                    }
                }
            }
        }

        public byte[] ToByteArray(bool compress, bool encrypt, bool encryptIndex, string[] compressionMethods, byte[] AES_KEY = null)
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    Footer.CompressionMethods = compressionMethods;
                    for (int i = 0; i < Index.Records.Length; i++)
                    {
                        PakIndexRecord indexRecord = Index.Records[i];
                        PakDataRecord dataRecord = DataRecords[i];
                        var metadata = dataRecord.WriteToStream(bw, compress, encrypt, compressionMethods, AES_KEY);
                        indexRecord.Metadata = metadata;
                    }
                    var (indexOffset, indexSize, hash) = Index.WriteToStream(bw, encryptIndex, AES_KEY);
                    Footer.IndexOffset = (ulong)indexOffset;
                    Footer.IndexSize = (ulong)indexSize;
                    Footer.Sha1Hash = hash;
                    bw.Write(new byte[16]);
                    bw.Write((byte)(encryptIndex ? 1 : 0));
                    Footer.WriteToStream(bw);
                    return ms.ToArray();
                }
            }
            
        }

        public static Pak CreateFromFolder(string folderPath)
        {
            var mpPath = Path.Combine(folderPath, mountpointFileName);
            if (!File.Exists(mpPath)) throw new FileNotFoundException("No mountpoint file present.");
            return new Pak();
        }

        public void DumpAllFiles(string unpackDir = null)
        {
            if(unpackDir == null)
            {
                unpackDir = _pakPath.Replace(Regex.Match(_pakPath, @"\..*").Value, "");
            }
            Directory.CreateDirectory(unpackDir);
            for(int i = 0; i < Index.RecordCount; i++)
            {
                var filename = Index.Records[i].FileName;
                var exportPath = Path.Combine(unpackDir, filename);
                Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
                File.WriteAllBytes(exportPath, DataRecords[i].Data);
            }
            var mpPath = Path.Combine(unpackDir, mountpointFileName);
            File.WriteAllText(mpPath, Index.MountPoint);
        }

        public void DumpFile(string filename, string unpackDir = null)
        {
            if (unpackDir == null)
            {
                unpackDir = _pakPath.Replace(Regex.Match(_pakPath, @"\..*").Value, "");
            }
            Directory.CreateDirectory(unpackDir);
            var indexRecord = Index.Records.Where(x => x.FileName == filename).FirstOrDefault();
            var indexOf = Array.IndexOf<PakIndexRecord>(Index.Records, indexRecord);
            var file = DataRecords[indexOf].Data;
            var exportPath = Path.Combine(unpackDir, filename);
            Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
            File.WriteAllBytes(exportPath, file);
            var mpPath = Path.Combine(unpackDir, mountpointFileName);
            File.WriteAllText(mpPath, Index.MountPoint);
        }

        public void DumpFiles(List<string> filenames, string unpackDir = null)
        {
            foreach(var filename in filenames)
            {
                var indexRecords = Index.Records.Where(x => x.FileName.EndsWith(filename)).ToList();
                foreach(var foundFile in indexRecords)
                {
                    DumpFile(foundFile.FileName, unpackDir);
                }
            }
            
        }

        public List<string> GetAllFilenames()
        {
            return Index.Records.Select(x => x.FileName).ToList();
        }

        public PakDataRecord[] DataRecords { get; set; }
        public bool IndexEncrypted { get; set; }
        public PakIndex Index { get; set; }
        public PakFooter Footer { get; set; }
    }
}
