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
        //static public string SQLServer = "localhost";
        //        static private SqlConnection conn = new SqlConnection("Server=" + SQLServer + ";Database=db_logist;User Id=logist;Password=8611;Connection Timeout=60;");
        static private SqlConnection conn =
            new SqlConnection(global::ExcelReader.Properties.Settings.Default.CollectConnectionString);

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

        static string sqlCommHead =
            @"select idHead, idParent,
              case 
                when isGroup = 1 then name 
                else name + ' {'+ cast(idHead as varChar(100))+'}'
                end name
                , comm, isGroup
              from i_tmpl_head
                order by name";

        static string commStr =
            @"select idStr,idHead,npp,resName,xlsName,isPrint,attr,dataType,dataSize,
                strFormat,isPos,isActive,comm,author--,dateCreate,operator,dateModify
            from i_tmpl_str
            where idHead = @idHead";

        static SqlDataAdapter tbStrAdapter = new SqlDataAdapter(commStr,  conn );

        static SqlCommandBuilder tbStrBuilder;

        public static void getTbHeadData(DataTable table) {
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommHead,conn);
            adapter.Fill(table);
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
            DataTable result = null;
            switch (tableName)
            {
                case "inVal":
                    result = new DataTable();
                    result.Columns.AddRange(
                     new DataColumn[] {
                        new DataColumn("Ip",Type.GetType("System.Byte[]")),
                        new DataColumn("Row_Id",Type.GetType("System.Int32")),
                        new DataColumn("BusId",Type.GetType("System.String")),
                        new DataColumn("Account",Type.GetType("System.String")),
                        new DataColumn("ContractNum",Type.GetType("System.String")),
                        new DataColumn("Date",Type.GetType("System.DateTime")),
                        new DataColumn("INN",Type.GetType("System.String")),
                        new DataColumn("CurrCode",Type.GetType("System.String")),
                        new DataColumn("Days",Type.GetType("System.Double")),
                        new DataColumn("FIO",Type.GetType("System.String"))
                    });
                    break;
            }
            return result;
        }

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

        public static void BulkWrite(string tableName, DataRow[] rows)
        {

            if (conn.State == ConnectionState.Closed) conn.Open();
            using (SqlBulkCopy bulkCopy =
                        new SqlBulkCopy(conn))
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

        public static void ExecuteNonQuery(string sqlCommand)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
            }
            catch (SqlException e)
            {
                conn.Close();
            }
            command = new SqlCommand(sqlCommand,conn);
            command.ExecuteNonQuery();
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

        static public DataTable executeSQL(string query)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand = new SqlCommand(query, conn);
            dataAdapter.SelectCommand.CommandTimeout = conn.ConnectionTimeout;
            if (conn.State == ConnectionState.Closed) conn.Open();
            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            conn.Close();
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

        struct DicSQLParam
        {
            public string ParName;
            public SqlDbType Type;
            public int Length;
        }

    }
}
