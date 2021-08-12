using LaPakguette.PakLib;
using LaPakguette.PakLib.Models;
using System;
using System.IO;

namespace LaPakguette.ConsoleUI
{
    class Program
    {
        //Console UI used for fast debugging without any UI

        private static readonly string _unrealPakPath = @"C:\Program Files\Epic Games\UE_4.25\Engine\Binaries\Win64\UnrealPak.exe";
        private static readonly string _pakPath2 = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor.pak";
        private const string path3 = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-WindowsNoEditor.pak";
        private static readonly string _pakPath2out = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\customout";
        private static readonly string _livepak = @"F:\Games\BNS_LIVE\BNSR\Content\Paks\Pak0-WindowsNoEditor.pak";
        private static readonly string _livepak2 = @"F:\Games\BNS_LIVE\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor.pak";
        private static readonly string _livepak3 = @"F:\Games\BNS_LIVE\BNSR\Content\Paks\Pak_F_LP_172-WindowsNoEditor.pak";
        
        private static readonly string BASE64_AES_KEY = @"0uX3+U5iXv4nJrU2DBA5zny5q7dgqU83uxWm3Ah0FlY=";
        private static byte[] _aesKey;

        static void Main(string[] args)
        {
            _aesKey = Convert.FromBase64String(BASE64_AES_KEY);
            var pak = Pak.FromFile(_livepak3, _aesKey);

            pak.ToFolder();
            var repack = pak.ToByteArray(true, true, false, CompressionMethod.Oodle);
            File.WriteAllBytes(_livepak3 + ".new", repack);
        }
    }
}
