using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using Controller.Interfaces;
using Model;


namespace Controller
{
    public class ReadFileStaadPRO : IReadFileStaadProable
    {
        // Parameters
        private FileStream fs;
        private StreamReader read;
        public decimal participacion;
        public string path;
        public decimal delta;

        #region IReadFileable Members

        public void ReadStaadPro()
        {
            Model.MDStaadPRO.init();
            fs = new FileStream(@path, FileMode.Open, FileAccess.Read);
            read = new StreamReader(fs, Encoding.UTF8);
            decimal fac = 1000;
            //try
            //{
                String node = "";
                while(read.Peek() >= 0)
                {
                    String cad = read.ReadLine();
                    Console.WriteLine(cad);
                    // Console.WriteLine(cad);
                    string pattern = @"\s+";
                    string[] line = Regex.Split(cad, pattern);
                    if (line.Length == 10 && line[2].ToString() == "1:CM")
                    {
                        // crea nuevo nodo
                        DataRow dr = Model.MDStaadPRO.dtGlobal.NewRow();
                        node = Convert.ToString(line[1]);
                        decimal dead = Math.Abs(Convert.ToDecimal(line[4]));
                        dr["nodo"] = node;
                        dr["cm"] = dead * fac;
                        /// TODO: en la linea anterior
                        dr["sv"] = ((dead * this.participacion));
                        // other data for report
                        dr["cmx"] = Convert.ToDecimal(line[3]);
                        dr["cmz"] = Convert.ToDecimal(line[5]);
                        Model.MDStaadPRO.dtGlobal.Rows.Add(dr);
                    }
                    if (line.Length == 9)
                    {
                        Console.WriteLine("fila leida " + line[1].Split(':')[0].ToString().Trim());
                        if (Convert.ToInt16(line[1].Split(':')[0].ToString().Trim()) > 4)
                            continue;
                        string nodo = String.Format("nodo='{0}'", node);
                        DataRow[] drs = Model.MDStaadPRO.dtGlobal.Select(nodo);
                        if (drs.Length > 0)
                        {
                            List<object[]> lst = rowData(line, line[1].Split(new char[] {':'}).Last().ToString().Trim());
                            foreach (object[] row in lst)
                            {
                                if (row[0].ToString() == "csxv" || row[0].ToString() == "cszv")
                                {
                                    decimal sv = (Convert.ToDecimal(drs[0]["sv"]) * fac);
                                    decimal csv = ((Math.Abs(Convert.ToDecimal(row[1])) * fac) + sv);
                                    drs[0][row[0].ToString()] = (csv);
                                    // formula para pdelta
                                    decimal dead = Convert.ToDecimal(drs[0]["cm"]);
                                    decimal live = Convert.ToDecimal(drs[0]["cv"]);
                                    //double acsfy = Math.Abs(Convert.ToDouble(row[1]));
                                    if (row[0].ToString() == "csxv")
                                    {
                                        //Math.Round((((dead + (0.5 * live) + fzx + sv) * delta) / 2), 2);
                                        drs[0]["pdsx"] = (Math.Round((((dead + (Convert.ToDecimal(0.5) * live) + csv) * delta) / 2), 2));
                                        drs[0]["pdsxa"] = (Math.Round((((dead + (Convert.ToDecimal(0.5) * live) + csv) * delta) / 2), 2));
                                    }

                                    if (row[0].ToString() == "cszv")
                                    {
                                        //Math.Round((((dead + (0.5 * live) + fzx + sv) * delta) / 2), 2);
                                        drs[0]["pdsz"] = (Math.Round((((dead + (Convert.ToDecimal(0.5) * live) + csv) * delta) / 2), 2));
                                        drs[0]["pdsza"] = (Math.Round((((dead + (Convert.ToDecimal(0.5) * live) + csv) * delta) / 2), 2));
                                    }
                                }
                                else
                                {
                                    if (row[0].ToString() == "cm" || row[0].ToString() == "cv" ||row[0].ToString() == "csx" || row[0].ToString() == "csz")
                                    {
                                        drs[0][row[0].ToString()] = (Math.Abs(Convert.ToDecimal(row[1])) * fac);
                                    }
                                    else
                                    {
                                        drs[0][row[0].ToString()] = Convert.ToDecimal(row[1]);
                                    }
                                }
                            }
                            Model.MDStaadPRO.dtGlobal.AcceptChanges();
                        }
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRORR");
            //    Console.WriteLine( ex.Message.ToString() );
            //}
            //finally
            //{
            //    close();        
            //}
            close();
        }

        private List<object[]> rowData(object[] row, String carga)
        {
            List<object[]> lst = new List<object[]>();
            try
            {
                object[] obj = new object[2];
                switch (carga)
                {
                    case "CV":
                        obj[0] = "cv";
                        obj[1] = Convert.ToDouble(row[3]);
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "cvx";
                        obj[1] = Convert.ToDouble(row[2]);
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "cvz";
                        obj[1] = Convert.ToDouble(row[4]);
                        lst.Add(obj);
                        break;
                    case "CSX":
                        obj[0] = "csx";
                        obj[1] = Convert.ToDouble(row[2]);
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "csxy";
                        obj[1] = Convert.ToDouble(row[3]);
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "csxz";
                        obj[1] = Convert.ToDouble(row[4]);
                        lst.Add(obj);
                        break;
                    case "CSZ":
                        obj[0] = "csz";
                        obj[1] = Convert.ToDouble(row[4]);
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "cszy";
                        obj[1] = Convert.ToDouble(row[3]);
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "cszx";
                        obj[1] = Convert.ToDouble(row[2]);
                        lst.Add(obj);
                        break;
                }
                obj = new object[2];
                switch (carga)
                {
                    case "CSX":
                        obj[0] = "csxv";
                        obj[1] = Convert.ToDouble(row[3]);
                        lst.Add(obj);
                        break;
                    case "CSZ":
                        obj[0] = "cszv";
                        obj[1] = Convert.ToDouble(row[3]);
                        lst.Add(obj);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return lst;
        }

        public void test()
        {
            for (int i = 0; i < Model.MDStaadPRO.dtGlobal.Rows.Count; i++)
            {
                for (int j = 0; j < Model.MDStaadPRO.dtGlobal.Columns.Count; j++)
                {
                    Console.Write(Model.MDStaadPRO.dtGlobal.Rows[i][j].ToString());
                    Console.Write(", ");
                }
                Console.WriteLine("");
            }
        }

        public void close()
        {
            read.Close();
            fs.Close();
        }

        #endregion
    }
}
