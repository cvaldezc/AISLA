using System;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using Controller.Interfaces;
using Model;

namespace Controller
{
    public class ReadFileEtabs : IReadFileable
    {
        private Excel.Application app;
        private Excel.Workbook book;
        private Excel.Worksheet sheet;

        public ReadFileEtabs(string path)
        {
            try
            {
                app = new Excel.Application();
                if (app == null)
                {
                    MessageBox.Show(null, "Se ha producido un error al iniciar la aplicacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                book = app.Workbooks.Open(@path,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch(System.IO.IOException ex){
                MessageBox.Show(null, ex.Message.ToString(), "Error path IO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message.ToString(), "Error GENERAl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region IReadFileExcelable Members
        
        public void selectSheet(String Name)
        {
            sheet = (Excel.Worksheet)book.Sheets[Name];
        }

        public void readETabs(String story)
        {
            //try
            //{
                // validando columnas y nombre de hoja
                // Selecciona hoja global
                //this.selectSheet("GLOBAL INPUT");
                if (sheet.Columns.Count >= 10)
                {
                    Model.MDEtabs.init();
                    // recoremos las filas que contenga la hoja de excel
                    #region ""Recorrido de la hoja de excel"
                    for (int i = 3; i < sheet.Rows.Count; i++)
                    {
                        if (sheet.Cells[i, 2].Value2 == null)
                        {
                            break;
                        }
                        string join = String.Format("joint='{0}'", sheet.Cells[i, 2].Value);
                        DataRow[] drs = Model.MDEtabs.dtGlobal.Select(join);
                        // drs = ds.Tables["GLOBAL"].Select(join);
                        if (drs.Length > 0)
                        {
                            List<object[]> lst = rowData(i);
                            foreach (object[] item in lst)
                            {
                                if (item[0].ToString().Equals("dead") || item[0].ToString().Equals("live") || item[0].ToString().Equals("fzx") || item[0].ToString().Equals("fzy") || item[0].ToString().Equals("fx") || item[0].ToString().Equals("fy"))
                                {
                                    drs[0][item[0].ToString()] = Math.Abs(Convert.ToDouble(item[1]));
                                }
                                else
                                {
                                    drs[0][item[0].ToString()] = Convert.ToDouble(item[1]);
                                }
                            }
                            Model.MDEtabs.dtGlobal.AcceptChanges();
                        }
                        else
                        {
                            DataRow dr = Model.MDEtabs.dtGlobal.NewRow();
                            dr["story"] = story != "" ? story : sheet.Cells[i, 1].Value;
                            dr["joint"] = sheet.Cells[i, 2].Value;
                            List<object[]> lst = rowData(i);
                            foreach (object[] item in lst)
                            {
                                if (item[0].ToString().Equals("dead") || item[0].ToString().Equals("live") || item[0].ToString().Equals("fzx") || item[0].ToString().Equals("fzy") || item[0].ToString().Equals("fx") || item[0].ToString().Equals("fy"))
                                {
                                    dr[item[0].ToString()] = Math.Abs(Convert.ToDouble(item[1]));
                                }
                                else
                                {
                                    dr[item[0].ToString()] = Convert.ToDouble(item[1]);
                                }
                            }

                            Model.MDEtabs.dtGlobal.Rows.Add(dr);
                        }
                    }
                    #endregion
                    // ds.Tables.Add(dtg);
                }
                else
                {
                    MessageBox.Show(null, "La hoja GLOBAL no cumple con el formato.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //    MessageBox.Show(null, ex.Message.ToString(), "Error Lectura", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //}
        }

        private List<object[]> rowData(int row)
        {
            List<object[]> lst = new List<object[]>();
            try
            {
                String name = sheet.Cells[row, 4].Value;
                object[] obj;
                obj = new object[2];
                //obj[0] = name.ToLower();
                switch (name)
                {
                    case "Dead":
                        obj[0] = "dead";
                        obj[1] = sheet.Cells[row, 7].Value2;
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "dfx";
                        obj[1] = sheet.Cells[row, 5].Value2;
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "dfy";
                        obj[1] = sheet.Cells[row, 6].Value;
                        lst.Add(obj);
                        break;
                    case "Live":
                        obj[0] = "live";
                        obj[1] = sheet.Cells[row, 7].Value;
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "lfx";
                        obj[1] = sheet.Cells[row, 5].Value;
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "lfy";
                        obj[1] = sheet.Cells[row, 6].Value;
                        lst.Add(obj);
                        break;
                    case "SX":
                        obj[0] = "fzx";
                        obj[1] = sheet.Cells[row, 7].Value;
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "sxx";
                        obj[1] = sheet.Cells[row, 5].Value;
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "sxy";
                        obj[1] = sheet.Cells[row, 6].Value;
                        lst.Add(obj);
                        break;
                    case "SY":
                        obj[0] = "fzy";
                        obj[1] = sheet.Cells[row, 7].Value;
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "syx";
                        obj[1] = sheet.Cells[row, 5].Value;
                        lst.Add(obj);
                        obj = new object[2];
                        obj[0] = "syy";
                        obj[1] = sheet.Cells[row, 6].Value;
                        lst.Add(obj);
                        break;
                }
                // lst.Add(obj);
                obj = new object[2];
                switch (name)
                {
                    case "SX":
                        obj[0] = "fx";
                        obj[1] = sheet.Cells[row, 5].Value;
                        lst.Add(obj);
                        break;
                    case "SY":
                        obj[0] = "fy";
                        obj[1] = sheet.Cells[row, 6].Value;
                        lst.Add(obj);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR " + ex.Message);
            }
            return lst;
        }

        public void calcFormulas(double delta, double cm, String story)
        {
            try
            {
                foreach (DataRow row in Model.MDEtabs.dtGlobal.Rows)
                {
                    double dead = Convert.ToDouble(row["dead"]);
                    double live = Convert.ToDouble(row["live"]);
                    double fzx = Convert.ToDouble(row["fzx"]);
                    double fzy = Convert.ToDouble(row["fzy"]);
                    row["sv"] = Math.Round(dead * cm, 2);
                    // dead + 0.5 * live + FZX + SV * DELTA / 2
                    double sv = Convert.ToDouble(row["sv"]);
                    row["pdsx"] = Math.Round((((dead + (0.5 * live) + fzx + sv) * delta) / 2), 2);
                    row["pdsy"] = Math.Round((((dead + (0.5 * live) + fzy + sv) * delta) / 2), 2);
                }
                inputSXSY(story);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void inputSXSY(String story)
        {
            try
            {
                // Realizar el ingreso en la tabla SX
                foreach (DataRow row in Model.MDEtabs.dtGlobal.Rows)
                {
                    // Aqui se ingresa los datos a la tabla sx
                    DataRow dr = Model.MDEtabs.dtSX.NewRow();
                    dr["story"] = story != "" ? story : row["story"];
                    dr["joint"] = row["joint"];
                    dr["combo"] = "SX";
                    dr["fz"] = row["fzx"];
                    dr["my"] = row["pdsx"];
                    dr["fx"] = row["fx"];
                    Model.MDEtabs.dtSX.Rows.Add(dr);

                    // Aqui se ingresa los datos a la tabla sy
                    DataRow d = Model.MDEtabs.dtSY.NewRow();
                    d["story"] = story != "" ? story : row["story"];
                    d["joint"] = row["joint"];
                    d["combo"] = "SY";
                    d["fz"] = row["fzy"];
                    d["mx"] = row["pdsy"];
                    d["fy"] = row["fy"];
                    Model.MDEtabs.dtSY.Rows.Add(d);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        public void close()
        {
            this.book.Close();
            this.app.Quit();
        }

        public void test()
        {
            for (int i = 0; i < Model.MDEtabs.dtGlobal.Rows.Count; i++)
            {
                for (int j = 0; j < Model.MDEtabs.dtGlobal.Columns.Count; j++)
                {
                    Console.Write(Model.MDEtabs.dtGlobal.Rows[i][j].ToString());
                    Console.Write(", ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++");
            for (int i = 0; i < Model.MDEtabs.dtSX.Rows.Count; i++)
            {
                for (int j = 0; j < Model.MDEtabs.dtSX.Columns.Count; j++)
                {
                    Console.Write(Model.MDEtabs.dtSX.Rows[i][j].ToString());
                    Console.Write(", ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("======================================================");
            for (int i = 0; i < Model.MDEtabs.dtSY.Rows.Count; i++)
            {
                for (int j = 0; j < Model.MDEtabs.dtSY.Columns.Count; j++)
                {
                    Console.Write(Model.MDEtabs.dtSY.Rows[i][j].ToString());
                    Console.Write(", ");
                }
                Console.WriteLine("");
            }
        }

        #endregion
    }
}
