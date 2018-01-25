using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

using System.Net;
using System.Net.Sockets;
//System.Net
using System.Data.OleDb;
using System.IO;


namespace ExcelReader
{
    public partial class MainForm : Form
    {

        private ExcelFile file;
        private Scan scan;
        private string fileName;

        public MainForm()
        {
            InitializeComponent();
//            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Text = GetLocalIPAddress();
            toolStripProgressBar1.ToolTipText = "111";
            this.CenterToScreen();
            //            fileName = "c:\\Users\\IKotvytskyi\\Documents\\test1.xlsx";
            // fileName = "c:\\Users\\IKotvytskyi\\Desktop\\4 Додаток 3_на 01.01.18_нерух.xlsb" ;
            file = new ExcelFile();
            file.onSheetChoise += this.getXlsSheet; // method for a excel sheet choice
            file.onStep += this.stepProgressBar;
            scan = new Scan();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.W | Keys.Alt:
                    changeContainerWidth(splitContainer1);
                    //Process action here.
                    return false;
            }
            return false;
        }
        
        private void Open_Click(object sender, EventArgs e) 
        {
            //            string fileName  = "c:\\Users\\IKotvytskyi\\Documents\\test.xlsx";


            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Excel Files|*.xlsx;*.xlsb;*.xlsx";
            openFileDialog1.Title = "Select a Excel file";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.  

                fileName = openFileDialog1.FileName;
                this.Text = String.Format("{0} -- Convert: {1}", GetLocalIPAddress(), Path.GetFileName(fileName));
                file.ReadFile(fileName);
                DataTable table = file.XlsTable;
                int columns = table.Columns.Count;
                dataGridView1.DataSource = table;
                MessageBox.Show("ok");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //данная строка кода позволяет загрузить данные в таблицу "collectDataSet.i_tmpl_str". 
            //При необходимости она может быть перемещена или удалена.
            this.i_tmpl_strTableAdapter.Fill(this.collectDataSet.i_tmpl_str);
            //данная строка кода позволяет загрузить данные в таблицу "collectDataSet.i_tmpl_head". 
            //При необходимости она может быть перемещена или удалена.
            this.i_tmpl_headTableAdapter.Fill(this.collectDataSet.i_tmpl_head);
        }

