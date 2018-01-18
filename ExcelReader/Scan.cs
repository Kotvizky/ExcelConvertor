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
                    if (field.IsExist)
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

        public void setValues(DataRow row) {
            foreach (Field field in this.FindAll(x => (x.IsActive & (x.Attr == 0)))) {
                field.Value = row[field.XlsName].ToString();
            }
        }

        public string getValue(string Name)
        {

            string result = String.Empty;
            Field field = this.Find(x => x.ResName == Name);
            switch (field.Attr) {
                case 0:
                    result = field.Value;
                    break;
                case 1:
                    result = getSQLValue(field);
                    break;
            }
            return result;
        }

        private string getSQLValue(Field field) {
            string preperedString = field.XlsName.Split(new Char[] { ' ', ')' })[0];
            string[] parameters = preperedString.Split(new Char[] { ' ',',', '(' });
            for (int i = 1; i < parameters.Length; i++) {
                parameters[i] = this.Find(x => x.ResName == parameters[i]).Value;
            }
            return SQLFunction.ExecuteFunction(parameters);
        }

        public string matching(DataColumn[] tableHead) { //TODO finish matching with report
            return "";
        }
    }
}
