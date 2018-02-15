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

        public string SQLTableName { private set; get; }

        #region Function parameters groups

        public ParamGroup FieldsParam { private set;  get; }

        public ParamGroup InParam { private set; get; }

        public ParamGroup OutParam { private set; get; }

        #endregion

        public DataTable ResTable { private set; get; }
        public int ResTableIndex { protected set; get; }
        private DataRow resRow;

        public override DataRow ResRow
        {
            get
            {
                return resRow;
            }
        }

        private void SetResRow(int index)
        {
            ResTableIndex = index;
            resRow = ResTable.Rows[index];
        }

        public bool initSQLTable()
        {
            ResTable = SQLFunction.getResultTable(SQLTableName);
            return (ResTable != null);
        }

        public DataRow SQLRow { private set; get; }

        public List<FunctionFields> Parameters { private set; get; }

        public void parseSQLParameter(List<FieldBase> fields)
        {
            if (FunctionName == "")
            {
                return;
            }
            //string[] splitName = XlsName.Split('(');
            List<string[]> impStructure = SQLFunction.getDescription(FunctionName);
            int index;
            Parameters = new List<FunctionFields>();
            if ((index = getParamIndex(impStructure[0], GroupNames.inTable)) > -1)
            {
                SQLTableName = impStructure[1][index].Trim();
            }
            string[] nameSplit = xlsName.Split('(');

            if ((index = getParamIndex(impStructure[0], GroupNames.tabFields)) > -1)
            {
                Parameters.Add(new FunctionFields(
                    fields.FindAll(x => (x.Attr == attrName.Field & x.Exist | x.Attr == attrName.Const)),
                    impStructure[1][index].Trim(),
                    nameSplit[1].Trim(),
                    GroupNames.inTable
                    )
                );
            }

            if ((index = getParamIndex(impStructure[0], GroupNames.inPar)) > -1)
            {
                Parameters.Add(new FunctionFields(
                    fields.FindAll(x => ((x.Attr == attrName.Field) & x.Exist | x.Attr == attrName.Const)),
                    impStructure[1][index],
                    nameSplit[2],
                    GroupNames.inPar
                    )
                );
            }

            if ((index = getParamIndex(impStructure[0], GroupNames.outPar)) > -1)
            {
                FunctionFields outPar = new FunctionFields(
                    fields.FindAll(x => (x.Attr == attrName.Answer)),
                    impStructure[1][index],
                    nameSplit[3],
                    GroupNames.outPar,
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

        public void initSQLParameter(List<FieldBase> fields) //parseSQLParameter 
        {
            if (FunctionName == "")
            {
                return;
            }

            string[] nameSplit = xlsName.Split('(');
            DataTable impStructure = SQLFunction.getFuncDescription(FunctionName);
            SQLTableName = impStructure.Rows[0][SqlParam[GroupNames.inTable]].ToString(); // impStructure[1][index].Trim();

            if (impStructure.Columns.Contains(SqlParam[GroupNames.tabFields]))
            {
                FieldsParam = new ParamGroup(
                    impStructure.Rows[0][SqlParam[GroupNames.tabFields]].ToString(),
                    nameSplit[1].Trim(),
                    GroupNames.tabFields,
                    Scan.FindAll(x => ((x.Attr == attrName.Field) || (x.Attr == attrName.Const)) && x.IsActive ),
                    this
                    );
            }

            if (impStructure.Columns.Contains(SqlParam[GroupNames.inPar]))
            {
                InParam = new ParamGroup(
                    impStructure.Rows[0][SqlParam[GroupNames.inPar]].ToString(),
                    nameSplit[2].Trim(),
                    GroupNames.inPar,
                    Scan.FindAll(x => ((x.Attr == attrName.Field) || (x.Attr == attrName.Const)) && x.IsActive),
                    this
                    );
            }

            if (impStructure.Columns.Contains(SqlParam[GroupNames.outPar]))
            {
                OutParam = new ParamGroup(
                    impStructure.Rows[0][SqlParam[GroupNames.outPar]].ToString(),
                    nameSplit[3].Trim(),
                    GroupNames.outPar,
                    Scan.FindAll(x => (x.Attr == attrName.Answer)  && x.IsActive),
                    this
                    );
            }

        }

        //string getValueFromSQLParam(string )

        private void setFunctionName()
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

        private int getParamIndex(string[] arrayNames, GroupNames group)
        {
            return Array.IndexOf(arrayNames, Enum.GetName(typeof(GroupNames), group));
        }

        public static readonly Dictionary<GroupNames, string> SqlParam
            = new Dictionary<GroupNames, string>
        {
            { GroupNames.inTable,    "inTable"},
            { GroupNames.tabFields,  "tabFields"},
            { GroupNames.inPar,      "inPar"},
            { GroupNames.outPar,      "outPar"}
        };


    }
}
