using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.IO;


namespace ExcelReader
{
    class ReportBuilder
    {
        public ReportBuilder(string jsonShema,DataTable _resTable)
        {
            var serializer = new JavaScriptSerializer();
            schema = serializer.DeserializeObject(jsonShema);
            string fileName = schema["fileName"];
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            fullFileName = $@"{directory}\tmpl\{fileName}";
            if (!File.Exists(fullFileName))
            {
                fullFileName = "";
            }
            oXL = new Excel.Application();
            oWB = oXL.Workbooks.Add(fullFileName);
            oSheet = oWB.ActiveSheet;
            resultTable = _resTable;
            insertTable(oSheet, 1, schema["startRow"]);
            oXL.Visible  = true;

        }

        Excel.Application oXL;
        Excel._Workbook oWB;
        Excel._Worksheet oSheet;
        string fullFileName;
        DataTable resultTable;

//        DataTable 
//object misvalue = System.Reflection.Missing.Value;

        dynamic schema;

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

        void insertTable(Excel._Worksheet oSheet,  int startCol = 1, int startRow = 1)
        {
            Excel.Range oRng;

            //string[] title = resultTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            //for (int i = 0; i < title.Count(); i++)
            //{
            //    string colName = title[i];
            //    var newName = scan.Find(x => x.IsActive && x.ResName == colName);
            //    if (newName != null)
            //    {
            //        if (newName.xlsColName != "") title[i] = newName.xlsColName;
            //    }
            //}
            //oRng = oSheet.get_Range(xlsAdress(startCol, startRow), xlsAdress(startCol - 1 + title.Length, startRow));
            //oRng.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            //oRng.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //oRng.Font.Bold = true;
            //oRng.Value2 = title;
            //oRng.Interior.Color = xlsColor;

            string[,] data = new string[resultTable.Rows.Count, resultTable.Columns.Count];

            CultureInfo curCI = new CultureInfo(CultureInfo.CurrentCulture.Name);
            curCI.NumberFormat.NumberDecimalSeparator = Properties.Settings.Default.NumberDecimalSeparator;
            System.Threading.Thread.CurrentThread.CurrentCulture = curCI;


            //Excel.Range rangeToInsert = oSheet.get_Range("a" + Convert.ToString(startRow), "z" + Convert.ToString(startRow));
            //Excel.Range rangeToInsertRow = rangeToInsert.EntireRow;
            //rangeToInsertRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown, true);
            //Excel.Range InsertedRowRange = oSheet.get_Range("a" + Convert.ToString(startRow + 1), "z" + Convert.ToString(startRow +1));
            //rangeToInsert.Copy(InsertedRowRange);


            
            for (int i = 0; i < resultTable.Rows.Count - 1; i++ )
            {
                Excel.Range rngToCopy = oSheet.get_Range($"{Convert.ToString(startRow + i + 1)}:{Convert.ToString(startRow + i + 1)}");
                rngToCopy.Copy();
                //                rngToCopy.Offset[1, 0].Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                //rngToCopy.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                rngToCopy.Insert();
            }

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
            //            oRng.TextToColumns();

            int totalRow = data.GetLength(0);

            for (int c = 0; c < resultTable.Columns.Count ; c++)
            {
                for (int r = 0; r < totalRow ; r++)
                {
                    oRng = oSheet.get_Range(xlsAdress(startCol+ c, startRow + 1 + r), 
                        xlsAdress(startCol + c, startRow + 1 + r));
                    if (oRng.Value != null)
                    {
                        oRng.TextToColumns();
                    }
                }
            }

            //            oRng.EntireColumn.AutoFit();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);
        }



    }
}
