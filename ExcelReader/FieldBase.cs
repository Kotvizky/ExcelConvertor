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
        
        public FieldBase()
        {

        }
        // fields in database
        public short Npp { set; get; }
        public string ResName { set; get; }
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

        public void initFields(
            short npp,
            string resName,
            string xlsName,
            bool isPrint,
            attrName attr,
            bool isActive,
            DataRow xlsRow,
            DataRow resRow,
            string typeVal,
            int dataSize
        )
        {
            Npp = npp;
            ResName = resName;
            XlsName = xlsName;
            IsPrint = isPrint;
            Attr = attr;
            IsActive = isActive;
            Type = Type.GetType(typeVal);
        }

        public Scan Scan;
        public abstract string InitValue();
    }
}
