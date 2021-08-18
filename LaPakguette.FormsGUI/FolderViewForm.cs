using LaPakguette.PakLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LaPakguette.FormsGUI
{
    public partial class FolderViewForm : Form
    {
        private PakGroup _pakGroup;
        private TreeNode _dummyNode = new TreeNode("dummyNode");
        private TreeNode _dummyRootNode;
        private List<string> _allFilesWithMP;
        internal void SetPakGroup(PakGroup pakGroup)
        {
            if (pakGroup != null)
            {
                contextMenuStrip1.Items.Clear();
                _pakGroup = pakGroup;
                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                _allFilesWithMP = _pakGroup.GetAllFilePathsWithMP();
                _dummyRootNode = new TreeNode("", new TreeNode[1]{ _dummyNode });
                treeView1.Nodes.Add(_dummyRootNode);
                treeView1.EndUpdate();
                treeView1.Nodes[0].Expand();
            }
        }
        public FolderViewForm()
        {
            InitializeComponent();
        }

        private string _rightClickedPath;

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if(node.Level < 3)
            {
                if(node.Nodes.Count > 0)
                    node.Nodes[0].Expand();
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            treeView1.BeginUpdate();
            TreeNode node = e.Node;
            if (node != null)
            {
                if(node.Nodes.Count == 1 && node.Nodes[0].Text == "dummyNode")
                {
                    node.Nodes.Clear();
                    var level = node.Level;
                    var cachedpathseparator = "/".ToCharArray();
                    var nodeFullPath = node.FullPath.TrimStart('/');
                    string previousSubPath = null;
                    foreach (var path in _allFilesWithMP.Where(x => x.StartsWith(nodeFullPath)))
                    {
                        var currentnode = node;
                        var pathSplit = path.Split(cachedpathseparator);
                        if(pathSplit.Length > level)
                        {
                            var subPath = pathSplit[level];
                            if(previousSubPath != null && previousSubPath == subPath) continue;
                            previousSubPath = subPath;
                            if(!currentnode.Nodes.ContainsKey(subPath))
                            {
                                if(Path.HasExtension(subPath) && pathSplit.Length - 1 == level)
                                {
                                    var newNode = new TreeNode(subPath);
                                    newNode.Name = subPath;
                                    currentnode.Nodes.Add(newNode);
                                }
                                else
                                {
                                    var dummyNode = new TreeNode("dummyNode");
                                    var newNode = new TreeNode(subPath, new TreeNode[1] { dummyNode });
                                    newNode.Name = subPath;
                                    currentnode.Nodes.Add(newNode);
                                }
                            }
                        }
                    }

                }
            }
            treeView1.EndUpdate();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
                _rightClickedPath = e.Node.FullPath.TrimStart('/');
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Path.HasExtension(_rightClickedPath))
            {
                saveFileDialog1.FileName = Path.GetFileName(_rightClickedPath);
                saveFileDialog1.DefaultExt = Path.GetExtension(_rightClickedPath);
                saveFileDialog1.Filter =
                    $"(*.{saveFileDialog1.DefaultExt})|*.{saveFileDialog1.DefaultExt}|All files (*.*)|*.*";
                var result = saveFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var file = _pakGroup.GetFileByPathWithMP(_rightClickedPath);
                    var filePath = saveFileDialog1.FileName;
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.WriteAllBytes(filePath, file.Data);
                }
            }
            else
            {
                var allFilenames = _pakGroup.GetAllFilePathsWithMP();
                var filenames = allFilenames.Where(x => x.StartsWith(_rightClickedPath)).ToList();
                var unpackDir = _pakGroup.Folder;
                foreach(var filename in filenames)
                {
                    var file = _pakGroup.GetFileByPathWithMP(filename);
                    var outfilepath = filename.Replace("../", "").Replace(file.Name, "");
                    var filePath = Path.Combine(unpackDir, outfilepath, file.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.WriteAllBytes(filePath, file.Data);
                }
            }
        }
    }
}
