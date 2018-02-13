using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldXls : FieldBase
    {
        private const string noData = "Н/Д";

        private string value;

        public override string InitValue()
        {
            ValidValue result = Validator(new ValidData() {
                Value = XlsRow[xlsName],
                Size = DataSize
            });

            if (result.Error == String.Empty) {
                ResRow[ResName] = result.Value;
            }
            else
            {
                result.Error = String.Format("{0}:{1};",ResName, result.Error);
            }
            return result.Error;
        }

        public override object Value
        {
            get
            {
                return ResRow[ResName];
            }
        }

        public Func<ValidData, ValidValue> Validator { set; get; }

        public struct ValidValue{
            public object Value;
            public string Error;
        }

        public struct ValidData
        {
            public object Value;
            public int Size;
        }
    }

    enum attrName : byte { Field = 0, Func, Answer, Const };
    enum funcParameter : byte { Table = 1, In, Out }
    enum spetialFields { Ip, RowId }
    enum paramGroup  { inTable, tabFields, inPar, outPar }
    enum serviseFields {IP,ROW_ID}
    enum dataType {String,DateTime,Double}

}


