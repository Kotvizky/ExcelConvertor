using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections;


namespace ExcelReader
{
    static class SQLFunction
    {
        static public SqlConnection conn { get; private set; } =
            new SqlConnection(global::ExcelReader.Properties.Settings.Default.CollectConnectionString);

//TODO change timeout

        static public bool connOpen()
        {
            bool result = true;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
            }
            catch (SqlException e)
            {
                // MessageBox.Show(e.ToString());
                conn.Close();
                result = false;
            }
            return result;
        }
        static private SqlCommand command;
        private const int fieldLenght = 150;
        public const string paramMark = "@params";
        public const string paramName = "@p";

        private static readonly Dictionary<string, DicSQLParam> ServiseParam
            = new Dictionary<string, DicSQLParam>
        {
            { "IP",  new DicSQLParam { ParName = "IP", Type = SqlDbType.Binary, Length = 4} },
            { "ROW_ID",  new DicSQLParam { ParName = "ROW_ID", Type = SqlDbType.Int, Length = 0} }
        };

        #region SQL code for tables

        static string sqlCommHead =
            @"select idHead, idParent,
            /*
                          case 
                            when isGroup = 1 then name 
                            else name + ' {'+ cast(idHead as varChar(100))+'}'
                            end name
            */ 
            name
                , comm, isGroup, idHead HeadIdTitle
              from thd
                order by [order], name";

        static string commStr =
            @"select idHead,idStr,npp,resName,xlsName,isPrint,attr,dataType,dataSize,
                isPos,isActive,xlsFormat,xlsColName,comm
            from tst
            where idHead = @idHead";

        #endregion

        static SqlDataAdapter tbStrAdapter = new SqlDataAdapter(commStr,  conn );
         
        static SqlDataAdapter tbHeadAdapter = new SqlDataAdapter(sqlCommHead, conn);

        static SqlCommandBuilder tbStrBuilder;

