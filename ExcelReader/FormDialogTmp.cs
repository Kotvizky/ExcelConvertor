using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ExcelReader
{
    public partial class FormDialogTmp : Form
    {
        public FormDialogTmp(string tableName, string serverName)
        {
            InitializeComponent();
            label1.Text = $"{label1.Text} on {serverName}";
            textBox1.Text = tableName;
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void bDrop_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
