using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class ParamAnswer : ParamBase
    {
        public ParamAnswer(string paramName, string outName, FieldFunc ownField)
            : base(paramName, outName, ownField)
        {
        }

        public ParamAnswer(string paramName, string outName, FieldFunc ownField, FieldBase field)
            : this(paramName, outName, ownField)
        {
            if (field != null)
            {
                Field = field;
                ((FieldAnswer)field).Param = this;
            }
        }

        public override object Value
        {
            get
            {
                return OwnField.SQLRow[this.ParamName];
            }
        }

        public override void InitField()
        {
            OwnField.ResRow[OutName] = Value;
        }
    }
}
