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
using System.Reflection;
using System.Net.Sockets;
using System.Collections;
//System.Net
using System.Data.OleDb;
using System.IO;
using System.Globalization;


namespace ExcelReader
{
    public partial class MainForm : Form
    {

        private ExcelFile file;
        private Scan scan;
        private string fileName;
        private ToolStripProgressBar progressBar;

        public MainForm()
        {
            InitializeComponent();
            
            this.Text = Scan.GetLocalIPAddress();

//            toolStripProgressBar1.ToolTipText = "111";
            this.CenterToScreen();
            //            fileName = "c:\\Users\\IKotvytskyi\\Documents\\test1.xlsx";
            // fileName = "c:\\Users\\IKotvytskyi\\Desktop\\4 Додаток 3_на 01.01.18_нерух.xlsb" ;
            file = new ExcelFile();
            file.onSheetChoise += this.getXlsSheet; // method for a excel sheet choice
            file.onStep += this.stepProgressBar;
            scan = new Scan();
            scan.onShowMessage += this.showStripMessage;
            scan.onInitProgressBar += this.initProgressBar;
            scan.onStepProgressBar += this.stepProgressBar;
            scan.onHideProgressBar += this.hideProgressBar;
            progressBar = toolStripProgressBar2;
            tabControl1.TabPages.Remove(tabPage1);
            //ArrayList elements = new ArrayList();
            foreach (dataType dataType in Enum.GetValues(typeof(dataType)))
            {
                dataTypeDataGrid3.Items.Add(dataType.ToString());
            }
            //dataTypeDataGrid3.Items.AddRange(elements.ToArray());
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.W | Keys.Alt:
                    changeContainerWidth(splitContainer1);
                    return false;

                case Keys.D | Keys.Alt:
                    if (dgvTemlpHead.CurrentCell.RowIndex >= 0)
                    {
                        string msg = String.Format("Копируем шаблон \"{0} -- {1}\"?",
                            dgvTemlpHead.Rows[dgvTemlpHead.CurrentCell.RowIndex].Cells["dgvTemplName"].Value,
                            dgvTemlpHead.Rows[dgvTemlpHead.CurrentCell.RowIndex].Cells["dgvTemplComm"].Value
                            );
                        DialogResult result = MessageBox.Show(msg, "Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            templCopy();
                        }
                    }
                    return false;
                case Keys.S | Keys.Control:
                    bindingNavigatorSaveItems1_Click(this);
                    bindingNavigatorSaveItems2_Click(this);
                    MessageBox.Show("Изменения в шаблонах сохранены!");

                    return false;

            }
            return false;
        }
        
