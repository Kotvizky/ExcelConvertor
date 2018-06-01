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
using BrightIdeasSoftware;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Web.Script.Serialization;

namespace ExcelReader
{
    public partial class MainForm : Form
    {

        DataTable tbString = new DataTable();

        private ExcelFile file;
        private Scan scan;
        private string fileName;
        private ToolStripProgressBar progressBar;
        private DataSet resDataSet = new DataSet();
        private BindingSource bsXlsTable = null;
        private BindingSource bsResTable = null;
        bool isAdmin = false;
        ProcList procList = null;

        string textMenuTmpTable;
        string TempTableServer;

        public MainForm()
        {
            InitializeComponent();

            //string json;
            //try
            //{
            //    json = @"{""join"":{""res"":""fn.Row_id_inn"",""xls"":""$Row_id""},
            //        ""replace"":[ {""xls"":""№_договора"",""res"":""ContractNumSQL""} ]
            //        }";
            //    object jsObj = new object();
            //    var serializer = new JavaScriptSerializer();
            //    dynamic data = serializer.DeserializeObject(json);
            //    MessageBox.Show(data.ToString());
            //}
            //catch (Exception eJson)
            //{
            //    MessageBox.Show(eJson.Message);
            //}

            //MessageBox.Show()

            textMenuTmpTable = tableToSQLToolStripMenuItem.Text;
            TempTableServer = Properties.Settings.Default.TmpSqlServer;
            changeMenuTmpServer();

            string admins = Properties.Settings.Default.admins;
            string[] arrayAdmin = admins.Split(',');
            int pos = Array.IndexOf(arrayAdmin,Environment.UserName.ToLower());
            isAdmin = (pos > -1);
            tbHeadClass.isAdmin = isAdmin;
            bindingNavigatorSaveItems2.Enabled = isAdmin;

            tbHeadClass.setProperty(olvDataTree,textTmplName);
            tbStrClass.setProperty(dgvTemlpStr, bNTmplStr);

            this.Text = Scan.GetLocalIPAddress();
            this.CenterToScreen();
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
            foreach (dataType dataType in Enum.GetValues(typeof(dataType)))
            {
                dataTypeDataGrid3.Items.Add(dataType.ToString());
            }

        }

        static class tbHeadClass {

            public static bool isAdmin = false;

            static TextBox textTmplName;

            public static DataTreeListView olvDataTree;

            public static DataTable tbHead = new DataTable();

            public static int Index
            {
                get
                {
                    DataRow row = ((DataRowView)olvDataTree.SelectedObject).Row;
                    int index = -1;
                    if (!((bool)(row["isGroup"]) || ( row["idHead"] == DBNull.Value) ))
                    {
                        index = (int)(row["idHead"]);
                    }
                    if (index > 0)
                    {
                        textTmplName.Text = String.Format("{0}\r\n{1}", 
                            row["name"],
                             row["comm"]
                             );
                        return (int)row["idHead"];
                    }
                    textTmplName.Text = "";
                    return index;
                }
            }

            public static int idParents
            {
                get
                {
                    DataRow row = ((DataRowView)olvDataTree.SelectedObject).Row;
                    return (int)(row["idParent"]);
                }
            }

            public static int SelectedRow
            {
                get
                {
                    return tbHead.Rows.IndexOf(
                        (((DataRowView)olvDataTree.SelectedObject).Row)
                        );
                }
            }

            public static void setProperty(DataTreeListView _olvDataTree, TextBox _textBox)
            {
                textTmplName = _textBox;
                textTmplName.Text = "___";
                olvDataTree = _olvDataTree;
                initOlvHeadTable();
                getData();
                olvDataTree.DataSource = tbHeadClass.tbHead;
                if (!isAdmin)
                {
                    olvDataTree.Columns.RemoveByKey("isGroup");
                }
                autosizeColumns();
            }

            public static void getData()
            {
                tbHead.Clear();
                SQLFunction.getTbHeadData(tbHead);
                tbHead.Columns["idHead"].AutoIncrement = true;
                tbHead.Columns["idHead"].AutoIncrementSeed = -1;
                tbHead.Columns["idHead"].AutoIncrementStep = -1;

            }


            public static void update()
            {
                SQLFunction.updateTbHeadData(tbHead);
//                getTbStrData(idHead);
            }


            static void initOlvHeadTable()
            {
                olvDataTree.KeyAspectName = "idHead";
                olvDataTree.ParentKeyAspectName = "idParent";
                olvDataTree.RootKeyValue = 0;
                olvDataTree.ShowKeyColumns = false;
                olvDataTree.CellEditActivation = ObjectListView.CellEditActivateMode.F2Only;

                // Make the decoration
                RowBorderDecoration rbd = new RowBorderDecoration();
                rbd.BorderPen = new Pen(Color.FromArgb(128, Color.LightSeaGreen), 2);
                rbd.BoundsPadding = new Size(1, 1);
                rbd.CornerRounding = 4.0f;

                // Put the decoration onto the hot item
                olvDataTree.HotItemStyle = new HotItemStyle();
                olvDataTree.HotItemStyle.Decoration = rbd;
                olvDataTree.UseHotItem = true;
            }

