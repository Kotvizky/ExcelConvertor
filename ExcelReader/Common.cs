using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    struct FunctionFields
    {
        public string Name;
        public FunctionParameter[] parameters;
        public bool Ready;
        public string MissingFields;

        public FunctionFields(List<Field> fields, string xlsField)
        {
            MissingFields = "";
            string preperedString = xlsField.Split(new Char[] { ' ', ')' })[0];
            string[] stringParameters = preperedString.Split(new Char[] { ' ', ',', '(' });
            Ready = true;
            Name = stringParameters[0];

            parameters = new FunctionParameter[stringParameters.Length - 1];
            for (int i = 0; i<parameters.Length; i++)
            {
                parameters[i].resName = stringParameters[i+1];
                Field field = fields.Find(x => (x.ResName == stringParameters[i + 1]) & x.Exist);
                if (field == null)
                {
                    MissingFields += "," + stringParameters[i+1];
                    Ready = false;
                }
            }
            if (!Ready)
            {
                MissingFields = MissingFields.Remove(0, 1);
            } 
        }
    }

    public struct FunctionParameter
    {
        public string sqlName;
        public string resName;
        public string Value;
    }

}