        public static void getTbHeadData(DataTable table) {
            tbStrBuilder = new SqlCommandBuilder(tbHeadAdapter);

            command = new SqlCommand(
                @"
                INSERT INTO [thd] (idParent,name, comm,isGroup) VALUES ( @idParent, @name, @comm,@isGroup);
                SELECT idHead, idParent,name, comm, isGroup, idHead HeadIdTitle FROM thd WHERE (IdHead = SCOPE_IDENTITY())
                ", conn);

            command.Parameters.Add("@idParent", SqlDbType.Int, 0,"idParent");
            command.Parameters.Add("@isGroup", SqlDbType.Int, 0, "isGroup");
            command.Parameters.Add("@name", SqlDbType.NVarChar, 250,"name");
            command.Parameters.Add("@comm", SqlDbType.NVarChar, 500,"comm");


            tbHeadAdapter.InsertCommand = command;

            tbHeadAdapter.Fill(table);
        }

        public static void updateTbHeadData(DataTable table)
        {
            //tbHeadAdapter.InsertCommand
            tbHeadAdapter.Update(table);
        }

        public static void intTbStrParam()
        {
            tbStrAdapter.SelectCommand.Parameters.Add(new SqlParameter("idHead", Type.GetType("System.Int32")));
            tbStrBuilder = new SqlCommandBuilder(tbStrAdapter);
        }

        public static void getTbStrData(DataTable table, int idHead)
        {
            table.Clear();
            tbStrAdapter.SelectCommand.Parameters[0].Value = idHead;
            tbStrAdapter.Fill(table);
        }

        public static void updateTbStrData(DataTable table)
        {
            tbStrAdapter.Update(table);
        }

        public static DataTable GetResultTable(string tableName)
        {
            DataTable tShema = SQLFunction.getShema(tableName);
            DataTable tTypes = SQLFunction.getSQLTypes();
            DataRow[] rType;
            DataTable tInVal = new DataTable();
            foreach (DataRow row in tShema.Rows)
            {
                string sqlType = row["Data_Type"].ToString();
                string sqlName = row["Column_Name"].ToString();

                rType = tTypes.Select(String.Format("sqlType = '{0}'", sqlName));
                if (rType.Length == 0)
                {
                    rType = tTypes.Select(String.Format("sqlType = '{0}'", sqlType));
                }
                tInVal.Columns.Add(row["Column_Name"].ToString(), Type.GetType(String.Format("System.{0}", rType[0]["csType"])));
            }
            return tInVal;
        }

        //public static DataTable GetResultTable(string tableName)
        //{
        //    DataTable result = null;
        //    switch (tableName)
        //    {
        //        case "inVal":
        //            result = new DataTable();
        //            result.Columns.AddRange(
        //             new DataColumn[] {
        //                new DataColumn("Ip",Type.GetType("System.Byte[]")),
        //                new DataColumn("Row_Id",Type.GetType("System.Int32")),
        //                new DataColumn("BusId",Type.GetType("System.String")),
        //                new DataColumn("Account",Type.GetType("System.String")),
        //                new DataColumn("ContractNum",Type.GetType("System.String")),
        //                new DataColumn("Date",Type.GetType("System.DateTime")),
        //                new DataColumn("INN",Type.GetType("System.String")),
        //                new DataColumn("CurrCode",Type.GetType("System.String")),
        //                new DataColumn("Days",Type.GetType("System.Double")),
        //                new DataColumn("FIO",Type.GetType("System.String")),
        //                new DataColumn("Suma",Type.GetType("System.Double")),
        //                new DataColumn("Date1",Type.GetType("System.DateTime")),
        //                new DataColumn("Suma1",Type.GetType("System.Double")),
        //                new DataColumn("Suma2",Type.GetType("System.Double"))
        //            });
        //            break;
        //    }
        //    return result;
        //}

        public static void InitCommand(string sqlCommand, string[] param, string sqlDelete = "")
        {
            conn.Open();
            command = new SqlCommand();
            command.Connection = conn;
            if (sqlDelete != "")
            {
                command.CommandText = sqlDelete;
                command.ExecuteNonQuery();
            }
            if (param.Length > 0)
            {
                string paramString = String.Empty;
                for (int i = 0; i < param.Length; i++)
                {
                    string iParamName = String.Format("@{0}", param[i]);
                    SqlParameter newParam;
                    if (ServiseParam.ContainsKey(param[i]))
                    {
                        DicSQLParam dicParam = ServiseParam[param[i]];
                        newParam = new SqlParameter(dicParam.ParName, dicParam.Type, dicParam.Length);
                    }
                    else
                    {
                        newParam = new SqlParameter(iParamName, SqlDbType.Text, fieldLenght);
                    }
                    command.Parameters.Add(newParam);
                    paramString += String.Format(",{0}", iParamName);
                }
            }
            command.CommandText = sqlCommand;
            command.Prepare();
        }

        public static void BulkWrite(string tableName, DataRow[] rows,SqlConnection myConn = null)
        {
            if (myConn == null)
            {
                myConn = conn;
            }

            if (myConn.State == ConnectionState.Closed) myConn.Open();
            using (SqlBulkCopy bulkCopy =
                        new SqlBulkCopy(myConn))
            {
                bulkCopy.DestinationTableName = tableName;
                try
                {
                    // Write from the source to the destination.
                    bulkCopy.WriteToServer(rows);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                finally
                {
                    // Close the SqlDataReader. The SqlBulkCopy
                    // object is automatically closed at the end
                    // of the using block.
                    // reader.Close();
                }
            }
        }

        public static void ExecuteNonQuery(string sqlCommand, bool closeConn = true, SqlConnection myConnection = null)
        {
            if (myConnection == null)
            {
                myConnection = conn;
            }

            try
            {
                if (myConnection.State == ConnectionState.Closed) myConnection.Open();
            }
            catch (SqlException e)
            {
                myConnection.Close();
            }
            command = new SqlCommand(sqlCommand, myConnection);
            command.ExecuteNonQuery();
            if (closeConn) myConnection.Close();

        }

        public static void conClose()
        {
            conn.Close();
        }

        public static void preperedInsert(ArrayList param = null)
        {
            if (param != null)
            {
                for (int i = 0; i < param.Count; i++ )
                {
                    command.Parameters[i].Value = param[i];
                }
            }
            command.ExecuteNonQuery();
        }

        static public void insertData(string query)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand Cmd = new SqlCommand(query, conn);
            conn.Open();
            int RowsAffected = Cmd.ExecuteNonQuery();
            conn.Close();
        }

        static public DataTable ExecuteSQL(string query)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand = new SqlCommand(query, conn);
            dataAdapter.SelectCommand.CommandTimeout = conn.ConnectionTimeout;
            if (conn.State == ConnectionState.Closed) conn.Open();
            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            //conn.Close(); - dataAdapter.Fill closes it automatically
            return table;
        }

        static public string ExecuteFunction(string[] parameters)
        {
            string parametersString = "";
            for (int i = 0; i < parameters.Length - 1; i++)
            {
                parametersString += String.Format(",@p{0}", i);
                //                parametersString = "," +/parametersString[i];
            }
            parametersString = parametersString.Remove(0, 1);
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand = new SqlCommand(
                String.Format("Select [CORP\\IKotvytskyi].[{0}]({1})", parameters[0], parametersString),
                conn);
            for (int i = 0; i < parameters.Length - 1; i++)
            {
                dataAdapter.SelectCommand.Parameters.Add("@p" + i.ToString(), SqlDbType.VarChar);
                dataAdapter.SelectCommand.Parameters[i].SqlValue = parameters[i + 1];
            }
            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            string result = "Н/Д";
            if (table.Rows.Count > 0)
            {
                result = table.Rows[0][0].ToString();
            }
            return result;
            //return "SQL" + String.Join("--", parameters);
        }

