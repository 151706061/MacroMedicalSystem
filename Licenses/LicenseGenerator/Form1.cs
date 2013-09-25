using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace  Macro.Licenses.LicenseGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Encrypt_Click(object sender, EventArgs e)
        {
           this.ResultString.Text= LicenseProvider.Encrypt(this.SourceString.Text.Trim()).ToString();
        }

        private void Decrypt_Click(object sender, EventArgs e)
        {
            this.ResultString.Text = LicenseProvider.Decrypt(this.SourceString.Text.Trim()).ToString();
        }
    }
}
