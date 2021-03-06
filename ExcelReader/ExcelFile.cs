﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;


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
                XlsTable = null;
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                // Get all Sheets in Excel File
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                DataRow[] sheetRows = dtSheet.Select("TABLE_NAME like '*$''' or TABLE_NAME like '*$' ");
                if (sheetRows.Length > 0) {
                    string sheetName = sheetRows[0]["TABLE_NAME"].ToString();
                    if (sheetRows.Length > 1) {
                        if (onSheetChoise != null)
                        {
                            object[] sheetList = new object[sheetRows.Length];
                            for (int i = 0; i < sheetRows.Length; i++)
                            {
                                sheetName = sheetRows[i]["TABLE_NAME"].ToString();
                                sheetList[i] = sheetName.Substring(0,sheetName.Length - 1);
                            }
                            sheetName = onSheetChoise(sheetList) + "$";
                        }
                    }
                    cmd.CommandText = "SELECT * FROM [" + sheetName + "] ";
                    XlsTable = new DataTable();
                    XlsTable.TableName = sheetName;
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(XlsTable);
                    XlsTable.TableName = "XlsTable";
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

        public void ExportToXls(DataTable resultTable, Scan scan, bool showSource = false)
        {

            var reportTemplate = scan.Find(x => x.ResName == "XLS_REP" && x.Attr == attrName.System);
            if (reportTemplate != null)
            {
                //dataToXls(oSheet, scan, resultTable, xlsColor, startCol, startRow);
                ReportBuilder repot = new ReportBuilder(reportTemplate.XlsName, resultTable);
            }
            else
            {

                Excel.Application oXL;
                Excel._Workbook oWB;
                Excel._Worksheet oSheet;
                //object misvalue = System.Reflection.Missing.Value;
                oXL = new Excel.Application();
                //-- new code
                string fileName = @"c:\Users\IKotvytskyi\Documents\my-template.xltx";

                if (!File.Exists(fileName))
                    fileName = "";
                oWB = oXL.Workbooks.Add(fileName);

                // <-- end code

                //            oWB = oXL.Workbooks.Add("");
                oSheet = oWB.ActiveSheet;

                drawTable(oSheet, scan, resultTable, Excel.XlRgbColor.rgbLightGreen);
                if (showSource)
                {
                    drawTable(oSheet, scan, XlsTable, Excel.XlRgbColor.rgbAqua, resultTable.Columns.Count + 2, 1);
                }
                oXL.Visible = true;
            }

        }


        //void drawTable(Excel._Worksheet oSheet, Scan scan, DataTable resultTable, Excel.XlRgbColor xlsColor,
        //    int startCol = 1, int startRow = 1)
        //{
        //    var reportTemplate = scan.Find(x => x.ResName == "XLS_REP" && x.Attr == attrName.System);
        //    if (reportTemplate == null)
        //    {
        //        dataToXls(oSheet, scan, resultTable, xlsColor, startCol, startRow);
        //    }
        //    else
        //    {

        //    }

        //}


        void drawTable(Excel._Worksheet oSheet, Scan scan,DataTable resultTable, Excel.XlRgbColor xlsColor,
            int startCol = 1, int startRow = 1 )
        {
            Excel.Range oRng;
            string[] title = resultTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            for (int i = 0; i < title.Count(); i++)
            {
                string colName = title[i];
                var newName = scan.Find(x => x.IsActive && x.ResName == colName);
                if (newName != null)
                {
                    if (newName.xlsColName != "") title[i] = newName.xlsColName;
                }
            }

            oRng = oSheet.get_Range(xlsAdress(startCol, startRow), xlsAdress(startCol - 1 + title.Length, startRow));
            oRng.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            oRng.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            oRng.Font.Bold = true;
            oRng.Value2 = title;
            oRng.Interior.Color = xlsColor;

            string[,] data = new string[resultTable.Rows.Count, resultTable.Columns.Count];

            CultureInfo curCI = new CultureInfo(CultureInfo.CurrentCulture.Name);
            curCI.NumberFormat.NumberDecimalSeparator = Properties.Settings.Default.NumberDecimalSeparator;
            System.Threading.Thread.CurrentThread.CurrentCulture = curCI;

            for (int r = 0; r < resultTable.Rows.Count; r++)
            {
                for (int c = 0; c < resultTable.Columns.Count; c++)
                {
                    if (resultTable.Rows[r][c].GetType().Name == "DateTime")
                        data[r, c] = ((DateTime)resultTable.Rows[r][c]).ToString(Properties.Settings.Default.ShortDatePattern);
                    else data[r, c] = resultTable.Rows[r][c].ToString();
                }
            }


            oRng = oSheet.get_Range(xlsAdress(startCol, startRow + 1),
                                    xlsAdress(startCol - 1 + data.GetLength(1), startRow - 1 + data.GetLength(0) + 1));
            oRng.Cells.Value = data;

            for (int i = 0; i < title.Count(); i++)
            {
                string colName = title[i];
                var newName = scan.Find(x => x.IsActive && x.ResName == colName);
                if (newName != null)
                {
                    if (newName.xlsFormat != "")
                    {
                        oRng = oSheet.get_Range(xlsAdress(startCol + i, startRow + 1),
                                                xlsAdress(startCol + i, startRow - 1 + data.GetLength(0) + 1));
                        oRng.EntireColumn.TextToColumns();
                        oRng.EntireColumn.NumberFormat = newName.xlsFormat;
                    }
                    //                        title[i] = newName.xlsColName;
                }
            }

            oRng.EntireColumn.AutoFit();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);
        }

        void insertTable(Excel._Worksheet oSheet, Scan scan, DataTable resultTable, Excel.XlRgbColor xlsColor,
            int startCol = 1, int startRow = 1)
        {
            Excel.Range oRng;
            string[] title = resultTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            for (int i = 0; i < title.Count(); i++)
            {
                string colName = title[i];
                var newName = scan.Find(x => x.IsActive && x.ResName == colName);
                if (newName != null)
                {
                    if (newName.xlsColName != "") title[i] = newName.xlsColName;
                }
            }
            oRng = oSheet.get_Range(xlsAdress(startCol, startRow), xlsAdress(startCol - 1 + title.Length, startRow));
            oRng.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            oRng.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            oRng.Font.Bold = true;
            oRng.Value2 = title;
            oRng.Interior.Color = xlsColor;

            string[,] data = new string[resultTable.Rows.Count, resultTable.Columns.Count];

            CultureInfo curCI = new CultureInfo(CultureInfo.CurrentCulture.Name);
            curCI.NumberFormat.NumberDecimalSeparator = Properties.Settings.Default.NumberDecimalSeparator;
            System.Threading.Thread.CurrentThread.CurrentCulture = curCI;

            for (int r = 0; r < resultTable.Rows.Count; r++)
            {
                for (int c = 0; c < resultTable.Columns.Count; c++)
                {
                    if (resultTable.Rows[r][c].GetType().Name == "DateTime")
                        data[r, c] = ((DateTime)resultTable.Rows[r][c]).ToString(Properties.Settings.Default.ShortDatePattern);
                    else data[r, c] = resultTable.Rows[r][c].ToString();
                }
            }


            oRng = oSheet.get_Range(xlsAdress(startCol, startRow + 1),
                                    xlsAdress(startCol - 1 + data.GetLength(1), startRow - 1 + data.GetLength(0) + 1));
            oRng.Cells.Value = data;

            for (int i = 0; i < title.Count(); i++)
            {
                string colName = title[i];
                var newName = scan.Find(x => x.IsActive && x.ResName == colName);
                if (newName != null)
                {
                    if (newName.xlsFormat != "")
                    {
                        oRng = oSheet.get_Range(xlsAdress(startCol + i, startRow + 1),
                                                xlsAdress(startCol + i, startRow - 1 + data.GetLength(0) + 1));
                        oRng.EntireColumn.TextToColumns();
                        oRng.EntireColumn.NumberFormat = newName.xlsFormat;
                    }
                }
            }

            oRng.EntireColumn.AutoFit();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);
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

