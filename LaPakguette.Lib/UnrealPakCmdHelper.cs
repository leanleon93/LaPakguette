using System;
using System.Collections.Generic;
using System.IO;

namespace LaPakguette.Lib
{
    internal class UnrealPakCmdHelper
    {
        private string _unrealPakPath;
        internal readonly string repackFilename = "repack.txt";
        internal UnrealPakCmdHelper(string unrealPakPath)
        {
            _unrealPakPath = unrealPakPath;
        }

        internal bool Unpack(string pakPath, string outDir)
        {
            var args = new string[4];
            args[0] = WrapPath(pakPath);
            args[1] = "-Extract";
            args[2] = WrapPath(outDir);
            var cryptoPath = Directory.GetCurrentDirectory() + "\\Crypto.json";
            args[3] = "-cryptokeys=\"" + cryptoPath + "\"";
            return RunCommand(args);
        }

        internal bool Repack(string inputFolder, string pakOutPath, bool compress, bool encrypt, bool encryptindex)
        {
            var args = new List<string>();
            args.Add(WrapPath(pakOutPath));
            var repackFilepath = inputFolder + "\\" + repackFilename;
            if (!File.Exists(repackFilepath))
            {
                return false;
            }
            args.Add("-Create=" + WrapPath(repackFilepath));
            var cryptoPath = Directory.GetCurrentDirectory() + "\\Crypto.json";
            args.Add("-cryptokeys=\"" + cryptoPath + "\"");
            if (compress)
            {
                args.Add("-compress -compressionformat=\"oodle\"");
            }
            if (encrypt)
            {
                args.Add("-encrypt");
            }
            if (encryptindex)
            {
                args.Add("-encryptindex");
            }
            return RunCommand(args.ToArray());
        }

        private string WrapPath(string path)
        {
            return "\"" + path + "\"";
        }

        private bool RunCommand(string[] args)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(_unrealPakPath),
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = _unrealPakPath,
                Arguments = String.Join(" ", args)
            };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }

    }
}
