using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelReader
{
    public static class FormPrompt
    {

        public static answer ShowDialog(string label, string caption,string message = "")
        {
            int btPosWith = 100;
            int btPosTop = 100;
            int btPosStart = 50;

            bool isSave = false;
            bool isCancel = false;

            Form prompt = new Form()
            {
                Width = 500,
                Height = 200
            };

            prompt.Text = caption;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = label };
            //NumericUpDown inputBox = new NumericUpDown() { Left = 50, Top = 50, Width = 400 };
            TextBox inputBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button btnOk = new Button()
                {
                    Text = "Ok",
                    Left = btPosStart,
                    Width = btPosWith,
                    Top = btPosTop
                };

            Button btnSave = new Button()
                {
                    Text = "Save",
                    Left = 2*btPosStart + btPosWith,
                    Width = btPosWith,
                    Top = btPosTop
                };

            Button btnCancel = new Button()
                {
                    Text = "Cancel",
                    Left = 3 * btPosStart + 2 * btPosWith,
                    Width = btPosWith,
                    Top = btPosTop
                };

            inputBox.Text = message;
            inputBox.BorderStyle = BorderStyle.FixedSingle;
            btnOk.Click += (sender, e) => { prompt.Close(); };
            btnSave.Click += (sender, e) => { isSave = true; prompt.Close(); };
            btnCancel.Click += (sender, e) => { isCancel = true; prompt.Close(); };
            prompt.Controls.Add(btnSave);
            prompt.Controls.Add(btnOk);
            prompt.Controls.Add(btnCancel);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.StartPosition = FormStartPosition.CenterParent;
            prompt.FormBorderStyle = FormBorderStyle.FixedSingle;
            prompt.ShowDialog();
            return new answer { message = inputBox.Text, save = isSave , cancel = isCancel};
        }

        public struct answer
        {
            public string message;
            public bool save;
            public bool cancel;
        }
    }
}