        static public int copyTemlpate(int idHead)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
            }
            catch (SqlException e)
            {
                conn.Close();
            }

            string SQLCommand = @"
              declare @newId int
              exec CopyTemlpById @id, @newId out
              select @newId";

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand = new SqlCommand(SQLCommand, conn);
            SqlParameter newParam = new SqlParameter("id", SqlDbType.Int);
            newParam.Value = idHead;
            dataAdapter.SelectCommand.Parameters.Add(newParam);
            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            conn.Close();
            return (int)table.Rows[0][0];
        }

        static public List<string[]> getDescription(string parameter)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand = new SqlCommand("select * from [dbo].[impFunc] where fnName = @name", conn);
            dataAdapter.SelectCommand.Parameters.Add("@name", SqlDbType.VarChar);
            dataAdapter.SelectCommand.Parameters[0].SqlValue = parameter;

            //fn_getUkrSotz

            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            List<string[]> res = null;

            if (table.Rows.Count > 0)

            {
                res = new List<string[]>();
                res.Add(table.Columns.Cast<DataColumn>()
                                                 .Select(x => x.ColumnName)
                                                 .ToArray());
                res.Add(table.Rows[0].ItemArray.Select(x => x.ToString()).ToArray());
            }

            return res;
        }

        static public DataTable getFuncDescription(string parameter, int tmpl)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            string sqlCmd = "select * from [dbo].[impFunc] where fnName = @name";
            if (tmpl > 0) sqlCmd = String.Format("{0} and tmpl like '%;{1};%'", sqlCmd, tmpl);
            dataAdapter.SelectCommand = new SqlCommand(sqlCmd, conn);
            dataAdapter.SelectCommand.Parameters.Add("@name", SqlDbType.VarChar);
            dataAdapter.SelectCommand.Parameters[0].SqlValue = parameter;

            //fn_getUkrSotz

            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            return table;
        }

        static public DataTable getFuncDescription(string parameter)
        {
            return getFuncDescription(parameter, -1);
        }

        static public DataTable getProcDescription(int tmpl)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            string sqlCmd = "select * from impProc where idHead = @id";
            dataAdapter.SelectCommand = new SqlCommand(sqlCmd, conn);
            dataAdapter.SelectCommand.Parameters.Add("@id", SqlDbType.Int);
            dataAdapter.SelectCommand.Parameters[0].Value = tmpl;
            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            return table;
        }

        static public DataTable getShema(string tableName)
        {
            connOpen();
            string[] restrictions = new string[4];
            restrictions[2] = tableName;
            DataTable table = conn.GetSchema("Columns", restrictions);
            conn.Close();
            return table;
        }

        static public DataTable getSQLTypes()
        {
            connOpen();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            string sqlCmd = "select sqlType, csType from match ";
            dataAdapter.SelectCommand = new SqlCommand(sqlCmd, conn);
            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            conn.Close();
            return table;
        }

        //static public DataTable getSQLTempTableTypes()
        //{
        //    connOpen();
        //    SqlDataAdapter dataAdapter = new SqlDataAdapter();
        //    string sqlCmd = "select csType,sqlType from [matchTmpType] ";
        //    dataAdapter.SelectCommand = new SqlCommand(sqlCmd, conn);
        //    DataTable table = new DataTable();
        //    dataAdapter.Fill(table);
        //    conn.Close();
        //    return table;
        //}

        public static string ExecuteProc(string sqlCommand, string outParam, string[] inParam = null)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
            }
            catch (SqlException e)
            {
                conn.Close();
            }
            string result = String.Empty;
            command = new SqlCommand(sqlCommand, conn);
            command.CommandType = CommandType.StoredProcedure;
            if ( (inParam != null) && (inParam.Length % 2 == 0))
            {
                for (int i = 0; i < inParam.Length; i += 2)
                {
                    command.Parameters.AddWithValue(inParam[i], inParam[ i + 1 ]);
                }
            }
            SqlParameter returnParameter = null;
            if (outParam != String.Empty)
            {
                returnParameter = new SqlParameter()
                {
                    ParameterName = outParam,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.InputOutput,
                    Value = ""
                };
                command.Parameters.Add(returnParameter);
            }
            command.ExecuteNonQuery();
            conn.Close();
            if (returnParameter != null)
            {
                result = returnParameter.Value.ToString();
            }
            return result;
        }

        struct DicSQLParam
        {
            public string ParName;
            public SqlDbType Type;
            public int Length;
        }

        public static SqlConnection getNewConnection(string dataSource, string dataBase)
        {
            var builder = new SqlConnectionStringBuilder(Properties.Settings.Default.CollectConnectionString);
            builder.DataSource = dataBase;
            builder.InitialCatalog = dataSource;
            SqlConnection newConnectinon = new SqlConnection(builder.ConnectionString);
            return newConnectinon;
        }
    }
}
