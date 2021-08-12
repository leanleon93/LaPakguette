namespace LaPakguette.PakLib.Models
{
    public class PakFileEntry
        {
            public PakFileEntry(string name, byte[] data)
            {
                Name = name;
                Data = data;
            }
            public string Name { get; private set; }
            public byte[] Data { get; private set; }
        }
}
