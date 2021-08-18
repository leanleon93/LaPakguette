using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LaPakguette.PakLib.Models;

namespace LaPakguette.FormsGUI
{
    public partial class FileSelectionForm : Form
    {
        private Pak _pak;
        private MainForm _parent;


        private string _rightClickedPath = "";

        public FileSelectionForm()
        {
            InitializeComponent();
        }

        public void SetParent(MainForm mainForm)
        {
            _parent = mainForm;
        }

        public void SetPak(Pak pak)
        {
            if (pak != null)
            {
                _pak = pak;
                filesTreeView.BeginUpdate();
                filesTreeView.Nodes.Clear();
                var allFiles = _pak.GetAllFilenames();
                filesTreeView.Nodes.Add(PopulateTreeNode(allFiles.ToArray(), "/"));
                filesTreeView.EndUpdate();
            }
        }

        internal static TreeNode PopulateTreeNode(string[] paths, string pathSeparator)
        {
            if (paths == null)
                return null;

            var thisnode = new TreeNode();
            var cachedpathseparator = pathSeparator.ToCharArray();
            foreach (var path in paths)
            {
                var currentnode = thisnode;
                foreach (var subPath in path.Split(cachedpathseparator)) currentnode = currentnode.Nodes[subPath] ??
                    currentnode.Nodes.Add(subPath, subPath);
            }

            return thisnode;
        }

        private void filesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                filesTreeView.SelectedNode = e.Node;
                _rightClickedPath = e.Node.FullPath.TrimStart('/');
                contextMenuStrip1.Items[1].Text =
                    Path.HasExtension(_rightClickedPath) ? "Replace..." : "Add to Directory...";
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Exporting: /" + _rightClickedPath;
            statusStrip1.Refresh();

            if (Path.HasExtension(_rightClickedPath))
            {
                saveFileDialog.FileName = Path.GetFileName(_rightClickedPath);
                saveFileDialog.DefaultExt = Path.GetExtension(_rightClickedPath);
                saveFileDialog.Filter =
                    $"(*.{saveFileDialog.DefaultExt})|*.{saveFileDialog.DefaultExt}|All files (*.*)|*.*";
                var result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var file = _pak.GetFile(_rightClickedPath);
                    var filePath = saveFileDialog.FileName;
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.WriteAllBytes(filePath, file.Data);
                }
            }
            else
            {
                var allFilenames = _pak.GetAllFilenames();
                var filenames = allFilenames.Where(x => x.StartsWith(_rightClickedPath)).ToList();
                var files = _pak.GetFiles(filenames);
                var unpackDir = _pak.PakPath.Replace(Regex.Match(_pak.PakPath, @"\..*").Value, "");
                foreach (var file in files)
                {
                    var filePath = Path.Combine(unpackDir, file.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.WriteAllBytes(filePath, file.Data);
                }
            }

            toolStripStatusLabel1.Text = "Ready";
            statusStrip1.Refresh();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Path.HasExtension(_rightClickedPath))
            {
                var result = openFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    toolStripStatusLabel1.Text = "Adding file to: /" + _rightClickedPath + "...";
                    statusStrip1.Refresh();
                    var filePath = openFileDialog.FileName;
                    if (filePath == "") return;

                    if (File.Exists(filePath))
                    {
                        _pak.AddOrReplaceFile(new PakFileEntry(
                            Path.Combine(_rightClickedPath, Path.GetFileName(filePath)).Replace("\\", "/"),
                            File.ReadAllBytes(filePath)));
                        var (pakPath, compress, encrypt, encryptIndex, compressionMethod) = _parent.GetRepackSettings();
                        var buffer = _pak.ToByteArray(compress, encrypt, encryptIndex, compressionMethod);
                        File.WriteAllBytes(pakPath, buffer);
                        _parent.ReloadPak(false, true);
                    }
                }
            }
            else
            {
                var result = openFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    toolStripStatusLabel1.Text = "Replacing: /" + _rightClickedPath + "...";
                    statusStrip1.Refresh();
                    var filePath = openFileDialog.FileName;
                    if (filePath == "") return;
                    if (File.Exists(filePath))
                    {
                        _pak.ReplaceFile(_rightClickedPath, File.ReadAllBytes(filePath));
                        var (pakPath, compress, encrypt, encryptIndex, compressionMethod) = _parent.GetRepackSettings();
                        var buffer = _pak.ToByteArray(compress, encrypt, encryptIndex, compressionMethod);
                        File.WriteAllBytes(pakPath, buffer);
                        _parent.ReloadPak(false, true);
                    }
                }
            }

            toolStripStatusLabel1.Text = "Ready";
            statusStrip1.Refresh();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Removing: /" + _rightClickedPath + "...";
            statusStrip1.Refresh();
            if (!Path.HasExtension(_rightClickedPath))
            {
                var allFilenames = _pak.GetAllFilenames();
                foreach (var file in allFilenames)
                {
                    var fileDir = Path.GetDirectoryName(file);
                    var rightClickedDir = Path.GetDirectoryName(_rightClickedPath + "/");
                    if (file.StartsWith(_rightClickedPath + "/") && fileDir == rightClickedDir)
                        _pak.RemoveFile(file);
                }
            }
            else
            {
                _pak.RemoveFile(_rightClickedPath);
            }

            var (pakPath, compress, encrypt, encryptIndex, compressionMethod) = _parent.GetRepackSettings();
            var buffer = _pak.ToByteArray(compress, encrypt, encryptIndex, compressionMethod);
            File.WriteAllBytes(pakPath, buffer);
            _parent.ReloadPak(false, true);
            toolStripStatusLabel1.Text = "Ready";
            statusStrip1.Refresh();
        }
    }
}