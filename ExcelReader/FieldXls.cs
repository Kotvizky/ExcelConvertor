using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldXls: FieldBase
    {
        private const string noData = "Н/Д";

        public bool isPos;

        public FieldXls(DataRow row, Scan scan) : base(row, scan)
        {
            isPos = (bool)row["isPos"];
        }

        public override string InitValue()
        {
            ValidValue result = Validator(new ValidData() {
                Value = XlsRow[xlsName],
                Size = DataSize,
                isPos = this.isPos
            });

            ResRow[ResName] = result.Value;
            if (result.Error != String.Empty) 
                result.Error = String.Format("{0}:{1};",ResName, result.Error);
            return result.Error;
        }

        public override object Value
        {
            get
            {
                return ResRow[ResName];
            }
        }

    }

}


