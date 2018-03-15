using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class ParamToSql : ParamBase
    {
        public ParamToSql(string paramName, string outName, FieldFunc ownField) 
            : base(paramName, outName, ownField)
        {
        }

        public ParamToSql(string paramName, string outName, FieldFunc ownField, FieldBase field)
            : base(paramName, outName, ownField)
        {
            Field = field;
            XlsExist = Field.Exist;
        }

        public override object Value
        {
            get
            {
                return Field.Value;
            }
        }

        public override void InitField()
        {
            OwnField.SqlRow[OutName] = Value;
            if (Value.GetType() == typeof(DateTime))
            {
                if ((DateTime)Value == new DateTime(0001,1,1))
                {
                    OwnField.SqlRow[OutName] = DBNull.Value;
                }
            }
        }
    }
}
