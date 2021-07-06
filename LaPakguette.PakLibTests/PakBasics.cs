using LaPakguette.PakLib.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace LaPakguette.PakLibTests
{
    //Unit tests used for simple library testing without any ui
    public class Tests
    {
        private string _testFileBasePath;
        private readonly string BASE64_AES_KEY = @"0uX3+U5iXv4nJrU2DBA5zny5q7dgqU83uxWm3Ah0FlY=";
        private byte[] _aesKey;


        [SetUp]
        public void Setup()
        {
            _testFileBasePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\TestFiles\");
            _aesKey = Convert.FromBase64String(BASE64_AES_KEY);
        }

        [Test]
        public void RepackTest()
        {

        }

        [Test]
        public void UnpackTest()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local.pak");
            var pakObj = new Pak(localPath, _aesKey);
            var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local");
            pakObj.DumpAllFiles(outPath);
            Assert.True(CompareUnpackResult());
            Directory.Delete(outPath, true);
        }

        private bool CompareUnpackResult()
        {
            var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local");
            var comparePath = Path.Combine(_testFileBasePath, @"compare\Pak0-Local");
            var outFiles = Directory.GetFiles(outPath);
            var compareFiles = Directory.GetFiles(comparePath);
            if (outFiles.Length != compareFiles.Length) return false;
            foreach(var file in compareFiles)
            {
                var filename = Path.GetFileName(file);
                var outFile = outFiles.ToList().Find(x => Path.GetFileName(x) == filename);
                if (outFile != null)
                {
                    var compFileBuffer = File.ReadAllBytes(file);
                    var outFileBuffer = File.ReadAllBytes(outFile);
                    if (!BuffersAreEqual(compFileBuffer, outFileBuffer)) return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool BuffersAreEqual(byte[] b1, byte[] b2)
        {
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);
    }
}