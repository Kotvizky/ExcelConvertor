using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldAnswer : FieldBase
    {
        public FieldAnswer(DataRow row, Scan scan) : base(row, scan)
        {
        }

        public ParamAnswer Param { get; set; }

        public override object Value
        {
            get
            {
                return Param.Value;
            }
        }

        public override string InitValue()
        {
            throw new NotImplementedException();
        }
    }
}
