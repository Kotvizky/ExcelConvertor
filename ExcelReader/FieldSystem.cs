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
        }

        public bool isField { get; protected set; } = false;

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
