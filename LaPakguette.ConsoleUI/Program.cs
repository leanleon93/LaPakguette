using LaPakguette.PakLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaPakguette.ConsoleUI
{
    //  Dev tests
    internal class Program
    {
        private static readonly string _livepak3 =
            @"F:\Games\BNS_LIVE\BNSR\Content\Paks\Pak_F_LP_172-WindowsNoEditor.pak";
        private static readonly string _pakFolderKRT = @"F:\Games\ALL_BNS\Blade & Soul_Test_KR\BNSR\Content\Paks";
        private static readonly string _pakFolderKR = @"F:\Games\ALL_BNS\Blade & Soul_KR\BNSR\Content\Paks";
        private static readonly string _pakFolderEU = @"F:\Games\ALL_BNS\BnS_UE4\BNSR\Content\Paks";
        private static readonly string _outPath = @"..\..\..\..\..\LaPakguette.PakLibTests\TestFiles\result\consoleResults";
        private static readonly string BASE64_AES_KEY = @"0uX3+U5iXv4nJrU2DBA5zny5q7dgqU83uxWm3Ah0FlY=";
        private static byte[] _aesKey;

        private static async Task Main(string[] args)
        {
            _aesKey = Convert.FromBase64String(BASE64_AES_KEY);
            //var pakGroup = PakGroup.FromFolder(@"F:\Games\ALL_BNS\Blade & Soul_Test_KR\BNSR\Content\Paks", _aesKey);
            //var file = pakGroup.GetFileByName("MI_UIFX_AchievementWinter_BG_02.uasset");
            //RepackUnencrypted();
            await DumpFullFileList();
            //var pak = Pak.FromFile(_livepak3, _aesKey);

            //pak.ToFolder();
            //var repack = pak.ToByteArray(true, true, false, CompressionMethod.Oodle);
            //File.WriteAllBytes(_livepak3 + ".new", repack);
        }

        //private static void RepackUnencrypted()
        //{
        //    var path = @"F:\Games\BNS_LIVE\BNSR\Content\Paks\broken\Pak_F_LP_2-WindowsNoEditor.pak";
        //    var unrealpakPath = @"C:\Program Files\Epic Games\UE_4.25\Engine\Binaries\Win64\UnrealPak.exe";
        //    var outPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        //    Directory.CreateDirectory(outPath);
        //    var pakhelper = new PakHandler(unrealpakPath);
        //    pakhelper.Unpack(path, outPath);
        //    pakhelper.Repack(path, true, false, false);
        //}

        private static async Task DumpFullFileList()
        {
            //DumpRegion("KRT", _pakFolderKRT);
            DumpRegion("KR", _pakFolderKR);
            //DumpRegion("EU", _pakFolderEU);
        }

        private static void DumpRegion(string name, string pakFolder)
        {
            var outPath = Path.Combine(Path.GetFullPath(_outPath), name);
            Directory.CreateDirectory(outPath);
            var group = PakGroup.FromFolder(pakFolder, _aesKey);
            var allFiles = group.GetAllFilePathsWithSha1HashWithMP();
            //File.WriteAllLines(Path.Combine(outPath, "allFiles.txt"), allFiles.Select(x => x.Key).ToList());

            var combinedPaths = new Dictionary<string, Dictionary<string, string>>();

            foreach (var file in allFiles)
            {
                var path = GetPathDepth(file.Key, 4);
                if (!combinedPaths.ContainsKey(path))
                {
                    combinedPaths.Add(path, new Dictionary<string, string>() { { file.Key, file.Value } });
                }
                else
                {
                    combinedPaths[path].Add(file.Key, file.Value);
                }
            }

            foreach (var bundle in combinedPaths)
            {
                string absolutePath = Path.Combine(outPath, bundle.Key);
                Directory.CreateDirectory(absolutePath);
                var filePath = Path.Combine(absolutePath, "files.csv");
                string[] lines = bundle.Value.Select(kvp => $"{kvp.Key.Replace("../", "").Replace(bundle.Key + "/", "")};{kvp.Value}").ToArray();
                File.WriteAllText(filePath, "Path;SHA1" + Environment.NewLine, Encoding.Unicode);
                File.AppendAllLines(filePath, lines, Encoding.Unicode);
            }

            Console.WriteLine($"{name} writing complete.");

        }

        static string GetPathDepth(string nestedPath, int depth)
        {
            string[] pathSegments = nestedPath.Replace("../", "").Split("/");

            if (depth >= 0 && depth < pathSegments.Length - 1)
            {
                string[] extractedSegments = new string[depth + 1];
                Array.Copy(pathSegments, extractedSegments, depth + 1);
                return string.Join(Path.DirectorySeparatorChar.ToString(), extractedSegments);
            }
            else
            {
                return Path.GetDirectoryName(nestedPath.Replace("../", "")); // Invalid depth or path has fewer segments
            }
        }

    }
}