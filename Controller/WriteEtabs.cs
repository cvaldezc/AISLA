using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Controller
{
    public class WriteEtabs : Interfaces.IWriteEtabs
    {

        public String path;
        public String destino;

        #region IWriteEtabs Members

        public void processe2kAislado(String story, Double cm)
        {
            if (File.Exists(path))
            {
                // before valid meets the requirements
                #region Read file
                String line = "";
                StringBuilder allbody = new StringBuilder();
                bool log = false;
                List<string> toText = new List<string>();
                List<string> textfile = File.ReadAllLines(path, Encoding.UTF8).ToList();

                // int count = 1;
                IDictionary<string, IDictionary<string, object>> properties = new Dictionary<string, IDictionary<string, object>>()
                {
                    #region Initialize properties validate
                    {
                        "$ LOAD PATTERNS", new Dictionary<string, object>()
                        {
                            { "exists", false },
                            { "clean", false },
                            { "index", -1 },
                            { "value", "  LOADPATTERN \"PDSX\"  TYPE  \"Seismic\"  SELFWEIGHT  0\r\n" +
                                       "  LOADPATTERN \"PDSY\"  TYPE  \"Seismic\"  SELFWEIGHT  0\r\n" +
                                       "  SEISMIC \"PDSX\"  \"User Loads\"\r\n" +
                                       "  SEISMIC \"PDSY\"  \"User Loads\"\r\n" },
                        }
                    },
                    {
                        "$ LOAD CASES", new Dictionary<string, object>()
                        {
                            { "exists", false },
                            { "clean", false },
                            { "index", -1 },
                            { "value", "\r\n"+
                                       "  LOADCASE \"PDSX\"  TYPE  \"Linear Static\"  INITCOND  \"PRESET\"\r\n"+
                                       "  LOADCASE \"PDSX\"  LOADPAT  \"PDSX\"  SF  1 \r\n"+
                                       "  LOADCASE \"PDSY\"  TYPE  \"Linear Static\"  INITCOND  \"PRESET\"\r\n"+
                                       "  LOADCASE \"PDSY\"  LOADPAT  \"PDSY\"  SF  1\r\n\r\n" },
                        }
                    },
                    {
                        "$ POINT OBJECT LOADS", new Dictionary<string, object>()
                        {
                            { "exists", false },
                            { "clean", true },
                            { "index", -1 },
                            { "value",  getAisladoObjectLoad(story).ToString()}
                        }
                    },
                    {
                        "$ FRAME OBJECT LOADS", new Dictionary<string, object>()
                        {
                            { "exists", false },
                            { "clean", true },
                            { "index", -1 },
                            { "value", "" },
                        }
                    },
                    {
                        "$ LOAD COMBINATIONS", new Dictionary<string, object>()
                        {
                            #region data extends
                            { "exists", false },
                            { "clean", false },
                            { "index", -1 },
                            { "value", "\r\n"+
                                        ""+ String.Format("  COMBO \"MV1\"  TYPE \"Linear Add\"", cm) +"\r\n" +
                                        "  COMBO \"MV1\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  \r\n" +
                                        "  COMBO \"MV1\"  LOADCASE \"Dead\"  SF 1.4 \r\n" +
                                        "  COMBO \"MV1\"  LOADCASE \"Live\"  SF 1.7 \r\n" +
                                        "\r\n" +
                                        "  COMBO \"MVSX2+\"  TYPE \"Linear Add\"  \r\n" +
                                        "  COMBO \"MVSX2+\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  \r\n" +
                                        "  COMBO \"MVSX2+\"  LOADCASE \"Dead\"  SF 1.25 \r\n" +
                                        "  COMBO \"MVSX2+\"  LOADCASE \"Live\"  SF 1.25 \r\n" +
                                        "  COMBO \"MVSX2+\"  LOADCASE \"SX\"  SF 1\r\n" +
                                        "  COMBO \"MVSX2+\"  LOADCASE \"SXV\"  SF 1 \r\n" +
                                        "" + String.Format("  COMBO \"MVSX2+\"  LOADCASE \"DEAD\"  SF {0}", cm) + "\r\n" +
                                        "  COMBO \"MVSX2+\"  LOADCASE \"PDSX\"  SF 1\r\n" +
                                        "\r\n" +
                                        "  COMBO \"MSX3+\"  TYPE \"Linear Add\"  \r\n" +
                                        "  COMBO \"MSX3+\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  \r\n" +
                                        "  COMBO \"MSX3+\"  LOADCASE \"Dead\"  SF 0.9 \r\n" +
                                        "  COMBO \"MSX3+\"  LOADCASE \"SX\"  SF 1\r\n" +
                                        "  COMBO \"MSX3+\"  LOADCASE \"SXV\"  SF 1 \r\n" +
                                        "  COMBO \"MSX3+\"  LOADCASE \"SV\"  SF 1\r\n" +
                                        "" + String.Format("  COMBO \"MSX3+\"  LOADCASE \"DEAD\"  SF {0}", cm) + "\r\n" +
                                        "  COMBO \"MSX3+\"  LOADCASE \"PDSX\"  SF 1\r\n" +
                                        "\r\n" +
                                        "  COMBO \"MVSX2-\"  TYPE \"Linear Add\"  \r\n" +
                                        "  COMBO \"MVSX2-\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  \r\n" +
                                        "  COMBO \"MVSX2-\"  LOADCASE \"Dead\"  SF 1.25 \r\n" +
                                        "  COMBO \"MVSX2-\"  LOADCASE \"Live\"  SF 1.25 \r\n" +
                                        "  COMBO \"MVSX2-\"  LOADCASE \"SX\"  SF -1\r\n" +
                                        "  COMBO \"MVSX2-\"  LOADCASE \"SXV\"  SF 1 \r\n" +
                                        "" + String.Format("  COMBO \"MVSX2-\"  LOADCASE \"DEAD\"  SF {0}", cm) + "\r\n" +
                                        "  COMBO \"MVSX2-\"  LOADCASE \"PDSX\"  SF -1\r\n" +
                                        "\r\n" +
                                        "  COMBO \"MSX3-\"  TYPE \"Linear Add\"  \r\n" +
                                        "  COMBO \"MSX3-\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  \r\n" +
                                        "  COMBO \"MSX3-\"  LOADCASE \"Dead\"  SF 0.9 \r\n" +
                                        "  COMBO \"MSX3-\"  LOADCASE \"SX\"  SF -1\r\n" +
                                        "  COMBO \"MSX3-\"  LOADCASE \"SXV\"  SF 1 \r\n" +
                                        "" + String.Format("  COMBO \"MSX3-\"  LOADCASE \"DEAD\"  SF {0}", cm) + "\r\n" +
                                        "  COMBO \"MSX3-\"  LOADCASE \"PDSX\"  SF -1\r\n" +
                                        "\r\n" +
                                        "  COMBO \"MVSY2+\"  TYPE \"Linear Add\"  \r\n" +
                                        "  COMBO \"MVSY2+\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\" \r\n" +
                                        "  COMBO \"MVSY2+\"  LOADCASE \"Dead\"  SF 1.25 \r\n" +
                                        "  COMBO \"MVSY2+\"  LOADCASE \"Live\"  SF 1.25 \r\n" +
                                        "  COMBO \"MVSY2+\"  LOADCASE \"SY\"  SF 1\r\n" +
                                        "  COMBO \"MVSY2+\"  LOADCASE \"SYV\"  SF 1 \r\n" +
                                        "" + String.Format("  COMBO \"MVSY2+\"  LOADCASE \"DEAD\"  SF {0}", cm) + "\r\n" +
                                        "  COMBO \"MVSY2+\"  LOADCASE \"PDSY\"  SF 1\r\n" +
                                        "\r\n" +
                                        "  COMBO \"MSY3+\"  TYPE \"Linear Add\"  \r\n" +
                                        "  COMBO \"MSY3+\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  \r\n" +
                                        "  COMBO \"MSY3+\"  LOADCASE \"Dead\"  SF 0.9 \r\n" +
                                        "  COMBO \"MSY3+\"  LOADCASE \"SY\"  SF 1\r\n" +
                                        "  COMBO \"MSY3+\"  LOADCASE \"SYV\"  SF 1 \r\n" +
                                        "" + String.Format("  COMBO \"MSY3+\"  LOADCASE \"DEAD\"  SF {0}", cm) + "\r\n" +
                                        "  COMBO \"MSY3+\"  LOADCASE \"PDSY\"  SF 1\r\n" +
                                        "\r\n" +
                                        "  COMBO \"MVSY2-\"  TYPE \"Linear Add\"  \r\n" +
                                        "  COMBO \"MVSY2-\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\" \r\n" +
                                        "  COMBO \"MVSY2-\"  LOADCASE \"Dead\"  SF 1.25 \r\n" +
                                        "  COMBO \"MVSY2-\"  LOADCASE \"Live\"  SF 1.25\r\n" +
                                        "  COMBO \"MVSY2-\"  LOADCASE \"SY\"  SF -1\r\n" +
                                        "  COMBO \"MVSY2-\"  LOADCASE \"SYV\"  SF 1 \r\n" +
                                        "" + String.Format("  COMBO \"MVSY2-\"  LOADCASE \"DEAD\"  SF {0}", cm) + "\r\n" +
                                        "  COMBO \"MVSY2-\"  LOADCASE \"PDSY\"  SF -1\r\n" +
                                        "\r\n" +
                                        "  COMBO \"MSY3-\"  TYPE \"Linear Add\" \r\n" +
                                        "  COMBO \"MSY3-\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  \r\n" +
                                        "  COMBO \"MSY3-\"  LOADCASE \"Dead\"  SF 0.9 \r\n" +
                                        "  COMBO \"MSY3-\"  LOADCASE \"SY\"  SF -1\r\n" +
                                        "  COMBO \"MSY3-\"  LOADCASE \"SYV\"  SF 1 \r\n" +
                                        "" + String.Format("  COMBO \"MSY3-\"  LOADCASE \"DEAD\"  SF {0}", cm) + "\r\n" +
                                        "  COMBO \"MSY3-\"  LOADCASE \"PDSY\"  SF -1\r\n"+
                                        "\r\n" +
                                        "  COMBO \"ENVOLVENTE\"  TYPE \"Envelope\" \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MV1\"  SF 1\r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSX2+\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSX3+\"  SF 1\r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSX2-\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSX3-\"  SF 1\r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSX2+\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSX3+\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSX2-\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSX3-\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSY2+\"  SF 1\r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSY3+\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSY2-\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSY3-\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSY2+\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSY3+\"  SF 1 \r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSY2-\"  SF 1\r\n" +
                                        "  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSY3-\"  SF 1 \r\n" }
                        }
                        #endregion
                    },
                    {
                        "$ LOG", new Dictionary<string, object>()
                        {
                            { "exists", true },
                            { "index", -1 }
                        }
                    },
                    #endregion
                };
                for (ushort i = 0; i < textfile.Count; i++)
                {
                    #region get index titles if exist
                    line = textfile[i].ToString();
                    toText.Add(line);
                    switch (line)
                    {
                        case "$ LOAD PATTERNS":
                            properties[line]["exists"] = true;
                            if ((bool)properties[line]["clean"])
                                i = cleanContentObject(i, textfile);
                            properties[line]["index"] = toText.Count - 1;
                            Console.WriteLine(line);
                            Console.WriteLine("INDEX " + i);
                            break;
                        case "$ LOAD CASES":
                            properties[line]["exists"] = true;
                            if ((bool)properties[line]["clean"])
                                i = cleanContentObject(i, textfile);
                            properties[line]["index"] = toText.Count - 1;
                            Console.WriteLine(line);
                            Console.WriteLine("INDEX " + i);
                            break;
                        case "$ POINT OBJECT LOADS":
                            properties[line]["exists"] = true;
                            if ((bool)properties[line]["clean"])
                                i = cleanContentObject(i, textfile);
                            properties[line]["index"] = toText.Count - 1;
                            Console.WriteLine(line);
                            Console.WriteLine("INDEX " + i);
                            break;
                        case "$ FRAME OBJECT LOADS":
                            properties[line]["exists"] = true;
                            if ((bool)properties[line]["clean"])
                                i = cleanContentObject(i, textfile);
                            properties[line]["index"] = toText.Count - 1;
                            Console.WriteLine(line);
                            Console.WriteLine("INDEX " + i);
                            break;
                        case "$ LOAD COMBINATIONS":
                            properties[line]["exists"] = true;
                            if ((bool)properties[line]["clean"])
                                i = cleanContentObject(i, textfile);
                            properties[line]["index"] = toText.Count - 1;
                            Console.WriteLine(line);
                            Console.WriteLine("INDEX " + i);
                            break;
                        case "$ LOG":
                            Console.WriteLine(line);
                            Console.WriteLine("INDEX " + i);
                            properties["$ LOG"]["index"] = toText.Count - 1;
                            break;
                    }
                    #endregion
                }

                foreach (string item in properties.Keys)
                {
                    #region add content for each title in the file exist and if not
                    if (!item.ToString().Equals("$ LOG"))
                    {
                        if (Convert.ToBoolean(properties[item]["exists"]))
                        {
                            Int16 index = Convert.ToInt16(properties[item]["index"]);
                            String body = "\r\n \r\n" +item+ "\r\n \r\n";
                            body += Convert.ToString(properties[item]["value"]);
                            body += "\r\n \r\n";
                            toText[index] = body;
                            Console.WriteLine(item);
                            Console.WriteLine(" EXISTS ADD");
                        }
                        else
                        {
                            Console.WriteLine(item);
                            Console.WriteLine(" NO EXISTS ADD");
                            Int16 index = Convert.ToInt16(properties["$ LOG"]["index"]);
                            String text = toText[index];
                            Console.WriteLine("INDEX " + index);
                            //Console.WriteLine("content " + text);
                            if (text.Equals("$ LOG"))
                                text = "";
                            text += "\r\n \r\n" + item + "\r\n \r\n";
                            text += Convert.ToString(properties[item]["value"]);
                            text += "\r\n"; 
                            toText[index] = text;
                            log = true;
                        }
                    }
                    #endregion
                }

                // now clean other line if exists
                // finally add $ LOG if only keys not exists
                if (log)
                {
                    #region add title $ LOG
                    UInt16 index = Convert.ToUInt16(properties["$ LOG"]["index"]);
                    String text = toText[index];
                    text += "\r\n$ LOG\r\n";
                    toText[index] = text;
                    #endregion
                }
                // create new body text file
                allbody.Append(string.Join("\r\n", toText.ToArray()));
                // obtener nombre de archivo
                #region create file aislado
                string[] par = this.path.Split(new char[] { '\\' });
                String direccion = destino;
                direccion += String.Format(@"\{0} - AISLADO.e2k", (par[par.Length - 1].Split('.')[0]));
                StreamWriter write = new StreamWriter(direccion, false, Encoding.ASCII);
                write.Write(allbody.ToString());
                write.Close();
                Console.WriteLine("FINISH WRITE FILE!!!");
                #endregion
                //read.Close();
                #endregion
            }
        }

        #endregion

        private StringBuilder getAisladoObjectLoad(string story)
        {
            StringBuilder _tmp = new StringBuilder(); ;
            // agregamos datos
            foreach (DataRow row in Model.MDEtabs.dtSX.Rows)
            {
                // POINTLOAD	1	Base	TYPE	FORCE	LC	PDSX	MY	12.44
                _tmp.AppendLine(String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  PDSX  MY  {2}", row["joint"], story, row["my"]));
            }
            _tmp.AppendLine("");
            foreach (DataRow row in Model.MDEtabs.dtSY.Rows)
            {
                _tmp.AppendLine(String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  PDSY  MX  -{2}", row["joint"], story, row["mx"]));
            }
            return _tmp;
        }

        private ushort cleanContentObject(ushort index, List<string> list)
        {
            Console.WriteLine("INDEX CLEAN DATA " + index);
            ushort _index = 0;
            index++;
            Console.WriteLine("INDEX CLEAN DATA INCREMENT " + index);
            try
            {
                for(ushort i = index; i < list.Count; i++)
                {
                    if (list[i].ToString().IndexOf("$") == 0)
                    {
                        _index = i;
                        break;
                    }
                }
                Console.WriteLine("INDEX CLEAN DATA FINISH " + _index);
                list = new List<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return --_index;
        }

        #region IWriteEtabs Members


        public void processe2kNoAislado(String story, Double cm)
        {
            if (File.Exists(path))
            {
                // iniciamos lectura de archivo
                using (StreamReader read = new StreamReader(this.path, Encoding.UTF8))
                {
                    String line;
                    StringBuilder Text = new StringBuilder();
                    bool load = false;
                    int count = 1;
                    for (int i = 0; i < File.ReadAllLines(path).Length; i++)
                    {
                        line = read.ReadLine();
                        #region switch
                        count++;
                        switch (line)
                        {
                            case "$ LOAD PATTERNS":
                                Text.AppendLine(line);
                                Text.AppendLine("  LOADPATTERN \"PDSX\"  TYPE  \"Seismic\"  SELFWEIGHT  0");
                                Text.AppendLine("  LOADPATTERN \"PDSY\"  TYPE  \"Seismic\"  SELFWEIGHT  0");
                                Text.AppendLine("  SEISMIC \"PDSX\"  \"User Loads\"");
                                Text.AppendLine("  SEISMIC \"PDSY\"  \"User Loads\"");
                                Text.AppendLine("  LOADPATTERN \"SXV\"  TYPE  \"Other\"  SELFWEIGHT  0");
                                Text.AppendLine("  LOADPATTERN \"SYV\"  TYPE  \"Other\"  SELFWEIGHT  0");
                                break;
                            case "$ POINT OBJECT LOADS":
                                load = true;
                                Text.AppendLine(line);
                                // agregamos datos
                                foreach (DataRow row in Model.MDEtabs.dtGlobal.Rows)
                                {
                                    // POINTLOAD  "2"  "Base"  TYPE "FORCE"  LC "Dead" FZ -23.88
                                    // POINTLOAD  "2"  "Base"  TYPE "FORCE"  LC "Live" FZ -.6488  
                                    // POINTLOAD  "2"  "Base"  TYPE "FORCE"  LC "SX"  FX 4.17  FZ -1.2363  MY 7.36
                                    Text.AppendLine(
                                        String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  Dead  FZ  -{2}",
                                            row["joint"], story, row["dead"]));
                                    Text.AppendLine(
                                        String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  Live  FZ  -{2}",
                                            row["joint"], story, row["live"]));
                                    Text.AppendLine(
                                        String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  SX  FX  {2}",
                                            row["joint"], story, row["fx"]));
                                    Text.AppendLine(
                                        String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  SY  FY  {2}",
                                            row["joint"], story, row["fy"]));
                                    Text.AppendLine(
                                        String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  SXV  FZ  -{2}",
                                            row["joint"], story, row["fzx"]));
                                    Text.AppendLine(
                                        String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  SYV  FZ  -{2}",
                                            row["joint"], story, row["fzy"]));
                                    Text.AppendLine(
                                        String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  PDSX  MY  {2}",
                                            row["joint"], story, row["pdsx"]));
                                    Text.AppendLine(
                                        String.Format("  POINTLOAD  {0}  {1}  TYPE  FORCE  LC  PDSY  MX  -{2}",
                                            row["joint"], story, row["pdsy"]));
                                }
                                Text.AppendLine("");
                                Text.AppendLine("");
                                break;
                            case "$ FRAME OBJECT LOADS":
                                load = false;
                                Text.AppendLine(line);
                                break;

                            case "$ LOAD CASES":
                                Text.AppendLine(line);
                                Text.AppendLine("  LOADCASE \"SXV\"  TYPE  \"Linear Static\"  INITCOND  \"PRESET\"");
                                Text.AppendLine("  LOADCASE \"SXV\"  LOADPAT  \"SXV\"  SF  1 ");
                                Text.AppendLine("  LOADCASE \"SYV\"  TYPE  \"Linear Static\"  INITCOND  \"PRESET\"");
                                Text.AppendLine("  LOADCASE \"SYV\"  LOADPAT  \"SYV\"  SF  1");
                                Text.AppendLine("");
                                Text.AppendLine("  LOADCASE \"PDSX\"  TYPE  \"Linear Static\"  INITCOND  \"PRESET\"");
                                Text.AppendLine("  LOADCASE \"PDSX\"  LOADPAT  \"PDSX\"  SF  1 ");
                                Text.AppendLine("  LOADCASE \"PDSY\"  TYPE  \"Linear Static\"  INITCOND  \"PRESET\"");
                                Text.AppendLine("  LOADCASE \"PDSY\"  LOADPAT  \"PDSY\"  SF  1");
                                Text.AppendLine("");
                                break;
                            case "$ LOAD COMBINATIONS":
                                Text.AppendLine(line);
                                Text.AppendLine(String.Format("  COMBO \"MV1\"  TYPE \"Linear Add\"", cm));
                                Text.AppendLine("  COMBO \"MV1\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  ");
                                Text.AppendLine("  COMBO \"MV1\"  LOADCASE \"Dead\"  SF 1.4 ");
                                Text.AppendLine("  COMBO \"MV1\"  LOADCASE \"Live\"  SF 1.7 ");
                                Text.AppendLine("");
                                Text.AppendLine("  COMBO \"MVSX2+\"  TYPE \"Linear Add\"  ");
                                Text.AppendLine("  COMBO \"MVSX2+\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  ");
                                Text.AppendLine("  COMBO \"MVSX2+\"  LOADCASE \"Dead\"  SF 1.25 ");
                                Text.AppendLine("  COMBO \"MVSX2+\"  LOADCASE \"Live\"  SF 1.25 ");
                                Text.AppendLine("  COMBO \"MVSX2+\"  LOADCASE \"SX\"  SF 1");
                                Text.AppendLine("  COMBO \"MVSX2+\"  LOADCASE \"SXV\"  SF 1 ");
                                Text.AppendLine(String.Format("  COMBO \"MVSX2+\"  LOADCASE \"DEAD\"  SF {0}", cm));
                                Text.AppendLine("  COMBO \"MVSX2+\"  LOADCASE \"PDSX\"  SF 1");
                                Text.AppendLine("");
                                Text.AppendLine("  COMBO \"MSX3+\"  TYPE \"Linear Add\"  ");
                                Text.AppendLine("  COMBO \"MSX3+\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  ");
                                Text.AppendLine("  COMBO \"MSX3+\"  LOADCASE \"Dead\"  SF 0.9 ");
                                Text.AppendLine("  COMBO \"MSX3+\"  LOADCASE \"SX\"  SF 1");
                                Text.AppendLine("  COMBO \"MSX3+\"  LOADCASE \"SXV\"  SF 1 ");
                                Text.AppendLine("  COMBO \"MSX3+\"  LOADCASE \"SV\"  SF 1");
                                Text.AppendLine(String.Format("  COMBO \"MSX3+\"  LOADCASE \"DEAD\"  SF {0}", cm));
                                Text.AppendLine("  COMBO \"MSX3+\"  LOADCASE \"PDSX\"  SF 1");
                                Text.AppendLine("");
                                Text.AppendLine("  COMBO \"MVSX2-\"  TYPE \"Linear Add\"  ");
                                Text.AppendLine("  COMBO \"MVSX2-\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  ");
                                Text.AppendLine("  COMBO \"MVSX2-\"  LOADCASE \"Dead\"  SF 1.25 ");
                                Text.AppendLine("  COMBO \"MVSX2-\"  LOADCASE \"Live\"  SF 1.25 ");
                                Text.AppendLine("  COMBO \"MVSX2-\"  LOADCASE \"SX\"  SF -1");
                                Text.AppendLine("  COMBO \"MVSX2-\"  LOADCASE \"SXV\"  SF 1 ");
                                Text.AppendLine(String.Format("  COMBO \"MVSX2-\"  LOADCASE \"DEAD\"  SF {0}", cm));
                                Text.AppendLine("  COMBO \"MVSX2-\"  LOADCASE \"PDSX\"  SF -1");
                                Text.AppendLine("");
                                Text.AppendLine("  COMBO \"MSX3-\"  TYPE \"Linear Add\"  ");
                                Text.AppendLine("  COMBO \"MSX3-\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  ");
                                Text.AppendLine("  COMBO \"MSX3-\"  LOADCASE \"Dead\"  SF 0.9 ");
                                Text.AppendLine("  COMBO \"MSX3-\"  LOADCASE \"SX\"  SF -1");
                                Text.AppendLine("  COMBO \"MSX3-\"  LOADCASE \"SXV\"  SF 1 ");
                                Text.AppendLine(String.Format("  COMBO \"MSX3-\"  LOADCASE \"DEAD\"  SF {0}", cm));
                                Text.AppendLine("  COMBO \"MSX3-\"  LOADCASE \"PDSX\"  SF -1");
                                Text.AppendLine("");
                                Text.AppendLine("  COMBO \"MVSY2+\"  TYPE \"Linear Add\"  ");
                                Text.AppendLine("  COMBO \"MVSY2+\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\" ");
                                Text.AppendLine("  COMBO \"MVSY2+\"  LOADCASE \"Dead\"  SF 1.25 ");
                                Text.AppendLine("  COMBO \"MVSY2+\"  LOADCASE \"Live\"  SF 1.25 ");
                                Text.AppendLine("  COMBO \"MVSY2+\"  LOADCASE \"SY\"  SF 1");
                                Text.AppendLine("  COMBO \"MVSY2+\"  LOADCASE \"SYV\"  SF 1 ");
                                Text.AppendLine(String.Format("  COMBO \"MVSY2+\"  LOADCASE \"DEAD\"  SF {0}", cm));
                                Text.AppendLine("  COMBO \"MVSY2+\"  LOADCASE \"PDSY\"  SF 1");
                                Text.AppendLine("");
                                Text.AppendLine("  COMBO \"MSY3+\"  TYPE \"Linear Add\"  ");
                                Text.AppendLine("  COMBO \"MSY3+\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  ");
                                Text.AppendLine("  COMBO \"MSY3+\"  LOADCASE \"Dead\"  SF 0.9 ");
                                Text.AppendLine("  COMBO \"MSY3+\"  LOADCASE \"SY\"  SF 1");
                                Text.AppendLine("  COMBO \"MSY3+\"  LOADCASE \"SYV\"  SF 1 ");
                                Text.AppendLine(String.Format("  COMBO \"MSY3+\"  LOADCASE \"DEAD\"  SF {0}", cm));
                                Text.AppendLine("  COMBO \"MSY3+\"  LOADCASE \"PDSY\"  SF 1");
                                Text.AppendLine("");
                                Text.AppendLine("  COMBO \"MVSY2-\"  TYPE \"Linear Add\"  ");
                                Text.AppendLine("  COMBO \"MVSY2-\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\" ");
                                Text.AppendLine("  COMBO \"MVSY2-\"  LOADCASE \"Dead\"  SF 1.25 ");
                                Text.AppendLine("  COMBO \"MVSY2-\"  LOADCASE \"Live\"  SF 1.25");
                                Text.AppendLine("  COMBO \"MVSY2-\"  LOADCASE \"SY\"  SF -1");
                                Text.AppendLine("  COMBO \"MVSY2-\"  LOADCASE \"SYV\"  SF 1 ");
                                Text.AppendLine(String.Format("  COMBO \"MVSY2-\"  LOADCASE \"DEAD\"  SF {0}", cm));
                                Text.AppendLine("  COMBO \"MVSY2-\"  LOADCASE \"PDSY\"  SF -1");
                                Text.AppendLine("");
                                Text.AppendLine("  COMBO \"MSY3-\"  TYPE \"Linear Add\" ");
                                Text.AppendLine("  COMBO \"MSY3-\"  DESIGN \"Concrete\"  COMBOTYPE \"Strength\"  ");
                                Text.AppendLine("  COMBO \"MSY3-\"  LOADCASE \"Dead\"  SF 0.9 ");
                                Text.AppendLine("  COMBO \"MSY3-\"  LOADCASE \"SY\"  SF -1");
                                Text.AppendLine("  COMBO \"MSY3-\"  LOADCASE \"SYV\"  SF 1 ");
                                Text.AppendLine(String.Format("  COMBO \"MSY3-\"  LOADCASE \"DEAD\"  SF {0}", cm));
                                Text.AppendLine("  COMBO \"MSY3-\"  LOADCASE \"PDSY\"  SF -1");

                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  TYPE \"Envelope\" ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MV1\"  SF 1");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSX2+\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSX3+\"  SF 1");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSX2-\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSX3-\"  SF 1");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSX2+\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSX3+\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSX2-\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSX3-\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSY2+\"  SF 1");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSY3+\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSY2-\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSY3-\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSY2+\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSY3+\"  SF 1 ");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MVSY2-\"  SF 1");
                                Text.AppendLine("  COMBO \"ENVOLVENTE\"  LOADCOMBO \"MSY3-\"  SF 1 ");
                                break;
                            default:
                                if (!load)
                                {
                                    Text.AppendLine(line);
                                }
                                break;
                        }
                        // Console.WriteLine(count);
                        #endregion
                    }

                    // obtener nombre de archivo
                    string[] par = this.path.Split(new char[] { '\\' });
                    String direccion = destino;
                    direccion += String.Format(@"\{0} - NoAislado.e2k", (par[par.Length - 1].Split('.')[0]));
                    StreamWriter write = new StreamWriter(direccion, false, Encoding.ASCII);
                    write.Write(Text.ToString());
                    write.Close();
                    Console.WriteLine("FINISH WRITE FILE!!!");
                    read.Close();
                }
            }
        }

        #endregion
    }
}
