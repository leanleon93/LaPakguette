
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
            this.label1 = new System.Windows.Forms.Label();
            this.uePakPathTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.selectUepakpathButton = new System.Windows.Forms.Button();
            this.pakselectButton = new System.Windows.Forms.Button();
            this.pakPathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.unPakButton = new System.Windows.Forms.Button();
            this.repakButton = new System.Windows.Forms.Button();
            this.compressCheckBox = new System.Windows.Forms.CheckBox();
            this.encryptCheckBox = new System.Windows.Forms.CheckBox();
            this.encryptIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path to UnrealPak.exe";
            // 
            // uePakPathTextBox
            // 
            this.uePakPathTextBox.Location = new System.Drawing.Point(32, 48);
            this.uePakPathTextBox.Name = "uePakPathTextBox";
            this.uePakPathTextBox.Size = new System.Drawing.Size(230, 23);
            this.uePakPathTextBox.TabIndex = 1;
            // 
            // selectUepakpathButton
            // 
            this.selectUepakpathButton.Location = new System.Drawing.Point(269, 48);
            this.selectUepakpathButton.Name = "selectUepakpathButton";
            this.selectUepakpathButton.Size = new System.Drawing.Size(75, 23);
            this.selectUepakpathButton.TabIndex = 1;
            this.selectUepakpathButton.Text = "Select";
            this.selectUepakpathButton.UseVisualStyleBackColor = true;
            this.selectUepakpathButton.Click += new System.EventHandler(this.selectUePathButton_click);
            // 
            // pakselectButton
            // 
            this.pakselectButton.Location = new System.Drawing.Point(269, 102);
            this.pakselectButton.Name = "pakselectButton";
            this.pakselectButton.Size = new System.Drawing.Size(75, 23);
            this.pakselectButton.TabIndex = 0;
            this.pakselectButton.Text = "Select";
            this.pakselectButton.UseVisualStyleBackColor = true;
            this.pakselectButton.Click += new System.EventHandler(this.pakselectButton_Click);
            // 
            // pakPathTextBox
            // 
            this.pakPathTextBox.Location = new System.Drawing.Point(32, 102);
            this.pakPathTextBox.Name = "pakPathTextBox";
            this.pakPathTextBox.Size = new System.Drawing.Size(230, 23);
            this.pakPathTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path to .pak";
            // 
            // unPakButton
            // 
            this.unPakButton.Location = new System.Drawing.Point(32, 155);
            this.unPakButton.Name = "unPakButton";
            this.unPakButton.Size = new System.Drawing.Size(312, 36);
            this.unPakButton.TabIndex = 6;
            this.unPakButton.Text = "Unpack";
            this.unPakButton.UseVisualStyleBackColor = true;
            this.unPakButton.Click += new System.EventHandler(this.unPakButton_Click);
            // 
            // repakButton
            // 
            this.repakButton.Location = new System.Drawing.Point(32, 299);
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
            this.compressCheckBox.Location = new System.Drawing.Point(32, 224);
            this.compressCheckBox.Name = "compressCheckBox";
            this.compressCheckBox.Size = new System.Drawing.Size(79, 19);
            this.compressCheckBox.TabIndex = 8;
            this.compressCheckBox.Text = "Compress";
            this.compressCheckBox.UseVisualStyleBackColor = true;
            // 
            // encryptCheckBox
            // 
            this.encryptCheckBox.AutoSize = true;
            this.encryptCheckBox.Location = new System.Drawing.Point(32, 249);
            this.encryptCheckBox.Name = "encryptCheckBox";
            this.encryptCheckBox.Size = new System.Drawing.Size(66, 19);
            this.encryptCheckBox.TabIndex = 9;
            this.encryptCheckBox.Text = "Encrypt";
            this.encryptCheckBox.UseVisualStyleBackColor = true;
            // 
            // encryptIndexCheckBox
            // 
            this.encryptIndexCheckBox.AutoSize = true;
            this.encryptIndexCheckBox.Location = new System.Drawing.Point(32, 274);
            this.encryptIndexCheckBox.Name = "encryptIndexCheckBox";
            this.encryptIndexCheckBox.Size = new System.Drawing.Size(98, 19);
            this.encryptIndexCheckBox.TabIndex = 10;
            this.encryptIndexCheckBox.Text = "Encrypt Index";
            this.encryptIndexCheckBox.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 348);
            this.Controls.Add(this.encryptIndexCheckBox);
            this.Controls.Add(this.encryptCheckBox);
            this.Controls.Add(this.compressCheckBox);
            this.Controls.Add(this.repakButton);
            this.Controls.Add(this.unPakButton);
            this.Controls.Add(this.pakselectButton);
            this.Controls.Add(this.pakPathTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.selectUepakpathButton);
            this.Controls.Add(this.uePakPathTextBox);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "UnrealPak Wrapper GUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox uePakPathTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button selectUepakpathButton;
        private System.Windows.Forms.Button pakselectButton;
        private System.Windows.Forms.TextBox pakPathTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button unPakButton;
        private System.Windows.Forms.Button repakButton;
        private System.Windows.Forms.CheckBox compressCheckBox;
        private System.Windows.Forms.CheckBox encryptCheckBox;
        private System.Windows.Forms.CheckBox encryptIndexCheckBox;
    }
}

