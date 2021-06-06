using LaPakguette.Lib;
using System;

namespace LaPakguette.ConsoleUI
{
    class Program
    {
        private static readonly string _unrealPakPath = @"C:\Program Files\Epic Games\UE_4.25\Engine\Binaries\Win64\UnrealPak.exe";
        private static readonly string _pakPath2 = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-WindowsNoEditor.pak";
        private static readonly string _pakPath2out = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\customout";
        static void Main(string[] args)
        {
            var pakHandler = new PakHandler(_unrealPakPath);
            pakHandler.Unpack(_pakPath2);
            pakHandler.Unpack(_pakPath2, _pakPath2out);
            pakHandler.Repack(_pakPath2, true);
            pakHandler.Create(_pakPath2out, _pakPath2 + ".new", true);
        }
    }
}
