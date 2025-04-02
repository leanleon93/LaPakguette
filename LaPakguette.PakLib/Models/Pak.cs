using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LaPakguette.PakLib.Models
{
    public class Pak
    {
        public const string MountpointFileName = "lapakguette_mp.txt";
        private readonly byte[] AES_KEY;
        public readonly string PakPath;

        private Pak(string folderPath, string mp, byte[] AES_KEY)
        {
            this.AES_KEY = AES_KEY;
            Footer = new PakFooter();
            Index = new PakIndex(mp);
            DataRecords = new PakDataRecord[0];
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var relPath = Path.GetRelativePath(folderPath, info.FullName);
                if (relPath == MountpointFileName)
                    continue;
                relPath = relPath.Replace('\\', '/');
                AddFile(new PakFileEntry(relPath, File.ReadAllBytes(file)));
            }
        }

        private Pak(string pakFilePath, byte[] AES_KEY, bool indexOnly = false)
        {
            PakPath = pakFilePath;
            this.AES_KEY = AES_KEY;
            using (var fs = new FileStream(pakFilePath, FileMode.Open))
            {
                InitPakFromStream(fs, indexOnly);
            }
        }

        private Pak(byte[] pakFileBuffer, byte[] AES_KEY, bool indexOnly = false)
        {
            this.AES_KEY = AES_KEY;
            using (var ms = new MemoryStream(pakFileBuffer))
            {
                InitPakFromStream(ms, indexOnly);
            }
        }

        private PakDataRecord[] DataRecords { get; set; }
        private bool IndexEncrypted { get; set; }
        private PakIndex Index { get; set; }
        private PakFooter Footer { get; set; }

        private void InitPakFromStream(Stream s, bool indexOnly)
        {
            using (var br = new BinaryReader(s))
            {
                br.BaseStream.Position = br.BaseStream.Length - 206;
                IndexEncrypted = br.ReadByte() == 1;
                Footer = new PakFooter(br);
                Index = new PakIndex(br, (long)Footer.IndexOffset, (int)Footer.IndexSize, IndexEncrypted, AES_KEY);
                if (indexOnly)
                    return;
                DataRecords = new PakDataRecord[Index.RecordCount];
                for (var i = 0; i < Index.RecordCount; i++)
                {
                    br.BaseStream.Position = (long)Index.Records[i].Metadata.DataRecordOffset;
                    DataRecords[i] = new PakDataRecord(br, Footer.CompressionMethods, AES_KEY);
                }
            }
        }

        public static Pak FromFile(string pakFilePath, byte[] AES_KEY)
        {
            return new Pak(pakFilePath, AES_KEY);
        }

        public static List<string> GetAllFilenames(string pakFilePath, byte[] AES_KEY)
        {
            var pak = new Pak(pakFilePath, AES_KEY, true);
            return pak.GetAllFilenames();
        }

        public static List<string> GetAllFilenamesWithMP(string pakFilePath, byte[] AES_KEY)
        {
            var pak = new Pak(pakFilePath, AES_KEY, true);
            return pak.GetAllFilenamesWithMP();
        }

        public static Dictionary<string, string> GetAllFilenamesWithSha1HashesWithMP(string pakFilePath, byte[] AES_KEY)
        {
            var pak = new Pak(pakFilePath, AES_KEY, true);
            return pak.GetAllFilenamesWithSha1HashesWithMP();
        }

        public static Pak FromBuffer(byte[] pakFileBuffer, byte[] AES_KEY)
        {
            try
            {
                return new Pak(pakFileBuffer, AES_KEY);
            }
            catch
            {
                return null;
            }

        }

        public static Pak FromFolder(string folderPath, byte[] AES_KEY = null)
        {
            try
            {
                var mpPath = Path.Combine(folderPath, MountpointFileName);
                if (!File.Exists(mpPath))
                    throw new FileNotFoundException("No mountpoint file present.");
                var mp = File.ReadAllText(mpPath);
                return new Pak(folderPath, mp, AES_KEY);
            }
            catch
            {
                return null;
            }

        }

        public byte[] ToByteArray(bool compress, bool encrypt, bool encryptIndex, CompressionMethod compressionMethod,
            byte[] AES_KEY = null)
        {
            if (AES_KEY == null)
                AES_KEY = this.AES_KEY;
            IndexEncrypted = encryptIndex;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    Footer.CompressionMethods = compressionMethod != CompressionMethod.None
                        ? new[] { compressionMethod.GetName() }
                        : new string[0];
                    for (var i = 0; i < Index.Records.Length; i++)
                    {
                        var indexRecord = Index.Records[i];
                        var dataRecord = DataRecords[i];
                        var metadata =
                            dataRecord.WriteToStream(bw, compress, encrypt, Footer.CompressionMethods, AES_KEY);
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

        public void AddOrReplaceFile(PakFileEntry newEntry)
        {
            var indexOf = GetFileIndex(newEntry.Name);
            if (indexOf == -1)
                AddFile(newEntry);
            else
                ReplaceFile(newEntry.Name, newEntry.Data);
        }

        public PakFileEntry GetFile(string filename, bool withMp = false)
        {
            if (withMp)
                filename = filename.Replace(Index.MountPoint, "");
            var indexOf = GetFileIndex(filename);
            if (indexOf == -1)
                return null;
            var file = DataRecords[indexOf].Data;
            return new PakFileEntry(filename, file);
        }

        public PakFileMetadata GetFileMetadata(string filename, bool withMp = false)
        {
            if (withMp)
                filename = filename.Replace(Index.MountPoint, "");
            var indexOf = GetFileIndex(filename);
            if (indexOf == -1)
                return null;
            return DataRecords[indexOf].Metadata;
        }

        public void ReplaceFile(string filename, byte[] newData)
        {
            var indexOf = GetFileIndex(filename);
            if (indexOf == -1)
                return;
            DataRecords[indexOf].Data = newData;
        }

        public void RemoveFile(string filename)
        {
            var indexOf = GetFileIndex(filename);
            if (indexOf == -1)
                return;
            DataRecords = DataRecords.RemoveAt(indexOf);
            Index.Records = Index.Records.RemoveAt(indexOf);
        }

        public void AddFile(PakFileEntry newEntry)
        {
            DataRecords = DataRecords.Add(new PakDataRecord(newEntry.Data));
            Index.Records = Index.Records.Add(new PakIndexRecord(newEntry.Name));
        }

        private int GetFileIndex(string filename)
        {
            var indexRecord = Index.Records.FirstOrDefault(x => x.FileName.Equals(filename, StringComparison.InvariantCultureIgnoreCase));
            if (indexRecord == null)
                return -1;
            return Array.IndexOf(Index.Records, indexRecord);
        }

        public List<PakFileEntry> GetFiles(List<string> filenames)
        {
            var result = new List<PakFileEntry>();
            foreach (var filename in filenames)
            {
                var file = GetFile(filename);
                if (file != null)
                    result.Add(file);
            }

            return result;
        }

        public List<PakFileEntry> GetAllFiles()
        {
            var result = new List<PakFileEntry>();
            for (var i = 0; i < Index.Records.Length; i++)
            {
                var indexRecord = Index.Records[i];
                var dataRecord = DataRecords[i];
                result.Add(new PakFileEntry(indexRecord.FileName, dataRecord.Data));
            }

            return result;
        }

        public bool ToFolder(string unpackDir = null)
        {
            try
            {
                if (unpackDir == null)
                    unpackDir = PakPath.Replace(Regex.Match(PakPath, @"\..*").Value, "");
                Directory.CreateDirectory(unpackDir);
                for (var i = 0; i < Index.RecordCount; i++)
                {
                    var filename = Index.Records[i].FileName;
                    var exportPath = Path.Combine(unpackDir, filename);
                    Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
                    File.WriteAllBytes(exportPath, DataRecords[i].Data);
                }

                var mpPath = Path.Combine(unpackDir, MountpointFileName);
                File.WriteAllText(mpPath, Index.MountPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetName()
        {
            return Path.GetFileName(PakPath);
        }

        public string GetMountPoint()
        {
            return Index.MountPoint;
        }

        public void SetMountPoint(string mountpoint)
        {
            Index.MountPoint = mountpoint;
        }

        public List<string> GetAllFilenamesWithMP()
        {
            var allFilenames = this.GetAllFilenames();
            for (int i = 0; i < allFilenames.Count; i++)
            {
                string file = allFilenames[i];
                file = Index.MountPoint + file;
                allFilenames[i] = file;
            }
            return allFilenames;
        }

        public Dictionary<string, string> GetAllFilenamesWithSha1HashesWithMP()
        {
            var allFilenames = this.GetAllFilenamesWithSha1Hashes();
            foreach (var item in allFilenames.ToList())
            {
                string file = item.Key;
                file = Index.MountPoint + file;
                allFilenames[file] = item.Value;
                allFilenames.Remove(item.Key);
            }
            return allFilenames;
        }

        public List<string> GetAllFilenames()
        {
            return Index.Records.Select(x => x.FileName).ToList();
        }

        public Dictionary<string, string> GetAllFilenamesWithSha1Hashes()
        {
            return Index.Records.ToDictionary(x => x.FileName, x => x.Metadata.DataRecordSha1HashString);
        }
    }
}