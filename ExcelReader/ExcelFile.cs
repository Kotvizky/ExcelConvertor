using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;



namespace ExcelReader
{
    class ExcelFile
    {

        public delegate string SheetChoice(object[] sheetList);

        public event SheetChoice onSheetChoise;

        public DataTable XlsTable { private set; get; }

        public DataTable ResTable { set; get; }

        public delegate void ProgressBarStep();
        public event ProgressBarStep onStep;

        public void ReadFile(string fileName) {
            FileName = fileName;
            ReadExcelFile();
        }

        public string FileName {private set; get; }

        private string GetConnectionString()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

            // XLSX - Excel 2007, 2010, 2012, 2013
            props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
            props["Extended Properties"] = "Excel 12.0 XML";
            //            props["Data Source"] = "C:\\MyExcel.xlsx";
            //            props["Data Source"] = "c:\\Users\\IKotvytskyi\\Documents\\test.xlsx";
            //            props["Connect Timeout"] = "30";
            //props["Data Source"] = "c:\\Users\\IKotvytskyi\\Documents\\test1.xlsx";
            props["Data Source"] = FileName;

            // XLS - Excel 2003 and Older
            //props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
            //props["Extended Properties"] = "Excel 8.0";
            //props["Data Source"] = "C:\\MyExcel.xls";

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }
            return sb.ToString();
        }

        private void ReadExcelFile()
        {

            string connectionString = GetConnectionString();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                // Get all Sheets in Excel File
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                DataRow[] sheetRows = dtSheet.Select("TABLE_NAME like '%$'");
                if (sheetRows.Length > 0) {
                    string sheetName = sheetRows[0]["TABLE_NAME"].ToString();
                    if (sheetRows.Length > 1) {
                        if (onSheetChoise != null)
                        {
                            object[] sheetList = new object[sheetRows.Length];
                            for (int i = 0; i < sheetRows.Length; i++)
                            {
                                sheetList[i] = sheetRows[i]["TABLE_NAME"].ToString();
                            }
                            sheetName = onSheetChoise(sheetList);
                        }
                    }
                    cmd.CommandText = "SELECT * FROM [" + sheetName + "] ";
                    XlsTable = new DataTable();
                    XlsTable.TableName = sheetName;
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(XlsTable);
                }
                cmd = null;
                conn.Close();
            }
        }

        public void InitResTable(Scan scan) {
            var resField = scan.FindAll(x => x.IsPrint & x.IsActive);
            ResTable = new DataTable(); 
            foreach (FieldXls field in resField) {
                if (field.Attr == attrName.Field)
                {
                    ResTable.Columns.Add(field.ResName, typeof(String));
                } else if ((field.Attr == attrName.Answer) & (field.ResName.IndexOf(".") > 0))
                {
                    ResTable.Columns.Add(field.ResName.Split('.')[1], typeof(String));
                }
            }
        }

        public void ExportToXls(DataTable resultTable, bool showSource = false)
        {
            Excel.Application oXL;
            Excel._Workbook oWB;
            Excel._Worksheet oSheet;
            object misvalue = System.Reflection.Missing.Value;
            oXL = new Excel.Application();
            oWB = oXL.Workbooks.Add("");
            oSheet = oWB.ActiveSheet;
            drawTable(oSheet,resultTable, Excel.XlRgbColor.rgbLightGreen);
            if (showSource)
            {
                drawTable(oSheet, XlsTable, Excel.XlRgbColor.rgbAqua, resultTable.Columns.Count + 2, 1  );
            }

            oXL.Visible = true;
        }

        void drawTable(Excel._Worksheet oSheet, DataTable resultTable, Excel.XlRgbColor xlsColor,
            int startCol = 1, int startRow = 1 )
        {
            Excel.Range oRng;
            string[] title = resultTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            oRng = oSheet.get_Range(xlsAdress(startCol, startRow), xlsAdress(startCol - 1 + title.Length, startRow));
            oRng.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            oRng.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            oRng.Font.Bold = true;
            oRng.Value2 = title;
            oRng.Interior.Color = xlsColor;

            string[,] data = new string[resultTable.Rows.Count, resultTable.Columns.Count];

            for (int r = 0; r < resultTable.Rows.Count; r++)
            {
                for (int c = 0; c < resultTable.Columns.Count; c++)
                {
                    data[r, c] = resultTable.Rows[r][c].ToString();
                }
            }

            oRng = oSheet.get_Range(xlsAdress(startCol, startRow +1),
                                    xlsAdress(startCol-1 + data.GetLength(1), startRow - 1 + data.GetLength(0) + 1));
            oRng.Value2 = data;
            oRng.EntireColumn.AutoFit();
        }

        private string xlsAdress(int col, int row)
        {
            col--;
            int diff = 64;
            int longht = 26;
            int num1 = col / longht;
            int num2 = col % longht;
            string res = (num1 > 0) ? String.Format("{0}{1}",
                                        (char)(num1 + diff), (char)(num2 + diff + 1)) 
                                        : String.Format("{0}", (char)(num2 + diff + 1));

            return String.Format("{0}{1}", res, row);
        }

        
    }
}

