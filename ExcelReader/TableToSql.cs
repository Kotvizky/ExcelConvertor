using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ExcelReader
{
    class TableToSql
    {

        public TableToSql(DataTable table,SqlConnection _conn)
        {
            xlsTable = table;
            tType = getType();
            tableName = "##" + Environment.UserName.ToLower();
            createTableSctipt = getCrateScript();
            conn = _conn;
        }

        SqlConnection conn;
        DataTable xlsTable;
        DataTable tType;
        string createTableSctipt;
        public string tableName { get; private set; }

        DataTable getType()
        {
            return SQLFunction.ExecuteSQL("select csType, sqlType from[matchTmpType] ");
        }

        string getCrateScript()
        {
            string query;

            query = $@"IF OBJECT_ID('tempdb..{tableName}') IS NOT NULL   DROP TABLE {tableName}
                    create table {tableName} (";

            foreach (DataColumn column in xlsTable.Columns)
            {
                string columnType = column.DataType.Name;
                if (column.ColumnName == "$ROW_ID")
                {
                    columnType = "int";
                }
                else
                {
                    foreach (DataRow row in tType.Rows)
                    {
                        if (row["csType"].ToString() == columnType)
                        {
                            columnType = row["sqlType"].ToString();
                            break;
                        }
                    }
                }
                query += $"\n[{column.ColumnName}]  {columnType} ,";
            }
            query = query.Remove(query.Length - 1, 1) + ")";
            return query;
        }

        public void createTempTable()
        {
            SQLFunction.ExecuteNonQuery(createTableSctipt, false,conn);
        }

        public void bulkData()
        {
            int rowCount = xlsTable.Rows.Count;
            DataTable sourceTable = xlsTable.Copy();
//            sourceTable.Columns.Remove("$ROW_ID");
            DataRow[] data = new DataRow[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                data[i] = sourceTable.Rows[i];
            }
//            MessageBox.Show(tabl.Rows.Count.ToString());
            SQLFunction.BulkWrite($"{tableName}", data,conn);
        }

        public void dropTemplate()
        {
            string query = $@"IF OBJECT_ID('tempdb..{tableName}') IS NOT NULL   DROP TABLE {tableName}";
            SQLFunction.ExecuteNonQuery(query, false,conn);
        }
    }
}
