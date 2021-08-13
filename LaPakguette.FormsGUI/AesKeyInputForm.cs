using System;
using System.Windows.Forms;

namespace LaPakguette.FormsGUI
{
    public partial class AesKeyInputForm : Form
    {
        public string AesKeyBase64 { get; set; }
        internal void SetInitText(string text)
        {
            textBox1.Text = text;
        }
        public AesKeyInputForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(textBox1.Text) && IsBase64String(textBox1.Text))
            {
                AesKeyBase64 = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid base64 key.");
            }
        }
        private static bool IsBase64String(string base64)
        {
           Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
           return Convert.TryFromBase64String(base64, buffer , out int bytesParsed);
        }
    }
}
