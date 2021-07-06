using LaPakguette.PakLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LaPakguette.PakLibTests
{
    //Unit tests used for simple library testing without any ui
    public class AesTests
    { 

        private const string data = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor.pak.new";
        private const string data2 = @"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor.pak.bak";
        private readonly string BASE64_AES_KEY = @"0uX3+U5iXv4nJrU2DBA5zny5q7dgqU83uxWm3Ah0FlY=";
        private byte[] _aesKey;

        [SetUp]
        public void Setup()
        {
            _aesKey = Convert.FromBase64String(BASE64_AES_KEY);
        }

        [Test]
        public void PakParseTest()
        {
            var data = File.ReadAllBytes(@"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor\PakListCheck.txt");
            var enc = AesHandler.EncryptAES(data, _aesKey);
            var result = AesHandler.DecryptAES(File.ReadAllBytes(@"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor\PakListCheck.txt.enc2"), _aesKey);
            File.WriteAllBytes(@"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor\PakListCheck.txt.enc", enc);
            File.WriteAllBytes(@"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor\PakListCheck.txt.new", result);
            
        }

        [Test]
        public void AesDecryptTest()
        {
            byte[] datab;
            using(var fs = new FileStream(data, FileMode.Open))
            {
                using(var br = new BinaryReader(fs))
                {
                    br.BaseStream.Position = 0x7A;
                    datab = br.ReadBytes(16);
                }
            }
            var result = AesHandler.DecryptAES(datab, _aesKey);
            File.WriteAllBytes(@"C:\Program Files (x86)\NCSOFT\BNSR\Content\Paks\Pak0-UFS_P-WindowsNoEditor\mp2.txt.new", result);
        }
    }
}
