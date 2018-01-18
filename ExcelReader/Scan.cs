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

            for (int i = 1; i < parameters.Length; i++) {
                parameters[i] = this.Find(x => x.ResName == parameters[i]).Value;
            }
            return SQLFunction.ExecuteFunction(parameters);
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

                string[] parameters = XlsNameToArray(field.XlsName);

                bool allFound = true;
                string functionMessage = "";

                for (int i = 1; i < parameters.Length; i++)
                {
                    string fieldName = parameters[i];
                    if (this.Find(x => (x.ResName == fieldName) & x.Exist ) == null )
                    {
                        functionMessage += String.Format(",{0}", fieldName);
                        allFound = true;
                    }
                }
                if (allFound)
                {
                    message += String.Format("\r\n(+):\t{0}", parameters[0]);
                } else
                {
                    message += String.Format("\r\n(-):\t{0},\t не найдены параметры ({1})", parameters[0], functionMessage.Remove(0,1));
                }


            }

            return message;
        }
    }
}
