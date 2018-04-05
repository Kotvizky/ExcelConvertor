using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    class ProcList : List<ProcSql>
    {
        static string itemNamePart = "spItem";

        public ProcList(int tmpl, ToolStripItemCollection items, DataTable resTable)
        {
            DataTable spTab =  SQLFunction.getProcDescription(tmpl);
            foreach (DataRow row in spTab.Rows) Add(new ProcSql(row));
            Items = items;
            Tmpl = tmpl;
            ItemName = String.Format("spItem{0}{1}", itemNamePart,tmpl);
            ResTable = resTable;
        }

        DataTable ResTable;

        int Tmpl;

        string ItemName;

        public ToolStripItemCollection ToolItems { private set; get; }

        private ToolStripItemCollection Items;

        public void insertButton()
        {
            var procs = FindAll(x => x.isFound);
            foreach (ProcSql proc in procs)
            {
                ToolStripButton itemButton = new ToolStripButton(proc.BtName, null, null,
                    String.Format("{0}_{1}",ItemName,proc.SpName));
                itemButton.Click += new EventHandler(cms_Opening);
                itemButton.Tag = proc.SpName;
                Items.Add(itemButton);
            }
        }

        public void deleteButton()
        {
            for (int i = Items.Count-1; i >= 0; i--)
            {
                if (Items[i].Name.StartsWith(itemNamePart))
                {
                    Items.RemoveAt(i);
                }
                //Items.Remove(item);
            }
        }

        void cms_Opening(object sender, EventArgs e)
        {
            string spName = (String)((sender as ToolStripItem).Tag);

            var proc = Find(x => x.isFound & x.SpName == spName);

            string message = spName;

            if (proc != null)
            {
                initSQLTable(proc);
                message = String.Format(
                    "{0}\n\rout: {1}",
                    message,
                    execProcOnServer(proc));
            }
            MessageBox.Show(message);
        }


        void initSQLTable(ProcSql proc)
        {
            DataTable inTable = SQLFunction.GetResultTable(proc.InTable);
            DataRow[] sqlRows = new DataRow[ResTable.Rows.Count];
            DataRowCollection resRows = ResTable.Rows;
            for (int i = 0; i < resRows.Count; i++)
            {
                DataRow newRow = inTable.Rows.Add();
                foreach (KeyValuePair<string, string> field in proc.inFields)
                {
                    switch (field.Key.ToLower())
                    {
                        case "ip":
                            newRow[field.Key] = Scan.GetLocalIPByte();
                            break;
                        case "row_id":
                            newRow[field.Key] = i;
                            break;
                        default:
                            newRow[field.Key] = resRows[i][field.Value];
                            break;
                    }
                }
                sqlRows[i] = newRow;
            }
            SQLFunction.ExecuteNonQuery(
                String.Format("delete from {0} where IP = 0x{1}", proc.InTable,Scan.GetLocalIPAddress(true))
                );
            SQLFunction.BulkWrite(proc.InTable, sqlRows);
        }

        string execProcOnServer(ProcSql proc)
        {
            string result = String.Empty;
            if (proc.InPar != "")
            {
                string[] inParSql = new string[2];
                inParSql[0] = proc.InPar;
                if (proc.InPar.ToLower() == "ip")
                {
                    inParSql[1] = Scan.GetLocalIPAddress();
                }
                result = SQLFunction.ExecuteProc(proc.SpName,proc.OutPar,inParSql);
            }
            else
            {
                result = SQLFunction.ExecuteProc(proc.SpName, proc.OutPar);
            }
            return result;
        }
    }
}
