using LaPakguette.Lib;
using System;

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
        
        static void Main(string[] args)
        {
            var pakHandler = new PakHandler(_unrealPakPath);
            pakHandler.Unpack(_livepak3);
            pakHandler.Repack(_livepak3, true, false, true);
        }
    }
}
