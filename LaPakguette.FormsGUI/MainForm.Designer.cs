
namespace LaPakguette.FormsGUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pakselectButton = new System.Windows.Forms.Button();
            this.pakPathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.unPakButton = new System.Windows.Forms.Button();
            this.repakButton = new System.Windows.Forms.Button();
            this.compressCheckBox = new System.Windows.Forms.CheckBox();
            this.encryptCheckBox = new System.Windows.Forms.CheckBox();
            this.encryptIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.oodleCompression = new System.Windows.Forms.RadioButton();
            this.zlibCompression = new System.Windows.Forms.RadioButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeAESKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pakselectButton
            // 
            this.pakselectButton.Location = new System.Drawing.Point(268, 57);
            this.pakselectButton.Name = "pakselectButton";
            this.pakselectButton.Size = new System.Drawing.Size(75, 23);
            this.pakselectButton.TabIndex = 0;
            this.pakselectButton.Text = "Select";
            this.pakselectButton.UseVisualStyleBackColor = true;
            this.pakselectButton.Click += new System.EventHandler(this.pakselectButton_Click);
            // 
            // pakPathTextBox
            // 
            this.pakPathTextBox.Location = new System.Drawing.Point(31, 57);
            this.pakPathTextBox.Name = "pakPathTextBox";
            this.pakPathTextBox.Size = new System.Drawing.Size(230, 23);
            this.pakPathTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path to .pak";
            // 
            // unPakButton
            // 
            this.unPakButton.Location = new System.Drawing.Point(31, 86);
            this.unPakButton.Name = "unPakButton";
            this.unPakButton.Size = new System.Drawing.Size(312, 36);
            this.unPakButton.TabIndex = 6;
            this.unPakButton.Text = "Unpack";
            this.unPakButton.UseVisualStyleBackColor = true;
            this.unPakButton.Click += new System.EventHandler(this.unPakButton_Click);
            // 
            // repakButton
            // 
            this.repakButton.Location = new System.Drawing.Point(31, 254);
            this.repakButton.Name = "repakButton";
            this.repakButton.Size = new System.Drawing.Size(312, 34);
            this.repakButton.TabIndex = 7;
            this.repakButton.Text = "Repack";
            this.repakButton.UseVisualStyleBackColor = true;
            this.repakButton.Click += new System.EventHandler(this.repakButton_Click);
            // 
            // compressCheckBox
            // 
            this.compressCheckBox.AutoSize = true;
            this.compressCheckBox.Location = new System.Drawing.Point(31, 151);
            this.compressCheckBox.Name = "compressCheckBox";
            this.compressCheckBox.Size = new System.Drawing.Size(79, 19);
            this.compressCheckBox.TabIndex = 8;
            this.compressCheckBox.Text = "Compress";
            this.compressCheckBox.UseVisualStyleBackColor = true;
            // 
            // encryptCheckBox
            // 
            this.encryptCheckBox.AutoSize = true;
            this.encryptCheckBox.Location = new System.Drawing.Point(31, 176);
            this.encryptCheckBox.Name = "encryptCheckBox";
            this.encryptCheckBox.Size = new System.Drawing.Size(66, 19);
            this.encryptCheckBox.TabIndex = 9;
            this.encryptCheckBox.Text = "Encrypt";
            this.encryptCheckBox.UseVisualStyleBackColor = true;
            // 
            // encryptIndexCheckBox
            // 
            this.encryptIndexCheckBox.AutoSize = true;
            this.encryptIndexCheckBox.Location = new System.Drawing.Point(31, 201);
            this.encryptIndexCheckBox.Name = "encryptIndexCheckBox";
            this.encryptIndexCheckBox.Size = new System.Drawing.Size(98, 19);
            this.encryptIndexCheckBox.TabIndex = 10;
            this.encryptIndexCheckBox.Text = "Encrypt Index";
            this.encryptIndexCheckBox.UseVisualStyleBackColor = true;
            // 
            // oodleCompression
            // 
            this.oodleCompression.AutoSize = true;
            this.oodleCompression.Checked = true;
            this.oodleCompression.Location = new System.Drawing.Point(31, 226);
            this.oodleCompression.Name = "oodleCompression";
            this.oodleCompression.Size = new System.Drawing.Size(55, 19);
            this.oodleCompression.TabIndex = 11;
            this.oodleCompression.TabStop = true;
            this.oodleCompression.Text = "oodle";
            this.oodleCompression.UseVisualStyleBackColor = true;
            // 
            // zlibCompression
            // 
            this.zlibCompression.AutoSize = true;
            this.zlibCompression.Location = new System.Drawing.Point(92, 226);
            this.zlibCompression.Name = "zlibCompression";
            this.zlibCompression.Size = new System.Drawing.Size(43, 19);
            this.zlibCompression.TabIndex = 11;
            this.zlibCompression.Text = "zlib";
            this.zlibCompression.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 307);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(375, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(87, 17);
            this.toolStripStatusLabel1.Text = "For UE4 .pak v9";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(273, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "by LEaN";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(375, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeAESKeyToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "Options";
            // 
            // changeAESKeyToolStripMenuItem
            // 
            this.changeAESKeyToolStripMenuItem.Name = "changeAESKeyToolStripMenuItem";
            this.changeAESKeyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.changeAESKeyToolStripMenuItem.Text = "Change AES-Key";
            this.changeAESKeyToolStripMenuItem.Click += new System.EventHandler(this.changeAESKeyToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 329);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.zlibCompression);
            this.Controls.Add(this.oodleCompression);
            this.Controls.Add(this.encryptIndexCheckBox);
            this.Controls.Add(this.encryptCheckBox);
            this.Controls.Add(this.compressCheckBox);
            this.Controls.Add(this.repakButton);
            this.Controls.Add(this.unPakButton);
            this.Controls.Add(this.pakselectButton);
            this.Controls.Add(this.pakPathTextBox);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LaPakguette";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button pakselectButton;
        private System.Windows.Forms.TextBox pakPathTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button unPakButton;
        private System.Windows.Forms.Button repakButton;
        private System.Windows.Forms.CheckBox compressCheckBox;
        private System.Windows.Forms.CheckBox encryptCheckBox;
        private System.Windows.Forms.CheckBox encryptIndexCheckBox;
        private System.Windows.Forms.RadioButton oodleCompression;
        private System.Windows.Forms.RadioButton zlibCompression;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeAESKeyToolStripMenuItem;
    }
}

