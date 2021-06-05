using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LaPakguette.Lib
{
    public class PakHandler
    {
        private const string quotes = "\"";
        private UnrealPakCmdHelper _unrealPakHelper;
        private string _pakPath;
        private string _mountPoint;

        public PakHandler(string unrealpakpath, string pakPathOrFolderPath)
        {
            if(!File.Exists(unrealpakpath) || (!File.Exists(pakPathOrFolderPath) && !Directory.Exists(pakPathOrFolderPath)))
            {
                throw new FileNotFoundException();
            }
            _unrealPakHelper = new UnrealPakCmdHelper(unrealpakpath);
            
            SetPakPath(pakPathOrFolderPath);
        }

        public void SetPakPath(string pakPath)
        {
            var fileAttr = File.GetAttributes(pakPath);
            var isDirectory = fileAttr.HasFlag(FileAttributes.Directory);
            if (isDirectory)
            {
                _pakPath = pakPath + "\\..\\" + Path.GetFileName(pakPath) + ".pak";
            }
            else
            {
                _pakPath = pakPath;
                SetMountPoint();
            }
        }

        public void Unpack()
        {
            if(_mountPoint != null)
            {
                var success = _unrealPakHelper.Unpack(_pakPath);
                if (success)
                {
                    CreateRepackFile();
                }
            }
        }

        public void Repack(bool compress = false, bool encrypt = false)
        {
            Backup();
            var success = _unrealPakHelper.Repack(_pakPath, compress, encrypt, true);
            if(success)
            {
                SetMountPoint(); //update mount point in case of directory only repack
            }
        }

        private void Backup()
        {
            if(File.Exists(_pakPath))
            {
                if (File.Exists(_pakPath + ".bak"))
                {
                    File.Delete(_pakPath + ".bak");
                }
                File.Copy(_pakPath, _pakPath + ".bak");
            }
        }

        private void CreateRepackFile()
        {
            var repackFolderPath = _pakPath.Replace(".pak", "");
            var repackFolders = Directory.GetDirectories(repackFolderPath);
            var repackFiles = Directory.GetFiles(repackFolderPath).Where(x => Path.GetFileName(x) != _unrealPakHelper.repackFilename);
            var repackLines = new List<string>();
            foreach (var folder in repackFolders)
            {
                var dirName = Path.GetFileName(folder);
                repackLines.Add(quotes + folder + "\\*\" " + quotes + _mountPoint + dirName + "/" + quotes);
            }
            foreach(var file in repackFiles)
            {
                var fileName = Path.GetFileName(file);
                repackLines.Add(quotes + file + quotes + " " + quotes + _mountPoint + fileName + quotes);
            }
            File.WriteAllLines(repackFolderPath + "\\" + _unrealPakHelper.repackFilename, repackLines);
        }

        private void SetMountPoint()
        {
            byte[] indexBuffer;
            bool indexIsEncrypted;
            using (var br = new BinaryReader(new FileStream(_pakPath, FileMode.Open)))
            {
                var encryptedIndexOffset = br.BaseStream.Length - 0xCE;
                br.BaseStream.Position = encryptedIndexOffset;
                indexIsEncrypted = br.ReadByte() == 1;
                var indexOffsetOffset = br.BaseStream.Length - 0xC5;
                br.BaseStream.Position = indexOffsetOffset;
                var indexOffset = br.ReadInt64();
                var indexLength = br.ReadInt64();
                br.BaseStream.Position = indexOffset;
                indexBuffer = br.ReadBytes((int)indexLength);
            }
            string mountPoint;
            var decrypted = indexIsEncrypted ? AesHandler.DecryptAES(indexBuffer) : indexBuffer;
            using (var br = new BinaryReader(new MemoryStream(decrypted)))
            {
                var mountPointLength = br.ReadInt32();
                mountPoint = Encoding.UTF8.GetString(br.ReadBytes(mountPointLength - 1)); // -1 to remove terminating 0 byte
            }
            _mountPoint = mountPoint;
        }

    }
}
