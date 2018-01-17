using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            this.CenterToScreen();
            //            fileName = "c:\\Users\\IKotvytskyi\\Documents\\test1.xlsx";
            fileName = "c:\\Users\\IKotvytskyi\\Desktop\\4 Додаток 3_на 01.01.18_нерух.xlsb" ;
            textBox1.Text = "Matching!";
            file = new ExcelFile();
            file.onSheetChoise += this.getXlsSheet; // method for a excel sheet choice
            scan = new Scan();
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
                this.Text = "Convert: " + Path.GetFileName(fileName);
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

        private void splitter2_DoubleClick(object sender, EventArgs e)
        {

            const float width = 0.8F;

            if (splitContainer1.SplitterDistance == (int)(splitContainer1.Width * width))
            {
                splitContainer1.SplitterDistance = (int)(splitContainer1.Width * (1- width));
            }
            else {
                splitContainer1.SplitterDistance = (int)(splitContainer1.Width * width);
            }
        }

        private int getGridIndex(string name) {
            string gridPrefix = "DataGrid3";
            return dataGridView3.Columns[name + gridPrefix].Index; ; 
        }

        private void toolStripButton2_Click(object sender, EventArgs e) 
        {
            // Matching 
            scan.Clear();

            string msg = String.Empty;
            for (int i = 0; i < dataGridView3.Rows.Count-1; i++) {
                scan.Add(new Field {
                    ResName = dataGridView3.Rows[i].Cells[getGridIndex("resName")].Value.ToString(),
                    XlsName = dataGridView3.Rows[i].Cells[getGridIndex("xlsName")].Value.ToString(),
                    IsPrint = (bool)dataGridView3.Rows[i].Cells[getGridIndex("isPrint")].Value,
                    Attr = (byte)dataGridView3.Rows[i].Cells[getGridIndex("attr")].Value,
                    IsActive = (bool)dataGridView3.Rows[i].Cells[getGridIndex("IsActive")].Value
                });
            }
            if (file.XlsTable != null) {
                foreach (DataColumn column in file.XlsTable.Columns) {
                    foreach (Field field in scan) {
                        if (field.Attr != 0) continue;
                        if (field.XlsName == column.ColumnName) {
                            field.IsExist = true;
                            break;
                        }
                    }
                }
            }
            textBox1.Hide();
            file.InitResTable(scan);
            dataGridView4.DataSource = file.ResTable;
            dataGridView4.Show();
            file.WriteResult(scan);
            MessageBox.Show("Ready!\n" + scan.AllFound().ToString() );
        }

        private void dataGridView1_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (((DataGridView)sender).Name == "dataGridView1") {
                string name = dataGridView1.Columns[e.ColumnIndex].Name;
                Clipboard.SetText(name);
                MessageBox.Show(String.Format("String \'{0}\' has been copied to clipboard",name));
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            file.CreateXls();
        }

        private void dataGridView3_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex == dataGridView3.NewRowIndex)
            {
                if (e.RowIndex > 0) {
                    int npp = 0;
                    int tmpNpp = 0;
                    for (int i = 1; i < e.RowIndex - 1; i++) {
                        tmpNpp = Int32.Parse(dataGridView3.Rows[i].Cells[2].Value.ToString());
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

    }
}
