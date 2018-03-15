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

        public FieldFunc(DataRow row, Scan scan) : base(row, scan)
        {
        }

        public event Scan.initProgressBar onInitProgressBar;

        public event Scan.hideProgressBar onHideProgressBar;

        public event Scan.stepProgressBar onStepProgressBar;


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

        public bool initSQLResult()
        {
            InSqlTable = SQLFunction.GetResultTable(SQLTableName);
            return (InSqlTable != null);
        }

        public DataRow SqlRow { private set; get; }

        public void initSQLParameter(List<FieldBase> fields) //parseSQLParameter 
        {
            if (FunctionName == "")
            {
                return;
            }

            string[] nameSplit = xlsName.Split('(');
            DataTable impStructure = SQLFunction.getFuncDescription(FunctionName);
            if (impStructure.Rows.Count == 0)
            {
                return;
            }
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
                FunctionName = XlsName.Split(new char[] { '(',' ','\n','\r' })[0].Trim();
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
                else if (param.Value.GetType().Equals(typeof(DateTime)))
                {
                    value = String.Format("'{0:yyyyMMdd}'",(DateTime)param.Value);
                }

                values += String.Format(",{0}", value);
            }
            ResSqlTable = SQLFunction.executeSQL(
                String.Format("select * from {0}({1}) order by row_id", FunctionName, values.Remove(0, 1)));
        }

        public void FillResult()
        {
            ExecServerFunc();
            onInitProgressBar?.Invoke(ResSqlTable.Rows.Count);

            DataRowCollection rows = Scan.ResTable.Rows;

            FieldBase clear = this.Scan.Find(x => ( x.Attr == attrName.System) && (x.ResName == "CLEAR_ROWS") && x.IsActive);

            if (clear != null)
            {
                Scan.ResTable.Rows.Clear();

            }

            foreach (DataRow row in ResSqlTable.Rows)
            {
                SqlCurrentRow = row;
                int rowId = (int)SqlCurrentRow["ROW_ID"];
                if (rows.Count > rowId )
                {
                    ResCurrentRow = rows[rowId];
                }
                else
                {
                    DataRow newRow = Scan.ResTable.NewRow();
                    if (Scan.ResTable.Columns.Contains("ROW_ID"))
                    {
                        newRow["ROW_ID"] = rowId + 1;
                    }
                    ResCurrentRow = newRow;
                    rows.Add(newRow);

                }
                foreach(ParamBase param in ParamsOut)
                {
                    param.InitField();
                }
                onStepProgressBar?.Invoke();
            }
            onHideProgressBar?.Invoke();

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
