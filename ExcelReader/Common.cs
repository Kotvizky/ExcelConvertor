using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{

    public struct FunctionParameter
    {
        public string SqlName;
        public string ResName;
        public string TableName;
        public object Value;
        public bool Service;
        public bool xlsExist;

        public string Print()
        {
            return String.Format("SqlName - {0};\t\t\tResName - {1};\t\t\tService - {2};\t\t\txlsExist - {3}",
                SqlName,ResName,Service.ToString(),xlsExist.ToString());
        } 
    }

    public struct ValidValue
    {
        public object Value;
        public string Error;
    }

    public struct ValidData
    {
        public object Value;
        public int Size;
        public bool isPos;
    }

    enum attrName : byte { Field = 0, Func, Answer, Const, Expr, System };
    enum funcParameter : byte { Table = 1, In, Out }
    enum spetialFields { Ip, RowId }
    enum GroupNames { inTable, tabFields, inPar, outPar }
    enum serviseFields { IP, ROW_ID }
    enum dataType { String, DateTime, Double }

}
