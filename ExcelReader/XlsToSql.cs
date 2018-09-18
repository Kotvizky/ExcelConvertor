using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace ExcelReader
{
    class XlsToSql
    {

        //public XlsToSql()
        //{
        //}

        //private TextBox textBox;

        ExcelFile file = new ExcelFile();


        #region fields array

        string[] fieldsAll = new string[] {
                "Кредитный договор",
                "№ расч#счета",
                "Программа кредитования",
                "Сегмент",
                "Проект",
                "Валюта договора",
                "ЕДРПОУ / ИНН",
                "ФИО Клиента",
                "Количество дней просрочки",
                "Бакет",
                "Дата передачи",
            };


        string[] fieldsSap = new string[] {
                "ACCNO" ,
                "ACCNO" ,
                "OVER"  ,
                "OVER"  ,
                "OVER"  ,
                "CODE_CUR"  ,
                "OKPO"  ,
                "NAME"  ,
                "DPD"   ,
                ""  ,
                "CUR_DATE",
                    };


        #endregion

        public string readList()
        {

            string report = string.Empty;
            string patch = @"c:\Users\IKotvytskyi\Documents\УкрСибБанк\";
            string[] lines = System.IO.File.ReadAllLines($"{patch}xls-file.txt ");
            int i = 0;
            DataTable resultTable = SQLFunction.GetResultTable("__ukrSib");

            foreach (string xlsPach in lines)
            {
                file.ReadFile($"{patch}{xlsPach}");
                DataTable table = file.XlsTable;
                int rowCount = 0;
                string missingFields = "";
                if (table != null)
                {
                    rowCount = table.Rows.Count;

                    string[] fields = fieldsAll;

                    if (table.Columns[0].ColumnName == fieldsSap[0])
                    {
                        fields = fieldsSap;
                    }

                    foreach (string colName in fields)
                    {
                        if (!table.Columns.Contains(colName) && colName != "")
                        {
                            missingFields += colName + "| ";
                        }
                    }

                    if (missingFields == "")
                        writeToSql(resultTable, xlsPach, table, fields);
                }

                report += $"\r\n{i++} {xlsPach} -- {rowCount} -- {missingFields}";
            }

            return report;
        }

        private static void writeToSql(DataTable resultTable, string xlsPach, DataTable table, string[] fields)
        {
            DataRow[] rows = new DataRow[table.Rows.Count];
            for (int count = 0; count < table.Rows.Count; count++)
            {
                DataRow newRow = resultTable.NewRow();
                for (int iField = 0; iField < fields.Length; iField++)
                {
                    if (fields[iField] != "")
                    {
                        if (table.Rows[count][fields[iField]] != DBNull.Value)
                            newRow[iField] = table.Rows[count][fields[iField]].ToString();
                    }
                }
                newRow["fname"] = xlsPach;
                newRow["id"] = count;
                rows[count] = newRow;
            }
            SQLFunction.BulkWrite("__ukrSib", rows);
        }
    }
}
