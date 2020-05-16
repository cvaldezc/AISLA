using Controller.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Constants;

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
        public bool error = false;
        public string raise = "";

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
            UInt16 count = 0;
            Boolean isValid = true;
            while (read.Peek() >= 0)
            {
                #region loop
                String cad = read.ReadLine();
                string pattern = @"\s+";
                string[] line = Regex.Split(cad, pattern);
                Console.WriteLine("LENGTH DE ARRAY " + line.Length);
                Console.WriteLine(string.Join(",", line));
                //Console.WriteLine("NUMBER LINE " + count);
                if (line.Length == 8 && count == 4)
                {
                    string ton = "(Mton)";
                    if (!line[1].Equals(ton) && !line[2].Equals(ton) && !line[3].Equals(ton))
                    {
                        raise = String.Format("{0} {1} {2}", line[1], line[2], line[3]);
                        isValid = false;
                    }
                }
                count++;
                if (isValid)
                {
                    if (line.Length == 10 && line[2].ToString() == "1:CM")
                    {
                        #region Crear Nodo
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
                        #endregion
                    }
                    if (line.Length == 9)
                    {
                        #region llenar tabla
                        // Console.WriteLine("fila leida " + line[1].Split(':')[0].ToString().Trim());
                        if (Convert.ToInt16(line[1].Split(':')[0].ToString().Trim()) > 4)
                            continue;
                        string nodo = String.Format("nodo='{0}'", node);
                        DataRow[] drs = Model.MDStaadPRO.dtGlobal.Select(nodo);
                        if (drs.Length > 0)
                        {
                            decimal dead = Convert.ToDecimal(drs[0]["cm"]);
                            List<object[]> lst = rowData(line, line[1].Split(new char[] { ':' }).Last().ToString().Trim(), dead);
                            foreach (object[] row in lst)
                            {
                                if (row[0].ToString() == "csxv" || row[0].ToString() == "cszv")
                                {

                                    decimal live = Convert.ToDecimal(drs[0]["cv"]);
                                    decimal sv = (Convert.ToDecimal(drs[0]["sv"])); // @cvaldezch multiplication 1000
                                    //((Math.Abs(Convert.ToDecimal(row[1])) * fac) + sv);
                                    decimal csv = ((Math.Abs(Convert.ToDecimal(row[1])) * fac) + sv);  // ((Math.Abs(Convert.ToDecimal(row[1])) + this.participacion) * dead);
                                    drs[0][row[0].ToString()] = (csv);
                                    // formula para pdelta
                                    //double acsfy = Math.Abs(Convert.ToDouble(row[1]));
                                    if (row[0].ToString() == "csxv")
                                    {
                                        //Math.Round((((dead + (0.5 * live) + fzx + sv) * delta) / 2), 2);
                                        drs[0]["pdsx"] = (Math.Round(((((dead * Convert.ToDecimal(1.25)) + (Convert.ToDecimal(0.75) * live) + csv) * delta) / 2), 2));
                                        drs[0]["pdsxa"] = (Math.Round(((((dead * Convert.ToDecimal(1.25)) + (Convert.ToDecimal(0.75) * live) + csv) * delta) / 2), 2));
                                        //drs[0]["pdsx"] = (Math.Round(((Convert.ToDecimal(drs[0]["csx"]) + this.participacion) * dead), 2));
                                        //drs[0]["pdsxa"] = (Math.Round(((Convert.ToDecimal(drs[0]["csx"]) + this.participacion) * dead), 2));

                                    }

                                    if (row[0].ToString() == "cszv")
                                    {
                                        //Math.Round((((dead + (0.5 * live) + fzx + sv) * delta) / 2), 2);
                                        drs[0]["pdsz"] = (Math.Round(((((dead * Convert.ToDecimal(1.25)) + (Convert.ToDecimal(0.75) * live) + csv) * delta) / 2), 2));
                                        drs[0]["pdsza"] = (Math.Round(((((dead * Convert.ToDecimal(1.25)) + (Convert.ToDecimal(0.75) * live) + csv) * delta) / 2), 2));
                                        //drs[0]["pdsz"] = (Math.Round(((Convert.ToDecimal(drs[0]["csz"]) + this.participacion) * dead), 2));
                                        //drs[0]["pdsza"] = (Math.Round(((Convert.ToDecimal(drs[0]["csz"]) + this.participacion) * dead), 2));
                                    }
                                }
                                else
                                {
                                    if (row[0].ToString() == "cm" || row[0].ToString() == "cv" || row[0].ToString() == "csx" || row[0].ToString() == "csz")
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
                    #endregion
                    }
                }
                else
                {
                    Console.WriteLine("ERROR DE UNIDAD - INCORRECTO");
                    break;
                }
                #endregion
            }
            error = !isValid;
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
            Console.WriteLine(Constants.DELIMITER_LOG_START);
            test();
            Console.WriteLine(Constants.DELIMITER_LOG_END);

            close();
        }

        private List<object[]> rowData(object[] row, String carga, decimal dead)
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
                        // here change Convert.ToDouble(row[3])
                        obj[0] = "csxy";
                        obj[1] = ((Convert.ToDecimal(row[3]) * 1000) + (this.participacion * dead));
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
                        // here change Convert.ToDouble(row[3])
                        obj[0] = "cszy";
                        obj[1] = ((Convert.ToDecimal(row[3]) * 1000) + (this.participacion * dead));
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
