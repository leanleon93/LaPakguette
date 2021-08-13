using System;
using System.IO;
using LaPakguette.PakLib.Models;

namespace LaPakguette.ConsoleUI
{
    internal class Program
    {
        private static readonly string _livepak3 =
            @"F:\Games\BNS_LIVE\BNSR\Content\Paks\Pak_F_LP_172-WindowsNoEditor.pak";

        private static readonly string BASE64_AES_KEY = @"0uX3+U5iXv4nJrU2DBA5zny5q7dgqU83uxWm3Ah0FlY=";
        private static byte[] _aesKey;

        private static void Main(string[] args)
        {
            _aesKey = Convert.FromBase64String(BASE64_AES_KEY);
            var pak = Pak.FromFile(_livepak3, _aesKey);

            pak.ToFolder();
            var repack = pak.ToByteArray(true, true, false, CompressionMethod.Oodle);
            File.WriteAllBytes(_livepak3 + ".new", repack);
        }
    }
}