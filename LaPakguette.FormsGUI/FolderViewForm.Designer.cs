
namespace LaPakguette.FormsGUI
{
    partial class FolderViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            treeView1 = new System.Windows.Forms.TreeView();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showMountpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            createModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            createRemovalModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // treeView1
            // 
            treeView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            treeView1.Location = new System.Drawing.Point(12, 12);
            treeView1.Name = "treeView1";
            treeView1.PathSeparator = "/";
            treeView1.Size = new System.Drawing.Size(494, 471);
            treeView1.TabIndex = 0;
            treeView1.BeforeExpand += treeView1_BeforeExpand;
            treeView1.AfterExpand += treeView1_AfterExpand;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { exportToolStripMenuItem, showMountpointToolStripMenuItem, createModToolStripMenuItem, createRemovalModToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(183, 114);
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            exportToolStripMenuItem.Text = "Export";
            exportToolStripMenuItem.Click += exportToolStripMenuItem_Click;
            // 
            // showMountpointToolStripMenuItem
            // 
            showMountpointToolStripMenuItem.Name = "showMountpointToolStripMenuItem";
            showMountpointToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            showMountpointToolStripMenuItem.Text = "Show Details";
            showMountpointToolStripMenuItem.Click += showMountpointToolStripMenuItem_Click;
            // 
            // createModToolStripMenuItem
            // 
            createModToolStripMenuItem.Name = "createModToolStripMenuItem";
            createModToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            createModToolStripMenuItem.Text = "Create Mod";
            createModToolStripMenuItem.Click += createModToolStripMenuItem_Click;
            // 
            // createRemovalModToolStripMenuItem
            // 
            createRemovalModToolStripMenuItem.Name = "createRemovalModToolStripMenuItem";
            createRemovalModToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            createRemovalModToolStripMenuItem.Text = "Create removal Mod";
            createRemovalModToolStripMenuItem.Click += createRemovalModToolStripMenuItem_Click;
            // 
            // FolderViewForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(518, 495);
            Controls.Add(treeView1);
            Name = "FolderViewForm";
            Text = "Pak folder";
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem showMountpointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createModToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createRemovalModToolStripMenuItem;
    }
}