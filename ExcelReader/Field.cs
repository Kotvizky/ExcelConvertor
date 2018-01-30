using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class Field
    {
        private const string noData = "Н/Д";

        // fields in database
        public string ResName { set; get; }
        private string xlsName;
        public string XlsName {
            set
            {
                xlsName = value;
                setFunctionName();
            }
            get
            {
                return xlsName;
            }
        }

        public bool IsPrint { set; get; }
        private attrName attr;
        public attrName Attr
        {
            set
            {
                attr = value;
                setFunctionName();
            }
            get
            {
                return attr;
            }
        }

        public string StrFormat { set; get; }
        public bool IsActive { set; get; }
        // <-- fields in database

        public bool Exist { set; get; }

        public string FunctionName { private set; get; }
        string SQLTable { set; get; }

        public List<FunctionFields> Parameters { private set; get; }

        public void setFunctionName() {
            if (attr != attrName.Func)
            {
                FunctionName = "";
            } else
            {
                FunctionName = XlsName.Split('(')[0];
            }
        }


        public void parseSQLParameter(List<Field> fields)
        {
            if (FunctionName == "")
            {
                return;
            }
            string[] splitName = XlsName.Split('(');
            List<string[]> impStructure = SQLFunction.getDescription(FunctionName);
            int index;
            Parameters = new List<FunctionFields>();
            if ((index = getParamIndex(impStructure[0], paramGroup.inTable)) >-1)
            {
                SQLTable = impStructure[1][index];
            }
            string[] nameSplit = xlsName.Split('(');

            if ((index = getParamIndex(impStructure[0], paramGroup.tabFields)) > -1)
            {
                Parameters.Add(new FunctionFields(
                    fields.FindAll(x => (x.Attr == attrName.Field & x.Exist | x.Attr == attrName.Const)),
                    impStructure[1][index],
                    nameSplit[1],
                    paramGroup.inTable
                    )
                );
            }

            if ((index = getParamIndex(impStructure[0], paramGroup.inPar)) > -1)
            {
                Parameters.Add(new FunctionFields(
                    fields.FindAll(x => ((x.Attr == attrName.Field) & x.Exist | x.Attr == attrName.Const)),
                    impStructure[1][index],
                    nameSplit[2], 
                    paramGroup.inPar
                    )
                );
            }

            if ((index = getParamIndex(impStructure[0], paramGroup.outPar)) > -1)
            {
                FunctionFields outPar = new FunctionFields(
                    fields.FindAll(x => (x.Attr == attrName.Answer)),
                    impStructure[1][index],
                    nameSplit[3],
                    paramGroup.outPar,
                    FunctionName
                    );
                Parameters.Add(outPar);
                for (int i = 0; i < outPar.parameters.Length; i++)
                {
                    if (outPar.parameters[i].xlsExist & (!outPar.parameters[i].Service))
                    {
                        string resField = String.Format("{0}.{1}", FunctionName, outPar.parameters[i].ResName);
                        Field outField = fields.Find(x => (x.ResName == resField));
                        outField.Exist = true;
                    }
                }

            }
        }

        private int getParamIndex(string [] arrayNames,  paramGroup group)
        {
            return  Array.IndexOf(arrayNames, Enum.GetName(typeof(paramGroup), group));
        }

        public bool findParameters(List<Field> fields)
        {
            //foreach (FunctionFields parameter in Parameters)
            //{
            //    FunctionFields = new FunctionFields(fields, XlsName);
            //    Exist = FunctionFields.Ready;
            //}
            //return Exist;
            return true;
        }

        private string value;
        public string Value {
            set {
                this.value = value;
            }
            get {
                if (this.Exist)
                {
                    return this.value;
                }
                else
                {
                    return noData;
                }
            }
        }
    }

    enum attrName : byte { Field = 0, Func, Answer, Const };
    enum funcParameter : byte { Table = 1, In, Out }
    enum spetialFields { Ip, RowId }
    enum paramGroup  { inTable, tabFields, inPar, outPar }
    enum serviseFields {IP,ROW_ID}

}


