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

        #region ** Function parameters groups **

        public ParamGroup ParamsField { private set;  get; }

        public ParamGroup ParamsIn { private set; get; }

        public ParamGroup ParamsOut { private set; get; }

        #endregion


        private DataTable ResSqlTable;

        public DataTable InSqlTable { private set; get; }
        public int ResTableIndex { protected set; get; }
        public DataRow ResCurrentRow { protected set; get; }
        public DataRow SqlCurrentRow { protected set; get; }

        public bool initSQLTable()
        {
            InSqlTable = SQLFunction.GetResultTable(SQLTableName);
            return (InSqlTable != null);
        }

        public DataRow SqlRow { private set; get; }

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
                ParamsField = new ParamGroup(
                    impStructure.Rows[0][SqlParam[GroupNames.tabFields]].ToString(),
                    nameSplit[1].Trim(),
                    GroupNames.tabFields,
                    Scan.FindAll(x => ((x.Attr == attrName.Field) || (x.Attr == attrName.Const)) && x.IsActive),
                    this
                    );
            }

            if (impStructure.Columns.Contains(SqlParam[GroupNames.inPar]))
            {
                ParamsIn = new ParamGroup(
                    impStructure.Rows[0][SqlParam[GroupNames.inPar]].ToString(),
                    nameSplit[2].Trim(),
                    GroupNames.inPar,
                    Scan.FindAll(x => ((x.Attr == attrName.Field) || (x.Attr == attrName.Const)) && x.IsActive),
                    this
                    );
            }

            if (impStructure.Columns.Contains(SqlParam[GroupNames.outPar]))
            {
                ParamsOut = new ParamGroup(
                    impStructure.Rows[0][SqlParam[GroupNames.outPar]].ToString(),
                    nameSplit[3].Trim(),
                    GroupNames.outPar,
                    Scan.FindAll(x => (x.Attr == attrName.Answer) && x.IsActive),
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

        #region ** Get Data From SQL **

        private void ClearSqlData()
        {
            SQLFunction.ExecuteNonQuery(
                String.Format("delete from {0} where IP = 0x{1} ", 
                            SQLTableName, 
                            Scan.GetLocalIPAddress(true)));

        }

        public void InsertToServer()
        {
            SQLFunction.BulkWrite(SQLTableName, RowsSql);
        }

        public void ReadFromServer()
        {
            foreach (ParamBase param in ParamsOut)
            {
                param.InitField();
            }
        }

        public DataRow[] RowsSql { get; set; }

        private void initSqlRows()
        {
            RowsSql = new DataRow[Scan.ResTable.Rows.Count];
        }

        public void InitSqlTable()
        {
            ClearSqlData();
            initSqlRows();
        }

        public void InitSqlValues(int i)
        {
            SqlRow = InSqlTable.NewRow();
            ResTableIndex = i;
            foreach (ParamBase param in ParamsField)
            {
                param.InitField();
            }
            RowsSql[i] = SqlRow;
        }

        public void ExecServerFunc()
        {
            string values = String.Empty;
            foreach (ParamBase param in ParamsIn)
            {
                string value = param.Value.ToString();
                if (param.ParamName == "IP")
                {
                    value = param.ToString();
                }

                values += String.Format(",{0}", value);
            }
            ResSqlTable = SQLFunction.executeSQL(
                String.Format("select * from {0}({1}) order by row_id", FunctionName, values.Remove(0, 1)));
        }

        public void FillResult()
        {
            ExecServerFunc();
            foreach (DataRow row in ResSqlTable.Rows)
            {
                SqlCurrentRow = row;
                ResCurrentRow = Scan.ResTable.Rows[(int)SqlCurrentRow["ROW_ID"]];
                foreach(ParamBase param in ParamsOut)
                {
                    param.InitField();
                }
            }

        }

        
        #endregion

        private int getParamIndex(string[] arrayNames, GroupNames group)
        {
            return Array.IndexOf(arrayNames, Enum.GetName(typeof(GroupNames), group));
        }

        public static readonly Dictionary<GroupNames, string> SqlParam
            = new Dictionary<GroupNames, string>
        {
            { GroupNames.inTable,   "inTable"},
            { GroupNames.tabFields, "tabFields"},
            { GroupNames.inPar,     "inPar"},
            { GroupNames.outPar,    "outPar"}
        };


    }
}
