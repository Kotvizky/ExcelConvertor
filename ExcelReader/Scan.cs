﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Net.NetworkInformation;



namespace ExcelReader
{
    class Scan : List<Field>
    {

        public delegate void ShowMessage(string message);
        public event ShowMessage onShowMessage;

        public bool AllFound()  {
            bool result = true;
            bool fieldsExist = false; 
            foreach (Field field in this) {
                if (field.Attr == 0) {
                    if (field.Exist)
                    {
                        fieldsExist = true;
                    }
                    else {
                        result = false;
                        break;
                    }
                } 
            }
            if (!fieldsExist) result = false;
            return result;
        }

        public DataTable resultTable { private set; get; }

        public void AddField(string resName,string xlsName,bool isPrint,
                            attrName attr=0, bool isActive = true) {
            this.Add(new Field
            {
                ResName = resName,
                XlsName = xlsName,
                IsPrint = isPrint,
                Attr = attr,
                IsActive = isActive
            });
        }

        public void initResutTable() { 
            var resField = FindAll(x => x.IsPrint & x.IsActive);
            resultTable = new DataTable(); 
            foreach (Field field in resField) {
                if (field.Attr == attrName.Field)
                {
                    resultTable.Columns.Add(field.ResName, typeof(string));
                } else if ((field.Attr == attrName.Answer) & (field.ResName.IndexOf(".") > 0))
                {
                    resultTable.Columns.Add(field.ResName.Split('.')[1], typeof(string));
                }
            }
        }

        public void SetValues(DataRow row) {
            foreach (Field field in this.FindAll(x => (x.IsActive & (x.Attr == 0) & x.Exist))) {
                field.Value = row[field.XlsName].ToString();
            }
        }

        public string GetValue(string Name)
        {

            string result = String.Empty;
            Field field = this.Find(x => x.ResName == Name);
            if (field == null) return "";
            switch (field.Attr) {
                case attrName.Field:
                    result = field.Value;
                    break;
                case attrName.Const:
                    result = field.XlsName;
                    break;
                case attrName.Func:
                    result = GetSQLValue(field);
                    break;
                //case attrName.Func:
                //    result = GetSQLValue(field);
                //    break;
            }
            return result;
        }

        private string[] XlsNameToArray(string XlsName)
        {
            string preperedString = XlsName.Split(new Char[] { ' ', ')' })[0];
            return preperedString.Split(new Char[] { ' ', ',', '(' });
        }

        private string GetSQLValue(Field field) {

            string[] parameters = XlsNameToArray(field.XlsName);
            string result = "";
            try {
                for (int i = 1; i < parameters.Length; i++)
                {
                    parameters[i] = this.Find(x => x.ResName == parameters[i]).Value;
                }
                result = SQLFunction.ExecuteFunction(parameters);
            } catch
            {
                result = "NoData";
            }
            return result;
        }

        public string Matching(DataColumnCollection tableHead) { //TODO finish matching with report
            string message = "Поля \r\n -----";
            foreach (Field field in this.FindAll(x => (x.Attr == attrName.Field) & x.IsActive))
            {
                if (tableHead.Contains(field.XlsName))
                {
                    message += String.Format("\r\n(+):\t{0} -> {1}", field.XlsName, field.ResName);
                    field.Exist = true;
                }
                else
                {
                    message += String.Format("\r\n(-):\t{0} -> {1}\tполе не найдено!", field.XlsName, field.ResName);
                }
            }

            message += "\r\n\r\nФункции \r\n -----";

            foreach (Field field in this.FindAll(x => (x.Attr == attrName.Func) & x.IsActive))
            {
                field.parseSQLParameter(this);
                foreach(FunctionFields functionFields in field.Parameters)
                {
                    message += String.Format("\r\n{0}  Ready - {1} < ====== \r\n",functionFields.Group,functionFields.Ready);
                    for (int i = 0; i < functionFields.parameters.Length; i++)  {
                        message += String.Format("\r\n{0}", functionFields.parameters[i].Print());
                    }
                }
            }
            return message;
        }

        public string insertString(Field field, DataTable table, string ip, bool clearData = false )
        {
            string result = "";
            if (clearData)
            {
                result = String.Format("delete from {0} ", field.SQLTable);
            }

            if (field.Attr != attrName.Func)
            {
                return result;
            }
            string fields = "";
            string values = "";
            FunctionFields tableParameters = field.Parameters.Find(x => (x.Group == paramGroup.inTable));
            foreach (DataRow row in table.Rows)
            {
                SetValues(row);
                for (int i = 0; i < tableParameters.parameters.Length; i++)
                {
                    fields += String.Format(",{0}", tableParameters.parameters[i].SqlName) ;
                    if (tableParameters.parameters[i].Service)
                    {
                        serviseFields fieldType = (serviseFields) Enum.Parse(typeof(serviseFields), tableParameters.parameters[i].SqlName);
                        switch (fieldType)
                        {
                            case (serviseFields.IP):
                                values += String.Format(", {0}", ip); 
                                break;
                            case (serviseFields.ROW_ID):
                                values += String.Format(", {0}" , table.Rows.IndexOf(row));
                                break;
                        }
                    }
                    else
                    {
                        values += String.Format(",'{0}'", GetValue(tableParameters.parameters[i].ResName).Replace("'","''")) ;
                    }
                }
                result += String.Format("\ninsert into {0}({1}) values({2})",
                    field.SQLTable,
                    fields.Remove(0, 1),
                    values.Remove(0, 1)
                    );
                fields = "";
                values = "";
            }
            return result;
        }

