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
using System.Reflection;


namespace ExcelReader
{
    class ReportBuilder
    {

        public ReportBuilder(string jsonSchema,DataTable _resTable)
        {
            var serializer = new JavaScriptSerializer();
            schema = serializer.DeserializeObject(jsonSchema);
            string fileName = schema["fileName"];
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            fullFileName = $@"{directory}\tmpl\{fileName}";
            if (!File.Exists(fullFileName))
            {
                fullFileName = "";
            }
            oXL = new Excel.Application();
            string formulaFile  = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{XLS_FUNC_PATCH}";

            if (File.Exists(formulaFile))
            {
                oXL.Workbooks.Add(formulaFile);
            }
            oWB = oXL.Workbooks.Add(fullFileName);
            oSheet = oWB.ActiveSheet;
            resultTable = _resTable;
            try
            {
                var reportRange = schema["reportRange"];
                CleanReport(reportRange);
            }
            catch (KeyNotFoundException e)
            {
            }

            RenemeCells();
            cuirveFormulaModify();

            insertTable(oSheet, 1, schema["startRow"]);
            oXL.Visible = true;
        }

        #region Variables

        Excel.Application oXL;
        Excel._Workbook oWB;
        Excel._Worksheet oSheet;
        string fullFileName;
        DataTable resultTable;

        dynamic schema;

        const string XLS_FUNC_PATCH = @"Microsoft\Office\sumpropua.xla";
        const string XLS_LOCAL_FUNC_PATCH = @"C:\Users\IKotvytskyi\AppData\Local\";
        readonly string[] CUIRSIVE = { "SummTotalCuirsive", "SummComissionCuisive" };
        const string EMPY_LINK = "#ССЫЛКА";

        #endregion  

        void CleanReport(object[] borders)
        {
            if (borders.Length > 0)
            {
                DeleteRows(1, (int)borders[0] - 1 );
            }
            if (borders.Length > 1)
            {
                DeleteRows((int)borders[1] - (int)borders[0] + 2, LastColumn());
            }
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

        void DeleteRows(int startCol, int finishCol)
        {
            if (startCol >= finishCol)
            {
                return;
            }

            Excel.Range range = (Excel.Range)oSheet.get_Range(xlsAdress(startCol, 1),xlsAdress(finishCol,1));
            range.EntireColumn.Delete(Missing.Value);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
        }

        int LastColumn()
        {
            return oSheet.UsedRange.Columns.Count;
        }

        void RenemeCells()
        {
            //var names = oSheet.Names;
            const string SEP = "__";

            List<string> names = new List<string>();
            dynamic workBook = oWB.Names;

            foreach (dynamic name in oWB.Names)
            {
                //string n 

                string nameLocal = name.NameLocal;
                string refersToLocal = name.RefersToLocal;
                string newName = String.Empty;
                if (nameLocal.Contains(SEP))
                {
                    if (refersToLocal.Contains(EMPY_LINK))
                    {
                        name.Delete();
                        continue;
                    }
                    newName = nameLocal.Substring(nameLocal.IndexOf(SEP) + 2);
                    workBook.Item(newName, Missing.Value, Missing.Value).RefersToLocal = name.RefersToLocal;
                }
                names.Add(name.NameLocal);     //  Name of cell 
            }
        }

        void cuirveFormulaModify()
        {
            foreach (string formulaAddress in CUIRSIVE)
            {
                dynamic workBook = oWB.Names;
                dynamic cell =  workBook.Item(formulaAddress, Missing.Value, Missing.Value).RefersToRange;
                string formula = cell.FormulaLocal;
                formula = formula.Replace($"'{XLS_LOCAL_FUNC_PATCH}{XLS_FUNC_PATCH}'!","");
                cell.FormulaLocal = formula;
            }
        }

        void insertTable(Excel._Worksheet oSheet,  int startCol = 1, int startRow = 1)
        {
            Excel.Range oRng;

            string[,] data = new string[resultTable.Rows.Count, resultTable.Columns.Count];

            CultureInfo curCI = new CultureInfo(CultureInfo.CurrentCulture.Name);
            curCI.NumberFormat.NumberDecimalSeparator = Properties.Settings.Default.NumberDecimalSeparator;
            System.Threading.Thread.CurrentThread.CurrentCulture = curCI;

            Excel.Range rngToCopy = oSheet.get_Range($"{Convert.ToString(startRow + 1 )}:{Convert.ToString(startRow + resultTable.Rows.Count - 1 )}");
//            rngToCopy.Copy();
            rngToCopy.Insert();

            for (int r = 0; r < resultTable.Rows.Count; r++)
            {
                for (int c = 0; c < resultTable.Columns.Count; c++)
                {
                    if (resultTable.Rows[r][c].GetType().Name == "DateTime")
                        data[r, c] = ((DateTime)resultTable.Rows[r][c]).ToString(Properties.Settings.Default.ShortDatePattern);
                    else data[r, c] = resultTable.Rows[r][c].ToString();
                }
            }

            oRng = oSheet.get_Range(xlsAdress(startCol, startRow ),
                                    xlsAdress(startCol - 1 + data.GetLength(1), startRow - 1 + data.GetLength(0) ));
            oRng.Cells.Value2 = data;


            for (int c = 0; c < data.GetLength(1); c++)
            {
                dynamic cell = oSheet.get_Range(xlsAdress(startCol + c, startRow),
                                        xlsAdress(startCol + c, startRow));

                if (cell.DisplayFormat.NumberFormatLocal != "General")
                {
                    oRng = oSheet.get_Range(xlsAdress(startCol + c, startRow),
                                            xlsAdress(startCol + c, startRow - 1 + data.GetLength(0)));
                    oRng.Cells.TextToColumns();
                }
            }
            //            oRng.EntireColumn.AutoFit();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);
        }

    }
}
