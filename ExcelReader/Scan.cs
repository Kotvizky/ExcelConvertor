using System;
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
using System.Globalization;



namespace ExcelReader
{
    class Scan : List<FieldBase>
    {
        public Scan()
        {
            printAllFields = false;
            ConvertList = new List<ValidFunc>();
            FillConvertList();
        }

        private void FillConvertList()
        {
            // string out type
            ConvertList.Add(new ValidFunc()
            {
                InType = dataType.String,
                OutType = dataType.String,
                Validator = ValidToStr
            }
            );
            ConvertList.Add(new ValidFunc()
            {
                InType = dataType.Double,
                OutType = dataType.String,
                Validator = ValidToStr
            });
            ConvertList.Add(new ValidFunc()
            {
                InType = dataType.DateTime,
                OutType = dataType.String,
                Validator = ValidToStr
            });

            // decimal out type
            ConvertList.Add(new ValidFunc()
            {
                InType = dataType.String,
                OutType = dataType.Double,
                Validator = ValidToDouble
            });
            ConvertList.Add(new ValidFunc()
            {
                InType = dataType.Double,
                OutType = dataType.Double,
                Validator = ValidToDouble
            });

            // dateTime out type
            ConvertList.Add(new ValidFunc()
            {
                InType = dataType.String,
                OutType = dataType.DateTime,
                Validator = ValidToDT
            });
            ConvertList.Add(new ValidFunc()
            {
                InType = dataType.DateTime,
                OutType = dataType.DateTime,
                Validator = ValidToDT
            });
        }

        public delegate void ShowMessage(string message);
        public event ShowMessage onShowMessage;

        public delegate void initProgressBar(int maximum);
        public event initProgressBar onInitProgressBar;

        public delegate void hideProgressBar();
        public event hideProgressBar onHideProgressBar;

        public delegate void stepProgressBar();
        public event stepProgressBar onStepProgressBar;

        private const string errNull = "null";
        private const string errConver = "err. convert";

        public bool printAllFields { private set; get; }
        private DataTable xlsTable;
        public DataTable ResTable { private set; get; }
        public DataRow XlsRow { private set; get;  }
        public DataRow ResRow { private set; get; }
        private List<ValidFunc> ConvertList;

        public void AddField(short npp, string resName,string xlsName,bool isPrint,
                            string type, short size,
                            attrName attr=0, bool isActive = true) {
            if (this.Count == 0) printAllFields = false;
            if ((resName == "*") && (attr == attrName.Const))
            {
                if ((xlsName == "*") && isPrint)
                {
                    printAllFields = true;
                }
                return;
            }

            FieldBase newField = null;

            switch (attr)
            {
                case attrName.Func:
                    newField = new FieldFunc();
                    break;
                default:
                    newField = new FieldXls();
                    break;
            }
            if (newField != null)
            {
                newField.Scan = this;
                newField.Npp = npp;
                newField.ResName = resName;
                newField.XlsName = xlsName;
                newField.IsPrint = isPrint;
                newField.Attr = attr;
                newField.IsActive = isActive;
                newField.Type = Type.GetType(String.Format("System.{0}",type));
                newField.DataSize = size;
                this.Add(newField);
            }
        }

