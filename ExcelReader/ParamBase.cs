using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    abstract class ParamBase
    {

        public ParamBase(string paramName, string outName)
        {
            ParamName = paramName;
            OutName = outName;
        }


        public ParamBase(string paramName, string outName, FieldFunc ownField) 
            :this(paramName, outName)
        {
            OwnField = ownField;
        }

        public virtual FieldBase OutField { protected set; get; }

        public string ParamName { set; get;  }
        public string OutName { set; get; }

        //public abstract object Value { set; get; }
        public bool Service { protected set; get; }
        public bool XlsExist { protected set; get; }

        public string Error {protected set; get;}

        protected FieldFunc OwnField; 

        public FieldBase Field {protected set; get; }

        public abstract void InitField();
        public abstract object Value { get; }

        public string Print()
        {
            return String.Format("SqlName - {0};\t\t\tResName - {1};\t\t\tService - {2};\t\t\txlsExist - {3}",
                OutName, ParamName, Service.ToString(), XlsExist.ToString());
        }

        public object[] fieldNames()
        {
            return new object[] { "SqlName", "ResName", "Service","txlsExist","Error" };
        }

        public object[] fieldValues()
        {
            return new object[] { OutName, ParamName, Service, XlsExist , Error};
        }


    }
}
