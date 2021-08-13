using LaPakguette.PakLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaPakguette.FormsGUI
{
    public partial class MainForm : Form
    {
        private string _pakPath;
        private readonly string _settingsPath = Directory.GetCurrentDirectory() + "\\LaPakguette.usersettings.json";
        private Pak _pak;

        private Settings _settings;
        private byte[] _aesKey;
        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.compressCheckBox.Checked = true;
            this.encryptIndexCheckBox.Checked = true;
            this.pakPathTextBox.ReadOnly = true;
            if (File.Exists(_settingsPath))
            {
                try
                {
                    _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_settingsPath));
                    _pakPath = _settings.LastSelectedPak;
                    pakPathTextBox.Text = _pakPath;
                }
                catch
                {
                    _settings = new Settings();
                    _aesKey = null;
                    File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(_settings));
                }
            }
            else
            {
                _settings = new Settings();
                _aesKey = null;
                File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(_settings));
            }
            if (!string.IsNullOrEmpty(_settings.AesKeyBase64))
            {
                _aesKey = Convert.FromBase64String(_settings.AesKeyBase64);
            }
            else
            {
                SetAesKeyWithDialog();
            }
            if (!string.IsNullOrEmpty(_pakPath))
            {
                try
                {
                    _pak = Pak.FromFile(_pakPath, _aesKey);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            AllFilesSelectedCheck();
        }

        private void SetAesKeyWithDialog()
        {
            var aesKeyForm = new AesKeyInputForm();
            aesKeyForm.ShowDialog();
            if(aesKeyForm.DialogResult == DialogResult.OK && !string.IsNullOrEmpty(aesKeyForm.AesKeyBase64))
            {
                _aesKey = Convert.FromBase64String(aesKeyForm.AesKeyBase64);
                _settings.AesKeyBase64 = aesKeyForm.AesKeyBase64;
                File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(_settings));
            }
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
                try
                {
                    _pak = Pak.FromFile(_pakPath, _aesKey);
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            AllFilesSelectedCheck();
        }


        private void unPakButton_Click(object sender, EventArgs e)
        {
            Func<bool> action = () => { return _pak.ToFolder(); };
            DoAsyncAction(action, "Unpack");
        }
        private void repakButton_Click(object sender, EventArgs e)
        {
            Func<bool> action = () => { return RepackToFile(); };
            DoAsyncAction(action, "Repack");
        }

        private bool RepackToFile()
        {
            try
            {
                var unpackDir = _pakPath.Replace(Regex.Match(_pakPath, @"\..*").Value, "");
                if(Directory.Exists(unpackDir))
                {
                    var files = Directory.GetFiles(unpackDir).Where(x => Path.GetFileName(x) != Pak.MountpointFileName);
                    var emptyFilenames = new List<string>();
                    foreach(var file in files)
                    {
                        if(new FileInfo(file).Length == 0 )
                        {
                            emptyFilenames.Add(Path.GetRelativePath(unpackDir, file));
                            continue;
                        }
                        var relFilePath = Path.GetRelativePath(unpackDir, file);
                        _pak.AddOrReplaceFile(new PakFileEntry(relFilePath, File.ReadAllBytes(file)));
                    }
                    var existingFiles = _pak.GetAllFilenames();
                    foreach (var existingFile in existingFiles)
                    {
                        if(files.FirstOrDefault(x => Path.GetRelativePath(unpackDir, x) == existingFile) == null)
                        {
                            //File deleted from unpack folder
                            _pak.RemoveFile(existingFile);
                        }
                        if(emptyFilenames.Any() && emptyFilenames.Contains(existingFile))
                        {
                            //File deleted from unpack folder
                            _pak.RemoveFile(existingFile);
                        }
                    }
                }
                var buffer = _pak.ToByteArray(compressCheckBox.Checked, encryptCheckBox.Checked, encryptIndexCheckBox.Checked, GetCompressionMethod());
                File.WriteAllBytes(_pakPath, buffer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private CompressionMethod GetCompressionMethod()
        {
            if(oodleCompression.Checked) return CompressionMethod.Oodle;
            if(zlibCompression.Checked) return CompressionMethod.Zlib;
            return CompressionMethod.None;
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
            var enabled = !string.IsNullOrEmpty(_pakPath) && File.Exists(_pakPath) && _pak != null;
            this.unPakButton.Enabled = enabled;
            this.repakButton.Enabled = enabled;
        }

        private void changeAESKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetAesKeyWithDialog();
        }
    }
}
