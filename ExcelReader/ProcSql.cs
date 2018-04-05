using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ExcelReader
{
    class ProcSql
    {
        public ProcSql(DataRow row)
        {
            SpName = (string)row["SpName"];
            BtName = (string)row["btName"];
            InTable = (string)row["InTable"];
            InPar = (string)row["inPar"];
            OutPar = (string)row["OutPar"];
            isFound = setInFields(row);
        }

        public string SpName { private set; get; }
        public string BtName { private set; get; }
        public string InTable { private set; get; }
        public string InPar { private set; get; }
        public string OutPar { private set; get; }
        public bool isFound { private set; get; }

        public Dictionary<string, string> inFields { private set; get; }

        bool setInFields(DataRow row)
        {
            bool result = true;
            string[] tabFields = ((string)row["tabFields"]).Split(',');
            string[] tmplFields = ((string)row["tmplFields"]).Split(',');
            if (tabFields.Length != tmplFields.Length) result = false;
            if (result)
            {
                foreach (string field in tabFields)
                {
                    if (field.Length == 0)
                    {
                        result = false;
                        break;
                    } 
                }
            }
            if (result)
            {
                foreach (string field in tmplFields)
                {
                    if (field.Length == 0)
                    {
                        result = false;
                        break;
                    }
                }
            }
            if (result)
            {
                inFields = new Dictionary<string, string>();
                for (int i = 0; i < tabFields.Length; i++)
                {
                    inFields.Add(tabFields[i].Trim(), tmplFields[i].Trim());
                }
            }
            return result;
        }

    }
}