        private void Open_Click(object sender, EventArgs e) 
        {
            //            string fileName  = "c:\\Users\\IKotvytskyi\\Documents\\test.xlsx";

            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Excel Files|*.xls;*.xlsx;*.xlsb;*.xlsx";
            openFileDialog1.Title = "Select a Excel file";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.  

                fileName = openFileDialog1.FileName;
                this.Text = String.Format("{0} -- Convert: {1}", Scan.GetLocalIPAddress(), Path.GetFileName(fileName));
                file.ReadFile(fileName);
                DataTable table = file.XlsTable;
                int columns = table.Columns.Count;
                DataColumn col = table.Columns.Add("ROW_ID", typeof(Int32));
                col.SetOrdinal(0);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i]["ROW_ID"] = i;
                }
                col = table.Columns.Add("Check_Result", typeof(String));
                col.SetOrdinal(1);

                dataGridView1.DataSource = table;

                //foreach (DataGridViewRow row in dataGridView1.Rows)
                //{
                //    row.HeaderCell.Value = String.Format("{0}", row.Index + 1);
                //}

                string[] enabledBtn = {
                    "ButtonOpen",
                    "ButtonMatching",
                    //"BottomProcessing",
                    //"ButtonStore"
                };
                activatStripMenu(enabledBtn);
                tabControl1.TabPages.Remove(tabPage1);
                textBox1.Text = String.Empty;
                MessageBox.Show("ok");
            }
        }

        private void activatStripMenu(string[] enableBtn)
        {
            foreach (ToolStripButton btn in toolStrip1.Items)
            {
                if (enableBtn.Contains(btn.Name))
                {
                    btn.Enabled = true;
                }
                else
                {
                    btn.Enabled = false;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //  данная строка кода позволяет загрузить данные в таблицу "collectDataSet.attrValue". При необходимости она может быть перемещена или удалена.
            this.attrValueTableAdapter.Fill(this.collectDataSet.attrValue);
            //данная строка кода позволяет загрузить данные в таблицу "collectDataSet.i_tmpl_str". 
            //При необходимости она может быть перемещена или удалена.
            this.i_tmpl_strTableAdapter.Fill(this.collectDataSet.i_tmpl_str);
            //данная строка кода позволяет загрузить данные в таблицу "collectDataSet.i_tmpl_head". 
            //При необходимости она может быть перемещена или удалена.
            this.i_tmpl_headTableAdapter.Fill(this.collectDataSet.i_tmpl_head);
        }

        private void bindingNavigatorSaveItems1_Click(object sender, EventArgs e = null)
        {
            this.Validate();
            this.itmplheadBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.collectDataSet);
        }

        private void bindingNavigatorSaveItems2_Click(object sender, EventArgs e = null)
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
            return dgvTemlpStr.Columns[name + gridPrefix].Index; ; 
        }

        private int getGridValue(string name,int rowIndex, DataGridView dgv)
        {
            string gridPrefix = "DataGrid3";
            return dgvTemlpStr.Columns[name + gridPrefix].Index; ;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DataTable result = scan.ResTable;

            if (result != null)
            {
                file.ExportToXls(result,scan.printAllFields);
            } else
            {
                MessageBox.Show("Сопоставьте входной файл с шаблона сначала!");
            }
        }

        private void matching_Click(object sender, EventArgs e) 
        {

            if (file.XlsTable == null) {
                MessageBox.Show("Сперва загрузите теблицу!");
                return;
            }
            scan.Clear();

            dgvTemlpStr.Sort(dgvTemlpStr.Columns[getGridIndex("npp")],ListSortDirection.Ascending);

            for (int i = 0; i < dgvTemlpStr.Rows.Count-1; i++) {
                scan.AddField(
                    npp: (short)dgvTemlpStr.Rows[i].Cells[getGridIndex("npp")].Value,
                    resName: dgvTemlpStr.Rows[i].Cells[getGridIndex("resName")].Value.ToString(),
                    xlsName: dgvTemlpStr.Rows[i].Cells[getGridIndex("xlsName")].Value.ToString(),
                    isPrint: (bool)dgvTemlpStr.Rows[i].Cells[getGridIndex("isPrint")].Value,
                    type: dgvTemlpStr.Rows[i].Cells[getGridIndex("dataType")].Value.ToString(),
                    size: (short)dgvTemlpStr.Rows[i].Cells[getGridIndex("dataSize")].Value,
                    attr:  (attrName)dgvTemlpStr.Rows[i].Cells[getGridIndex("attr")].Value,
                    isActive:  (bool)dgvTemlpStr.Rows[i].Cells[getGridIndex("IsActive")].Value
                    );
            }

            textBox1.Text = scan.Matching(file.XlsTable);

            if (!scan.AllFound)
            {
                MessageBox.Show("Не все поля их шаблона найдены!\n Смотрите подробный отчет");
                tabControl1.TabPages.Remove(tabPage1);
                return;
            }

            scan.InitXlsFields();
            scan.ChechFields();
            activatStripMenu(
                new string[]  {
                "ButtonOpen",
                "ButtonMatching",
                "BottomProcessing",
                //"ButtonStore"
                }
            );

            dataGridView4.Columns.Clear();
            dataGridView4.DataSource = scan.ResTable;
            dataGridView4.Show();

            if (!tabControl1.TabPages.Contains(tabPage1))
            {
                tabControl1.TabPages.Insert(0, tabPage1);
                tabControl1.SelectedIndex = 0;
            }
        }

        private void processing_Click(object sender, EventArgs e)
        {
            // String IP = Scan.GetLocalIPAddress();
            if (!tabControl1.TabPages.Contains(tabPage1))
            {
                tabControl1.TabPages.Insert(0, tabPage1);
                tabControl1.SelectedIndex = 0;
            }

            //scan.initResultFromXls(file.XlsTable);

            scan.InitAllField();
            scan.Processing();
            dataGridView4.AutoGenerateColumns = false;
            dataGridView4.Columns.Clear();
            dataGridView4.AutoGenerateColumns = true;
            //dataGridView4.AutoGenerateColumnsChanged;

            //dataGridView4.DataSource = scan.ResTable;


            //scan.WriteResult(file.XlsTable);

            showStripMessage("");

            activatStripMenu(
                new string[]  {
                "ButtonOpen",
                "ButtonMatching",
                "BottomProcessing",
                "ButtonStore"
                }
            );


            MessageBox.Show("ok");
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
        
        private void dgvTemlpStr_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex == dgvTemlpStr.NewRowIndex)
            {
                if (e.RowIndex > 0) {
                    int npp = 0;
                    int tmpNpp = 0;
                    for (int i = 1; i < e.RowIndex - 1; i++) {
                        tmpNpp = Int32.Parse(dgvTemlpStr.Rows[i].Cells[getGridIndex("npp")].Value.ToString());
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

        private void dgvTemlpStr_DataError(object sender, DataGridViewDataErrorEventArgs anError)
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


        private void dgvTemlpStr_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = dgvTemlpStr;
            if (Control.ModifierKeys == Keys.Control)
            {
                if ((grid.Rows[e.RowIndex].Cells[e.ColumnIndex] != null) 
                    & (grid.Rows[e.RowIndex].Cells[e.ColumnIndex] != null) )
                {
                    string funcName = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    funcName = funcName.Split('(')[0];
                    List<string[]> res = SQLFunction.getDescription(funcName);
                    string message = "";
                    for (int i = 0; i < res[0].Length; i++)
                    {
                        message += String.Format("\n{0}\t\t{1}",res[0][i], res[1][i]);
                    }
                    
                    Clipboard.SetText(String.Format("{0}\n{1}", funcName, message));
                    this.fKimpHeadimpStrBindingSource.EndEdit();
                    grid.EndEdit();
                    MessageBox.Show("Данные скопированы в буфер");
                }
                //                MessageBox.Show("Mouse");
            }
        }

        private void dgvTemlpStr_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //MessageBox.Show(e.ToString());
        }

        private void dgvTemlpStr_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(e.ToString());
        }


        private DataTable joinTable(DataTable tab0, DataTable tab1) {
            tab0.Columns.Add("Row_Id", typeof(Int32));
            for (int i = 0; i < tab0.Rows.Count; i++)
            {
                tab0.Rows[i]["Row_id"] = i;
            }

            var c = from t0 in tab0.AsEnumerable()
                    join t1 in tab1.AsEnumerable() on t0.Field<int>("Row_Id") equals t1.Field<int>("Row_Id")
                    select new {
                        Row_Id =
                            t0.Field<int>("Row_Id")
                    };

            DataTable table = ConvertToDataTable(c);

            //table = ConvertToDataTable(c);

            return table;
        }

        #region PorgressBar

        public void initProgressBar(int maximum)
        {
            progressBar.Minimum = 1;
            progressBar.Maximum = maximum;
            progressBar.Value = 1;
            progressBar.Step = 1;
            progressBar.Visible = true;
        }

        public void hideProgressBar()
        {
            progressBar.Visible = false;
        }

        public void stepProgressBar(){
            progressBar.PerformStep();
        }

        #endregion

        #region LinqDatatable
        //For LinQ Application
        public DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names   
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow   
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
        #endregion

        private void dgvTemlpStr_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void dgvTemlpStr_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_DefaultValuesNeeded(object sender,
            System.Windows.Forms.DataGridViewRowEventArgs e)
        {
            e.Row.Cells["attr"].Value = 0;
        }

        public void showStripMessage(string message)
        {
            toolStripStatusLabel1.Visible = true;
            if (message == "")
            {
                toolStripStatusLabel1.Text = "";
            } else
            {
                toolStripStatusLabel1.Text = message;
                this.Update();
            }
        }

        private void toolCopy_Click(object sender, EventArgs e)
        {

            //dataGridView2.Update();
            //dataGridView2.Refresh();
            int res = templCopy();

            MessageBox.Show(res.ToString());

        }

        private int templCopy()
        {
            int res = 0;
            if (dgvTemlpHead.Rows[dgvTemlpHead.CurrentCell.RowIndex].Cells["dgvTemplIdHead"] != null)
            {
                int IdHead = (int)dgvTemlpHead.Rows[dgvTemlpHead.CurrentCell.RowIndex].Cells["dgvTemplIdHead"].Value;
                res = SQLFunction.copyTemlpate(IdHead);
                Form1_Load(this, null);
                int newRow = dgvTemlpHead.Rows.Count;
                dgvTemlpHead.CurrentCell = dgvTemlpHead.Rows[newRow - 2].Cells[0];
            }
            return res;
        }

        class my1 {
            public override string ToString()
            {
                return "My object";
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            string[] formats = { "yyyyMMdd", "HHmmss" };

            string dateString =  "20130816" ;

            DateTime parsedDate;

            DateTime.TryParseExact(dateString, formats, null,
                                                DateTimeStyles.AllowWhiteSpaces |
                                                DateTimeStyles.AdjustToUniversal,
                                                out parsedDate);

            my1 my1 = new my1();

            object myDate = my1;

            MessageBox.Show(parsedDate.ToString() + " -- " + ((object)myDate).ToString());

        }

        private void toolStripButton1_Click_2(object sender, EventArgs e)
        {
            int col = 174;
            col--;
            int diff = 64;
            int longht = 26;
            int num1 = col / longht;
            int num2 = col % longht;

            string res = (num1 > 0) ? String.Format("{0}{1}", (char)(num1 + diff), (char)(num2 + diff+1)) : String.Format("{0}", (char)(num2 + diff+1));

            MessageBox.Show(res);

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dgvTemlpStr.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvTemlpStr.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }
    }
}
