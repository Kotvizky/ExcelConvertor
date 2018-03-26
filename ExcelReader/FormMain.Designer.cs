namespace ExcelReader
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ckResult = new System.Windows.Forms.CheckBox();
            this.ckTmplFields = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtColumnFilter = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.dgvTableXls = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtResFilter = new System.Windows.Forms.TextBox();
            this.btResFilter = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtResApply = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dgvRes = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.olvDataTree = new BrightIdeasSoftware.DataTreeListView();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.textTmplName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtRowsFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvTemlpStr = new System.Windows.Forms.DataGridView();
            this.nppDataGrid3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resNameDataGrid3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xlsNameDataGrid3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isPrintDataGrid3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.attrDataGrid3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.attrValueBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.collectDataSet = new ExcelReader.CollectDataSet();
            this.dataTypeDataGrid3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataSizeDataGrid3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.strFormatData3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isPosDataGrid3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.isActiveDataGrid3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.commDataGrid3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.authorDataGrid3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idHeadDataGrid3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idStrDataGrid3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fKimpHeadimpStrBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.itmplheadBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.bNTmplStr = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem1 = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem1 = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorSaveItems2 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.ButtonMatching = new System.Windows.Forms.ToolStripButton();
            this.BottomProcessing = new System.Windows.Forms.ToolStripButton();
            this.ButtonStore = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar2 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.i_tmpl_headTableAdapter = new ExcelReader.CollectDataSetTableAdapters.i_tmpl_headTableAdapter();
            this.i_tmpl_strTableAdapter = new ExcelReader.CollectDataSetTableAdapters.i_tmpl_strTableAdapter();
            this.i_tmpl_strBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableAdapterManager = new ExcelReader.CollectDataSetTableAdapters.TableAdapterManager();
            this.i_tmpl_headBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.attrValueTableAdapter = new ExcelReader.CollectDataSetTableAdapters.attrValueTableAdapter();
            this.highlightTextRenderer1 = new BrightIdeasSoftware.HighlightTextRenderer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableXls)).BeginInit();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRes)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvDataTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemlpStr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.attrValueBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fKimpHeadimpStrBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itmplheadBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bNTmplStr)).BeginInit();
            this.bNTmplStr.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.i_tmpl_strBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.i_tmpl_headBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.ckResult);
            this.splitContainer2.Panel1.Controls.Add(this.ckTmplFields);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.txtColumnFilter);
            this.splitContainer2.Panel1.Controls.Add(this.splitter1);
            this.splitContainer2.Panel1.Controls.Add(this.dgvTableXls);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.Controls.Add(this.panel2);
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(522, 578);
            this.splitContainer2.SplitterDistance = 261;
            this.splitContainer2.TabIndex = 0;
            // 
            // ckResult
            // 
            this.ckResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ckResult.AutoSize = true;
            this.ckResult.Enabled = false;
            this.ckResult.Location = new System.Drawing.Point(436, 231);
            this.ckResult.Name = "ckResult";
            this.ckResult.Size = new System.Drawing.Size(85, 17);
            this.ckResult.TabIndex = 11;
            this.ckResult.Text = "Res. relation";
            this.ckResult.UseVisualStyleBackColor = true;
            this.ckResult.CheckedChanged += new System.EventHandler(this.btResult_CheckedChanged);
            // 
            // ckTmplFields
            // 
            this.ckTmplFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ckTmplFields.AutoSize = true;
            this.ckTmplFields.Enabled = false;
            this.ckTmplFields.Location = new System.Drawing.Point(360, 231);
            this.ckTmplFields.Name = "ckTmplFields";
            this.ckTmplFields.Size = new System.Drawing.Size(72, 17);
            this.ckTmplFields.TabIndex = 10;
            this.ckTmplFields.Text = "tmpl fields";
            this.ckTmplFields.UseVisualStyleBackColor = true;
            this.ckTmplFields.CheckedChanged += new System.EventHandler(this.check_fieldAllFilter);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 235);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Column filter";
            // 
            // txtColumnFilter
            // 
            this.txtColumnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColumnFilter.Location = new System.Drawing.Point(71, 231);
            this.txtColumnFilter.Name = "txtColumnFilter";
            this.txtColumnFilter.Size = new System.Drawing.Size(269, 20);
            this.txtColumnFilter.TabIndex = 6;
            this.txtColumnFilter.TextChanged += new System.EventHandler(this.textColumnFilter_TextChanged);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 258);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(522, 3);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // dgvTableXls
            // 
            this.dgvTableXls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTableXls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableXls.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.dgvTableXls.Location = new System.Drawing.Point(0, 0);
            this.dgvTableXls.Name = "dgvTableXls";
            this.dgvTableXls.ShowCellErrors = false;
            this.dgvTableXls.Size = new System.Drawing.Size(522, 220);
            this.dgvTableXls.TabIndex = 3;
            this.dgvTableXls.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTableXls_CellContentClick);
            this.dgvTableXls.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseDoubleClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtResFilter);
            this.panel2.Controls.Add(this.btResFilter);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtResApply);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 264);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(522, 49);
            this.panel2.TabIndex = 3;
            // 
            // txtResFilter
            // 
            this.txtResFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResFilter.Location = new System.Drawing.Point(58, 25);
            this.txtResFilter.Name = "txtResFilter";
            this.txtResFilter.Size = new System.Drawing.Size(457, 20);
            this.txtResFilter.TabIndex = 3;
            this.txtResFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtResFilter_KeyPress);
            // 
            // btResFilter
            // 
            this.btResFilter.Location = new System.Drawing.Point(18, 24);
            this.btResFilter.Name = "btResFilter";
            this.btResFilter.Size = new System.Drawing.Size(31, 21);
            this.btResFilter.TabIndex = 2;
            this.btResFilter.Text = "/\\";
            this.btResFilter.UseVisualStyleBackColor = true;
            this.btResFilter.Click += new System.EventHandler(this.btResFilter_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "select * from ResTab where";
            // 
            // txtResApply
            // 
            this.txtResApply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResApply.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResApply.Location = new System.Drawing.Point(144, 3);
            this.txtResApply.Name = "txtResApply";
            this.txtResApply.ReadOnly = true;
            this.txtResApply.Size = new System.Drawing.Size(371, 20);
            this.txtResApply.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(519, 259);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvRes);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(511, 233);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Result";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvRes
            // 
            this.dgvRes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRes.Location = new System.Drawing.Point(3, 3);
            this.dgvRes.Name = "dgvRes";
            this.dgvRes.Size = new System.Drawing.Size(505, 227);
            this.dgvRes.TabIndex = 1;
            this.dgvRes.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(511, 233);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Matching";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(505, 227);
            this.textBox1.TabIndex = 0;
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(297, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 578);
            this.splitter2.TabIndex = 0;
            this.splitter2.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(12, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel1.Controls.Add(this.splitter2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(831, 580);
            this.splitContainer1.SplitterDistance = 302;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            this.splitContainer1.DoubleClick += new System.EventHandler(this.splitContainer1_DoubleClick);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.AutoScroll = true;
            this.splitContainer3.Panel1.Controls.Add(this.olvDataTree);
            this.splitContainer3.Panel1.Controls.Add(this.splitter3);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.AutoScroll = true;
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Size = new System.Drawing.Size(297, 578);
            this.splitContainer3.SplitterDistance = 200;
            this.splitContainer3.TabIndex = 1;
            // 
            // olvDataTree
            // 
            this.olvDataTree.CellEditUseWholeCell = false;
            this.olvDataTree.DataSource = null;
            this.olvDataTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvDataTree.Location = new System.Drawing.Point(0, 0);
            this.olvDataTree.Name = "olvDataTree";
            this.olvDataTree.RootKeyValueString = "";
            this.olvDataTree.ShowGroups = false;
            this.olvDataTree.Size = new System.Drawing.Size(297, 197);
            this.olvDataTree.TabIndex = 3;
            this.olvDataTree.UseCompatibleStateImageBehavior = false;
            this.olvDataTree.View = System.Windows.Forms.View.Details;
            this.olvDataTree.VirtualMode = true;
            this.olvDataTree.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.olvDataTree_MouseDoubleClick);
            // 
            // splitter3
            // 
            this.splitter3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(0, 197);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(297, 3);
            this.splitter3.TabIndex = 0;
            this.splitter3.TabStop = false;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer4.IsSplitterFixed = true;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.splitContainer4.Panel1.Controls.Add(this.textTmplName);
            this.splitContainer4.Panel1MinSize = 15;
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.panel1);
            this.splitContainer4.Panel2.Controls.Add(this.dgvTemlpStr);
            this.splitContainer4.Panel2.Controls.Add(this.bNTmplStr);
            this.splitContainer4.Size = new System.Drawing.Size(297, 374);
            this.splitContainer4.SplitterDistance = 40;
            this.splitContainer4.TabIndex = 2;
            // 
            // textTmplName
            // 
            this.textTmplName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textTmplName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textTmplName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textTmplName.Location = new System.Drawing.Point(0, 0);
            this.textTmplName.Multiline = true;
            this.textTmplName.Name = "textTmplName";
            this.textTmplName.Size = new System.Drawing.Size(297, 40);
            this.textTmplName.TabIndex = 1;
            this.textTmplName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtRowsFilter);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 303);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(297, 27);
            this.panel1.TabIndex = 2;
            // 
            // txtRowsFilter
            // 
            this.txtRowsFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRowsFilter.Location = new System.Drawing.Point(74, 4);
            this.txtRowsFilter.Name = "txtRowsFilter";
            this.txtRowsFilter.Size = new System.Drawing.Size(213, 20);
            this.txtRowsFilter.TabIndex = 9;
            this.txtRowsFilter.TextChanged += new System.EventHandler(this.txtRowsFilter_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Rows filter";
            // 
            // dgvTemlpStr
            // 
            this.dgvTemlpStr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTemlpStr.AutoGenerateColumns = false;
            this.dgvTemlpStr.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.dgvTemlpStr.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTemlpStr.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nppDataGrid3,
            this.resNameDataGrid3,
            this.xlsNameDataGrid3,
            this.isPrintDataGrid3,
            this.attrDataGrid3,
            this.dataTypeDataGrid3,
            this.dataSizeDataGrid3,
            this.strFormatData3,
            this.isPosDataGrid3,
            this.isActiveDataGrid3,
            this.commDataGrid3,
            this.authorDataGrid3,
            this.idHeadDataGrid3,
            this.idStrDataGrid3});
            this.dgvTemlpStr.DataSource = this.fKimpHeadimpStrBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTemlpStr.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTemlpStr.Location = new System.Drawing.Point(0, 25);
            this.dgvTemlpStr.Name = "dgvTemlpStr";
            this.dgvTemlpStr.Size = new System.Drawing.Size(291, 272);
            this.dgvTemlpStr.TabIndex = 0;
            this.dgvTemlpStr.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemlpStr_CellContentClick);
            this.dgvTemlpStr.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemlpStr_CellDoubleClick);
            this.dgvTemlpStr.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvTemlpStr_CellMouseDoubleClick);
            this.dgvTemlpStr.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemlpStr_CellMouseEnter);
            this.dgvTemlpStr.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemlpStr_CellValidated);
            this.dgvTemlpStr.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvTemlpStr_CellValidating);
            this.dgvTemlpStr.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvTemlpStr_DataError);
            this.dgvTemlpStr.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvTemlpStr_RowsAdded);
            // 
            // nppDataGrid3
            // 
            this.nppDataGrid3.DataPropertyName = "npp";
            this.nppDataGrid3.FillWeight = 40F;
            this.nppDataGrid3.HeaderText = "npp";
            this.nppDataGrid3.Name = "nppDataGrid3";
            this.nppDataGrid3.Width = 50;
            // 
            // resNameDataGrid3
            // 
            this.resNameDataGrid3.DataPropertyName = "resName";
            this.resNameDataGrid3.HeaderText = "resName";
            this.resNameDataGrid3.Name = "resNameDataGrid3";
            this.resNameDataGrid3.Width = 74;
            // 
            // xlsNameDataGrid3
            // 
            this.xlsNameDataGrid3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.xlsNameDataGrid3.DataPropertyName = "xlsName";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.xlsNameDataGrid3.DefaultCellStyle = dataGridViewCellStyle1;
            this.xlsNameDataGrid3.HeaderText = "xlsName";
            this.xlsNameDataGrid3.MaxInputLength = 1000;
            this.xlsNameDataGrid3.Name = "xlsNameDataGrid3";
            this.xlsNameDataGrid3.Width = 5;
            // 
            // isPrintDataGrid3
            // 
            this.isPrintDataGrid3.DataPropertyName = "isPrint";
            this.isPrintDataGrid3.HeaderText = "isPrint";
            this.isPrintDataGrid3.Name = "isPrintDataGrid3";
            this.isPrintDataGrid3.Width = 41;
            // 
            // attrDataGrid3
            // 
            this.attrDataGrid3.DataPropertyName = "attr";
            this.attrDataGrid3.DataSource = this.attrValueBindingSource;
            this.attrDataGrid3.DisplayMember = "name";
            this.attrDataGrid3.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.attrDataGrid3.HeaderText = "attr";
            this.attrDataGrid3.Name = "attrDataGrid3";
            this.attrDataGrid3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.attrDataGrid3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.attrDataGrid3.ValueMember = "id";
            this.attrDataGrid3.Width = 47;
            // 
            // attrValueBindingSource
            // 
            this.attrValueBindingSource.DataMember = "attrValue";
            this.attrValueBindingSource.DataSource = this.collectDataSet;
            // 
            // collectDataSet
            // 
            this.collectDataSet.DataSetName = "CollectDataSet";
            this.collectDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dataTypeDataGrid3
            // 
            this.dataTypeDataGrid3.DataPropertyName = "dataType";
            this.dataTypeDataGrid3.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.dataTypeDataGrid3.HeaderText = "dataType";
            this.dataTypeDataGrid3.Name = "dataTypeDataGrid3";
            this.dataTypeDataGrid3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataTypeDataGrid3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataTypeDataGrid3.Width = 77;
            // 
            // dataSizeDataGrid3
            // 
            this.dataSizeDataGrid3.DataPropertyName = "dataSize";
            this.dataSizeDataGrid3.HeaderText = "dataSize";
            this.dataSizeDataGrid3.Name = "dataSizeDataGrid3";
            this.dataSizeDataGrid3.Width = 73;
            // 
            // strFormatData3
            // 
            this.strFormatData3.DataPropertyName = "strFormat";
            this.strFormatData3.HeaderText = "strFormat";
            this.strFormatData3.Name = "strFormatData3";
            this.strFormatData3.Width = 75;
            // 
            // isPosDataGrid3
            // 
            this.isPosDataGrid3.DataPropertyName = "isPos";
            this.isPosDataGrid3.HeaderText = "isPos";
            this.isPosDataGrid3.Name = "isPosDataGrid3";
            // 
            // isActiveDataGrid3
            // 
            this.isActiveDataGrid3.DataPropertyName = "isActive";
            this.isActiveDataGrid3.HeaderText = "isActive";
            this.isActiveDataGrid3.Name = "isActiveDataGrid3";
            this.isActiveDataGrid3.Width = 50;
            // 
            // commDataGrid3
            // 
            this.commDataGrid3.DataPropertyName = "comm";
            this.commDataGrid3.HeaderText = "comm";
            this.commDataGrid3.Name = "commDataGrid3";
            this.commDataGrid3.Width = 60;
            // 
            // authorDataGrid3
            // 
            this.authorDataGrid3.DataPropertyName = "author";
            this.authorDataGrid3.HeaderText = "author";
            this.authorDataGrid3.Name = "authorDataGrid3";
            this.authorDataGrid3.Width = 62;
            // 
            // idHeadDataGrid3
            // 
            this.idHeadDataGrid3.DataPropertyName = "idHead";
            this.idHeadDataGrid3.HeaderText = "idHead";
            this.idHeadDataGrid3.Name = "idHeadDataGrid3";
            this.idHeadDataGrid3.Visible = false;
            this.idHeadDataGrid3.Width = 66;
            // 
            // idStrDataGrid3
            // 
            this.idStrDataGrid3.DataPropertyName = "idStr";
            this.idStrDataGrid3.HeaderText = "idStr";
            this.idStrDataGrid3.Name = "idStrDataGrid3";
            this.idStrDataGrid3.ReadOnly = true;
            this.idStrDataGrid3.Width = 53;
            // 
            // fKimpHeadimpStrBindingSource
            // 
            this.fKimpHeadimpStrBindingSource.DataMember = "FK_impHead_impStr";
            this.fKimpHeadimpStrBindingSource.DataSource = this.itmplheadBindingSource;
            this.fKimpHeadimpStrBindingSource.CurrentChanged += new System.EventHandler(this.fKimpHeadimpStrBindingSource_CurrentChanged);
            // 
            // itmplheadBindingSource
            // 
            this.itmplheadBindingSource.DataMember = "i_tmpl_head";
            this.itmplheadBindingSource.DataSource = this.bindingSource1;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = this.collectDataSet;
            this.bindingSource1.Position = 0;
            // 
            // bNTmplStr
            // 
            this.bNTmplStr.AddNewItem = this.bindingNavigatorAddNewItem1;
            this.bNTmplStr.CountItem = this.bindingNavigatorCountItem1;
            this.bNTmplStr.DeleteItem = this.bindingNavigatorDeleteItem1;
            this.bNTmplStr.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem1,
            this.bindingNavigatorMovePreviousItem1,
            this.bindingNavigatorSeparator3,
            this.bindingNavigatorPositionItem1,
            this.bindingNavigatorCountItem1,
            this.bindingNavigatorSeparator4,
            this.bindingNavigatorMoveNextItem1,
            this.bindingNavigatorMoveLastItem1,
            this.bindingNavigatorSeparator5,
            this.bindingNavigatorAddNewItem1,
            this.bindingNavigatorDeleteItem1,
            this.bindingNavigatorSaveItems2});
            this.bNTmplStr.Location = new System.Drawing.Point(0, 0);
            this.bNTmplStr.MoveFirstItem = this.bindingNavigatorMoveFirstItem1;
            this.bNTmplStr.MoveLastItem = this.bindingNavigatorMoveLastItem1;
            this.bNTmplStr.MoveNextItem = this.bindingNavigatorMoveNextItem1;
            this.bNTmplStr.MovePreviousItem = this.bindingNavigatorMovePreviousItem1;
            this.bNTmplStr.Name = "bNTmplStr";
            this.bNTmplStr.PositionItem = this.bindingNavigatorPositionItem1;
            this.bNTmplStr.Size = new System.Drawing.Size(297, 25);
            this.bNTmplStr.TabIndex = 1;
            this.bNTmplStr.Text = "bindingNavigator2";
            // 
            // bindingNavigatorAddNewItem1
            // 
            this.bindingNavigatorAddNewItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem1.Image")));
            this.bindingNavigatorAddNewItem1.Name = "bindingNavigatorAddNewItem1";
            this.bindingNavigatorAddNewItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem1.Text = "Добавить";
            // 
            // bindingNavigatorCountItem1
            // 
            this.bindingNavigatorCountItem1.Name = "bindingNavigatorCountItem1";
            this.bindingNavigatorCountItem1.Size = new System.Drawing.Size(43, 22);
            this.bindingNavigatorCountItem1.Text = "для {0}";
            this.bindingNavigatorCountItem1.ToolTipText = "Общее число элементов";
            // 
            // bindingNavigatorDeleteItem1
            // 
            this.bindingNavigatorDeleteItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem1.Image")));
            this.bindingNavigatorDeleteItem1.Name = "bindingNavigatorDeleteItem1";
            this.bindingNavigatorDeleteItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem1.Text = "Удалить";
            // 
            // bindingNavigatorMoveFirstItem1
            // 
            this.bindingNavigatorMoveFirstItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem1.Image")));
            this.bindingNavigatorMoveFirstItem1.Name = "bindingNavigatorMoveFirstItem1";
            this.bindingNavigatorMoveFirstItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem1.Text = "Переместить в начало";
            // 
            // bindingNavigatorMovePreviousItem1
            // 
            this.bindingNavigatorMovePreviousItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem1.Image")));
            this.bindingNavigatorMovePreviousItem1.Name = "bindingNavigatorMovePreviousItem1";
            this.bindingNavigatorMovePreviousItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem1.Text = "Переместить назад";
            // 
            // bindingNavigatorSeparator3
            // 
            this.bindingNavigatorSeparator3.Name = "bindingNavigatorSeparator3";
            this.bindingNavigatorSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem1
            // 
            this.bindingNavigatorPositionItem1.AccessibleName = "Положение";
            this.bindingNavigatorPositionItem1.AutoSize = false;
            this.bindingNavigatorPositionItem1.Name = "bindingNavigatorPositionItem1";
            this.bindingNavigatorPositionItem1.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem1.Text = "0";
            this.bindingNavigatorPositionItem1.ToolTipText = "Текущее положение";
            // 
            // bindingNavigatorSeparator4
            // 
            this.bindingNavigatorSeparator4.Name = "bindingNavigatorSeparator4";
            this.bindingNavigatorSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem1
            // 
            this.bindingNavigatorMoveNextItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem1.Image")));
            this.bindingNavigatorMoveNextItem1.Name = "bindingNavigatorMoveNextItem1";
            this.bindingNavigatorMoveNextItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem1.Text = "Переместить вперед";
            // 
            // bindingNavigatorMoveLastItem1
            // 
            this.bindingNavigatorMoveLastItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem1.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem1.Image")));
            this.bindingNavigatorMoveLastItem1.Name = "bindingNavigatorMoveLastItem1";
            this.bindingNavigatorMoveLastItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem1.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem1.Text = "Переместить в конец";
            // 
            // bindingNavigatorSeparator5
            // 
            this.bindingNavigatorSeparator5.Name = "bindingNavigatorSeparator5";
            this.bindingNavigatorSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorSaveItems2
            // 
            this.bindingNavigatorSaveItems2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorSaveItems2.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorSaveItems2.Image")));
            this.bindingNavigatorSaveItems2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bindingNavigatorSaveItems2.Name = "bindingNavigatorSaveItems2";
            this.bindingNavigatorSaveItems2.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorSaveItems2.Text = "toolStripButton1";
            this.bindingNavigatorSaveItems2.ToolTipText = "Сохранить данные";
            this.bindingNavigatorSaveItems2.Click += new System.EventHandler(this.bindingNavigatorSaveItems2_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ButtonOpen,
            this.ButtonMatching,
            this.BottomProcessing,
            this.ButtonStore});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(855, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ButtonOpen
            // 
            this.ButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("ButtonOpen.Image")));
            this.ButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonOpen.Name = "ButtonOpen";
            this.ButtonOpen.Size = new System.Drawing.Size(40, 22);
            this.ButtonOpen.Text = "&Open";
            this.ButtonOpen.Click += new System.EventHandler(this.Open_Click);
            // 
            // ButtonMatching
            // 
            this.ButtonMatching.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ButtonMatching.Enabled = false;
            this.ButtonMatching.Image = ((System.Drawing.Image)(resources.GetObject("ButtonMatching.Image")));
            this.ButtonMatching.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonMatching.Name = "ButtonMatching";
            this.ButtonMatching.Size = new System.Drawing.Size(62, 22);
            this.ButtonMatching.Text = "&Matching";
            this.ButtonMatching.ToolTipText = "Matching";
            this.ButtonMatching.Click += new System.EventHandler(this.matching_Click);
            // 
            // BottomProcessing
            // 
            this.BottomProcessing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.BottomProcessing.Enabled = false;
            this.BottomProcessing.Image = ((System.Drawing.Image)(resources.GetObject("BottomProcessing.Image")));
            this.BottomProcessing.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BottomProcessing.Name = "BottomProcessing";
            this.BottomProcessing.Size = new System.Drawing.Size(68, 22);
            this.BottomProcessing.Text = "&Processing";
            this.BottomProcessing.Click += new System.EventHandler(this.processing_Click);
            // 
            // ButtonStore
            // 
            this.ButtonStore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ButtonStore.Enabled = false;
            this.ButtonStore.Image = ((System.Drawing.Image)(resources.GetObject("ButtonStore.Image")));
            this.ButtonStore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonStore.Name = "ButtonStore";
            this.ButtonStore.Size = new System.Drawing.Size(38, 22);
            this.ButtonStore.Text = "&Store";
            this.ButtonStore.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar2,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 618);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStrip1.Size = new System.Drawing.Size(855, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar2
            // 
            this.toolStripProgressBar2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar2.AutoToolTip = true;
            this.toolStripProgressBar2.Name = "toolStripProgressBar2";
            this.toolStripProgressBar2.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(31, 17);
            this.toolStripStatusLabel1.Text = "Start";
            // 
            // i_tmpl_headTableAdapter
            // 
            this.i_tmpl_headTableAdapter.ClearBeforeFill = true;
            // 
            // i_tmpl_strTableAdapter
            // 
            this.i_tmpl_strTableAdapter.ClearBeforeFill = true;
            // 
            // i_tmpl_strBindingSource
            // 
            this.i_tmpl_strBindingSource.DataMember = "i_tmpl_str";
            this.i_tmpl_strBindingSource.DataSource = this.collectDataSet;
            // 
            // tableAdapterManager
            // 
            this.tableAdapterManager.attrValueTableAdapter = null;
            this.tableAdapterManager.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager.i_tmpl_headTableAdapter = this.i_tmpl_headTableAdapter;
            this.tableAdapterManager.i_tmpl_strTableAdapter = this.i_tmpl_strTableAdapter;
            this.tableAdapterManager.UpdateOrder = ExcelReader.CollectDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // i_tmpl_headBindingSource
            // 
            this.i_tmpl_headBindingSource.DataMember = "i_tmpl_head";
            this.i_tmpl_headBindingSource.DataSource = this.collectDataSet;
            // 
            // attrValueTableAdapter
            // 
            this.attrValueTableAdapter.ClearBeforeFill = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 640);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableXls)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRes)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvDataTree)).EndInit();
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemlpStr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.attrValueBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collectDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fKimpHeadimpStrBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itmplheadBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bNTmplStr)).EndInit();
            this.bNTmplStr.ResumeLayout(false);
            this.bNTmplStr.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.i_tmpl_strBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.i_tmpl_headBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridView dgvTableXls;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.DataGridView dgvTemlpStr;
        private System.Windows.Forms.BindingSource bindingSource1;
        private CollectDataSet collectDataSet;
        private System.Windows.Forms.BindingSource itmplheadBindingSource;
        private CollectDataSetTableAdapters.i_tmpl_headTableAdapter i_tmpl_headTableAdapter;
        private System.Windows.Forms.BindingSource fKimpHeadimpStrBindingSource;
        private CollectDataSetTableAdapters.i_tmpl_strTableAdapter i_tmpl_strTableAdapter;
        private System.Windows.Forms.BindingSource i_tmpl_strBindingSource;
        private CollectDataSetTableAdapters.TableAdapterManager tableAdapterManager;
        private System.Windows.Forms.BindingSource i_tmpl_headBindingSource;
        private System.Windows.Forms.BindingNavigator bNTmplStr;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem1;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator3;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator4;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator5;
        private System.Windows.Forms.ToolStripButton bindingNavigatorSaveItems2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton ButtonOpen;
        private System.Windows.Forms.ToolStripButton ButtonMatching;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dgvRes;
        private System.Windows.Forms.ToolStripButton ButtonStore;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripButton BottomProcessing;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.BindingSource attrValueBindingSource;
        private CollectDataSetTableAdapters.attrValueTableAdapter attrValueTableAdapter;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtColumnFilter;
        private BrightIdeasSoftware.DataTreeListView olvDataTree;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.TextBox textTmplName;
        private System.Windows.Forms.CheckBox ckTmplFields;
        private System.Windows.Forms.CheckBox ckResult;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRowsFilter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtResFilter;
        private System.Windows.Forms.Button btResFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtResApply;
        private BrightIdeasSoftware.HighlightTextRenderer highlightTextRenderer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn nppDataGrid3;
        private System.Windows.Forms.DataGridViewTextBoxColumn resNameDataGrid3;
        private System.Windows.Forms.DataGridViewTextBoxColumn xlsNameDataGrid3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isPrintDataGrid3;
        private System.Windows.Forms.DataGridViewComboBoxColumn attrDataGrid3;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataTypeDataGrid3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataSizeDataGrid3;
        private System.Windows.Forms.DataGridViewTextBoxColumn strFormatData3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isPosDataGrid3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isActiveDataGrid3;
        private System.Windows.Forms.DataGridViewTextBoxColumn commDataGrid3;
        private System.Windows.Forms.DataGridViewTextBoxColumn authorDataGrid3;
        private System.Windows.Forms.DataGridViewTextBoxColumn idHeadDataGrid3;
        private System.Windows.Forms.DataGridViewTextBoxColumn idStrDataGrid3;
    }
}