        public bool AllFound()  {
            bool result = true;
            bool fieldsExist = false; 
            foreach (FieldBase field in this) {
                switch (field.Attr)
                {
                    case attrName.Field: 
                        if (field.Exist)
                        {
                            fieldsExist = true;
                        }
                        else {
                            result = false;
                            break;
                        }
                        break;
                    case attrName.Func:

                        foreach (FunctionFields funcField in ((FieldFunc)field).Parameters)
                        {
                            FunctionParameter[] funcParams = funcField.parameters;
                            for (int i = 0; i < funcParams.Length; i++)
                            {
                                if (!funcParams[i].xlsExist)
                                {
                                    result = false;
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            if (!fieldsExist) result = false;
            return result;
        }

        #region Validation functions

        public static FieldXls.ValidValue ValidToStr(FieldXls.ValidData value)
        {
            FieldXls.ValidValue result = new FieldXls.ValidValue()
                { Value = value.Value.ToString(), Error = String.Empty };

            string strResult = (string)value.Value.ToString();

            if ((value.Size>0) && (strResult.Length > value.Size))
            {
                strResult = strResult.Substring(0, value.Size);
                result.Error = String.Format("str>{0}", value.Size);
            }
            return result;
        }

        public static FieldXls.ValidValue ValidToDouble(FieldXls.ValidData tabValue)
        {

            FieldXls.ValidValue result = new FieldXls.ValidValue()
                { Value = 0m, Error = String.Empty };

            object value = tabValue.Value;
            if (value.GetType().Equals(typeof(String)))
            {
                value = ((string)value).Replace(',','.');
            }
            if (value == DBNull.Value)
            {
                result.Error = errNull;
                result.Value = 0D;
            }
            else
            {
                try
                {
                    result.Value =  Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    if (ex is FormatException)
                        // FormatException
                        result.Error = errConver;

                    throw;
                }
            }
            return result;
        }

        public static FieldXls.ValidValue ValidToDT(FieldXls.ValidData tabValue)
        {

            FieldXls.ValidValue result = new FieldXls.ValidValue()
            { Value = new DateTime(0001,1,1), Error = String.Empty };

            object value = tabValue.Value;
            if (value == DBNull.Value)
            {
                result.Error = errNull;
            }
            else
            {
                try
                {
                    result.Value = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                {
                    result.Error = errConver;
                }
            }
            return result;
        }

        #endregion


        #region Initialization Result table
        public void InitXlsFields()
        {
            var resField = FindAll(x => x.IsActive && (x.Attr == attrName.Field)).OrderBy(x => x.Npp);

            ResTable = new DataTable();
            foreach (FieldXls field in resField)
            {
                if (field.Attr == attrName.Field)
                {
                    ResTable.Columns.Add(field.ResName, field.Type);
                }
                else if ((field.Attr == attrName.Answer) & (field.ResName.IndexOf(".") > 0))
                {
                    ResTable.Columns.Add(field.ResName.Split('.')[1], typeof(string));
                }
                else if (field.Attr == attrName.Const)
                {
                    ResTable.Columns.Add(field.ResName, typeof(string));
                }
            }
        }

        public void InitAllField()
        {
            List<FieldBase> newFields = this.FindAll(x => x.IsActive && x.IsPrint ).OrderBy(x => x.Npp).ToList();

            foreach (FieldBase field in newFields)
            {
                if (field.Attr == attrName.Field) continue;
                DataColumn column = null;
                if ((field.Attr == attrName.Answer) & (field.ResName.IndexOf(".") > 0))
                {
                    column = ResTable.Columns.Add(field.ResName.Split('.')[1], field.Type);
                }
                else if (field.Attr == attrName.Const)
                {
                    column = ResTable.Columns.Add(field.ResName, field.Type);
                }
                if (column != null)
                {
                    column.SetOrdinal(newFields.IndexOf(field));
                }
            }
        }

        private void delFiledsFromRes()
        {
            var fields = FindAll(x => x.IsActive && !x.IsPrint && (x.Attr == attrName.Field));
            DataColumnCollection columns = ResTable.Columns;
            foreach (FieldBase field in fields)
            {
                if (columns.Contains(field.ResName)) columns.Remove(columns[field.ResName]);
            }
        }


        public void InitResutTable() { 
            var resField = FindAll(x => x.IsActive &&( x.Attr != attrName.Func) ).OrderBy(x => x.Npp);
            
            ResTable = new DataTable();
            foreach (FieldXls field in resField) {
                if (field.Attr == attrName.Field)
                {
                    ResTable.Columns.Add(field.ResName, field.Type);
                }
                else if ((field.Attr == attrName.Answer) & (field.ResName.IndexOf(".") > 0))
                {
                    ResTable.Columns.Add(field.ResName.Split('.')[1], typeof(string));
                }
                else if (field.Attr == attrName.Const)
                {
                    ResTable.Columns.Add(field.ResName, typeof(string));
                }
            }
        }

        public void initResultFromXls(DataTable table) {
            var resField = FindAll(x => x.IsPrint & x.IsActive & (x.Attr != attrName.Func)).OrderBy(x => x.Npp);
            ResTable = table.Copy();
            int i = -1;
            foreach (FieldXls field in resField)
            {
                DataColumn column = null;
                if ((field.Attr == attrName.Field)  )
                {
                    if (ResTable.Columns.Contains(field.ResName))
                    {
                        continue;
                    }
                    i++;
                    column = ResTable.Columns.Add(field.ResName, typeof(string));
                }
                else if ((field.Attr == attrName.Answer) & (field.ResName.IndexOf(".") > 0)  )
                {
                    if (ResTable.Columns.Contains(field.ResName.Split('.')[1]))
                    {
                        continue;
                    }
                    i++;
                    column = ResTable.Columns.Add(field.ResName.Split('.')[1], typeof(string));
                }
                else if ( (field.Attr == attrName.Const) )
                {
                    if (ResTable.Columns.Contains(field.ResName)) continue;
                    i++;
                    column = ResTable.Columns.Add(field.ResName, typeof(string));
                }
                if (column != null)
                {
                    column.SetOrdinal(i);
                }
            }
        }
        #endregion


        public void SetValues(DataRow row) { //TODO remove set method "SetValues"
            foreach (FieldXls field in this.FindAll(x => (x.IsActive & (x.Attr == 0) & x.Exist))) {
                /// !!! field.Value = row[field.XlsName].ToString();
            }
        }

        public string GetValue(string Name) //TODO remove this "GetValue"
        {
            string result = String.Empty;
            FieldBase field = this.Find(x => x.ResName == Name);
            if (field == null) return "";
            switch (field.Attr) {
                case attrName.Field:
                    /// !!!! result = ((FieldXls)field).Value;
                    break;
                case attrName.Const:
                    result = field.XlsName;
                    break;
                case attrName.Func:
                    //!!! result = GetSQLValue((FieldFunc)field);
                    break;
            }
            return result;
        }

        private string[] XlsNameToArray(string XlsName)
        {
            string preperedString = XlsName.Split(new Char[] { ' ', ')' })[0];
            return preperedString.Split(new Char[] { ' ', ',', '(' });
        }


        //private string GetSQLValue(FieldFunc field)
        //{

        //    string[] parameters = XlsNameToArray(field.XlsName);
        //    string result = "";
        //    try
        //    {
        //        for (int i = 1; i < parameters.Length; i++)
        //        {
        //            parameters[i] = this.Find(x => x.ResName == parameters[i]).Value;
        //        }
        //        result = SQLFunction.ExecuteFunction(parameters);
        //    }
        //    catch
        //    {
        //        result = "NoData";
        //    }
        //    return result;
        //}

        public string Matching(DataTable table) { 

            xlsTable = table;
            DataColumnCollection columns = xlsTable.Columns;

            string message = "Поля \r\n -----";
            foreach (FieldXls field in this.FindAll(x => (x.Attr == attrName.Field) & x.IsActive))
            {
                if (columns.Contains(field.XlsName))
                {
                    string reportValidation = setValidator(field,columns[field.XlsName]);
                    if (reportValidation != String.Empty)
                    {
                        message += String.Format("\r\n(+):\t{0} -> {1}, convert error {2}", field.XlsName, field.ResName,reportValidation);
                        field.Exist = false;
                    }
                    else
                    {
                        message += String.Format("\r\n(+):\t{0} -> {1}, {2}", field.XlsName, field.ResName,field.Validator.Method.Name);
                        field.Exist = true;
                    }
                }
                else
                {
                    message += String.Format("\r\n(-):\t{0} -> {1}\tполе не найдено!", field.XlsName, field.ResName);
                }
            }

            message += "\r\n\r\nФункции \r\n -----";

            foreach (FieldBase fieldType in this.FindAll(x => (x.Attr == attrName.Func) && x.IsActive))
            {
                FieldFunc field = (FieldFunc)fieldType;
                field.parseSQLParameter(this);
                foreach(FunctionFields functionFields in field.Parameters)
                {
                    message += String.Format("\r\r\n{0}  Ready - {1} < ====== \r\n", functionFields.Group,functionFields.Ready);
                    for (int i = 0; i < functionFields.parameters.Length; i++)  {
                        message += String.Format("\r\n{0}", functionFields.parameters[i].Print());
                    }
                }
            }
            return message;
        }

        public void ChechFields()
        {
            string ErrorFied = "Errors";
            var fields = this.FindAll(x => x.GetType().Equals(typeof(FieldXls)) && (x.Attr == attrName.Field));
            ResTable.Rows.Clear();
            if (!ResTable.Columns.Contains(ErrorFied))
            {
                ResTable.Columns.Add(ErrorFied,typeof(String)).SetOrdinal(0);
            }

            foreach (DataRow curRow in xlsTable.Rows)
            {
                XlsRow = curRow;
                ResRow = ResTable.NewRow();
                ResTable.Rows.Add(ResRow);
                foreach (FieldXls field in fields)
                {
                    string error = field.InitValue();
                    ResRow[ErrorFied] = ResRow[ErrorFied] + error; 
                }
            }

        }

        string setValidator(FieldXls field,DataColumn column)
        {
            //string inType = field.Type.Name;
            //string outType = column.DataType.Name;

            string result = String.Empty;
            bool convert = true;

            try
            {
                dataType outType= (dataType)Enum.Parse(typeof(dataType), field.Type.Name);
                dataType inType = (dataType)Enum.Parse(typeof(dataType), column.DataType.Name);

                var func = ConvertList.Find(x => (x.InType == inType) && (x.OutType == outType));

                if (func != null)
                {
                    field.Validator = func.Validator;
                }
                else
                {
                    convert = false;
                }

            }
            catch (ArgumentException)
            {
                convert = false;
            }

            if (!convert)
            {
                result = String.Format("{0} {1}->{2}", errConver, column.DataType.Name, field.Type.Name);
            }

            return result;
        }

        public string insertString(FieldFunc field, DataTable table, string ip, bool clearData = false )
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
                /// !!!SetValues(row); 
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

        public void insertData(FieldFunc field, DataTable table, bool clearData = false)
        {
            onShowMessage?.Invoke("insertData");
            string sqlDelete = "";
            if (clearData)
            {
                sqlDelete = String.Format("delete from {0} where IP = 0x{1} ", field.SQLTable, GetLocalIPAddress(true));
            }

            SQLFunction.clearTable(sqlDelete);



            byte[] ip = GetLocalIPByte();
            FunctionParameter[] parameters = field.Parameters.Find(x => (x.Group == paramGroup.inTable)).parameters;
/*            string[] fieldsArray = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                fieldsArray[i] = parameters[i].SqlName;
            }
            string insertCommand = String.Format("insert into {0}({1}) values (@{2})",
                    field.SQLTable, String.Join(",", fieldsArray), String.Join(",@", fieldsArray));
            SQLFunction.initCommand(insertCommand, fieldsArray, sqlDelete);
*/
            DataTable inTable = new DataTable();

            for (int i = 0; i < parameters.Length; i++)
            {
                DataColumn column = new DataColumn();
                if (parameters[i].Service && (parameters[i].SqlName == serviseFields.IP.ToString()))
                {
                    inTable.Columns.Add(new DataColumn(
                                            parameters[i].SqlName,
                                            Type.GetType("System.Byte[]")));
                }
                else if (parameters[i].Service && (parameters[i].SqlName == serviseFields.ROW_ID.ToString()))
                {
                    inTable.Columns.Add(new DataColumn(
                                            parameters[i].SqlName,
                                            Type.GetType("System.Int32")));
                }
                else
                {
                    inTable.Columns.Add(new DataColumn(
                                            parameters[i].SqlName,
                                            Type.GetType("System.String")));
                }
            }

            onInitProgressBar?.Invoke(table.Rows.Count);

            DataRow[] rows = new DataRow[table.Rows.Count];
            //foreach (DataRow row in table.Rows)
            for (int iRow = 0; iRow < table.Rows.Count; iRow++)
            {
                DataRow row = table.Rows[iRow];
                rows[iRow] = inTable.NewRow();

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].TableName != null)
                    {
                        rows[iRow][parameters[i].SqlName] = row[parameters[i].TableName];
                    }
                    else if (parameters[i].Value != null)
                    {
                        rows[iRow][parameters[i].SqlName] = parameters[i].Value;
                    }
                    else if (parameters[i].SqlName == serviseFields.ROW_ID.ToString())
                    {
                        rows[iRow][parameters[i].SqlName] = table.Rows.IndexOf(row);
                    }
                }
                onStepProgressBar?.Invoke();
            }

            SQLFunction.bulkWrite(field.SQLTable, rows);
            /*

        foreach (DataRow row in table.Rows)
        {

            //SQLFunction.preperedInsert(values);

                            ArrayList values = new ArrayList(parameters.Length);
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].TableName != null)
                                {
                                    values.Add(row[parameters[i].TableName]);
                                }
                                else if (parameters[i].Value!= null)
                                {
                                    values.Add(parameters[i].Value);
                                }
                                else if (parameters[i].SqlName == serviseFields.ROW_ID.ToString())
                                {
                                    values.Add(table.Rows.IndexOf(row));
                                }
                            }
                            SQLFunction.preperedInsert(values);

            onStepProgressBar?.Invoke();
            }
            */
            onHideProgressBar?.Invoke();

        }

        public string functionString(FieldFunc field,string ip)
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

        public void columnsRename(FieldFunc field)
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
        {  
            onShowMessage?.Invoke("WriteResult");
            onInitProgressBar?.Invoke(XlsTable.Rows.Count);
            foreach (DataRow row in XlsTable.Rows)
            {
                DataRow newRow = ResTable.NewRow();
                SetValues(row);
                foreach (DataColumn column in ResTable.Columns)
                {
                    if (column.ColumnName == "ROW_ID") continue;
                    newRow[column.ColumnName] = GetValue(column.ColumnName);
                }
                ResTable.Rows.Add(newRow);
                onStepProgressBar?.Invoke();
            }
            onHideProgressBar?.Invoke();
            onShowMessage?.Invoke("");
        }

        public void initSQLData(FieldFunc field, DataTable table, bool clearData = false)
        {

            onShowMessage?.Invoke("Insert Data");
            insertData(field, table, clearData);

            string resFunction = functionString(field, GetLocalIPAddress());
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

        public class ValidFunc
        {
            public dataType InType { set; get; }
            public dataType OutType { set; get; }
            public Func<FieldXls.ValidData, FieldXls.ValidValue> Validator { set; get; }
        }

    }
}
