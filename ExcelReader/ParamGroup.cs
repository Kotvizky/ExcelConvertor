using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class ParamGroup : List<ParamBase>
    {

//        public ParamGroup(string sqlParam, string tmplParam,

        public ParamGroup(string sqlParam, string tmplPatam,
            GroupNames paramGroup, List<FieldBase> fields, FieldFunc ownField)
        {
            string[] sqlArray = sqlParam.Split(')')[0].Split(new Char[] { ' ', ',' });
            string[] paramArray = tmplPatam.Split(new Char[] { ' ', ',', ')' });
            GroupName = paramGroup.ToString();

            for (int i = 0; i < sqlArray.Length; i++)
            {

                if (i >= paramArray.Length)
                {
                    msgError += String.Format(";missed param. {0}", sqlArray[i]);
                    if (GroupNames.outPar != paramGroup) AllFound = false;
                    break;
                }

                string resName = paramArray[i];
                if (paramGroup == GroupNames.outPar)
//                    resName = String.Format("{0}.{1}",ownField.FunctionName,resName);
                    resName = String.Format("{0}.{1}", ownField.ResName, resName);
                FieldBase field = fields.Find(x => x.ResName == resName);
                bool isSystem = !paramArray[i].Any(Char.IsLower);
                if ((field == null) && !isSystem)
                {
                    msgError += String.Format(";missed field {0}", paramArray[i]);
                    if (GroupNames.outPar != paramGroup) AllFound = false;
                    continue;
                }

                ParamBase newParam = null;
                switch (paramGroup)
                {
                    case GroupNames.outPar:
                        newParam = new ParamAnswer(paramArray[i], sqlArray[i], ownField, field);
                        break;
                    default:
                        if (isSystem)
                            newParam = new ParamSys(paramArray[i], sqlArray[i], ownField);
                        else newParam = new ParamToSql(paramArray[i], sqlArray[i], ownField, field);
                        break;
                }
                if (newParam != null) Add(newParam);
            }
            if ((paramGroup != GroupNames.outPar) && AllFound)
            {
                foreach (ParamBase param in this)
                {
                    if (!param.XlsExist)
                    {
                        AllFound = false;
                        break;
                    }
                }
            }
        }

        public string GroupName { protected set; get; }

        public bool AllFound { protected set; get; } = true;

        public string msgError { protected set; get; } = String.Empty;

        //public void AddParameters(string sqlParam, string tmplParam, 
        //    GroupNames paramGroup, List<FieldBase> fields, FieldFunc ownField)
        //{

        //    string[] sqlArray = sqlParam.Split(new Char[] { ' ', ',' });
        //    string[] paramArray = tmplParam.Split(new Char[] { ' ', ',', ')' });

        //    for (int i = 0; i < sqlArray.Length; i++)
        //    {

        //        if ( i >= paramArray.Length ) {
        //            msgError += String.Format("missed param. {0}", sqlArray[i]);
        //            AllFound = false;
        //            break;
        //        }

        //        FieldBase field = fields.Find(x => x.ResName == tmplParam);
        //        bool isSystem = !tmplParam.Any(Char.IsLower);
        //        if ((field == null) && !isSystem)
        //        {
        //            msgError += String.Format("missed field {0}", tmplParam);
        //            AllFound = false;
        //            continue;
        //        }

        //        ParamBase newParam = null;
        //        switch (paramGroup) {
        //            case GroupNames.outPar:
        //                newParam  = new ParamAnswer(tmplParam, sqlArray[i], ownField);
        //                break;
        //            default:
        //                if (isSystem) 
        //                    newParam = new ParamSys(tmplParam, sqlArray[i],ownField);
        //                else newParam = new ParamToSql(tmplParam, sqlArray[i], ownField, field);
        //                break;
        //        }      
        //    }

        //}


    }
}
