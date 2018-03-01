using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldSystem : FieldBase
    {
        public override object Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string InitValue()
        {
            return this.ToString();
            //throw new NotImplementedException();
        }
    }
}
