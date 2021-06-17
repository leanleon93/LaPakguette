using LaPakguette.PakLib.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LaPakguette.PakLibTests
{
    //Unit tests used for simple library testing without any ui
    public class Tests
    {
        private const string path = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor.pak.bak";
        private const string path2 = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor.pak";
        private const string path4 = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor.pak.new";
        private const string path3 = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-WindowsNoEditor.pak";

        private static readonly byte[] AES_KEY = new byte[] { 0x38, 0x32, 0x46, 0xF1, 0xAC, 0x08, 0x11, 0xFC, 0x28, 0xCE, 0x17, 0x1F, 0x3E, 0x63, 0xC9, 0xC9, 0xAB, 0xCE, 0xE1, 0x37, 0xFF, 0x1C, 0xF1, 0xEF, 0x4A, 0x6B, 0x8A, 0x47, 0xBA, 0xB5, 0xAA, 0x7C };


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PakParseTest()
        {
            var pak2 = new Pak(path4, AES_KEY);
            pak2.DumpAllFiles();
            //var pak3 = new Pak(path3, AES_KEY);
            //var files = new List<string>();
            //files.Add(".ini");
            //pak3.DumpFiles(files);
        }

        [Test]
        public void RepakTest()
        {
            var origData = File.ReadAllBytes(path2);
            var pak2 = new Pak(path2, AES_KEY);
            var newData = pak2.ToByteArray(pak2.Footer.CompressionMethods.Length > 0, pak2.Index.Records.Any(x => x.Metadata.IsEncrypted), pak2.IndexEncrypted, new string[1] { "Zlib" }, AES_KEY);
            File.WriteAllBytes(path2 + ".new", newData);
        }
    }
}