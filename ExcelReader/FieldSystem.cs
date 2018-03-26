using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldSystem : FieldBase
    {

        public FieldSystem(DataRow row, Scan scan) : base(row, scan)
        {
            if (xlsName == Scan.ROW_ID)
            {
                isField = true;
                isRowId = true;
                Type = Type.GetType("System.Int32");
            }
            else if (ResName.Substring(0, Scan.SHEMA.Length) == Scan.SHEMA)
            {
                isShema = true;
                funcName = FieldFunc.getFuncName(xlsName);
            }
        }

        public bool isField { get; protected set; } = false;
        public bool isShema { get; protected set; } = false;
        public string funcName { get; protected set; } 

        bool isRowId = false;

        public override object Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string InitValue()
        {
            ResRow[ResName] = XlsRow[xlsName];
            return String.Empty;
            //throw new NotImplementedException();
        }
    }
}
