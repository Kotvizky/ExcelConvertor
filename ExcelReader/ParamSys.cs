using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class ParamSys : ParamBase
    {
        public ParamSys(string paramName, string outName, FieldFunc ownField) : base(paramName, outName, ownField)
        {
            Service = true;
            if (!Enum.IsDefined(typeof(serviseFields), outName))
            {
                Error = String.Format("not a system field", outName);
            }
            else
            {
                XlsExist = true;
                if (outName == serviseFields.IP.ToString())
                {
                    SysValue = Scan.GetLocalIPByte();
                }
            }
        }


        public object SysValue { protected set; get; } = null;


        public override object Value
        {
            get
            {
                if (SysValue != null)
                {
                    return SysValue;
                }
                return OwnField.ResTableIndex;
            }
        }

        public override void InitField()
        {
            OwnField.ResRow[OutName] = Value;
        }
    }
}
