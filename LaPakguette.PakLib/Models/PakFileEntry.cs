using System.IO;

namespace LaPakguette.PakLib.Models
{
    public class PakFileEntry
    {
        public PakFileEntry(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        public string Name { get; }
        public byte[] Data { get; }
        public void SaveToFile(string outFolder)
        {
            var outPath = Path.Combine(outFolder, Name);
            File.WriteAllBytes(outPath, Data);
        }
    }
}