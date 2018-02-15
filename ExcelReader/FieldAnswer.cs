using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldAnswer : FieldBase
    {
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
