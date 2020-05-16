using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Controller.Interfaces;
using Excel = Microsoft.Office.Interop.Excel;

namespace Controller
{
    public class Exportar
    {
        private Excel.Application app;
        private Excel.Workbook wb;
        private Excel.Worksheet ws;
        protected bool status;
        private string path, ext;

        public Exportar(String _path, String _ext)
        {
            try
            {
                status = true;
                app = new Excel.Application();
                this.path = _path;
                this.ext = _ext;
                if (app == null)
                {
                    Console.WriteLine("Excel is not installed!");
                    status = false;
                }
                if (this.path == "")
                {
                    Console.WriteLine("path not found");
                    status = false;
                }
                if (this.ext == "")
                {
                    Console.WriteLine("path not found");
                    status = false;
                }
            }
            catch (Exception ex)
            {
                status = false;
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                closeExcel();
            }
            
        }

        public void LlenarDatos()
        {
            try
            {
                object myvalue = System.Reflection.Missing.Value;
                wb = app.Workbooks.Add(myvalue);
                ///++++
                ws = wb.Worksheets[1] as Microsoft.Office.Interop.Excel.Worksheet;
                wb.Sheets.Add(After: wb.Sheets[wb.Sheets.Count]);
                wb.Sheets.Add(After: wb.Sheets[wb.Sheets.Count]);
                for (int i = 1; i <= wb.Sheets.Count; i++)
                {
                    ws = wb.Worksheets[i] as Microsoft.Office.Interop.Excel.Worksheet;
                    switch (i)
                    {
                        case 1:
                            ws.Name = "ENTRADAS";
                            break;
                        case 2:
                            ws.Name = "SALIDAS";
                            break;
                        case 3:
                            ws.Name = "CONSOLIDADO";
                            break;
                    }
                    ws.Columns.AutoFit();
                    // TITULOS
                    ws.Cells[2, 3] = "FUERZAS";
                    ws.Cells[2, 6] = "MOMENTOS";
                    ws.Cells[3, 1] = "NODO";
                    // FUERZAS
                    ws.Cells[3, 3] = "FX";
                    ws.Cells[3, 4] = "FY";
                    ws.Cells[3, 5] = "FZ";
                    // MOMENTOS
                    ws.Cells[3, 6] = "MX";
                    ws.Cells[3, 7] = "MY";
                    ws.Cells[3, 8] = "MZ";
                }
                
                if (this.ext == "std")
                {
                    // llamos a funcion
                    StaadPro();
                }
                else if (this.ext == "e2k")
                {
                    // llamos a funcion
                    Etabs();
                }
                else
                {
                    closeExcel();
                }
                closeExcel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void Etabs()
        {
            try
            {
                DataRow[] dr = Model.MDEtabs.dtGlobal.Select(null, null, DataViewRowState.CurrentRows);
                int rindex = 4, ri = 0;
                #region "load data"
                foreach (DataRow row in dr)
                {
                    ri = rindex;
                    for (int i = 1; i <= wb.Sheets.Count; i++)
                    {
                        ws = wb.Worksheets[i] as Microsoft.Office.Interop.Excel.Worksheet;
                        rindex = ri;
                        // Dead
                        // FUERZAS
                        ws.Cells[rindex, 1] = row["joint"];
                        ws.Cells[rindex, 2] = "CM";
                        ws.Cells[rindex, 3] = row["dfx"];
                        ws.Cells[rindex, 4] = row["dfy"];
                        ws.Cells[rindex, 5] = row["dead"];
                        // MOMENTOS
                        ws.Cells[rindex, 6] = "0";
                        ws.Cells[rindex, 7] = "0";
                        ws.Cells[rindex, 8] = "0";
                        // Live
                        rindex++;
                        // FUERZAS
                        ws.Cells[rindex, 1] = row["joint"];
                        ws.Cells[rindex, 2] = "CV";
                        ws.Cells[rindex, 3] = row["lfx"];
                        ws.Cells[rindex, 4] = row["lfy"];
                        ws.Cells[rindex, 5] = row["live"];
                        // MOMENTOS
                        ws.Cells[rindex, 6] = "0";
                        ws.Cells[rindex, 7] = "0";
                        ws.Cells[rindex, 8] = "0";
                        // fzx
                        rindex++;
                        // FUERZAS
                        ws.Cells[rindex, 1] = row["joint"];
                        ws.Cells[rindex, 2] = "CSX";
                        ws.Cells[rindex, 3] = row["sxx"];
                        ws.Cells[rindex, 4] = row["sxy"];
                        ws.Cells[rindex, 5] = row["fzx"];
                        // MOMENTOS
                        ws.Cells[rindex, 6] = "0";
                        ws.Cells[rindex, 7] = row["pdsx"];
                        ws.Cells[rindex, 8] = "0";
                        // fzy
                        rindex++;
                        // FUERZAS
                        ws.Cells[rindex, 1] = row["joint"];
                        ws.Cells[rindex, 2] = "CSY";
                        ws.Cells[rindex, 3] = row["syx"];
                        ws.Cells[rindex, 4] = row["syy"];
                        ws.Cells[rindex, 5] = row["fzy"];
                        // MOMENTOS
                        ws.Cells[rindex, 6] = row["pdsy"];
                        ws.Cells[rindex, 7] = "0";
                        ws.Cells[rindex, 8] = "0";
                        rindex++;
                    }
                    ri = rindex;
                }
                #endregion
                object myvalue = System.Reflection.Missing.Value;
                #region "load format file"
                for (int i = 1; i <= wb.Sheets.Count; i++)
                {
                    ws = wb.Worksheets[i] as Microsoft.Office.Interop.Excel.Worksheet;
                    if (i == 1)
                    {
                        ws.get_Range("F1", "H1").EntireColumn.Delete();
                    }
                    if (i == 2)
                    {
                        ws.get_Range("C1","E1").EntireColumn.Delete();
                    }
                    if (i == 1 || i == 2)
                    {
                        ws.get_Range("A2", "E" + rindex).Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        ws.get_Range("C2", "E2").Merge(true);
                        ws.get_Range("C2").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("C2").Cells.Font.Bold = true;
                        //ws.get_Range("F2", "H2").Merge(true);
                        //ws.get_Range("F2").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        //ws.get_Range("F2").Cells.Font.Bold = true;
                        ws.get_Range("A3", "E3").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("A3", "E3").Font.Bold = true;
                        ws.get_Range("A3", "A" + rindex).Font.Bold = true;
                        ws.get_Range("A3", "A" + rindex).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    }
                    
                    if (i == 3)
                    {
                        ws.get_Range("A2", "H" + rindex).Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.get_Range("C2", "E2").Merge(true);
                        ws.get_Range("C2").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("C2").Cells.Font.Bold = true;
                        ws.get_Range("F2", "H2").Merge(true);
                        ws.get_Range("F2").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("F2").Cells.Font.Bold = true;
                        ws.get_Range("A3", "H3").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("A3", "H3").Font.Bold = true;
                        ws.get_Range("A3", "A" + rindex).Font.Bold = true;
                        ws.get_Range("A3", "A" + rindex).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    }

                }
                #endregion
                this.path = String.Format(@"{0}/REPORTE-ETABS.xlsx", this.path);
                wb.SaveAs(path, Excel.XlFileFormat.xlOpenXMLWorkbook, myvalue,
                myvalue, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlUserResolution, true,
                myvalue, myvalue, myvalue);
                wb.Close(true, myvalue, myvalue);
                app.Quit();
                status = false;
                //releaseObject(ws);
                //releaseObject(wb);
                //releaseObject(app);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void StaadPro()
        {
            try
            {
                DataRow[] dr = Model.MDStaadPRO.dtGlobal.Select(null, null, DataViewRowState.CurrentRows);
                int rindex = 4, ri = 0;
                #region "load data"
                int fac = 1;
                foreach (DataRow row in dr)
                {
                    ri = rindex;
                    for (int i = 1; i <= wb.Sheets.Count; i++)
                    {
                        ws = wb.Worksheets[i] as Microsoft.Office.Interop.Excel.Worksheet;
                        rindex = ri;
                        // Dead
                        // FUERZAS
                        ws.Cells[rindex, 1] = row["nodo"];
                        ws.Cells[rindex, 2] = "CM";
                        ws.Cells[rindex, 3] = (Convert.ToDecimal(row["cmx"]) / fac);
                        ws.Cells[rindex, 4] = (Convert.ToDecimal(row["cm"])); // entre 1000
                        ws.Cells[rindex, 5] = (Convert.ToDecimal(row["cmz"]) / fac);
                        // MOMENTOS
                        ws.Cells[rindex, 6] = "0";
                        ws.Cells[rindex, 7] = "0";
                        ws.Cells[rindex, 8] = "0";
                        // Live
                        rindex++;
                        // FUERZAS
                        ws.Cells[rindex, 1] = row["nodo"];
                        ws.Cells[rindex, 2] = "CV";
                        ws.Cells[rindex, 3] = (Convert.ToDecimal(row["cvx"]) / fac);
                        ws.Cells[rindex, 4] = (Convert.ToDecimal(row["cv"])); // entre 1000
                        ws.Cells[rindex, 5] = (Convert.ToDecimal(row["cvz"]) / fac);
                        // MOMENTOS
                        ws.Cells[rindex, 6] = "0";
                        ws.Cells[rindex, 7] = "0";
                        ws.Cells[rindex, 8] = "0";
                        // fzx
                        rindex++;
                        // FUERZAS
                        ws.Cells[rindex, 1] = row["nodo"];
                        
                        ws.Cells[rindex, 3] = (Convert.ToDecimal(row["csx"]) / fac);
                        if (i == 1)
                        {
                            ws.Cells[rindex, 2] = "CSX";
                            ws.Cells[rindex, 4] = ((Convert.ToDecimal(row["csxv"]) - Convert.ToDecimal(row["sv"])) / fac); // row["csxv"];
                        } else
                        {
                            ws.Cells[rindex, 2] = "CSVX";
                            ws.Cells[rindex, 4] = (Convert.ToDecimal(row["csxy"]) / fac); // row["csxv"];
                        }
                        ws.Cells[rindex, 5] = (Convert.ToDecimal(row["csxz"]) / fac);
                        // MOMENTOS
                        ws.Cells[rindex, 6] = "0";
                        ws.Cells[rindex, 7] = "0";
                        ws.Cells[rindex, 8] = (Convert.ToDecimal(row["pdsz"]) ); // entre 1000
                        // fzy
                        rindex++;
                        // FUERZAS
                        ws.Cells[rindex, 1] = row["nodo"];
                        
                        ws.Cells[rindex, 3] = (Convert.ToDecimal(row["cszx"]) / fac);
                        if (i == 1)
                        {
                            ws.Cells[rindex, 2] = "CSZ";
                            ws.Cells[rindex, 4] = ((Convert.ToDecimal(row["cszv"]) - Convert.ToDecimal(row["sv"])) / fac); // row["cszv"];
                        }
                        else
                        {
                            ws.Cells[rindex, 2] = "CSVZ";
                            ws.Cells[rindex, 4] = ((Convert.ToDecimal(row["cszy"]) - Convert.ToDecimal(row["sv"])) / fac); // row["cszv"];
                        }
                        ws.Cells[rindex, 5] = (Convert.ToDecimal(row["csz"]) / fac);
                        // MOMENTOS
                        ws.Cells[rindex, 6] = (Convert.ToDecimal(row["pdsx"]) ); // entre 1000
                        ws.Cells[rindex, 7] = "0";
                        ws.Cells[rindex, 8] = "0";
                        rindex++;
                    }
                }
                #endregion
                object myvalue = System.Reflection.Missing.Value;
                #region "load format file"
                for (int i = 1; i <= wb.Sheets.Count; i++)
                {
                    ws = wb.Worksheets[i] as Excel.Worksheet;
                    if (i == 1)
                    {
                        ws.get_Range("F1", "H1").EntireColumn.Delete();
                    }
                    if (i == 2)
                    {
                        ws.get_Range("C1", "E1").EntireColumn.Delete();
                    }
                    if (i == 1 || i == 2)
                    {
                        ws.get_Range("A2", "E" + rindex).Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        ws.get_Range("C2", "E2").Merge(true);
                        ws.get_Range("C2").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("C2").Cells.Font.Bold = true;
                        //ws.get_Range("F2", "H2").Merge(true);
                        //ws.get_Range("F2").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        //ws.get_Range("F2").Cells.Font.Bold = true;
                        ws.get_Range("A3", "E3").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("A3", "E3").Font.Bold = true;
                        ws.get_Range("A3", "A" + rindex).Font.Bold = true;
                        ws.get_Range("A3", "A" + rindex).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    }
                    if (i == 3)
                    {
                        ws.get_Range("A2", "H" + rindex).Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.get_Range("C2", "E2").Merge(true);
                        ws.get_Range("C2").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("C2").Cells.Font.Bold = true;
                        ws.get_Range("F2", "H2").Merge(true);
                        ws.get_Range("F2").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("F2").Cells.Font.Bold = true;
                        ws.get_Range("A3", "H3").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        ws.get_Range("A3", "H3").Font.Bold = true;
                        ws.get_Range("A3", "A" + rindex).Font.Bold = true;
                        ws.get_Range("A3", "A" + rindex).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    }
                }
                #endregion
                this.path = String.Format(@"{0}/REPORTE-STAADPRO.xlsx", this.path);
                wb.SaveAs(path, Excel.XlFileFormat.xlOpenXMLWorkbook, myvalue,
                myvalue, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlUserResolution, true,
                myvalue, myvalue, myvalue);
                wb.Close(true, myvalue, myvalue);
                app.Quit();
                status = false;
                //releaseObject(ws);
                //releaseObject(wb);
                //releaseObject(app);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }


        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        private void closeExcel(){
            if (!status)
            {
                wb.Close();
                app.Quit();
                releaseObject(ws);
                releaseObject(wb);
                releaseObject(app);
            }
        }

    }
}
