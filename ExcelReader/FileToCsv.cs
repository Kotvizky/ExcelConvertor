using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;


namespace ExcelReader
{
    class FileToCsv
    {

        public FileToCsv(DataTable _resTable, DataTable _xlsTable, string _fileName, string _schema)
        {
            resTable = _resTable;
            xlsTable = _xlsTable;
            fileName = _fileName;
            objShema(_schema);
            schemaCheck();
            if (!schemaError)
            {
                createCsvFromRes();
                tableToFile(csvTable, fileName);
            }
        }

        DataTable resTable;
        DataTable xlsTable;
        DataTable csvTable;
        Join join;

        string fileName;
        public string schemaJson { get; private set; }
        public string wrongFields { get; private set; } = string.Empty;

        dynamic schema;

        public bool schemaError { get; private set; } = false;

        void objShema(string _schemaJson)
        {
            schemaJson = _schemaJson;
            var serializer = new JavaScriptSerializer();
            schema = serializer.DeserializeObject(schemaJson);
        }

        void schemaCheck()
        {

            join = new Join() {
                xls = schema["join"]["xls"] ,
                res = schema["join"]["res"]
            };

            checkWrongFields(xlsTable, join.xls, "xls");
            checkWrongFields(resTable, join.res, "res");

            foreach (object[] pair in schema["replace"])
            {
                checkWrongFields(xlsTable, pair[0].ToString(), "xls");
                checkWrongFields(resTable, pair[1].ToString(), "res");
            }
            if (wrongFields != string.Empty)
            {
                wrongFields = wrongFields.Remove(wrongFields.Length - 2, 2);
            }
        }

        void checkWrongFields(DataTable table, string fieldName, string tableName)
        {
            if (!table.Columns.Contains(fieldName))
            {
                wrongFields += $"{tableName}: {fieldName}; ";
                schemaError = true;
            }
        }

        void createCsvFromRes()
        {
            csvTable = xlsTable.Clone();
            for (int i = 0; i < resTable.Rows.Count; i++)
            {
                DataRow resRow = resTable.Rows[i];
                DataRow xlsRow = xlsTable.Select($"{join.xls} = {resRow[join.res].ToString()}")[0];
                csvTable.Rows.Add(xlsRow.ItemArray);
                DataRow csvRow = csvTable.Rows[i];
                foreach (object[] pair in schema["replace"])
                {
                    csvRow[pair[0].ToString()] = resRow[pair[1].ToString()];
                }
            }
            if (csvTable.Columns.Contains(Constants.ROW_ID))
            {
                csvTable.Columns.Remove(Constants.ROW_ID);
            }
            foreach (object[] pair in schema["replace"])
            {
                if (pair.Length == 3)
                {
                    csvTable.Columns[pair[0].ToString()].ColumnName = pair[2].ToString();
                }
            }

        }

        public static void tableToFile(DataTable table, string fileName) {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(Constants.SEP, columnNames));

            foreach (DataRow row in table.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString().Replace(Constants.SEP,"").Replace("'", "’"));
                sb.AppendLine(string.Join(Constants.SEP, fields));
            }
            File.WriteAllText(fileName, sb.ToString(), Encoding.GetEncoding(Constants.CODEPAGE));
        }

        struct Join 
        {
            public string res;
            public string xls;
        }

        static class Constants
        {
            public const string SEP = "|";
            public const string CODEPAGE = "windows-1251";
            public const string ROW_ID = "$ROW_ID"; // system field 
        }

    }
}
