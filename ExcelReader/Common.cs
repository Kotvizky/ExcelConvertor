using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    struct FunctionFields
    {
        public paramGroup Group;
        public FunctionParameter[] parameters;
        public bool Ready;
        public string missingFields;

        public FunctionFields(List<Field> fields, string SQLString,
            string paramString, paramGroup group, string fildPrexid = "")
        {
            missingFields = "";

            string[] SQLArray = SQLString.Split(new Char[] { ' ',',' });
            string[] paramArray = paramString.Split(new Char[] { ' ', ',', ')' });

            Ready = true;
            Group = group;
            char devider = ';';

            parameters = new FunctionParameter[SQLArray.Length];
            for (int i = 0; i<parameters.Length; i++)
            {
                parameters[i].SqlName = SQLArray[i];
                if (!SQLArray[i].Any(Char.IsLower))
                {
                    parameters[i].Service = true;
                    if (!Enum.IsDefined(typeof(serviseFields), SQLArray[i])){
                        missingFields += String.Format("{0} '{1}'- не системное поле ", devider, SQLArray[i]);
                        Ready = false;
                    } else
                    {
                        parameters[i].xlsExist = true;
                    }
                }
                else
                {
                    if (i < paramArray.Length)
                    {
                        parameters[i].ResName = paramArray[i];
                        string resName;
                        if ( group == paramGroup.outPar )
                        {
                            resName = String.Format("{0}.{1}", fildPrexid, paramArray[i] );
                        }
                        else
                        {
                            resName = paramArray[i];
                        }
                        Field field = fields.Find(x => (x.ResName == resName) );
                        if (field == null)
                        {
                            missingFields += devider + paramArray[i];
                            Ready = false;
                        }
                        else
                        {
                            parameters[i].xlsExist = true;
                        }
                    }
                    else
                    {
                        missingFields += String.Format("{0} '{1}'- н.с. SQL ", devider, SQLArray[i]);
                        Ready = false;
                    }
                }
            }
            if (missingFields != "")
            {
                missingFields = missingFields.Remove(0, 1);
            } 
        }
    }

    public struct FunctionParameter
    {
        public string SqlName;
        public string ResName;
        public bool Service;
        public bool xlsExist;
        public string Print()
        {
            return String.Format("SqlName - {0};\t\tResName - {1};\t\tService - {2};\t\txlsExist - {3}",
                SqlName,ResName,Service.ToString(),xlsExist.ToString());
        } 
    }

}