        private void bindingNavigatorSaveItems1_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.itmplheadBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.collectDataSet);
        }

        private void bindingNavigatorSaveItems2_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.fKimpHeadimpStrBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.collectDataSet);
        }

        private void changeContainerWidth(SplitContainer container) {
            const float width = 0.8F;

            if (container.SplitterDistance == (int)(container.Width * width))
            {
                container.SplitterDistance = (int)(container.Width * (1 - width));
            }
            else
            {
                container.SplitterDistance = (int)(container.Width * width);
            }
        }

        private int getGridIndex(string name) {
            string gridPrefix = "DataGrid3";
            return dataGridView3.Columns[name + gridPrefix].Index; ; 
        }

        private void toolStripButton2_Click(object sender, EventArgs e) 
        {

            if (file.XlsTable == null) {
                MessageBox.Show("Сперва загрузите теблицу!");
                return;
            }
            scan.Clear();

            string msg = String.Empty;
            dataGridView3.Sort(dataGridView3.Columns[getGridIndex("npp")],ListSortDirection.Ascending);

            for (int i = 0; i < dataGridView3.Rows.Count-1; i++) {
                scan.Add(new Field {
                    ResName = dataGridView3.Rows[i].Cells[getGridIndex("resName")].Value.ToString(),
                    XlsName = dataGridView3.Rows[i].Cells[getGridIndex("xlsName")].Value.ToString(),
                    IsPrint = (bool)dataGridView3.Rows[i].Cells[getGridIndex("isPrint")].Value,
                    Attr = (byte)dataGridView3.Rows[i].Cells[getGridIndex("attr")].Value,
                    IsActive = (bool)dataGridView3.Rows[i].Cells[getGridIndex("IsActive")].Value
                });
            }
            file.InitResTable(scan);
            dataGridView4.DataSource = file.ResTable;
            textBox1.Text = scan.Matching(file.XlsTable.Columns);
            toolStripProgressBar1.Minimum = 1;
            toolStripProgressBar1.Maximum = file.XlsTable.Rows.Count;
            toolStripProgressBar1.Value = 1;
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Visible = true;
            file.WriteResult(scan);
            toolStripProgressBar1.Visible = false;
//            MessageBox.Show("Got data!");
            dataGridView4.Show();
//            MessageBox.Show("Ready!\n" + scan.AllFound().ToString() );
        }

        private void dataGridView1_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((((DataGridView)sender).Name == "dataGridView1")
                & (e.Button == MouseButtons.Right)) {
                string name = dataGridView1.Columns[e.ColumnIndex].Name;
                Clipboard.SetText(name);
                MessageBox.Show(String.Format("String \'{0}\' has been copied to clipboard",name));
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (file.ResTable != null)
            {
                file.ExportToXls();
            } else
            {
                MessageBox.Show("Сопоставьте входной файл с шаблона сначала!");
            }
        }

        private void dataGridView3_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex == dataGridView3.NewRowIndex)
            {
                if (e.RowIndex > 0) {
                    int npp = 0;
                    int tmpNpp = 0;
                    for (int i = 1; i < e.RowIndex - 1; i++) {
                        tmpNpp = Int32.Parse(dataGridView3.Rows[i].Cells[getGridIndex("npp")].Value.ToString());
                        if (npp < tmpNpp) {
                            npp = tmpNpp;
                        }
                    }
                    npp = ((npp/10) + 1) * 10;
                    ((DataGridView)sender).Rows[e.RowIndex - 1].Cells[getGridIndex("npp")].Value = npp.ToString();

                    // set default value

                    ((DataGridView)sender).Rows[e.RowIndex - 1].Cells[getGridIndex("isPrint")].Value = true;
                    ((DataGridView)sender).Rows[e.RowIndex - 1].Cells[getGridIndex("attr")].Value = 0;
                    ((DataGridView)sender).Rows[e.RowIndex - 1].Cells[getGridIndex("IsActive")].Value = true;

                }
            }
        }

        public string getXlsSheet(object[] sheetList)
        {
            DialogSheets form = new DialogSheets(sheetList);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
            string choosenSheet = form.sheet;
            if (choosenSheet == null) {
                choosenSheet = sheetList[0].ToString();
            }
            form.Dispose();
            MessageBox.Show(choosenSheet);
            return choosenSheet;
        }

        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            MessageBox.Show(anError.Exception.Message);
            anError.ThrowException = false;
            anError.Cancel = true;
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void splitContainer1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Panel");
        }

        private void splitContainer1_DoubleClick(object sender, EventArgs e)
        {
            changeContainerWidth(splitContainer1);
        }

        public void stepProgressBar(){
            toolStripProgressBar1.PerformStep();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = dataGridView3;
            if (Control.ModifierKeys == Keys.Control)
            {
                if ((grid.Rows[e.RowIndex].Cells[e.ColumnIndex] != null) 
                    & (grid.Rows[e.RowIndex].Cells[e.ColumnIndex] != null) )
                {
                    string funcName = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    string res = SQLFunction.getDescription(funcName);
                    Clipboard.SetText(String.Format("{0}\n{1}", funcName, res));
                    this.fKimpHeadimpStrBindingSource.EndEdit();
                    grid.EndEdit();


                    MessageBox.Show("Данные скопированы в буфер");
                }


                //                MessageBox.Show("Mouse");
            }
        }

        

        private void dataGridView3_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //MessageBox.Show(e.ToString());
        }

        private void dataGridView3_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(e.ToString());
        }


        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            String IP = GetLocalIPAddress();
        }

        private void dataGridView3_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void dataGridView3_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex > 0)
            //{
            //    //                if ((byte)dataGridView3.Rows[e.RowIndex].Cells[arrIndex].Value == 1)
            //    int arrIndex = getGridIndex("attr");
            //    int nameIndex = getGridIndex("xlsName");
            //    if (dataGridView3.Rows[e.RowIndex].Cells[arrIndex].Value != null)
            //    {
            //        if ((dataGridView3.Rows[e.RowIndex].Cells[nameIndex].Value != null)
            //             & (dataGridView3.Rows[e.RowIndex].Cells[arrIndex].Value != null))
                          
            //        { 
            //            if (
            //                ((byte)dataGridView3.Rows[e.RowIndex].Cells[arrIndex].Value == 1)
            //                & (e.ColumnIndex == nameIndex)
            //                )
            //            {
            //                String funcName = dataGridView3.Rows[e.RowIndex].Cells[nameIndex].Value.ToString();
            //                string res = SQLFunction.getDescription(funcName);
            //                toolTip1.Show(String.Format("{0}\n{1}", funcName, res), this, 
            //                        Cursor.Position.X- this.DesktopLocation.X, Cursor.Position.Y- this.DesktopLocation.Y,5000);
            //            }
            //        }
            //    }
            //}

        }

        private void dataGridView1_DefaultValuesNeeded(object sender,
            System.Windows.Forms.DataGridViewRowEventArgs e)
        {
            e.Row.Cells["attr"].Value = 0;
        }
    }
}
