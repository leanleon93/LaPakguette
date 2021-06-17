using LaPakguette.Lib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LaPakguette.Lib
{
    /// <summary>
    /// Handler for BNS UE4 .pak files wrapping UnrealPak
    /// </summary>
    public class PakHandler
    {
        private const string quotes = "\"";
        private UnrealPakCmdHelper _unrealPakHelper;
        private byte[] _aesKey;

        /// <summary>
        /// Handler for BNS UE4 .pak files wrapping UnrealPak
        /// </summary>
        /// <param name="unrealpakpath">Path to UnrealPak.exe</param>
        public PakHandler(string unrealpakpath)
        {
            SetupAesKeyFromCryptoJson();
            if (!File.Exists(unrealpakpath))
            {
                throw new FileNotFoundException();
            }
            _unrealPakHelper = new UnrealPakCmdHelper(unrealpakpath);
        }

        private void SetupAesKeyFromCryptoJson()
        {
            var cryptoPath = Directory.GetCurrentDirectory() + "\\Crypto.json";
            if (!File.Exists(cryptoPath))
            {
                throw new FileNotFoundException();
            }
            CryptoModel crypto = JsonConvert.DeserializeObject<CryptoModel>(File.ReadAllText(cryptoPath));
            var basedAesKey = crypto.EncryptionKey.Key;
            if(!string.IsNullOrEmpty(basedAesKey) && basedAesKey.ToLower() != "null")
                _aesKey = Convert.FromBase64String(basedAesKey);
        }

        /// <summary>
        /// Unpack a .pak file
        /// </summary>
        /// <param name="pakPath">File to unpack</param>
        /// <param name="unpackDir">Custom unpack directory<br />(Default next to .pak file)</param>
        public bool Unpack(string pakPath, string unpackDir = null)
        {
            if (unpackDir == null)
            {
                unpackDir = pakPath.Replace(".pak", "");
            }
            if (!File.Exists(pakPath))
            {
                throw new FileNotFoundException("The .pak file could not be found!");
            }
            var mountpoint = GetMountPoint(pakPath);
            if (mountpoint == null) return false;
            var success = _unrealPakHelper.Unpack(pakPath, unpackDir);
            if (success)
            {
                CreateRepackFile(pakPath, mountpoint, unpackDir);
            }
            return success;
        }

        /// <summary>
        /// Create a .pak file from a repack folder<br />A repack.txt must exist in the folder.
        /// </summary>
        /// <param name="repackFolder">Repack folder with pak content and repack.txt file.</param>
        /// <param name="pakOutPath">Custom output path of .pak file<br />(Default next to repack folder)</param>
        /// <param name="compress">Enable oodle and zlib compression.</param>
        /// <param name="encrypt">Enable content encryption.</param>
        /// <param name="encryptIndex">Enable index encryption.</param>
        public bool Create(string repackFolder, string pakOutPath = null, bool compress = true, bool encrypt = false, bool encryptIndex = true)
        {
            if (pakOutPath == null)
            {
                pakOutPath = repackFolder + ".pak";
            }
            if (File.Exists(pakOutPath))
            {
                Backup(pakOutPath);
            }
            return _unrealPakHelper.Repack(repackFolder, pakOutPath, compress, encrypt, encryptIndex);
        }

        /// <summary>
        /// Repack a .pak file from the default unpack folder.
        /// </summary>
        /// <param name="pakPath">Path to .pak file.</param>
        /// <param name="compress">Enable oodle and zlib compression.</param>
        /// <param name="encrypt">Enable content encryption.</param>
        /// <param name="encryptIndex">Enable index encryption.</param>
        public bool Repack(string pakPath, bool compress = false, bool encrypt = false, bool encryptIndex = true)
        {
            if (File.Exists(pakPath))
            {
                Backup(pakPath);
            }
            var repackFolderPath = pakPath.Replace(".pak", "");
            if (!Directory.Exists(repackFolderPath))
            {
                throw new DirectoryNotFoundException("The repack folder could not be found!");
            }
            return _unrealPakHelper.Repack(repackFolderPath, pakPath, compress, encrypt, encryptIndex);
        }

        private void Backup(string pakPath)
        {
            if (File.Exists(pakPath))
            {
                if (File.Exists(pakPath + ".bak"))
                {
                    File.Delete(pakPath + ".bak");
                }
                File.Copy(pakPath, pakPath + ".bak");
            }
        }

        private void CreateRepackFile(string pakPath, string mountPoint, string customUnpackDir)
        {
            var repackFolderPath = customUnpackDir ?? pakPath.Replace(".pak", "");
            var repackFolders = Directory.GetDirectories(repackFolderPath);
            var repackFiles = Directory.GetFiles(repackFolderPath).Where(x => Path.GetFileName(x) != _unrealPakHelper.repackFilename);
            var repackLines = new List<string>();
            foreach (var folder in repackFolders)
            {
                var dirName = Path.GetFileName(folder);
                repackLines.Add(quotes + folder + "\\*\" " + quotes + mountPoint + dirName + "/" + quotes);
            }
            foreach (var file in repackFiles)
            {
                var fileName = Path.GetFileName(file);
                repackLines.Add(quotes + file + quotes + " " + quotes + mountPoint + fileName + quotes);
            }
            File.WriteAllLines(repackFolderPath + "\\" + _unrealPakHelper.repackFilename, repackLines);
        }

        private string GetMountPoint(string pakPath)
        {
            try
            {
                byte[] indexBuffer;
                bool indexIsEncrypted;
                using (var br = new BinaryReader(new FileStream(pakPath, FileMode.Open)))
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
                var decrypted = indexIsEncrypted ? AesHandler.DecryptAES(indexBuffer, _aesKey) : indexBuffer;
                using (var br = new BinaryReader(new MemoryStream(decrypted)))
                {
                    var mountPointLength = br.ReadInt32();
                    mountPoint = Encoding.UTF8.GetString(br.ReadBytes(mountPointLength - 1)); // -1 to remove terminating 0 byte
                }
                return mountPoint;
            }
            catch
            {
                return null;
            }
        }

    }
}
