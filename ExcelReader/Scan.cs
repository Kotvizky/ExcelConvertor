using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ExcelReader
{
    class Scan : List<Field>
    {
        public bool AllFound()  {
            bool result = true;
            bool fieldsExist = false; 
            foreach (Field field in this) {
                if (field.Attr == 0) {
                    if (field.Exist)
                    {
                        fieldsExist = true;
                    }
                    else {
                        result = false;
                        break;
                    }
                } 
            }
            if (!fieldsExist) result = false;
            return result;
        }

        public void SetValues(DataRow row) {
            foreach (Field field in this.FindAll(x => (x.IsActive & (x.Attr == 0) & x.Exist))) {
                field.Value = row[field.XlsName].ToString();
            }
        }

        public string GetValue(string Name)
        {

            string result = String.Empty;
            Field field = this.Find(x => x.ResName == Name);
            switch (field.Attr) {
                case 0:
                    result = field.Value;
                    break;
                case 1:
                    result = GetSQLValue(field);
                    break;
            }
            return result;
        }

        private string[] XlsNameToArray(string XlsName)
        {
            string preperedString = XlsName.Split(new Char[] { ' ', ')' })[0];
            return preperedString.Split(new Char[] { ' ', ',', '(' });
        }

        private string GetSQLValue(Field field) {

            string[] parameters = XlsNameToArray(field.XlsName);
            string result = "";
            try {
                for (int i = 1; i < parameters.Length; i++)
                {
                    parameters[i] = this.Find(x => x.ResName == parameters[i]).Value;
                }
                result = SQLFunction.ExecuteFunction(parameters);
            } catch
            {
                result = "NoData";
            }
            return result;
        }

        public string Matching(DataColumnCollection tableHead) { //TODO finish matching with report
            string message = "Поля \r\n -----";
            foreach (Field field in this.FindAll(x => (x.Attr == 0) & x.IsActive))
            {
                if (tableHead.Contains(field.XlsName))
                {
                    message += String.Format("\r\n(+):\t{0} -> {1}", field.XlsName, field.ResName);
                    field.Exist = true;
                }
                else
                {
                    message += String.Format("\r\n(-):\t{0} -> {1}\tполе не найдено!", field.XlsName, field.ResName);
                }
            }

            message += "\r\n\r\nФункции \r\n -----";

            foreach (Field field in this.FindAll(x => (x.Attr == 1) & x.IsActive))
            {

                if (field.findParameters(this))
                {
                    message += String.Format("\r\n(+):\t{0}", field.FunctionFields.Name);
                }
                else
                {
                    field.Exist = false;
                    message += String.Format("\r\n(-):\t{0},\t не найдены параметры ({1})", 
                        field.FunctionFields.Name,
                        field.FunctionFields.MissingFields
                        );
                }
            }
            return message;
        }
    }
}
