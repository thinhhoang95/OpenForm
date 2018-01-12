using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenForm.Result
{
    public class ResultPresenter
    {
        public DataSet data = new DataSet();
        public DataTable table = new DataTable();
        public ResultPresenter()
        {
            data.Tables.Add(table);
        }
        public void addCol(string colName)
        {
            if(!table.Columns.Contains(colName))
            table.Columns.Add(new DataColumn(colName));
        }
        public void addRow()
        {
            table.Rows.Add();
        }
        public void setVal(string colName, string val)
        {
            table.Rows[table.Rows.Count - 1][colName] = val;
        }
        public static string toAlphabetic(int num)
        {
            if (num == -1) return "-";
            char m = (char)(num + 65);
            return m.ToString();
        }
    }
}
