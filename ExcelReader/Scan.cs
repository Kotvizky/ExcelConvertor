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
            return this.Find(x => x.ResName == Name).Value;
        }

        //TODO get value from SQL function
        private string getSQLValue(Field field) {
            return "";
        }

        public string matching(DataColumn[] tableHead) { //TODO finish matching with report
            return "";
        }
    }
}
