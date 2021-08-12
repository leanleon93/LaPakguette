using LaPakguette.PakLib.Models;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace LaPakguette.PakLibTests
{
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
        public void SetupFromFileTest()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local.pak");
            Assert.DoesNotThrow(() =>
            {
                var pakObj = Pak.FromFile(localPath, _aesKey);
                Assert.NotNull(pakObj);
            });
        }

        [Test]
        public void SetupFromEncryptedFileTest()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local_compress_encrypt.pak");
            Assert.DoesNotThrow(() =>
            {
                var pakObj = Pak.FromFile(localPath, _aesKey);
                Assert.NotNull(pakObj);
            });
        }

        [Test]
        public void SetupFromFolderEqualsFileTest()
        {
            var folderPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local");
            Assert.DoesNotThrow(() =>
            {
                var pakObj = Pak.FromFolder(folderPath, _aesKey);
                Assert.NotNull(pakObj);
                var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local");
                pakObj.ToFolder(outPath);
                Assert.True(UnpackResultEqual(outPath));
                Directory.Delete(outPath, true);
            });
        }

        [Test]
        public void SetupFromBufferTest()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local.pak");
            Assert.DoesNotThrow(() =>
            {
                var buffer = File.ReadAllBytes(localPath);
                var pakObj = Pak.FromBuffer(buffer, _aesKey);
                Assert.NotNull(pakObj);
            });
        }

        [Test]
        public void RepackTestOodle()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local.pak");
            Assert.DoesNotThrow(() =>
            {
                var pakObj = Pak.FromFile(localPath, _aesKey);
                Assert.NotNull(pakObj);
                var repackedBuffer = pakObj.ToByteArray(true, false, false, CompressionMethod.Oodle, _aesKey);
                var comparePath = Path.Combine(_testFileBasePath, @"compare\Pak0-Local_compress.pak");
                var origBuffer = File.ReadAllBytes(comparePath);
                var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local_compress.pak");
                File.WriteAllBytes(outPath, repackedBuffer);
                Assert.True(BuffersAreEqual(repackedBuffer, origBuffer));
                File.Delete(outPath);
            });
        }

        [Test]
        public void RepackTestOodleEncrypted()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local.pak");
            Assert.DoesNotThrow(() =>
            {
                var pakObj = Pak.FromFile(localPath, _aesKey);
                Assert.NotNull(pakObj);
                var repackedBuffer = pakObj.ToByteArray(true, true, true, CompressionMethod.Oodle, _aesKey);
                var comparePath = Path.Combine(_testFileBasePath, @"compare\Pak0-Local_compress_encrypt.pak");
                var origBuffer = File.ReadAllBytes(comparePath);
                var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local_compress_encrypt.pak");
                File.WriteAllBytes(outPath, repackedBuffer);
                Assert.True(BuffersAreEqual(repackedBuffer, origBuffer));
                File.Delete(outPath);
            });
        }

        [Test]
        public void RepackTestZlibEncryptedNoException() //Tested with the unpack zlib test
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local.pak");
            Assert.DoesNotThrow(() =>
            {
                var pakObj = Pak.FromFile(localPath, _aesKey);
                Assert.NotNull(pakObj);
                var repackedBuffer = pakObj.ToByteArray(true, true, true, CompressionMethod.Zlib, _aesKey);
                var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local_zlib.pak");
            });
        }

        [Test]
        public void RepackTestNothing()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local.pak");
            Assert.DoesNotThrow(() =>
            {
                var pakObj = Pak.FromFile(localPath, _aesKey);
                Assert.NotNull(pakObj);
                var repackedBuffer = pakObj.ToByteArray(false, false, false, CompressionMethod.None, _aesKey);
                var comparePath = Path.Combine(_testFileBasePath, @"compare\Pak0-Local_nothing.pak");
                var origBuffer = File.ReadAllBytes(comparePath);
                var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local_nothing.pak");
                File.WriteAllBytes(outPath, repackedBuffer);
                Assert.True(BuffersAreEqual(repackedBuffer, origBuffer));
                File.Delete(outPath);
            });
        }

        [Test]
        public void UnpackTest()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local.pak");
            var pakObj = Pak.FromFile(localPath, _aesKey);
            var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local");
            pakObj.ToFolder(outPath);
            Assert.True(UnpackResultEqual(outPath));
            Directory.Delete(outPath, true);
        }

        [Test]
        public void UnpackZlibTest()
        {
            var localPath = Path.Combine(_testFileBasePath, @"in\Pak0-Local_zlib.pak");
            var pakObj = Pak.FromFile(localPath, _aesKey);
            var outPath = Path.Combine(_testFileBasePath, @"result\Pak0-Local_zlib");
            pakObj.ToFolder(outPath);
            Assert.True(UnpackResultEqual(outPath));
            Directory.Delete(outPath, true);
        }

        private bool UnpackResultEqual(string outPath)
        {
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