using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;



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

        static public string ExecuteFunction(string[] parameters) {
            string parametersString = "";
            for (int i = 0; i < parameters.Length - 1; i++) {
                parametersString += String.Format(",@p{0}", i);
//                parametersString = "," +/parametersString[i];
            }
            parametersString = parametersString.Remove(0, 1);
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            dataAdapter.SelectCommand = new SqlCommand(
                String.Format("Select [CORP\\IKotvytskyi].[{0}]({1})", parameters[0], parametersString),
                conn);
            for (int i = 0; i < parameters.Length-1; i++)
            {
                dataAdapter.SelectCommand.Parameters.Add("@p"+i.ToString(), SqlDbType.VarChar);
                dataAdapter.SelectCommand.Parameters[i].SqlValue = parameters[i+1];
            }
            DataTable table = new DataTable();
            dataAdapter.Fill(table);
            string result = "Н/Д";
            if (table.Rows.Count > 0) {
                result = table.Rows[0][0].ToString();
            }
            return result;
            //return "SQL" + String.Join("--", parameters);
        }

    }
}
