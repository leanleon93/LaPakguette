using LaPakguette.Lib;
using LaPakguette.Lib.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaPakguette.FormsGUI
{
    public partial class MainForm : Form
    {
        private string _uePakPath, _pakPath;
        private readonly string _settingsPath = Directory.GetCurrentDirectory() + "\\settings.json";
        private readonly string _cryptoPath = Directory.GetCurrentDirectory() + "\\Crypto.json";
        private PakHandler _pakHandler;

        private Settings _settings;

        public MainForm()
        {
            if (!File.Exists(_cryptoPath))
            {
                MessageBox.Show("Crypto file does not exist.\nCreated a new Crypto.json\nPlease fill with AES key.", "Crypto error");
                var cryptoModel = new CryptoModel()
                {
                    BEnablePakSigning = true,
                    BEnablePakIndexEncryption = true,
                    BEnablePakIniEncryption = true,
                    BEnablePakUAssetEncryption = true,
                    BEnablePakFullAssetEncryption = false,
                    BDataCryptoRequired = true,
                    PakEncryptionRequired = true,
                    PakSigningRequired = true,
                    SecondaryEncryptionKeys = new System.Collections.Generic.List<object>(),
                    Type = "1",
                    SigningKey = null,
                    EncryptionKey = new EncryptionKey
                    {
                        Type = "1",
                        Name = "null",
                        Guid = "null",
                        Key = null
                    },
                    Types = new Types
                    {
                        UnrealBuildToolEncryptionAndSigningCryptoSettingsUnrealBuildToolVersion4000CultureneutralpublicKeyTokennull = "1",
                        UnrealBuildToolEncryptionAndSigningEncryptionKeyUnrealBuildToolVersion4000CultureneutralpublicKeyTokennull = "2",
                        UnrealBuildToolEncryptionAndSigningSigningKeyPairUnrealBuildToolVersion4000CultureneutralpublicKeyTokennull = "3",
                        UnrealBuildToolEncryptionAndSigningSigningKeyUnrealBuildToolVersion4000CultureneutralpublicKeyTokennull = "4"
                    }
                };
                File.WriteAllText(_cryptoPath, JsonConvert.SerializeObject(cryptoModel, new JsonSerializerSettings {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include
                }));
            }
            else
            {
                var cryptoData = JsonConvert.DeserializeObject<CryptoModel>(File.ReadAllText(_cryptoPath));
                if(string.IsNullOrEmpty(cryptoData.EncryptionKey.Key) || cryptoData.EncryptionKey.Key.ToLower() == "null")
                {
                    MessageBox.Show("No encryption key set in Crypto.json", "Crypto error");
                }
            }
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.compressCheckBox.Checked = true;
            this.encryptIndexCheckBox.Checked = true;
            this.uePakPathTextBox.ReadOnly = true;
            this.pakPathTextBox.ReadOnly = true;
            if (File.Exists(_settingsPath))
            {
                try
                {
                    _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsPath));
                    _uePakPath = _settings.UnrealPakPath;
                    if(!string.IsNullOrEmpty(_uePakPath) && File.Exists(_uePakPath))
                    {
                        _pakHandler = new PakHandler(_uePakPath);
                    }
                    _pakPath = _settings.LastSelectedPak;
                    pakPathTextBox.Text = _pakPath;
                    uePakPathTextBox.Text = _uePakPath;
                }
                catch
                {
                    _settings = new Settings();
                }
            }
            else
            {
                _settings = new Settings();
            }
            AllFilesSelectedCheck();
        }

        private void selectUePathButton_click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "UnrealPak.exe";
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                _uePakPath = openFileDialog1.FileName;
                uePakPathTextBox.Text = _uePakPath;
                _settings.UnrealPakPath = _uePakPath;
                File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(_settings));
                try
                {
                    _pakHandler = new PakHandler(_uePakPath);
                }
                catch(Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                
            }
            openFileDialog1.FileName = "";
            AllFilesSelectedCheck();
        }

        private void pakselectButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                _pakPath = openFileDialog1.FileName;
                pakPathTextBox.Text = _pakPath;
                _settings.LastSelectedPak = _pakPath;
                File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(_settings));
            }
            AllFilesSelectedCheck();
        }


        private void unPakButton_Click(object sender, EventArgs e)
        {
            Func<bool> action = () => { return _pakHandler.Unpack(_pakPath); };
            DoAsyncAction(action, "Unpack");
        }
        private void repakButton_Click(object sender, EventArgs e)
        {
            Func<bool> action = () => { return _pakHandler.Repack(_pakPath, compressCheckBox.Checked, encryptCheckBox.Checked, encryptIndexCheckBox.Checked); };
            DoAsyncAction(action, "Repack");
        }

        private async void DoAsyncAction(Func<bool> action, string message)
        {
            this.unPakButton.Enabled = false;
            this.repakButton.Enabled = false;
            bool success = await Task.Run(action);
            DisplaySuccessMessage(message, success);
            this.unPakButton.Enabled = true;
            this.repakButton.Enabled = true;
        }

        private void DisplaySuccessMessage(string message, bool success)
        {
            string caption = "";
            message = message + (success ? " successful" : " failed");
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);
        }

        private void AllFilesSelectedCheck()
        {
            var enabled = !string.IsNullOrEmpty(_uePakPath) && !string.IsNullOrEmpty(_pakPath) && File.Exists(_uePakPath) && File.Exists(_pakPath) && _pakHandler != null;
            this.unPakButton.Enabled = enabled;
            this.repakButton.Enabled = enabled;
        }
    }
}
