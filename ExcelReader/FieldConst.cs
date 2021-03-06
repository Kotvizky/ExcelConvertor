﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class FieldConst : FieldBase
    {
        object value;

        public FieldConst(DataRow row, Scan scan) : base(row, scan)
        {
        }

        public override object Value
        {
            get
            {
                return value;
            }
        }

        public override string InitValue()
        {
            ValidValue result = Validator(new ValidData()
            {
                Value = xlsName,
                Size = DataSize
            });

            if (result.Error == "")
            {
                value = result.Value;
            }
            else
            {
                result.Error = String.Format("{0}:{1};", ResName, result.Error);
                value = null;
            }
            return result.Error;
        }

    }
}