            static void autosizeColumns()
            {
                foreach (ColumnHeader col in olvDataTree.Columns)
                {
                    //auto resize column width

                    int colWidthBeforeAutoResize = col.Width;
                    col.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                    int colWidthAfterAutoResizeByHeader = col.Width;
                    col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    int colWidthAfterAutoResizeByContent = col.Width;

                    if (colWidthAfterAutoResizeByHeader > colWidthAfterAutoResizeByContent)
                        col.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                    //first column
                    if (col.Index == 0)
                        //we have to manually take care of tree structure, checkbox and image
                        col.Width += 16 + 16 + olvDataTree.SmallImageSize.Width;
                    //last column
                    else if (col.Index == olvDataTree.Columns.Count - 1)
                        //avoid "fill free space" bug
                        if (colWidthBeforeAutoResize > colWidthAfterAutoResizeByContent)
                            col.Width = colWidthBeforeAutoResize;
                        else
                            col.Width = colWidthAfterAutoResizeByContent;
                }
            }
        }

        static class tbStrClass {

            static public int idHead { get; private set; }

            static public BindingNavigator Bng { get; private set; }

            public static DataView dvString;

            static DataGridView Dgv;

            public static DataTable tbString = new DataTable();

            public static void setProperty(DataGridView dgv, BindingNavigator bng)
            {
                Bng = bng;
                Dgv = dgv;
                Dgv.AutoGenerateColumns = false;
                Dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dvString = new DataView(tbString);
                BindingSource bs = new BindingSource() { DataSource = tbString };
                tbStrClass.Bng.BindingSource = bs;
                Dgv.DataSource = bs;
                SQLFunction.intTbStrParam();
            }

            static void autoSizeColumn()
            {
                //Dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                //Dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }

            public static void getTbStrData(int _idHead)
            {
                idHead = _idHead;

                if (idHead <0)
                {
                    tbString.Clear();
                    disabledGrid();
                }
                else
                {
                    enableGrid();
                    SQLFunction.getTbStrData(tbString, idHead);
                    tbString.Columns["idHead"].DefaultValue = idHead;
                    if (tbString.Columns.Contains("xlsName"))
                    {
                        tbString.Columns["xlsName"].DefaultValue = "";
                    }
                    setTbProperty();
                    //autoSizeColumn();
                    Dgv.Sort(Dgv.Columns["nppDataGrid3"], ListSortDirection.Ascending);
                    Dgv.Columns["nppDataGrid3"].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                }
            }

            static void enableGrid()
            {
                Bng.Enabled = true;
                Dgv.Enabled = true;
            }

            static void disabledGrid()
            {
                Bng.Enabled = false;
                Dgv.Enabled = false;
            }

            static void setTbProperty()
            {
                tbString.Columns["resName"].AllowDBNull = false;
            }

            public static void update()
            {
                SQLFunction.updateTbStrData(tbString);
                getTbStrData(idHead);
            }
        }

