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

        public delegate void ProgressBarStep();
        public event ProgressBarStep onStep;

        public void ReadFile(string fileName) {
            FileName = fileName;
            ReadExcelFile();
        }

        public DataTable XlsTable { private set; get; }

        public DataTable ResTable { set; get; }

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
            foreach (Field field in resField) {
                ResTable.Columns.Add(field.ResName, typeof(string));
            }
        }

        public void WriteResult(Scan scan) {  //TODO Add event for strip progress bar
            foreach (DataRow row in XlsTable.Rows) {
                DataRow newRow = ResTable.NewRow();
                scan.SetValues(row);
                foreach (DataColumn column in ResTable.Columns) {
                    newRow[column.ColumnName] = scan.GetValue(column.ColumnName);
                }
                ResTable.Rows.Add(newRow);
                onStep?.Invoke();
            }
        }

        public void ExportToXls()
        {
            Microsoft.Office.Interop.Excel.Application oXL;
            Microsoft.Office.Interop.Excel._Workbook oWB;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            Microsoft.Office.Interop.Excel.Range oRng;
            object misvalue = System.Reflection.Missing.Value;

            oXL = new Microsoft.Office.Interop.Excel.Application();

            oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
            oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            oSheet.Application.ReferenceStyle = Excel.XlReferenceStyle.xlR1C1;

            string[] title = ResTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            oRng = oSheet.get_Range(xlsAdress(1, 1), xlsAdress(title.Length, 1));
            oRng.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            oRng.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            oRng.Font.Bold = true;
            oRng.Value2 = title;
            oRng.Interior.Color = Excel.XlRgbColor.rgbLightGreen;
            // oRng.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);

            string[,] data = new string[ResTable.Rows.Count, ResTable.Columns.Count];

            //            ResTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

            for (int r = 0; r < ResTable.Rows.Count; r++) {
                for (int c = 0; c < ResTable.Columns.Count; c++) {
                    data[r,c] = ResTable.Rows[r][c].ToString();
                }
            }

            oRng = oSheet.get_Range(xlsAdress(1, 2), xlsAdress(data.GetLength(1), data.GetLength(0) + 1 ));
            oRng.Value2 = data;
            oRng.EntireColumn.AutoFit();

            oXL.Visible = true;
        }

        private string xlsAdress(int col, int row)
        {
            return String.Format("{0}{1}", (char)(col + 64), row);
        }

        
    }
}

