namespace LaPakguette.FormsGUI
{
    public class Settings
    {
        public Settings()
        {
            AesKeyBase64 = "";
        }

        public string LastSelectedPak { get; set; }
        public string AesKeyBase64 { get; set; }
    }
}