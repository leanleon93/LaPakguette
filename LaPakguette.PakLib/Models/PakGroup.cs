﻿using System;
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
            var files = Directory.EnumerateFiles(folderPath, "*.pak", SearchOption.AllDirectories);
            foreach (var file in files)
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
            if (_allFilePathsCache != null)
                return _allFilePathsCache;
            var result = new List<string>();
            foreach (var pakName in _paks)
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
            if (_allFilePathsWithMPCache != null)
                return _allFilePathsWithMPCache;
            var result = new List<string>();
            foreach (var pakName in _paks)
            {
                var paths = Pak.GetAllFilenamesWithMP(pakName, _aesKey);
                result.AddRange(paths);
            }
            _allFilePathsWithMPCache = result;
            return result;
        }

        private Dictionary<string, string> _allFilenamesWithSha1HashesWithMPCache;
        public Dictionary<string, string> GetAllFilePathsWithSha1HashWithMP()
        {
            if (_allFilenamesWithSha1HashesWithMPCache != null)
                return _allFilenamesWithSha1HashesWithMPCache;
            var result = new Dictionary<string, string>();
            foreach (var pakName in _paks)
            {
                var paths = Pak.GetAllFilenamesWithSha1HashesWithMP(pakName, _aesKey);
                foreach (var path in paths)
                {
                    if (result.TryGetValue(path.Key, out var hash))
                    {
                        if (hash != path.Value)
                        {
                            throw new Exception("Key already exists");
                        }
                        //else ignore
                        continue;
                    }
                    result.Add(path.Key, path.Value);
                }
            }
            _allFilenamesWithSha1HashesWithMPCache = result;
            return result;
        }

        private Dictionary<string, List<string>> _allFilePathsByPakCache;

        public Dictionary<string, List<string>> GetAllFilePathsByPak()
        {
            if (_allFilePathsByPakCache != null)
                return _allFilePathsByPakCache;
            var result = new Dictionary<string, List<string>>();
            foreach (var pakName in _paks)
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
            if (_allFilePathsByPakWithMPCache != null)
                return _allFilePathsByPakWithMPCache;
            var result = new Dictionary<string, List<string>>();
            foreach (var pakName in _paks)
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

        private readonly Dictionary<string, Pak> _pakCache = new Dictionary<string, Pak>();

        public PakFileEntry GetFileByPathWithMP(string filePath)
        {
            var allFilePaths = GetAllFilePathsByPakWithMP();
            var pak = allFilePaths.FirstOrDefault(x => x.Value.Contains(filePath, StringComparer.OrdinalIgnoreCase));
            if (pak.Key == null)
                return null;
            Pak pakObj;
            if (_pakCache.ContainsKey(pak.Key))
            {
                pakObj = _pakCache[pak.Key];
            }
            else
            {
                pakObj = Pak.FromFile(pak.Key, _aesKey);
                _pakCache.Add(pak.Key, pakObj);
            }

            return pakObj.GetFile(filePath, true);
        }

        public string GetPakNameByPathWithMP(string filePath)
        {
            var allFilePaths = GetAllFilePathsByPakWithMP();
            var pak = allFilePaths.FirstOrDefault(x => x.Value.Contains(filePath, StringComparer.OrdinalIgnoreCase));

            return Path.GetFileName(pak.Key);
        }

        public PakFileMetadata GetFileMetadataByPathWithMP(string filePath)
        {
            var allFilePaths = GetAllFilePathsByPakWithMP();
            var pak = allFilePaths.FirstOrDefault(x => x.Value.Contains(filePath, StringComparer.OrdinalIgnoreCase));
            if (pak.Key == null)
                return null;
            Pak pakObj;
            if (_pakCache.ContainsKey(pak.Key))
            {
                pakObj = _pakCache[pak.Key];
            }
            else
            {
                pakObj = Pak.FromFile(pak.Key, _aesKey);
                _pakCache.Add(pak.Key, pakObj);
            }

            return pakObj.GetFileMetadata(filePath, true);
        }

        public PakFileEntry GetFileByPath(string filePath)
        {
            var allFilePaths = GetAllFilePathsByPak();
            foreach (var pak in allFilePaths)
            {
                foreach (var file in pak.Value)
                {
                    if (file.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        var pakObj = Pak.FromFile(pak.Key, _aesKey);
                        return pakObj.GetFile(filePath);
                    }
                }
            }
            return null;
        }

        public PakFileEntry GetFileByName(string filename)
        {
            var allFilePaths = GetAllFilePathsByPak();
            foreach (var pak in allFilePaths)
            {
                var found = pak.Value.Find(x => Path.GetFileName(x).Equals(filename, StringComparison.OrdinalIgnoreCase));
                if (found != null)
                {
                    var pakObj = Pak.FromFile(pak.Key, _aesKey);
                    return pakObj.GetFile(found);
                }
            }
            return null;
        }
    }
}
