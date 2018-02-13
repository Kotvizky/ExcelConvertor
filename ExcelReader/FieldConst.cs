using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldConst : FieldBase
    {

        object value;

        public override object Value
        {
            get
            {
                return value;
            }
        }

        public override string InitValue()
        {
            throw new NotImplementedException();
        }
    }
}
