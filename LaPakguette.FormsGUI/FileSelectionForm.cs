using LaPakguette.PakLib.Models;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LaPakguette.FormsGUI
{
    public partial class FileSelectionForm : Form
    {
        private Pak _pak;
        private MainForm _parent;
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
            if(pak != null)
            {
                _pak = pak;
                filesTreeView.Nodes.Clear();
                var allFiles = _pak.GetAllFilenames();
                filesTreeView.Nodes.Add(PopulateTreeNode(allFiles.ToArray(), "/"));
            }
        }
        private TreeNode PopulateTreeNode(string[] paths, string pathSeparator)
        {
            if (paths == null)
                return null;

            TreeNode thisnode = new TreeNode();
            TreeNode currentnode;
            char[] cachedpathseparator = pathSeparator.ToCharArray();
            foreach (string path in paths)            {
                currentnode = thisnode;
                foreach (string subPath in path.Split(cachedpathseparator))
                {
                    if (null == currentnode.Nodes[subPath])
                        currentnode = currentnode.Nodes.Add(subPath, subPath);
                    else
                        currentnode = currentnode.Nodes[subPath];                   
                }
            }

            return thisnode;
        }


        private string _rightClickedPath = "";
        private void filesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                filesTreeView.SelectedNode = e.Node;
                _rightClickedPath = e.Node.FullPath.TrimStart('/');
                contextMenuStrip1.Items[1].Text = Path.HasExtension(_rightClickedPath) ? "Replace..." : "Add to Directory...";
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            toolStripStatusLabel1.Text = "Exporting: /" + _rightClickedPath;
            statusStrip1.Refresh();
            
            if (Path.HasExtension(_rightClickedPath))
            {
                saveFileDialog.FileName = Path.GetFileName(_rightClickedPath);
                saveFileDialog.DefaultExt = Path.GetExtension(_rightClickedPath);
                saveFileDialog.Filter = $"(*.{saveFileDialog.DefaultExt})|*.{saveFileDialog.DefaultExt}|All files (*.*)|*.*";
                var result = saveFileDialog.ShowDialog();
                if(result == DialogResult.OK)
                {
                    var file = _pak.GetFile(_rightClickedPath);
                    var filePath = saveFileDialog.FileName;
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    System.IO.File.WriteAllBytes(filePath, file.Data);
                }
            }
            else
            {
                var allFilenames = _pak.GetAllFilenames();
                var filenames = allFilenames.Where(x => x.StartsWith(_rightClickedPath)).ToList();
                var files = _pak.GetFiles(filenames);
                var unpackDir = _pak.PakPath.Replace(Regex.Match(_pak.PakPath, @"\..*").Value, "");
                foreach(var file in files)
                {
                    var filePath = Path.Combine(unpackDir, file.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    System.IO.File.WriteAllBytes(filePath, file.Data);
                }
            }
            
            toolStripStatusLabel1.Text = "Ready";
            statusStrip1.Refresh();
        }

        private void replaceToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (!Path.HasExtension(_rightClickedPath))
            {
                var result = openFileDialog.ShowDialog();
                if(result == DialogResult.OK)
                {
                    toolStripStatusLabel1.Text = "Adding file to: /" + _rightClickedPath + "...";
                    statusStrip1.Refresh();
                    var filePath = openFileDialog.FileName;
                    if(filePath == "") return;

                    if (System.IO.File.Exists(filePath))
                    {
                        _pak.AddOrReplaceFile(new PakFileEntry(Path.Combine(_rightClickedPath, Path.GetFileName(filePath)).Replace("\\", "/"), File.ReadAllBytes(filePath)));
                        var (pakPath, compress, encrypt, encryptIndex, compressionMethod) = _parent.GetRepackSettings();
                        var buffer = _pak.ToByteArray(compress, encrypt, encryptIndex, compressionMethod);
                        System.IO.File.WriteAllBytes(pakPath, buffer);
                        _parent.ReloadPak(false, true);
                    }
                }
            }
            else
            {
                var result = openFileDialog.ShowDialog();
                if(result == DialogResult.OK)
                {
                    toolStripStatusLabel1.Text = "Replacing: /" + _rightClickedPath + "...";
                    statusStrip1.Refresh();
                    var filePath = openFileDialog.FileName;
                    if(filePath == "") return;
                    if (System.IO.File.Exists(filePath))
                    {
                        _pak.ReplaceFile(_rightClickedPath, System.IO.File.ReadAllBytes(filePath));
                        var (pakPath, compress, encrypt, encryptIndex, compressionMethod) = _parent.GetRepackSettings();
                        var buffer = _pak.ToByteArray(compress, encrypt, encryptIndex, compressionMethod);
                        System.IO.File.WriteAllBytes(pakPath, buffer);
                        _parent.ReloadPak(false, true);
                    }
                }
            }
            toolStripStatusLabel1.Text = "Ready";
            statusStrip1.Refresh();
        }

        private void removeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            toolStripStatusLabel1.Text = "Removing: /" + _rightClickedPath + "...";
            statusStrip1.Refresh();
            if (!Path.HasExtension(_rightClickedPath))
            {
                var allFilenames = _pak.GetAllFilenames();
                foreach(var file in allFilenames)
                {
                    if (file.StartsWith(_rightClickedPath))
                    {
                        _pak.RemoveFile(file);
                    }
                }
            }
            else
            {
                _pak.RemoveFile(_rightClickedPath);
            }
            var (pakPath, compress, encrypt, encryptIndex, compressionMethod) = _parent.GetRepackSettings();
            var buffer = _pak.ToByteArray(compress, encrypt, encryptIndex, compressionMethod);
            System.IO.File.WriteAllBytes(pakPath, buffer);
            _parent.ReloadPak(false, true);
            toolStripStatusLabel1.Text = "Ready";
            statusStrip1.Refresh();
        }
    }
}