        private void olvDataTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tbStrClass.getTbStrData(tbHeadClass.Index);
            string[] enabledBtn;
            if (file.XlsTable != null)
            {
                enabledBtn = new string[]{
                        "ButtonOpen",
                        "ButtonMatching",
                        //"BottomProcessing",
                        //"ButtonStore"
                    };
            }
            else enabledBtn = new string[] { "ButtonOpen" };
            activatStripMenu(enabledBtn);
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {

                case Keys.W | Keys.Alt:
                    changeContainerWidth(splitContainer1);
                    return false;

                case Keys.D | Keys.Alt:
                    //if (dgvTemlpHead.CurrentCell.RowIndex >= 0)
                    //{
                    //    string msg = String.Format("Копируем шаблон \"{0} -- {1}\"?",
                    //        dgvTemlpHead.Rows[dgvTemlpHead.CurrentCell.RowIndex].Cells["dgvTemplName"].Value,
                    //        dgvTemlpHead.Rows[dgvTemlpHead.CurrentCell.RowIndex].Cells["dgvTemplComm"].Value
                    //        );
                    //    DialogResult result = MessageBox.Show(msg, "Warning",
                    //    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    //    if (result == DialogResult.Yes)
                    //    {
                    //        templCopy();
                    //    }
                    //}
                    return false;
                case Keys.S | Keys.Control:
                    if (olvDataTree.Focused)
                    {
                        tbHeadClass.update();

                        MessageBox.Show("Изменения в структуре шаблонов сохранены!");
                    }


                    //bindingNavigatorSaveItems1_Click(this);
                    //bindingNavigatorSaveItems2_Click(this);
                    //MessageBox.Show("Изменения в шаблонах сохранены!");

                    return false;
                case Keys.F5:
                    if (olvDataTree.Focused)
                        tbHeadClass.update();

                        //tbHeadClass.setProperty(olvDataTree, textTmplName);
                        return false;
                case Keys.Insert:
                    if (dgvTemlpStr.Focused) {
                        if (tbStrClass.tbString.Rows.Count == 0) {
                            return false;
                        }
                        DataRow row = tbStrClass.tbString.NewRow();
                        string newRow = "NewRow";
                        row["resName"] = newRow;
                        row["xlsName"] = "";
                        if (dgvTemlpStr.SelectedCells.Count > 0)
                        {
                            int rowIndex = dgvTemlpStr.SelectedCells[0].RowIndex;
                            object value = dgvTemlpStr.Rows[rowIndex].Cells[0].Value;
                            if (value != DBNull.Value)
                            {
                                tbStrClass.tbString.Rows.InsertAt(row, rowIndex);
                                row["npp"] = Convert.ToInt32(value.ToString());
                                if (dgvTemlpStr.Rows[rowIndex].Cells["resNameDataGrid3"].Value.ToString() != newRow)
                                {
                                    rowIndex++;
                                }
                                dgvTemlpStr.CurrentCell = dgvTemlpStr[0, rowIndex];
                            }
                        }
                    }
                    if (olvDataTree.Focused)
                    {
                        //int idParent = 
                        DataRow row = tbHeadClass.tbHead.NewRow();
                        row["idParent"] = tbHeadClass.idParents;
                        row["isGroup"] = 0;
                        tbHeadClass.tbHead.Rows.InsertAt(row, tbHeadClass.SelectedRow);
                        //    dgvTemlpStr.Rows[dgvTemlpStr.]
                    }
                    return false;

                case Keys.Enter :
                    bool result = false;
                    if (dgvTemlpStr.IsCurrentCellInEditMode)
                    {
                        dgvTemlpStr.EndEdit();
                        dgvTemlpStr_KeyDown(dgvTemlpStr, new KeyEventArgs(Keys.Enter));
                        result = true;
                    }
                    return result;

                case Keys.Delete:
                    if (dgvTemlpStr.Focused)
                    {
                        bindingNavigatorDeleteItem1.PerformClick();
                    }
                    return false;
                case Keys.Y | Keys.Control:
                    if (dgvTemlpStr.Focused)
                    {
                        if (dgvTemlpStr.CurrentRow.Index < dgvTemlpStr.NewRowIndex) {
                            //dgvTemlpStr.Rows.Insert(dgvTemlpStr.CurrentRow.Index, dgvTemlpStr.CurrentRow.Clone());
                            //tbStrClass.tbString.BeginLoadData();
                            DataRow distRow = tbStrClass.tbString.NewRow();
                            distRow.ItemArray = (dgvTemlpStr.CurrentRow.DataBoundItem as DataRowView).Row.ItemArray.Clone() as object[];
                            int rowIndex = dgvTemlpStr.SelectedCells[0].RowIndex;
                            tbStrClass.tbString.Rows.InsertAt(distRow,rowIndex);
                            //tbStrClass.tbString.EndLoadData();
                            MessageBox.Show(
                                String.Format("Строка с номером {0} скопирована!", distRow["npp"].ToString())
                            );
                        }

                        return true;
                    }

                        //tbHeadClass.setProperty(olvDataTree, textTmplName);
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

                string ROW_ID = Scan.ROW_ID;
                fileName = openFileDialog1.FileName;
                this.Text = String.Format("{0} -- Convert: {1}", Scan.GetLocalIPAddress(), Path.GetFileName(fileName));
                file.ReadFile(fileName);
                DataTable table = file.XlsTable;
                if (table == null)
                {
                    MessageBox.Show("Таблица не найдена!");
                    return;
                }
                int columns = table.Columns.Count;
                DataColumn col = table.Columns.Add(ROW_ID, typeof(Int32));
                col.SetOrdinal(0);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i][ROW_ID] = i + 1;
                }
                col.ReadOnly = true;
                //col = table.Columns.Add("Check_Result", typeof(String));
                //col.SetOrdinal(1);
                dgvTableXls.DataSource = table;
                string[] enabledBtn;
                if (tbStrClass.tbString.Rows.Count > 0)
                {
                    enabledBtn = new string[]{
                        "ButtonOpen",
                        "ButtonMatching",
                        //"BottomProcessing",
                        //"ButtonStore"
                    };
                }
                else enabledBtn = new string[] { "ButtonOpen" };

