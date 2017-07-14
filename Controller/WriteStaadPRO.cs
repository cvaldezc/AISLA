using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Controller
{
    public class WriteStaadPRO : Interfaces.IWriteStaadPROable
    {
        /// <summary>
        /// Params
        /// path : Address file base where overwrite 
        /// destiny : Address file destiny
        /// </summary>
        ///
        public List<string> listToText = new List<string>();
        public String path = "";
        public String destiny = "";
        private Dictionary<string, IDictionary<string, object>> labels = new Dictionary<string, IDictionary<string, object>>()
        {
            {
                "LOAD 1 CM", new Dictionary<string, object>() { { "exist", false }, { "clean", false }, { "index", -1 } }
            },
            {
                "LOAD 2 CV", new Dictionary<string, object>() { { "exist", false}, { "clean", false}, { "index", -1 } }
            },
            {
                "LOAD 3 CSX", new Dictionary<string, object>() { { "exist", false}, { "clean", false}, { "index", -1 } }
            },
            {
                "LOAD 4 CSZ", new Dictionary<string, object>() { { "exist", false}, { "clean", false}, { "index", -1 } }
            },
            {
                "LOAD 5 CSXV", new Dictionary<string, object>() { { "exist", false}, { "clean", false}, { "index", -1 } }
            },
            {
                "LOAD 6 CSZV", new Dictionary<string, object>() { { "exist", false}, { "clean", false}, { "index", -1 } }
            },
            {
                "LOAD 7 PDSX", new Dictionary<string, object>() { { "exist", false}, { "clean", false}, { "index", -1 } }
            },
            {
                "LOAD 8 PDSZ", new Dictionary<string, object>() { { "exist", false}, { "clean", false}, { "index", -1 } }
            },
        };
        private StringBuilder loadcm = new StringBuilder();
        private StringBuilder loadcv = new StringBuilder();
        private StringBuilder loadcsx = new StringBuilder();
        private StringBuilder loadcsz = new StringBuilder();
        private StringBuilder loadcsxv = new StringBuilder();
        private StringBuilder loadcszv = new StringBuilder();
        private StringBuilder loadpdsx = new StringBuilder();
        private StringBuilder loadpdsz = new StringBuilder();
        private StringBuilder loadsv = new StringBuilder();
        private StringBuilder loadapdsx = new StringBuilder();
        private StringBuilder loadapdsz = new StringBuilder();
        // private StringBuilder Text = new StringBuilder();
        private StringBuilder combos = new StringBuilder();

        public void initData()
        {
            combos.AppendLine("LOAD COMB 9 1.4CM+1.7CV");
            combos.AppendLine("1 1.4 2 1.7 ");
            combos.AppendLine("LOAD COMB 10 1.25CM+1.25CV+CSX+PDX");
            combos.AppendLine("1 1.25 2 1.25 3 1.0 5 1.0 7 1.0");
            combos.AppendLine("LOAD COMB 11 1.25CM+1.25CV +CSZ+PDZ");
            combos.AppendLine("1 1.25 2 1.25 4 1.0 6 1.0 8 1.0");
            combos.AppendLine("LOAD COMB 12 1.25CM+1.25CV-CSX-PDX");
            combos.AppendLine("1 1.25 2 1.25 3 -1.0 5 -1.0 7 -1.0 ");
            combos.AppendLine("LOAD COMB 13 1.25CM+1.25CV-CSZ-PDZ");
            combos.AppendLine("1 1.25 2 1.25 4 -1.0 6 -1.0 8 -1.0 ");
            combos.AppendLine("LOAD COMB 14 0.9CM-CSX-PDX");
            combos.AppendLine("1 0.9 3 -1.0 5 1.0 7 -1.0");
            combos.AppendLine("LOAD COMB 15 0.9CM+CSX+PDX");
            combos.AppendLine("1 0.9 3 1.0 5 1.0 7 1.0 ");
            combos.AppendLine("LOAD COMB 16 0.9CM-CSZ-PDX");
            combos.AppendLine("1 0.9 4 -1.0 6 1.0 8 -1.0");
            combos.AppendLine("LOAD COMB 17 0.9CM+CSZ+PDZ");
            combos.AppendLine("1 0.9 4 1.0 6 1.0 8 1.0");
            combos.AppendLine("LOAD COMB 18 CM+CV");
            combos.AppendLine("1 1.0 2 1.0");
            combos.AppendLine("PERFORM ANALYSIS");
        }

        #region IWriteStaadPROable Members

        public StringBuilder readFile()
        {
            List<string> toText = new List<string>();
            StringBuilder text = new StringBuilder();
            if (File.Exists(this.path))
            {
                toText = File.ReadAllLines(path, Encoding.UTF8).ToList<string>();
                UInt16 count = 0;
                this.listToText = toText;
                /// not process loop for version old not used
                for (int i = 0; i < 0; i++)
                {
                    string line = toText[i];
                    text.AppendLine(line);
                    #region "other data" 
                    switch (line)
                    {
                        case "LOAD 1 CM":
                            labels[line]["exist"] = true;
                            labels[line]["index"] = count;
                            break;
                        case "LOAD 2 CV":
                            labels[line]["exist"] = true;
                            labels[line]["index"] = count;
                            break;
                        case "LOAD 3 CSX":
                            labels[line]["exist"] = true;
                            labels[line]["index"] = count;
                            break;
                        case "LOAD 4 CSZ":
                            labels[line]["exist"] = true;
                            labels[line]["index"] = count;
                            break;
                        case "LOAD 5 CSXV":
                            labels[line]["exist"] = true;
                            labels[line]["index"] = count;
                            break;
                        case "LOAD 6 CSZV":
                            labels[line]["exist"] = true;
                            labels[line]["index"] = count;
                            break;
                        case "LOAD 7 PDSX":
                            labels[line]["exist"] = true;
                            labels[line]["index"] = count;
                            break;
                        case "LOAD 8 PDSZ":
                            labels[line]["exist"] = true;
                            labels[line]["index"] = count;
                            break;
                    }
                    // count++;

                    #endregion
                }
                // end block
            }
            return text;
        }

        public void processAisladoSTD(StringBuilder Text)
        {
            preload();
            StringBuilder tmpAislado = new StringBuilder();

            //here clean content back to file
            Console.WriteLine("LIST SIZE "+ this.listToText.Count);
            List<string> tmp = new List<string>();
            for (int i = 0; i < this.listToText.Count; i++)
            {
                #region "clean content file isolated"
                string line = this.listToText[i];
                switch (line)
                {
                    case "LOAD 5 CSXV":
                        #region "Clear method isolated load 5"
                        i = i + 1;
                        //tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 6 CSZV" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 6 CSZV":
                        #region "Clear method isolated load 6"
                        i = i + 1;
                        //tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 7 PDSX" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 7 PDSX":
                        #region "Clear method isolated load 7"
                        i = i + 1;
                        //tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 8 PDSZ" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 8 PDSZ":
                        #region "Clear method isolated load 8"
                        i = i + 1;
                        //tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD COMB 9 1.4CM+1.7CV" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD COMB 9 1.4CM+1.7CV":
                        #region "Clear method isolated combo 9"
                        // i = i + 1;
                        // tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "LOAD COMB 9 1.4CM+1.7CV")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "PRINT ANALYSIS RESULTS" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    default:
                        tmp.Add(line);
                        break;
                }
                #endregion
            }

            // list convert to string
            for (int i = 0; i < tmp.Count; i++)
            {
                tmpAislado.AppendLine(tmp[i]);
            }
            // agregar LOAD CM
            // loadcm.AppendLine("LOAD 2 CV");
            //loadcm.Append("JOINT LOAD");
            // tmpAislado.Replace("LOAD 2 CV", loadcm.Insert(0, String.Format("JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD CV
            // loadcv.AppendLine("LOAD 3 CSX");
            //loadcv.Append("JOINT LOAD");
            // tmpAislado.Replace("LOAD 3 CSX", loadcv.Insert(0, String.Format("JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD CSX
            // tmpText.Replace("LOAD 3 CSX", loadcsx.Insert(0, String.Format("LOAD 3 CSX{0}", Environment.NewLine)).ToString());
            // agregar LOAD CSZ
            // tmpText.Replace("LOAD 4 CSZ", loadcsz.Insert(0, String.Format("LOAD 4 CSZ{0}", Environment.NewLine)).ToString());
            // agregar LOAD CSXV
            String tsv = "";
            tsv = loadsv.ToString();
            tsv = tsv + "PERFORM ANALYSIS";
            tmpAislado.Replace("PERFORM ANALYSIS", tsv.Insert(0, String.Format("LOAD 5 CSXV{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD CSZV
            String tmsv = "";
            tmsv = loadsv.ToString();
            tmsv = tmsv + "PERFORM ANALYSIS";
            tmpAislado.Replace("PERFORM ANALYSIS", tmsv.Insert(0, String.Format("LOAD 6 CSZV{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD PDSX
            loadapdsx.Append("PERFORM ANALYSIS");
            tmpAislado.Replace("PERFORM ANALYSIS", loadapdsx.Insert(0, String.Format("LOAD 7 PDSX{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD PDSZ
            loadapdsz.Append("PERFORM ANALYSIS");
            tmpAislado.Replace("PERFORM ANALYSIS", loadapdsz.Insert(0, String.Format("LOAD 8 PDSZ{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar combos
            tmpAislado.Replace("PERFORM ANALYSIS", combos.ToString());
            // clean if exists
            tmpAislado.Replace("LOAD LIST 1 5 TO 18", String.Format("{0}", Environment.NewLine));
            // agregar
            tmpAislado.Replace("PRINT ANALYSIS RESULTS", String.Format("PRINT ANALYSIS RESULTS{0}LOAD LIST 1 5 TO 18", Environment.NewLine));
            //Escribiendo archivo Aislado
            // obtener nombre de archivo
            string[] par = this.path.Split(new char[] { '\\' });
            String direccion = destiny;
            direccion += String.Format(@"\{0} - AISLADO.std", (par[par.Length - 1].Split(new char[] { '.' }).First()));
            StreamWriter write = new StreamWriter(direccion, false, Encoding.ASCII);
            write.Write(tmpAislado.ToString());
            write.Close();
            Console.WriteLine("FINISH WRITE FILE!!!");
        }

        public void processNoAisladoSTD(StringBuilder Text)
        {
            //try
            //{
            preload();
            StringBuilder tmpNOAislado = new StringBuilder();

            //here clean content back to file
            Console.WriteLine("LIST SIZE " + this.listToText.Count);
            List<string> tmp = new List<string>();
            for (int i = 0; i < this.listToText.Count; i++)
            {
                #region "clean content file not isolated"
                string line = this.listToText[i];
                switch (line)
                {
                    case "LOAD 1 CM":
                        #region "Clear method not isolated load 1"
                        i = i + 1;
                        tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD" || ltmp == "LOAD 2 CV" || ltmp == "PERFORM ANALYSIS")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 2 CV" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 2 CV":
                        #region "Clear method not isolated load 2 "
                        i = i + 1;
                        tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD" || ltmp == "LOAD 3 CSX" || ltmp == "PERFORM ANALYSIS")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 3 CSX" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 3 CSX":
                        #region "Clear method not isolated load 3"
                        i = i + 1;
                        tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD" || ltmp == "LOAD 4 CSZ" || ltmp == "PERFORM ANALYSIS")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 4 CSZ" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 4 CSZ":
                        #region "Clear method not isolated load 4"
                        i = i + 1;
                        tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD" || ltmp == "LOAD 5 CSXV" || ltmp == "PERFORM ANALYSIS")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 5 CSXV" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 5 CSXV":
                        #region "Clear method not isolated load 5"
                        i = i + 1;
                        //tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 6 CSZV" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 6 CSZV":
                        #region "Clear method isolated load 6"
                        i = i + 1;
                        //tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 7 PDSX" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 7 PDSX":
                        #region "Clear method isolated load 7"
                        i = i + 1;
                        //tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD 8 PDSZ" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD 8 PDSZ":
                        #region "Clear method isolated load 8"
                        i = i + 1;
                        //tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "JOINT LOAD")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "LOAD COMB 9 1.4CM+1.7CV" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    case "LOAD COMB 9 1.4CM+1.7CV":
                        #region "Clear method isolated combo 9"
                        // i = i + 1;
                        // tmp.Add(line);
                        for (int zv = i; zv < this.listToText.Count; zv++)
                        {
                            string ltmp = this.listToText[zv];
                            if (ltmp == "LOAD COMB 9 1.4CM+1.7CV")
                            {
                                for (int c = zv; c < this.listToText.Count; c++)
                                {
                                    string sltmp = this.listToText[c];
                                    if (sltmp == "PRINT ANALYSIS RESULTS" || sltmp == "PERFORM ANALYSIS")
                                    {
                                        i = c - 1;
                                        break;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                //tmp.Add(ltmp);
                            }
                        }
                        #endregion
                        break;
                    default:
                        tmp.Add(line);
                        break;
                }
                #endregion
            }

            // list convert to string
            for (int i = 0; i < tmp.Count; i++)
            {
                tmpNOAislado.AppendLine(tmp[i]);
            }

            // agregar LOAD CM
            loadcm.Append("LOAD 2 CV");
            tmpNOAislado.Replace("LOAD 2 CV", loadcm.Insert(0, String.Format("JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD CV
            loadcv.Append("LOAD 3 CSX");
            tmpNOAislado.Replace("LOAD 3 CSX", loadcv.Insert(0, String.Format("JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD CSX
            tmpNOAislado.Replace("LOAD 3 CSX", loadcsx.Insert(0, String.Format("LOAD 3 CSX{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD CSZ
            tmpNOAislado.Replace("LOAD 4 CSZ", loadcsz.Insert(0, String.Format("LOAD 4 CSZ{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD CSXV
            loadcsxv.Append("PERFORM ANALYSIS");
            tmpNOAislado.Replace("PERFORM ANALYSIS", loadcsxv.Insert(0, String.Format("LOAD 5 CSXV{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD CSZV
            loadcszv.Append("PERFORM ANALYSIS");
            tmpNOAislado.Replace("PERFORM ANALYSIS", loadcszv.Insert(0, string.Format("LOAD 6 CSZV{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD PDSX
            loadpdsx.Append("PERFORM ANALYSIS");
            tmpNOAislado.Replace("PERFORM ANALYSIS", loadpdsx.Insert(0, String.Format("LOAD 7 PDSX{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar LOAD PDSZ
            loadpdsz.Append("PERFORM ANALYSIS");
            tmpNOAislado.Replace("PERFORM ANALYSIS", loadpdsz.Insert(0, String.Format("LOAD 8 PDSZ{0}JOINT LOAD{0}", Environment.NewLine)).ToString());
            // agregar combos
            tmpNOAislado.Replace("PERFORM ANALYSIS", combos.ToString());
            // agregar
            // clean if exists
            tmpNOAislado.Replace("LOAD LIST 1 5 TO 18", String.Format("{0}", Environment.NewLine));
            tmpNOAislado.Replace("PRINT ANALYSIS RESULTS", String.Format("PRINT ANALYSIS RESULTS{0}LOAD LIST 1 5 TO 18", Environment.NewLine));
            //Escribiendo archivo Aislado
            // obtener nombre de archivo
            string[] par = this.path.Split(new char[] { '\\' });
            String direccion = destiny;
            direccion += String.Format(@"\{0} - NOAISLADO.std", (par[par.Length - 1].Split(new char[] {'.'}).First()));
            StreamWriter write = new StreamWriter(direccion, false, Encoding.ASCII);
            write.Write(tmpNOAislado.ToString());
            write.Close();
            Console.WriteLine("FINISH WRITE FILE!!!");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }

        #endregion

        public void preload()
        {
            loadcm = new StringBuilder("");
            loadcv = new StringBuilder("");
            loadcsx = new StringBuilder("");
            loadcsz = new StringBuilder("");
            loadcsxv = new StringBuilder("");
            loadcszv = new StringBuilder("");
            loadpdsx = new StringBuilder("");
            loadpdsz = new StringBuilder("");
            loadsv = new StringBuilder("");
            loadapdsx = new StringBuilder("");
            loadapdsz = new StringBuilder("");
            foreach (DataRow row in Model.MDStaadPRO.dtGlobal.Rows)
            {
                // carga muerta CM
                loadcm.AppendLine(String.Format("{0} FY -{1}", row["nodo"], row["cm"]));
                // carga viva CV
                loadcv.AppendLine(String.Format("{0} FY -{1}", row["nodo"], row["cv"]));
                // carga CSX
                loadcsx.AppendLine(String.Format("{0} FX {1}", row["nodo"], row["csx"]));
                // carga CSZ
                loadcsz.AppendLine(String.Format("{0} FZ {1}", row["nodo"], row["csz"]));
                // carga CSXV
                loadcsxv.AppendLine(String.Format("{0} FY -{1}", row["nodo"], row["csxv"]));
                //carga CSZV
                loadcszv.AppendLine(String.Format("{0} FY -{1}", row["nodo"], row["cszv"]));
                // carga PDSX
                loadpdsx.AppendLine(String.Format("{0} MZ -{1}", row["nodo"], row["pdsx"]));
                // carga PDSZ
                loadpdsz.AppendLine(String.Format("{0} MX {1}", row["nodo"], row["pdsz"]));
                // File Aislado
                // carga sv
                loadsv.AppendLine(String.Format("{0} FY -{1}", row["nodo"], row["sv"]));
                // carga PDSX Aislado
                loadapdsx.AppendLine(String.Format("{0} MZ -{1}", row["nodo"], row["pdsxa"]));
                // carga PDSZ Aislado
                loadapdsz.AppendLine(String.Format("{0} MX {1}", row["nodo"], row["pdsza"]));
            }
        }
    }
}