        public void insertData(Field field, DataTable table,  bool clearData = false)
        {
            onShowMessage?.Invoke("insertData");
            string sqlDelete = "";
            if (clearData)
            {
                sqlDelete = String.Format("delete from {0} where IP = 0x{1} ", field.SQLTable,GetLocalIPAddress(true));
            }
            byte[] ip = GetLocalIPByte();
            FunctionParameter[] parameters = field.Parameters.Find(x => (x.Group == paramGroup.inTable)).parameters;
            string[] fieldsArray = new string[parameters.Length] ;
            for (int i = 0; i < parameters.Length; i++)
            {
                fieldsArray[i] = parameters[i].SqlName;
            }
            string insertCommand = String.Format("insert into {0}({1}) values (@{2})", 
                    field.SQLTable, String.Join(",", fieldsArray), String.Join(",@", fieldsArray));
            SQLFunction.initCommand(insertCommand, fieldsArray, sqlDelete);

            string fields = "";
            foreach (DataRow row in table.Rows)
            {
                //!!!SetValues(row);
                SetValues(row);
                ArrayList values = new ArrayList(parameters.Length);
                for (int i = 0; i < parameters.Length; i++)
                {
                    fields += String.Format(",{0}", parameters[i].SqlName) ;
                    if (parameters[i].Service)
                    {
                        serviseFields fieldType = (serviseFields) Enum.Parse(typeof(serviseFields), parameters[i].SqlName);
                        switch (fieldType)
                        {
                            case (serviseFields.IP):
                                values.Add(ip); 
                                break;
                            case (serviseFields.ROW_ID):
                                values.Add(table.Rows.IndexOf(row));
                                break;
                        }
                    }
                    else
                    {
                        values.Add(GetValue(parameters[i].ResName));
                        ///!!!values.Add("xxx");

                    }
                }
                SQLFunction.preperedInsert(values);
            }
        }

        public string funcionString(Field field,string ip)
        {

            FunctionFields funcParameters = field.Parameters.Find(x => (x.Group == paramGroup.inPar));
            string values = String.Empty;
            for (int i = 0; i < funcParameters.parameters.Length; i++)
            {
                if (funcParameters.parameters[i].Service)
                {
                    serviseFields fieldType = (serviseFields)Enum.Parse(typeof(serviseFields), funcParameters.parameters[i].SqlName);
                    switch (fieldType)
                    {
                        case (serviseFields.IP):
                            values += String.Format(", '{0}'", ip);
                            break;
                    }
                }
                else
                {
                    values += String.Format(",'{0}'", GetValue(funcParameters.parameters[i].ResName).Replace("'", "''"));
                }
            }
            return String.Format("select * from {0}({1}) order by row_id",field.FunctionName,values.Remove(0,1));
        }

        public void columnsRename(Field field)
        {
            FunctionParameter[] parameters = field.Parameters.Find(x => (x.Group == paramGroup.outPar)).parameters;
            for(int i = 0; i < parameters.Length; i++)
            {
                if (field.resTable.Columns.Contains(parameters[i].SqlName) & (parameters[i].ResName != null))
                {
                    field.resTable.Columns[parameters[i].SqlName].ColumnName = parameters[i].ResName;
                }
            }
        }

        public void WriteResult(DataTable XlsTable)
        {  //TODO Add event for strip progress bar
            onShowMessage?.Invoke("WriteResult");
            foreach (DataRow row in XlsTable.Rows)
            {
                DataRow newRow = resultTable.NewRow();
                SetValues(row);
                foreach (DataColumn column in resultTable.Columns)
                {
                    newRow[column.ColumnName] = GetValue(column.ColumnName);
                }
                resultTable.Rows.Add(newRow);
            }
            onShowMessage?.Invoke("");
        }


        public void initData(Field field, DataTable table, bool clearData = false)
        {

            onShowMessage?.Invoke("Insert Data");
            insertData(field, table, clearData);

            string resFunction = funcionString(field, GetLocalIPAddress());
            onShowMessage?.Invoke("Get SQL Data");
            field.resTable = SQLFunction.executeSQL(resFunction);
            columnsRename(field);
            //File.WriteAllText("C:\\Users\\IKotvytskyi\\Desktop\\query.sql", resInsert);
            //File.WriteAllText("C:\\Users\\IKotvytskyi\\Desktop\\function.sql", resFunction);
        }

        public static string GetLocalIPAddress(bool isHex = false)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (isHex)
                    {
                        string[] ipStr = ip.ToString().Split('.');
                        byte[] bytes = new byte[ipStr.Length];
                        for (int i = 0; i < ipStr.Length; i++)
                        {
                            bytes[i] = Convert.ToByte(Int32.Parse(ipStr[i]));
                        }

                        //PhysicalAddress address = adapter.GetPhysicalAddress();
                        //byte[] bytes = address.GetAddressBytes();

                        Array.Resize<byte>(ref bytes, 4);
                        return BitConverter.ToString(bytes).Replace("-", String.Empty);
                    }
                    else
                    {
                        return ip.ToString();
                    }
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static byte[] GetLocalIPByte()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {

                    string[] ipStr = ip.ToString().Split('.');
                    byte[] bytes = new byte[ipStr.Length];
                    for (int i = 0; i < ipStr.Length; i++)
                    {
                        bytes[i] = Convert.ToByte(Int32.Parse(ipStr[i]));
                    }

                    //PhysicalAddress address = adapter.GetPhysicalAddress();
                    //byte[] bytes = address.GetAddressBytes();

                    //Array.Resize<byte>(ref bytes, 4);

                    //byte[] ipByte = BitConverter.GetBytes(ip.Address);
                    //Array.Resize<byte>(ref ipByte, 4);
                    return bytes;
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


    }
}