                activatStripMenu(enabledBtn);
                tabControl1.TabPages.Remove(tabPage1);
                textBox1.Text = String.Empty;
                MessageBox.Show(String.Format("Файл \"{0}\" загружен", Path.GetFileName(fileName)));
                this.dgvRes.SelectionChanged -= new System.EventHandler(this.dgvRes_SelectionChanged);
                resDataSet.Clear();
                if (procList != null)
                {
                    procList.deleteButton();
                }

            }
        }

        private void activatStripMenu(string[] enableBtn)
        {
            //            foreach (ToolStripButton btn in toolStrip1.Items)
            foreach (ToolStripItem btn in toolStrip1.Items)
            {
                if (btn is ToolStripButton)
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //  данная строка кода позволяет загрузить данные в таблицу "collectDataSet.attrValue". При необходимости она может быть перемещена или удалена.
            this.attrValueTableAdapter.Fill(this.collectDataSet.attrValue);
            //данная строка кода позволяет загрузить данные в таблицу "collectDataSet.i_tmpl_str". 
            //При необходимости она может быть перемещена или удалена.
            //this.i_tmpl_strTableAdapter.Fill(this.collectDataSet.i_tmpl_str);
            //данная строка кода позволяет загрузить данные в таблицу "collectDataSet.i_tmpl_head". 
            //При необходимости она может быть перемещена или удалена.
            //this.i_tmpl_headTableAdapter.Fill(this.collectDataSet.i_tmpl_head);
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
            tbStrClass.update();
            MessageBox.Show("Шаблон сохранен!");
            //this.fKimpHeadimpStrBindingSource.EndEdit();
            //this.tableAdapterManager.UpdateAll(this.collectDataSet);
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
                file.ExportToXls(result, scan, scan.printAllFields);
            } else
            {
                MessageBox.Show("Сопоставьте входной файл с шаблона сначала!");
            }
        }

        private void matching_Click(object sender, EventArgs e) 
        {
            this.dgvRes.SelectionChanged -= new System.EventHandler(this.dgvRes_SelectionChanged);
            bsXlsTable = null;
            bsResTable = null;
            ckTmplFields.Enabled = true;
            ckResult.Enabled = true;

            if (file.XlsTable == null) {
                MessageBox.Show("Сперва загрузите теблицу!");
                return;
            }
            scan.Clear();

            dgvTemlpStr.Sort(dgvTemlpStr.Columns[getGridIndex("npp")],ListSortDirection.Ascending);

            foreach (DataRow row in tbStrClass.tbString.Rows)
            {
                scan.AddField(row);
            }

            textBox1.Text = scan.Matching(file.XlsTable);

            if (!scan.AllFound)
            {
                MessageBox.Show("Не все поля из шаблона найдены!\n Смотрите подробный отчет");
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

            dgvRes.Columns.Clear();
            //dgvRes.DataSource = scan.ResTable;
            SetBsResTable();
            dgvRes.Show();

            if (!tabControl1.TabPages.Contains(tabPage1))
            {
                tabControl1.TabPages.Insert(0, tabPage1);
                tabControl1.SelectedIndex = 0;
            }
            ClearXlsBinding();
            this.dgvRes.SelectionChanged += new System.EventHandler(this.dgvRes_SelectionChanged);
            if (procList != null)
            {
                procList.deleteButton();
            }
        }

        void SetBsResTable()
        {
            DataTable resTable = scan.ResTable;
            DataTable xlsTable = file.XlsTable;
            if (resDataSet.Tables.Contains(resTable.TableName))
            {
                if (resDataSet.Relations.Contains("RelationId"))
                {
                    resDataSet.Relations.Remove("RelationId");
                    if (resDataSet.Tables[xlsTable.TableName].Constraints.Contains("RelationId"))
                        resDataSet.Tables[xlsTable.TableName].Constraints.Remove("RelationId");
                    //resDataSet.Tables[resTable.TableName].Constraints.Remove("RelationId");
                }
                //ConstraintCollection collection = resDataSet.Tables[resTable.TableName].Constraints;
                //for (int i = collection.Count - 1; i >= 0; --i)
                //{
                //    if (collection[i] is System.Data.ForeignKeyConstraint )
                //    {
                //        collection.Remove(collection[i]);
                //    }
                //}

                //DataSet DS = resDataSet;
                //for (int i = DS.Relations.Count - 1; i >= 0; i--)
                //    DS.Relations.Remove(DS.Relations[i]);

                //resDataSet.Tables[resTable.TableName].Constraints.Clear();
                resDataSet.Tables.Remove(resTable.TableName);
            }


            resDataSet.Tables.Add(resTable);
            bsResTable = new BindingSource();
            bsResTable.DataSource = resDataSet;
            bsResTable.DataMember = "ResTable";
            dgvTableXls.DataSource = bsXlsTable;
            dgvRes.DataSource = bsResTable;
        }

        private void processing_Click(object sender, EventArgs e)
        {
            // String IP = Scan.GetLocalIPAddress();
            this.dgvRes.SelectionChanged -= new System.EventHandler(this.dgvRes_SelectionChanged);
            if (!tabControl1.TabPages.Contains(tabPage1))
            {
                tabControl1.TabPages.Insert(0, tabPage1);
                tabControl1.SelectedIndex = 0;
            }

            //scan.initResultFromXls(file.XlsTable);

            scan.Processing();
            dgvRes.AutoGenerateColumns = false;
            dgvRes.Columns.Clear();
            dgvRes.AutoGenerateColumns = true;
            showStripMessage("");

            ClearXlsBinding();
            activatStripMenu(
                new string[]  {
                "ButtonOpen",
                "ButtonMatching",
                //"BottomProcessing",
                "ButtonStore"
                }
            );
            this.dgvRes.SelectionChanged += new System.EventHandler(this.dgvRes_SelectionChanged);
            procList = new ProcList(scan.idHead, tsResultTable.Items,scan.ResTable);
            procList.insertButton();
            MessageBox.Show("ok");
        }

        private void dataGridView1_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((((DataGridView)sender).Name == "dgvTableXls")
                & (e.Button == MouseButtons.Right)) {
                string name = dgvTableXls.Columns[e.ColumnIndex].Name;
                Clipboard.SetText(name);
                MessageBox.Show(String.Format("String \'{0}\' has been copied to clipboard",name));
            }
        }
        
        private void dgvTemlpStr_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if ((e.RowIndex == dgvTemlpStr.NewRowIndex-1) || (e.RowIndex == dgvTemlpStr.NewRowIndex ))
            {
                if (e.RowIndex > 0) {

                    int npp = int.MinValue;
                    foreach (DataRow rd in tbStrClass.tbString.Rows)
                    {
                        short accountLevel = rd.Field<short>("npp");
                        npp = Math.Max(npp, accountLevel);
                    }

                    DataGridView dgv = ((DataGridView)sender);
                    //DataGridViewColumn col = dgv.Columns[getGridIndex("npp")];
                    //((DataGridView)sender).Sort(col,ListSortDirection.Ascending);
                    int index = e.RowIndex;
                    if (e.RowIndex == dgvTemlpStr.NewRowIndex) index--;

                    DataGridViewRow row = dgv.Rows[index];

                    //int npp = Int32.Parse(dgvTemlpStr.Rows[index - 1].Cells[getGridIndex("npp")].Value.ToString());
                    npp = ((npp/10) + 1) * 10;
                    row.Cells[getGridIndex("npp")].Value = npp.ToString();

                    // set default value
                    row.Cells[getGridIndex("DataType")].Value = "String";
                    row.Cells[getGridIndex("DataSize")].Value = 0;
                    row.Cells[getGridIndex("isPrint")].Value = true;
                    row.Cells[getGridIndex("attr")].Value = 0;
                    row.Cells[getGridIndex("IsActive")].Value = true;
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
            return choosenSheet;
        }

        private void dgvTemlpStr_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            if (sender is DataGridView)
            {
                DataGridView dgv = (DataGridView)sender;
                dgv.CurrentCell = dgv.Rows[anError.RowIndex].Cells[anError.ColumnIndex];
            }
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
            progressBar.Maximum = Math.Max(maximum,1);
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
            //if (dgvTemlpHead.Rows[dgvTemlpHead.CurrentCell.RowIndex].Cells["dgvTemplIdHead"] != null)
            //{
            //    int IdHead = (int)dgvTemlpHead.Rows[dgvTemlpHead.CurrentCell.RowIndex].Cells["dgvTemplIdHead"].Value;
            //    res = SQLFunction.copyTemlpate(IdHead);
            //    Form1_Load(this, null);
            //    int newRow = dgvTemlpHead.Rows.Count;
            //    dgvTemlpHead.CurrentCell = dgvTemlpHead.Rows[newRow - 2].Cells[0];
            //}
            return res;
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

            MessageBox.Show(parsedDate.ToString());

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

        private void textColumnFilter_TextChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(textColumnFilter.Text);
            if (dgvTableXls.DataSource == null) return;
            dgvTableXls_changeColumns(txtColumnFilter.Text);
        }

        private void dgvTableXls_changeColumns(string filter)
        {
            if (filter == String.Empty)
            {
                foreach (DataGridViewColumn col in dgvTableXls.Columns)
                {
                    col.Visible = true;
                }
            }
            else
            {
                foreach (DataGridViewColumn col in dgvTableXls.Columns)
                {
                    bool res = false;
                    foreach(string str in filter.Split(';'))
                    {
                        if (col.Name.ToLower().Contains(str.ToLower()))
                        {
                            res = true;
                            break;
                        }
                    }

                    if (res)
                    {
                        col.Visible = true;
                    }
                    else
                    {
                        col.Visible = false;
                    }
                }
            }
        }

        private void toolStripButton1_Click_3(object sender, EventArgs e)
        {
            showCalcGrid();
        }

        void showCalcGrid()
        {

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            DataTable table = new DataTable();

            //table.Locale = System.Globalization
            //    CultureInfo.InvariantCulture;
            //table.Locale.NumberFormat.NumberDecimalSeparator = ".";

            // Create the first column.
            DataColumn priceColumn = new DataColumn("price", Type.GetType("System.Decimal"));
//            priceColumn.DataType = System.Type.GetType("System.decimal");
            priceColumn.ColumnName = "price";
            priceColumn.DefaultValue = "50,1";


            // Create third column.
            DataColumn totalColumn = new DataColumn();
            totalColumn.DataType = System.Type.GetType("System.String");
            totalColumn.ColumnName = "total";
            totalColumn.Expression = "[price]";

            // Add columns to DataTable.
            table.Columns.Add(priceColumn);
            table.Columns.Add(totalColumn);

            DataRow row = table.NewRow();
            table.Rows.Add(row);
            DataView view = new DataView(table);
            dgvTableXls.DataSource = view;

        }

        private void toolStripButton1_Click_4(object sender, EventArgs e)
        {
            double a = 0.003;
            MessageBox.Show(a.ToString(CultureInfo.InvariantCulture));
        }

        private void dgvTableXls_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void fKimpHeadimpStrBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void check_fieldAllFilter(object sender, EventArgs e)
        {
            if (ckTmplFields.Checked)
            {
                string filter = scan.GetXlsFields();
                int len = filter.Length;
                if (len > 0) filter = filter.Remove(len-1,1);
                txtColumnFilter.Text = filter;
                dgvTableXls_changeColumns(filter);
                txtColumnFilter.ReadOnly = true;
            }

            else
            {
                txtColumnFilter.Text = "";
                txtColumnFilter.ReadOnly = false;
            }

        }

        void ClearXlsBinding()
        {
            ckResult.Checked = false;
            ChahgeXlsBinding();
            if (resDataSet.Relations.Contains("RelationId"))
            {
                //resDataSet.Relations.Remove("RelationId");
                //resDataSet.Tables.Remove(file.XlsTable);
                bsXlsTable = null;
                bsResTable = null;
            }
        }

        void ChahgeXlsBinding()
        {
            if (ckResult.Checked)
            {
                FieldBase field = scan.Find(x => (x.Attr == attrName.System) && (x.XlsName == Scan.ROW_ID) && x.IsActive && x.IsPrint);
                if (field == null)
                {
                    ckResult.Checked = false;
                    MessageBox.Show("Can't find ROW_ID field!");
                    return;
                }
                if (bsXlsTable == null)
                {
                    DataTable xlsTable = file.XlsTable;
                    DataTable resTable = scan.ResTable;


                    resTable.Locale = new System.Globalization.CultureInfo(xlsTable.Locale.Name);

                    if (!resDataSet.Relations.Contains("RelationId"))
                    {
                        if (resDataSet.Tables.Contains(xlsTable.TableName))
                        {
                            resDataSet.Tables.Remove(xlsTable.TableName);
                        }
                        resDataSet.Tables.Add(xlsTable);
                        if (resDataSet.Tables.Contains(resTable.TableName))
                        {
                            resDataSet.Tables.Remove(resTable.TableName);
                        }
                        resDataSet.Tables.Add(resTable);
                        try
                        {
                            resDataSet.Relations.Add(
                                "RelationId",
                                resTable.Columns[field.ResName],
                                xlsTable.Columns[Scan.ROW_ID]);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }

                    bsXlsTable = new BindingSource();
                    bsXlsTable.DataSource = bsResTable;
                    bsXlsTable.DataMember = "RelationId";
                }
                dgvTableXls.DataSource = bsXlsTable;
            }
            else
            {
                dgvTableXls.DataSource = file.XlsTable;
            }
        }

        private void btResult_CheckedChanged(object sender, EventArgs e)
        {
            ChahgeXlsBinding();
        }

        private void txtRowsFilter_TextChanged(object sender, EventArgs e)
        {
            (dgvTemlpStr.DataSource as BindingSource).Filter =
                String.Format("[xlsName] like '%{0}%' or [resName] like '%{0}%' or comm like '%{0}%'", txtRowsFilter.Text);
        }

        private void btResFilter_Click(object sender, EventArgs e)
        {
            if (dgvRes.DataSource == null) return;
            try
            {
                (dgvRes.DataSource as BindingSource).Filter = txtResFilter.Text;
                txtResApply.Text = txtResFilter.Text;
            }
            catch (Exception ex)
            {
                if (ex is EvaluateException || ex is EvaluateException)
                {
                    (dgvRes.DataSource as BindingSource).Filter = txtResApply.Text;
                    MessageBox.Show(ex.Message);
                }
                else throw;
            }
        }

        private void txtResFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if(e.KeyChar = 13 )
            if (e.KeyChar == '\r')
            {
                btResFilter_Click(sender, null);
            }
        }

        private void dgvTemlpStr_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private void dgvTemlpStr_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void dgvTemlpStr_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!isAdmin) return;

            if (ModifierKeys.HasFlag(Keys.Control))
            {

                DataGridView dgv = (DataGridView)sender;
                if ((attrName)dgv.Rows[e.RowIndex].Cells["attrDataGrid3"].Value == attrName.Func)
                {
                    string funcText = dgv.Rows[e.RowIndex].Cells["xlsNameDataGrid3"].Value.ToString();
                    bool funcInRow = false;
                    string name = FieldFunc.getFuncName(funcText);
                    if (name.Length > 0 )
                    {
                        string expr = String.Format(
                            "resName like 'SHEMA%' and isActive = 1 and attr = 5 and xlsName like '%{0}%'",
//                            "resName like 'SHEMA%' and isActive = 1 ", // and xlsName like '%{0}%'",
                            name);
                        DataRow[] rows = tbStrClass.tbString.Select(expr);
                        DataTable table = null;
                        if (rows.Length > 0)
                        {
                            table = FieldFunc.getShema();
                            table.Columns.Add("fnName", Type.GetType("System.String"));

                            string[] param = rows[0]["xlsName"].ToString().Split('(');
                            DataRow row = table.NewRow();

                            row["fnName"] = name;
                            if (param.Length > 0) row[GroupNames.tabFields.ToString()] = param[1].Split(')')[0];
                            if (param.Length > 1) row[GroupNames.inPar.ToString()] = param[2].Split(')')[0];
                            if (param.Length > 2) row[GroupNames.outPar.ToString()] = param[3].Split(')')[0];

                            table.Rows.Add(row);
                            funcInRow = true;

                        }
                        else
                        {
                            table = SQLFunction.getFuncDescription(name, tbStrClass.idHead);
                        }


                        if (table.Rows.Count > 0)
                        {
                            FormFunc form = new FormFunc(table, funcText, funcInRow);
                            form.ShowDialog();
                            form.StartPosition = FormStartPosition.CenterParent;
                            if (form.func != String.Empty)
                            {
                                dgv.Rows[e.RowIndex].Cells["xlsNameDataGrid3"].Value = form.func;
                            }
                            if (form.shema != String.Empty)
                            {
                                rows[0]["xlsName"] = form.shema;
                            }
                            form.Dispose();
                        }
                        else
                            MessageBox.Show(String.Format("Can't found function '{0}', template '{1}'", name, tbStrClass.idHead));
                    }

                }
            }
        }

        private void dgvTemlpStr_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dvg = (sender as DataGridView);
            if (( e.RowIndex < 0 ) || (dvg.Rows[e.RowIndex].Cells["attrDataGrid3"].Value == null) )return;

            string cellValue = dvg.Rows[e.RowIndex].Cells["attrDataGrid3"].Value.ToString();

            switch (cellValue)
            {
                case "0":
                    e.CellStyle.BackColor = Color.PeachPuff;
                    break;
                case "2":
                    e.CellStyle.BackColor = Color.Khaki;
                    break;
                case "3":
                    e.CellStyle.BackColor = Color.LightBlue;
                    break;
                case "5":
                    e.CellStyle.BackColor = Color.LightSlateGray;
                    break;
            }
        }

        private void dgvRes_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);

            if (dgv.SelectedCells.Count < 2) return;

            double summTotal = 0;
            Int32 count = 0;

            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                double outValue = 0;

                if (Double.TryParse(cell.Value.ToString(), out outValue))
                {
                    summTotal += outValue;
                    count++;
                }
            }
            summTotal = Math.Round(summTotal, 4);

            tsTextSumm.Text =  String.Format("{0:### ### ###.00}/{1}",summTotal,count);

            //int sum = 0;
            //for (int i = 0; i < dataGridView1.Rows.Count; ++i)
            //{
            //    sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value);
            //}
            //label1.text = sum.ToString();
        }

        private void dgvRes_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dvg = (sender as DataGridView);
            if ((e.RowIndex < 0) | !dvg.Columns.Contains(Scan.ACTIVE_FIELD)) return;
            bool cellValueBool = false;
            object cellValue = dvg.Rows[e.RowIndex].Cells[Scan.ACTIVE_FIELD].Value;
            if (!((cellValue == DBNull.Value) || (cellValue == null)))
            {
                cellValueBool = (bool)dvg.Rows[e.RowIndex].Cells[Scan.ACTIVE_FIELD].Value;
            }
            if (!cellValueBool)
            {
                e.CellStyle.BackColor = Color.PaleVioletRed;
            }
        }

        private void dgvTemlpStr_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);
            if (e.RowIndex > 0 & dgv.Columns[e.ColumnIndex].Name == "attrDataGrid3")
                dgv.InvalidateRow(e.RowIndex);
        }

        private void dgvRes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);
            if (e.RowIndex > 0 & dgv.Columns[e.ColumnIndex].Name == Scan.ACTIVE_FIELD)
                dgv.InvalidateRow(e.RowIndex);
        }

        void changeActiveRow(short option)
        {
            bool value = Convert.ToBoolean(option);
            if (txtResFilter.Text.Length > 0)
            {
                DataRow[] rows = scan.ResTable.Select(txtResFilter.Text);
                foreach (DataRow row in rows)
                {
                    if (option == 3) value = !Convert.ToBoolean(row[Scan.ACTIVE_FIELD]);
                    row[Scan.ACTIVE_FIELD] = value;
                }
            }
            else
            {
                foreach (DataRow row in scan.ResTable.Rows)
                {
                    if (option == 3) value = !Convert.ToBoolean(row[Scan.ACTIVE_FIELD]);
                    row[Scan.ACTIVE_FIELD] = value;
                }
            }
            dgvRes.Refresh();
        }

        private void tsbActPlus_Click(object sender, EventArgs e)
        {
            changeActiveRow(1);
        }

        private void tsbActMinus_Click(object sender, EventArgs e)
        {
            changeActiveRow(0);
        }

        private void tsbActChange_Click(object sender, EventArgs e)
        {
            changeActiveRow(3);
        }

        private void dgvTemlpStr_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            DataGridView dataGridView1 = (DataGridView)sender;
            int iColumn = dataGridView1.CurrentCell.ColumnIndex;
            int iRow = dataGridView1.CurrentCell.RowIndex;
            if (iColumn == dataGridView1.ColumnCount - 1)
            {
                if (dataGridView1.RowCount > (iRow + 1))
                {
                    dataGridView1.CurrentCell = dataGridView1[1, iRow + 1];
                }
                else
                {
                    //focus next control
                }
            }
            else
                dataGridView1.CurrentCell = dataGridView1[iColumn + 1, iRow];

        }

        private void dgvTemlpStr_Move(object sender, EventArgs e)
        {
        }

        private void dgvTemlpStr_CursorChanged(object sender, EventArgs e)
        {
        }

        private void toolStripButton1_Click_5(object sender, EventArgs e)
        {
            string admins = Properties.Settings.Default.admins;
            string[] arrayAdmin = admins.Split(',');
            int pos = Array.IndexOf(arrayAdmin, Environment.UserName.ToLower());
            isAdmin = (pos > -1);
            MessageBox.Show(
                String.Format("Имена: {0}\n\rПользователь: {1}\n\rIsAdmin:{2}", admins, Environment.UserName.ToLower(),isAdmin)
                );
        }

        private void iPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string IP = Scan.GetLocalIPAddress(true);
            Clipboard.SetText(IP);
            MessageBox.Show(
                String.Format("Ip has copied to Clipboard ({0})",IP)
                );
        }

        private void tableToSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!isAdmin) return;
            string query = "";
            if (file.XlsTable != null || true)
            {
                try {
              //      throw new System.ArgumentException("Parameter cannot be null", "original");
                    TableToSql tmp = new TableToSql(file.XlsTable,
                        SQLFunction.getNewConnection("tempDb", TempTableServer));
                    tmp.createTempTable();
                    tmp.bulkData();
                    FormDialogTmp form = new FormDialogTmp(tmp.tableName, TempTableServer);
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog();
                    form.Dispose();
                    tmp.dropTemplate();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            else {
                query = "Table doesn't exist";
            }
        }

        private void xlsToSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XlsToSql reader = new XlsToSql(textBox1);
            reader.readList();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TimedFilter(this.olvDataTree, ((TextBox)sender).Text,0);
        }

        public void TimedFilter(ObjectListView olv, string txt, int matchKind)
        {
            TextMatchFilter filter = null;
            if (!String.IsNullOrEmpty(txt))
            {
                switch (matchKind)
                {
                    case 0:
                    default:
                        filter = TextMatchFilter.Contains(olv, txt);
                        break;
                    case 1:
                        filter = TextMatchFilter.Prefix(olv, txt);
                        break;
                    case 2:
                        filter = TextMatchFilter.Regex(olv, txt);
                        break;
                }
            }

            // Text highlighting requires at least a default renderer
            if (olv.DefaultRenderer == null)
                olv.DefaultRenderer = new HighlightTextRenderer(filter);

            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();

            olv.AdditionalFilter = filter;
            //olv.Invalidate();
//            stopWatch.Stop();

            IList objects = olv.Objects as IList;

            //if (objects == null)
            //    this.ToolStripStatus1 = prefixForNextSelectionMessage =
            //        String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
            //else
            //    this.ToolStripStatus1 = prefixForNextSelectionMessage =
            //        String.Format("Filtered {0} items down to {1} items in {2}ms",
            //                      objects.Count,
            //                      olv.Items.Count,
            //                      stopWatch.ElapsedMilliseconds);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void setTmpServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPrompt.answer formAnswer;
            formAnswer = FormPrompt.ShowDialog("Server name", "Input SQL server name for temporary table:", 
                Properties.Settings.Default.TmpSqlServer);
            if (formAnswer.cancel)
                return;
            if (formAnswer.save) {
                Properties.Settings.Default.TmpSqlServer = formAnswer.message;
                Properties.Settings.Default.Save();
            }
            TempTableServer = formAnswer.message;
            TempTableServer = formAnswer.message;
            changeMenuTmpServer();
        }

        public void changeMenuTmpServer() {
            tableToSQLToolStripMenuItem.Text = $"{textMenuTmpTable} to {TempTableServer }";
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable result = scan.ResTable;
            DataTable xlsTable = file.XlsTable;
            string now = DateTime.Now.ToString("yyyyMMdd");
            if ((result != null) && (xlsTable != null) )
            {
                var field = scan.Find(x => x.Attr == attrName.CsvShemaAdr && x.IsActive);
                if (field == null )
                {
                    MessageBox.Show("Can't find schema (the type's 'адрес-csv' in the template)");
                    return;
                }
                string addrelations = field.XlsName;

                string rNumber = "";
                var rField = scan.Find(x => (x.ResName == "RNumber") && (x.Attr == attrName.Const));
                if (rField != null)
                {
                    rNumber = rField.XlsName;
                }

                string tmplNum = tbHeadClass.Index.ToString();

                string fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    + $@"\{now}_{rNumber}_({tmplNum}).csv";

                
                try
                {
                    FileToCsv csvFile = new FileToCsv(result, file.XlsTable, fileName, addrelations);
                    if ( csvFile.schemaError )
                    {
                        MessageBox.Show($"Matching error: \r\n{csvFile.wrongFields}");
                    }
                    else
                    {
                        MessageBox.Show($"File {fileName} \r\n has been saved!");
                    }
                }
                catch (NotImplementedException impEx)
                {
                    MessageBox.Show(impEx.Message);
                }
                return;
            }

            if (xlsTable != null)
            {
                string xlsName = this.fileName.Split('\\').Last().Split('.')[0];

                string fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    + $@"\{now}_{xlsName}.csv";
                DataTable csvTable = xlsTable.Copy();
                if (csvTable.Columns.Contains("$ROW_ID"))
                {
                    csvTable.Columns.Remove("$ROW_ID");
                }
                FileToCsv.tableToFile(csvTable, fileName);
                MessageBox.Show($"File {fileName} \r\n has been saved!");
                return;
            }
            MessageBox.Show("Result table doesn't exist");
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //olvDataTree.RefreshObjects();
        }
    }
}
	
