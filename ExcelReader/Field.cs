using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class Field
    {
        private const string noData = "Н/Д";

        // fields in database
        public string ResName { set; get; }
        public string XlsName { set; get; }
        public bool IsPrint { set; get; }
        public byte Attr { set; get; }
        public string StrFormat { set; get; }
        public bool IsActive { set; get; }
        // <-- fields in database

        public bool Exist { set; get; }
        private string value;
        public string Value {
            set {
                this.value = value;
            }
            get {
                if (this.Exist)
                {
                    return this.value;
                }
                else
                {
                    return noData;
                }
            }
        }
    }


}
