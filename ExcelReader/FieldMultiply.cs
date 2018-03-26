using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldMultiply : FieldBase
    {

        public FieldMultiply(DataRow row, Scan scan) : base(row, scan)
        {
            //initFilds();
        }

        public String initFilds()
        {
            string result = String.Empty;
            FullName = xlsName;
            try
            {
                xlsName = FieldFunc.getFuncName(FullName);
                string[] param = FullName.Split('(');
                NameVal1 = param[1].Split(',')[0].Trim();
                Type = Type.GetType(String.Format(
                    "System.{0}",
                    param[1].Split(',')[1].Split(')')[0].Trim()
                    ));

                NameVal2 = param[2].Split(',')[0].Trim();
                Type2 = Type.GetType(String.Format(
                    "System.{0}",
                    param[2].Split(',')[1].Split(')')[0].Trim()
                    ));
            }
            catch (Exception e)
            {
                result = e.Source;
            }
            return result;
        }

        public Func<ValidData, ValidValue> Validator1 { set; get; } = null;

        public string[] ResTableFields {private set; get; }

        public void SetTableFields(DataColumnCollection Columns)
        {
            
            int col = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].ColumnName.StartsWith(xlsName)) col++;
            }
            ResTableFields = new string[col];
            col = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].ColumnName.StartsWith(xlsName) )
                {
                    ResTableFields[col] = Columns[i].ColumnName;
                    col++;
                };
            }
        }

        public string FullName { private set; get; }
        public string NameVal1 { private set; get; }
        public string NameVal2 { private set; get; }
        public Type Type2 { set; get; }

        public override object Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public string InitMultValue(string fieldName)
        {
            string FieldValue = fieldName.Substring(XlsName.Length, fieldName.Length - XlsName.Length);

            ValidValue result = Validator(new ValidData()
            {
                Value = FieldValue,
                Size = DataSize,
            });

            ResRow[NameVal1] = result.Value;

            if (result.Error != String.Empty)
                result.Error = String.Format("{0}:{1};", ResName, result.Error);


            string result1 = result.Error;

            result = Validator1(new ValidData()
            {
                Value = XlsRow[fieldName],
                Size = DataSize,
            });

            ResRow[NameVal2] = result.Value;

            if (result.Error != String.Empty)
                result.Error = String.Format("{0}:{1};", ResName, result.Error);


            string result2 = result.Error;


            return result1 + result2;
        }


        public override string InitValue()
        {
            throw new NotImplementedException();
        }
    }
}
