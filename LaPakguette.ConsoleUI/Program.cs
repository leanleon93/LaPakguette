using LaPakguette.Lib;
using System;

namespace LaPakguette.ConsoleUI
{
    class Program
    {
        private static readonly string _unrealPakPath = @"C:\Program Files\Epic Games\UE_4.25\Engine\Binaries\Win64\UnrealPak.exe";
        private static readonly string _pakPath = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak_F_LP_171-WindowsNoEditor.pak";
        static void Main(string[] args)
        {
            var pakHandler = new PakHandler(_unrealPakPath, _pakPath);
            //pakHandler.Unpack();
            pakHandler.Repack(true);
        }
    }
}
