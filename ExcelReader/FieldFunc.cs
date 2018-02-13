using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldFunc : FieldBase
    {

        public override string XlsName
        {
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

        public override attrName Attr
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

        public override string InitValue()
        {
            throw new NotImplementedException();
        }

        public override object Value
        {
            get
            {
                return ToString();
            }
        }

        public string FunctionName { private set; get; }
        public string SQLTable { private set; get; }

        public DataTable resTable;

        public List<FunctionFields> Parameters { private set; get; }

        public void setFunctionName()
        {
            if (attr != attrName.Func)
            {
                FunctionName = "";
            }
            else
            {
                FunctionName = XlsName.Split('(')[0].Trim();
            }
        }

        public void parseSQLParameter(List<FieldBase> fields)
        {
            if (FunctionName == "")
            {
                return;
            }
            string[] splitName = XlsName.Split('(');
            List<string[]> impStructure = SQLFunction.getDescription(FunctionName);
            int index;
            Parameters = new List<FunctionFields>();
            if ((index = getParamIndex(impStructure[0], paramGroup.inTable)) > -1)
            {
                SQLTable = impStructure[1][index].Trim();
            }
            string[] nameSplit = xlsName.Split('(');

            if ((index = getParamIndex(impStructure[0], paramGroup.tabFields)) > -1)
            {
                Parameters.Add(new FunctionFields(
                    fields.FindAll(x => (x.Attr == attrName.Field & x.Exist | x.Attr == attrName.Const)),
                    impStructure[1][index].Trim(),
                    nameSplit[1].Trim(),
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
                        FieldBase outField = fields.Find(x => (x.ResName == resField));
                        outField.Exist = true;
                    }
                }

            }
        }

        private int getParamIndex(string[] arrayNames, paramGroup group)
        {
            return Array.IndexOf(arrayNames, Enum.GetName(typeof(paramGroup), group));
        }

    }
}
