using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelReader
{

    public partial class FormFunc : Form
    {

        public string func = String.Empty;

        public FormFunc(DataTable table, string func)
        {
            InitializeComponent();
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                tFunc.Text = row["fnName"].ToString();
                tInPar.Text = row["inPar"].ToString();
                tOutPar.Text = row["outPar"].ToString();
                ttabFields.Text = row["tabFields"].ToString();

                string[] param = func.Split('(');
                if (param.Length > 0) tStrTabFields.Text = param[1].Split(')')[0];
                if (param.Length > 1) tStrInPar.Text = param[2].Split(')')[0];
                if (param.Length > 2) tStrOutPar.Text = param[3].Split(')')[0];

            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const string message =
                "Save fields to DB?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string query = String.Format(
                        "update impFunc set tabFields = '{0}', inPar = '{1}', outPar = '{2}' where fnName = '{3}'",
                        ttabFields.Text, tInPar.Text, tOutPar.Text, tFunc.Text);
                SQLFunction.ExecuteNonQuery(query);
            }

            func = String.Format("{0} \r\n({1})\r\n({2})\r\n({3})", tFunc.Text,
                tStrTabFields.Text, tStrInPar.Text, tStrOutPar.Text);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        //protected override bool ProcessCmdKey(ref Message message, Keys keys)
        //{
        //    switch (keys)
        //    {
        //        case Keys.C | Keys.Alt:
        //            Copy();
        //            return false;
        //        case Keys.V | Keys.Alt:
        //            Paste();
        //            return false;
        //    }
        //    return false;
        //}


        private void Copy()
        {
            Clipboard.SetText(
                String.Format("{0}\n{1}\n{2}",
                ttabFields.Text, tInPar.Text, tOutPar.Text)
            );
        }

        private void Paste()
        {
            string[] param = Clipboard.GetText().Split('\n');
            if (param.Count() == 3)
            {
                ttabFields.Text = param[0];
                tInPar.Text = param[1];
                tOutPar.Text = param[2];
            }

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }
    }
}