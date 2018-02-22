using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ExcelReader
{
    class FieldExpr : FieldBase
    {
        public override object Value
        {
            get
            {
                return null;
            }
        }

        string serviseField {
            get
            {
                return String.Format("${0}", ResName);
            }
        }

        public void InitField()
        {
            DataTable table = Scan.ResTable;
            string result = String.Empty;
            if (!table.Columns.Contains(ResName))
            {
                DataColumn column = new DataColumn(serviseField, Type);
                column.Expression = XlsName;
                table.Columns.Add(column);
                table.Columns.Add(new DataColumn(ResName, Type));
            }
        }

        public override string InitValue()
        {
            string result = String.Empty;
            if (Scan.ResTable.Columns.Contains(ResName))
            {
                if (Type.Equals(typeof(double)) && DataSize > 0 )
                {
                    Scan.ResRow[ResName] = Math.Round((double)Scan.ResRow[serviseField], 2);
                }
                else
                {
                    Scan.ResRow[ResName] = Scan.ResRow[serviseField];
                }
            }
            else
            {
                result = "Exist";
            }
            return result;
        }

        public void RemoveField()
        {
            DataColumnCollection columns = Scan.ResTable.Columns;
            if (columns.Contains(serviseField))
            {
                columns.Remove(serviseField);
                if (!IsPrint) columns.Remove(ResName);

            }

        }
    }
}
