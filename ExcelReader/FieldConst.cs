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
            ValidValue result = Validator(new ValidData()
            {
                Value = xlsName,
                Size = DataSize
            });

            if (result.Error == String.Empty)
            {
                value = result.Value;
            }
            else
            {
                result.Error = String.Format("{0}:{1};", ResName, result.Error);
                value = null;
            }
            return result.Error;
        }

        //public Func<ValidData, ValidValue> Validator { set; get; }

    }
}
