using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    struct FunctionFields
    {
        public GroupNames Group;
        public FunctionParameter[] parameters;
        public bool Ready;
        public string missingFields;

        public FunctionFields(List<FieldBase> fields, string SQLString,
            string paramString, GroupNames group, string fildPrexid = "")
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
                        if (SQLArray[i] == serviseFields.IP.ToString())
                        {
                            parameters[i].Value = Scan.GetLocalIPByte();
                        }
                    }
                }
                else
                {
                    if (i < paramArray.Length)
                    {
                        parameters[i].ResName = paramArray[i];
                        string resName;
                        if ( group == GroupNames.outPar )
                        {
                            resName = String.Format("{0}.{1}", fildPrexid, paramArray[i] );
                        }
                        else
                        {
                            resName = paramArray[i];
                        }
                        FieldBase field = fields.Find(x => (x.ResName == resName) );
                        if (field == null)
                        {
                            missingFields += devider + paramArray[i];
                            Ready = false;
                        }
                        else
                        {
                            parameters[i].xlsExist = true;
                            if (field.Attr == attrName.Field)
                            {
                                parameters[i].TableName = field.XlsName;
                            }
                            else if (field.Attr == attrName.Const)
                            {
                                parameters[i].Value = field.XlsName;
                            }
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
        public string TableName;
        public object Value;
        public bool Service;
        public bool xlsExist;

        public string Print()
        {
            return String.Format("SqlName - {0};\t\t\tResName - {1};\t\t\tService - {2};\t\t\txlsExist - {3}",
                SqlName,ResName,Service.ToString(),xlsExist.ToString());
        } 
    }

    public struct ValidValue
    {
        public object Value;
        public string Error;
    }

    public struct ValidData
    {
        public object Value;
        public int Size;
    }

    enum attrName : byte { Field = 0, Func, Answer, Const };
    enum funcParameter : byte { Table = 1, In, Out }
    enum spetialFields { Ip, RowId }
    enum GroupNames { inTable, tabFields, inPar, outPar }
    enum serviseFields { IP, ROW_ID }
    enum dataType { String, DateTime, Double }

}
