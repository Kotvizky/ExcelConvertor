using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ExcelReader
{
    abstract class FieldBase
    {
        
        public FieldBase(DataRow row, Scan scan)
        {
            Npp = (short)row["npp"];
            ResName = (string)row["resName"];
            XlsName = (string)row["xlsName"];
            IsPrint = (bool)row["isPrint"];
            Attr = (attrName)row["attr"];
            IsActive = (bool)row["isActive"];
            Type = Type.GetType(String.Format("System.{0}", (string)row["DataType"] ));
            DataSize = (short)row["dataSize"];
            xlsColName = row["xlsColName"] == DBNull.Value? "":(string)row["xlsColName"];
            xlsFormat = row["xlsFormat"] == DBNull.Value ? "" : (string)row["xlsFormat"];
            Scan = scan;
        }

        public short Npp { set; get; }
        public string ResName { set; get; }
        public Scan Scan;
        protected string xlsName;
        public virtual string XlsName
        {
            set
            {
                xlsName = value;
            }
            get
            {
                return xlsName;
            }
        }
        public bool IsPrint { set; get; }
        protected attrName attr;
        public virtual attrName Attr
        {
            set
            {
                attr = value;
            }
            get
            {
                return attr;
            }
        }
        public string xlsColName { set; get; }
        public string xlsFormat { set; get; }

        public DataRow XlsRow {
            get
            {
                return Scan.XlsRow;
            }
        }

        public virtual DataRow ResRow {
            get
            {
                return Scan.ResRow;
            }
        }
        public Type Type { set; get; }
        public short DataSize { set; get; }

        public string StrFormat { set; get; }
        public bool IsActive { set; get; }

        public Func<ValidData, ValidValue> Validator { set; get; } = null;

        public bool Exist { set; get; }

        public abstract object Value { get; }

        public abstract string InitValue();
    }
}
