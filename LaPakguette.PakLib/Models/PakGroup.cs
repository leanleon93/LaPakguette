using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LaPakguette.PakLib.Models
{
    public class PakGroup
    {
        private readonly List<string> _paks;
        private readonly byte[] _aesKey;
        public string Folder { get; set; }
        private PakGroup(string folderPath, byte[] AES_KEY)
        {
            Folder = folderPath;
            _aesKey = AES_KEY;
            _paks = new List<string>();
            foreach(var file in Directory.EnumerateFiles(folderPath).Where(x => x.EndsWith(".pak")))
            {
                _paks.Add(file);
            }
        }

        public static PakGroup FromFolder(string folderPath, byte[] AES_KEY)
        {
            return new PakGroup(folderPath, AES_KEY);
        }

        private List<string> _allFilePathsCache;
        public List<string> GetAllFilePaths()
        {
            if(_allFilePathsCache != null) return _allFilePathsCache;
            var result = new List<string>();
            foreach(var pakName in _paks)
            {
                var paths = Pak.GetAllFilenames(pakName, _aesKey);
                result.AddRange(paths);
            }
            _allFilePathsCache = result;
            return result;
        }

        private List<string> _allFilePathsWithMPCache;
        public List<string> GetAllFilePathsWithMP()
        {
            if(_allFilePathsWithMPCache != null) return _allFilePathsWithMPCache;
            var result = new List<string>();
            foreach(var pakName in _paks)
            {
                var paths = Pak.GetAllFilenamesWithMP(pakName, _aesKey);
                result.AddRange(paths);
            }
            _allFilePathsWithMPCache = result;
            return result;
        }

        private Dictionary<string, List<string>> _allFilePathsByPakCache;

        public Dictionary<string, List<string>> GetAllFilePathsByPak()
        {
            if(_allFilePathsByPakCache != null) return _allFilePathsByPakCache;
            var result = new Dictionary<string, List<string>>();
            foreach(var pakName in _paks)
            {
                var paths = Pak.GetAllFilenames(pakName, _aesKey);
                result.Add(pakName, paths);
            }
            _allFilePathsByPakCache = result;
            return result;
        }

        private Dictionary<string, List<string>> _allFilePathsByPakWithMPCache;

        public Dictionary<string, List<string>> GetAllFilePathsByPakWithMP()
        {
            if(_allFilePathsByPakWithMPCache != null) return _allFilePathsByPakWithMPCache;
            var result = new Dictionary<string, List<string>>();
            foreach(var pakName in _paks)
            {
                var paths = Pak.GetAllFilenamesWithMP(pakName, _aesKey);
                result.Add(pakName, paths);
            }
            _allFilePathsByPakWithMPCache = result;
            return result;
        }

        public List<string> GetPakNames()
        {
            return _paks;
        }

        public Pak GetPakByName(string pakName)
        {
            if (_paks.Contains(pakName))
            {
                return Pak.FromFile(pakName, _aesKey);
            }
            return null;
        }

        public PakFileEntry GetFileByPathWithMP(string filePath)
        {
            var allFilePaths = GetAllFilePathsByPakWithMP();
            foreach(var pak in allFilePaths)
            {
                foreach(var file in pak.Value)
                {
                    if(file == filePath)
                    {
                        var pakObj = Pak.FromFile(pak.Key, _aesKey);
                        return pakObj.GetFile(filePath, true);
                    }
                }
            }
            return null;
        }

        public PakFileEntry GetFileByPath(string filePath)
        {
            var allFilePaths = GetAllFilePathsByPak();
            foreach(var pak in allFilePaths)
            {
                foreach(var file in pak.Value)
                {
                    if(file == filePath)
                    {
                        var pakObj = Pak.FromFile(pak.Key, _aesKey);
                        return pakObj.GetFile(filePath);
                    }
                }
            }
            return null;
        }
    }
}
