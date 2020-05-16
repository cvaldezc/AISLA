using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Model
{
    public class MDStaadPRO
    {

        public static DataTable dtGlobal;

        public static void init()
        {
            dtGlobal = new DataTable("GLOBAL");
            dtGlobal.Columns.Add("nodo", typeof(string));
            dtGlobal.Columns.Add("sv", typeof(decimal));
            dtGlobal.Columns.Add("cm", typeof(decimal));
            dtGlobal.Columns.Add("cv", typeof(decimal));
            dtGlobal.Columns.Add("csx", typeof(decimal));
            dtGlobal.Columns.Add("csz", typeof(decimal));
            dtGlobal.Columns.Add("csxv", typeof(decimal));
            dtGlobal.Columns.Add("cszv", typeof(decimal));
            dtGlobal.Columns.Add("pdsx", typeof(decimal));
            dtGlobal.Columns.Add("pdsz", typeof(decimal));
            dtGlobal.Columns.Add("pdsxa", typeof(decimal));
            dtGlobal.Columns.Add("pdsza", typeof(decimal));
            // input dead
            dtGlobal.Columns.Add("cmx", typeof(decimal));
            dtGlobal.Columns.Add("cmz", typeof(decimal));
            // input live
            dtGlobal.Columns.Add("cvx", typeof(decimal));
            dtGlobal.Columns.Add("cvz", typeof(decimal));
            // input csx
            dtGlobal.Columns.Add("csxy", typeof(decimal));
            dtGlobal.Columns.Add("csxz", typeof(decimal));
            // input csy
            dtGlobal.Columns.Add("cszx", typeof(decimal));
            dtGlobal.Columns.Add("cszy", typeof(decimal));
        }

    }
}
