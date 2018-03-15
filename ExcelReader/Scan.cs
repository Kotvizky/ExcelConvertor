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

        public static string ROW_ID = "$ROW_ID";

        const string ErrorFied = "Errors";

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
        private const string errNegative = "err. negative";

        public bool AllFound { get; private set; }

        public bool printAllFields { private set; get; }
        private DataTable xlsTable;
        public DataTable ResTable { private set; get; }
        public DataRow XlsRow { private set; get;  }
        public DataRow ResRow { private set; get; }
        private List<ValidFunc> ConvertList;

        //public void AddField(DataRow row,
        //    short npp, string resName,string xlsName,bool isPrint,
        //                    string type, short size, bool isPos,
        //                    attrName attr=0,bool isActive = true)

        public void AddField(DataRow row)
        {
            attrName attr = (attrName)row["attr"];
            bool isPrint = (bool)row["isPrint"];

            if (this.Count == 0) printAllFields = false;
            if ((row["resName"].ToString() == "*") && (attr == attrName.Const))
            {
                if ((row["xlsName"].ToString() == "*") && isPrint)
                {
                    printAllFields = true;
                }
                return;
            }

            FieldBase newField = null;

            switch (attr)
            {
                case attrName.Answer:
                    newField = new FieldAnswer(row, this);
                    break;
                case attrName.Const:
                    newField = new FieldConst(row, this);
                    break;
                case attrName.Expr:
                    newField = new FieldExpr(row, this);
                    break;
                case attrName.Func:
                    newField = new FieldFunc(row, this);
                    ((FieldFunc)newField).onInitProgressBar += this.onInitProgressBar;
                    ((FieldFunc)newField).onStepProgressBar += this.onStepProgressBar;
                    ((FieldFunc)newField).onHideProgressBar += this.onHideProgressBar;
                    break;
                case attrName.System:
                    newField = new FieldSystem(row, this);
                    break;
                default:
                    newField = new FieldXls(row, this);
                    break;
            }
            if (newField != null)
            {
                this.Add(newField);
            }
        }


        #region Validation functions

        public static ValidValue ValidToStr(ValidData value)
        {
            ValidValue result = new ValidValue()
                { Value = value.Value.ToString(), Error = String.Empty };

            string strResult = (string)value.Value.ToString();

            if ((value.Size>0) && (strResult.Length > value.Size))
            {
                strResult = strResult.Substring(0, value.Size);
                result.Error = String.Format("str>{0}", value.Size);
            }
            return result;
        }

        public static ValidValue ValidToDouble(ValidData tabValue)
        {

            ValidValue result = new ValidValue()
                { Value = 0m, Error = String.Empty };

            object value = tabValue.Value;
            if (value == DBNull.Value)
            {
                if (Properties.Settings.Default.ShowNullDoubleError) result.Error = errNull;
                return result;
            }


            double outValue;
            switch (value.GetType().Name)
            {
                case "String":
                    if (!Double.TryParse((string)value, out outValue))
                    {
                        result.Error = errConver;
                        return result;
                    }
                    break;
                case "Double":
                    outValue = (double)value;
                    break;
                default:
                    result.Error = errConver;
                    return result;
            }

            if (tabValue.Size > 0)
                outValue = Math.Round(outValue, tabValue.Size);
            if (tabValue.isPos)
            {
                if (outValue < 0)
                    result.Error = errNegative;
                outValue = Math.Max(outValue, 0);
            }
            result.Value = outValue;

            return result;
        }

        public static ValidValue ValidToDT(ValidData tabValue)
        {

            ValidValue result = new ValidValue()
            { Value = new DateTime(0001,1,1), Error = String.Empty };

            object value = tabValue.Value;
            if (value == DBNull.Value)
            {
                //result.Value = new DateTime(1800, 1, 1);
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


        public void Processing()
        {
            InitAllFields();
            GetFunctionResult();
            writeConstants();
            AddExpressions();
            RemoveNotPrint();
        }

        void AddExpressions()
        {
            List<FieldBase> fields = FindAll(x => (x.Attr == attrName.Expr) && x.IsActive);
            foreach(FieldExpr field in fields) field.InitField();
            foreach(DataRow row in ResTable.Rows)
            {
                ResRow = row;
                foreach (FieldExpr field in fields) field.InitValue();
            }
            fields.Reverse();
            foreach (FieldExpr field in fields) field.RemoveField();
        }

        void GetFunctionResult()
        {
            foreach(FieldFunc field in FindAll(x => (x.Attr == attrName.Func) && x.IsActive))
            {
                if (field.initSQLResult())
                {
                    field.InitSqlTable();
                    for (int i = 0; i < ResTable.Rows.Count; i++)
                    {
                        ResRow = this.ResTable.Rows[i];
                        field.InitSqlValues(i);
                    }
                    field.InsertToServer();
                    field.FillResult();
                }
            }
            onHideProgressBar?.Invoke();
        }

        void writeConstants()
        {
            List<FieldBase> fieldsConst = FindAll(x => (x.Attr == attrName.Const) && x.IsPrint);
            if (fieldsConst.Count > 0)
            {
                foreach (DataRow row in ResTable.Rows)
                {
                    foreach (var field in fieldsConst)
                    {
                        row[field.ResName] = field.Value;
                    }
                }
            }

        }

        public void InitXlsFields()
        {
            var resField = FindAll(x => x.IsActive 
                    && ((x.Attr == attrName.Field)  || ((x.Attr == attrName.System) && (x as FieldSystem).isField))
                    ).OrderBy(x => x.Npp);

            ResTable = new DataTable();
            ResTable.TableName = "ResTable";
            ResTable.Locale = CultureInfo.InvariantCulture;
            foreach (FieldBase field in resField)
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
                else if (field.Attr == attrName.System)
                {
                    ResTable.Columns.Add(field.ResName, field.Type);
                }
            }
        }

        void InitAllFields()
        {
            DataColumnCollection columns = ResTable.Columns;
            List<FieldBase> newFields = this.FindAll(x => x.IsActive && (x.Attr != attrName.Expr)).OrderBy(x => x.Npp).ToList();

            foreach (FieldBase field in newFields)
            {
                DataColumn column = null;
                int npp = newFields.IndexOf(field);
                string colNume = ((field.Attr == attrName.Answer) & (field.ResName.IndexOf(".") > 0)) 
                    ? field.ResName.Split('.')[1]
                    :field.ResName;
                if (columns.Contains(colNume))
                    column = columns[colNume];
                else column = ResTable.Columns.Add(colNume, field.Type);
                column.SetOrdinal(npp);
            }
        }

        void RemoveNotPrint()
        {
            List<FieldBase> newFields = this.FindAll(x => x.IsActive && !x.IsPrint ).OrderBy(x => x.Npp).ToList();
            DataColumnCollection columns = ResTable.Columns;
            foreach (FieldBase field in newFields)
            {
                if (columns.Contains(field.ResName))
                {
                    columns.Remove(columns[field.ResName]);
                    continue;
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
            AllFound = true;
            DataColumnCollection columns = xlsTable.Columns;

            string message = "Поля \r\n -----";
            foreach (FieldXls field in this.FindAll(x => (x.Attr == attrName.Field) & x.IsActive))
            {
                if (columns.Contains(field.XlsName))
                {
                    string reportValidation = setValidator(field,columns[field.XlsName].DataType.Name);
                    if (reportValidation != String.Empty)
                    {
                        message += String.Format("\r\n(+):\t{0} -> {1}, convert error {2}", field.XlsName, field.ResName,reportValidation);
                        field.Exist = false;
                        AllFound = false;
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
                    AllFound = false;
                }
            }

            message += "\r\n\r\nКонстанты \r\n -----";

            foreach (FieldBase field in this.FindAll(x => (x.Attr == attrName.Const) & x.IsActive))
            {
                string reportValidation = setValidator(field, dataType.String.ToString());
                if (reportValidation != String.Empty)
                {
                    message += String.Format("\r\n{0} -> {1}\r\n", field.ResName, reportValidation);
                }
                else 
                {
                    string reportInit = field.InitValue();
                    if (reportInit == String.Empty)
                    {
                        message += String.Format("\r\n{0} -> {1} ({2})\r\n", field.ResName, field.Validator.Method.Name,field.Value);
                        field.Exist = true;
                    }
                    else
                    {
                        message += String.Format("\r\n{0} -> {1} (convert error {2})\r\n", field.ResName, field.Validator.Method.Name, reportInit);
                        AllFound = false;
                    }
                }
            }

            message += "\r\n\r\nФункции \r\n -----";

            foreach (FieldFunc fieldFunc in this.FindAll(x => (x.Attr == attrName.Func) && x.IsActive))
            {

                fieldFunc.initSQLParameter(this);

                ArrayList paramGpoups = new ArrayList(new ParamGroup[]  {
                    fieldFunc.ParamsField,
                    fieldFunc.ParamsIn,
                    fieldFunc.ParamsOut
                });

                string[] groupName = new string[] { "ParamsField", "ParamsIn", "ParamsOut" };

                //foreach (ParamGroup paramGroup in paramGpoups)
                for (int i = 0; i < paramGpoups.Count; i++)
                {
                    ParamGroup paramGroup = (ParamGroup)paramGpoups[i];

                    if (paramGroup == null) {
                        message += String.Format("\r\n\r\n{0}  Not Ready ",
                            groupName[i]);
                            AllFound = false;
                    }
                    else
                    { 
                        message += String.Format("\r\n\r\n{0}  Ready - {1} < ====== {2} \r\n---\r\n", 
                            paramGroup.GroupName, paramGroup.AllFound,paramGroup.msgError);

                        if (!paramGroup.AllFound) AllFound = false;

                        foreach (ParamBase param in paramGroup)
                        {
                            message += String.Format("\r\n{0}", param.Print());
                        }
                    }

                }
            }

            message += "\r\n Ответ сервера \r\n -----";

            foreach (FieldAnswer field in this.FindAll(x => ((x.Attr == attrName.Answer) && x.IsActive))) {
                if (field.Param == null)
                {
                    message += String.Format("\r\nParameter for field {0} not found" , field.ResRow);
                    AllFound = false;
                }
                else
                {
                    message += String.Format("\r\n {0} ok", field.ResName);
                }
            }

            return message;
        }

        public void ChechFields()
        {
            ResTable.Rows.Clear();
            if (!ResTable.Columns.Contains(ErrorFied))
            {
                ResTable.Columns.Add(ErrorFied,typeof(String)).SetOrdinal(0);
            }

            onInitProgressBar?.Invoke(xlsTable.Rows.Count);
            var fields = this.FindAll(x => ((x.Attr == attrName.Field) 
                | ( (x.Attr == attrName.System) && (x as FieldSystem).isField )) && x.IsActive);
            foreach (DataRow curRow in xlsTable.Rows)
            {
                XlsRow = curRow;
                ResRow = ResTable.NewRow();
                ResTable.Rows.Add(ResRow);
                foreach (FieldBase field in fields)
                {
                    string error = field.InitValue();
                    ResRow[ErrorFied] = ResRow[ErrorFied] + error;
                    onStepProgressBar?.Invoke();
                }
            }
            onHideProgressBar?.Invoke();
        }

        string setValidator(FieldBase field, string inTypeStr)
        {
            string result = String.Empty;
            bool convert = true;
            try
            {
                dataType inType = (dataType)Enum.Parse(typeof(dataType), inTypeStr);
                dataType outType = (dataType)Enum.Parse(typeof(dataType), field.Type.Name);
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
                result = String.Format("{0} {1}->{2}", errConver, inTypeStr, field.Type.Name);
            }
            return result;
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
            public Func<ValidData, ValidValue> Validator { set; get; }
        }


        public string GetXlsFields()
        {
            string result = "";
            var resField = FindAll(x => x.IsActive
                    && ((x.Attr == attrName.Field) || ((x.Attr == attrName.System) && (x as FieldSystem).isField))
                    ).OrderBy(x => x.Npp);
            foreach (FieldBase field in resField) result += String.Format("{0};", field.XlsName);
            return result;
        }

    }
}
