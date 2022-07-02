using System;
using System.IO;
using LaPakguette.PakLib.Models;
using LaPakguette.Lib;

namespace LaPakguette.ConsoleUI
{
    //  Dev tests
    internal class Program
    {
        private static readonly string _livepak3 =
            @"F:\Games\BNS_LIVE\BNSR\Content\Paks\Pak_F_LP_172-WindowsNoEditor.pak";
        private static readonly string _pakFolder = @"F:\Games\BNS_LIVE\BNSR\Content\Paks";
        private static readonly string _outPath = @"..\..\..\..\..\LaPakguette.PakLibTests\TestFiles\result\consoleResults";
        private static readonly string BASE64_AES_KEY = @"0uX3+U5iXv4nJrU2DBA5zny5q7dgqU83uxWm3Ah0FlY=";
        private static byte[] _aesKey;

        private static void Main(string[] args)
        {
            _aesKey = Convert.FromBase64String(BASE64_AES_KEY);
            var pakGroup = PakGroup.FromFolder(@"F:\Games\BNS EU\BnS_UE4\BNSR\Content\Paks", _aesKey);
            var file = pakGroup.GetFileByName("Skill_Trait_Icon_4.uasset");
            //RepackUnencrypted();
            DumpFullFileList();
            //var pak = Pak.FromFile(_livepak3, _aesKey);

            //pak.ToFolder();
            //var repack = pak.ToByteArray(true, true, false, CompressionMethod.Oodle);
            //File.WriteAllBytes(_livepak3 + ".new", repack);
        }

        private static void RepackUnencrypted()
        {
            var path = @"F:\Games\BNS_LIVE\BNSR\Content\Paks\broken\Pak_F_LP_2-WindowsNoEditor.pak";
            var unrealpakPath = @"C:\Program Files\Epic Games\UE_4.25\Engine\Binaries\Win64\UnrealPak.exe";
            var outPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            Directory.CreateDirectory(outPath);
            var pakhelper = new PakHandler(unrealpakPath);
            pakhelper.Unpack(path, outPath);
            pakhelper.Repack(path, true, false, false);
        }

        private static void DumpFullFileList()
        {
            var outPath = Path.GetFullPath(_outPath);
            Directory.CreateDirectory(outPath);
            var group = PakGroup.FromFolder(_pakFolder, _aesKey);
            var allFiles = group.GetAllFilePaths();
            File.WriteAllLines(Path.Combine(outPath, "allFiles.txt"), allFiles);
            var allFilesByPak = group.GetAllFilePathsByPak();
            foreach(var pak in allFilesByPak)
            {
                File.WriteAllLines(Path.Combine(outPath, $"{(Path.GetFileNameWithoutExtension(pak.Key))}.txt"), pak.Value);
            }
            var file = group.GetFileByPath("xml64.dat");
            file.SaveToFile(outPath);
        }
    }
}