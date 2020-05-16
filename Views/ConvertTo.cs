using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Controller;

namespace Views
{
    public partial class ConvertTo : Form
    {

        public ConvertTo()
        {
            InitializeComponent();
            sismoVertical();
            chckText();
            checkedDefaultEtabs();
            CheckForIllegalCrossThreadCalls = false;
            this.btnBaseNotIsolated.Enabled = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnProcesar_Click(object sender, EventArgs e)
        {
            BarraProgreso.status = 0;
            statusBar();
            //Thread t2 = new Thread(new ThreadStart(statusBar));
            Thread t1 = new Thread(new ThreadStart(ProcessFiles));
            BarraProgreso.tarea1 = t1;
            //BarraProgreso.tarea2 = t2;
            BarraProgreso.tarea1.IsBackground = true;
            BarraProgreso.tarea1.Start();
            //BarraProgreso.tarea2.Start();
            //ProcessFiles();
        }

        private void btnSelArchivoBase_Click(object sender, EventArgs e)
        {
            this.oFD = new OpenFileDialog();
            this.oFD.Filter = "Importados (*.e2k, *.std) | *.e2k; *.std";
            if (this.oFD.ShowDialog() == DialogResult.OK)
            {
                this.txtArchivoBase.Text = this.oFD.FileName;
                if (this.oFD.FileName.Split(new char[] { '.' }).Last() == "std")
                {
                    this.btnBaseNotIsolated.Enabled = true;
                }
                else
                {
                    this.btnBaseNotIsolated.Enabled = false;
                }
            }
        }

        private void btnSelArchivoFormato_Click(object sender, EventArgs e)
        {
            this.oFD = new OpenFileDialog();
            this.oFD.Filter = "Formato (*.xlxs, *.txt) | *.xlsx; *.txt";
            if (this.oFD.ShowDialog() == DialogResult.OK)
            {
                this.txtArchivoFormato.Text = this.oFD.FileName;
            }
        }

        private void btnSelDestino_Click(object sender, EventArgs e)
        {
            //this.fBD = new FolderBrowserDialog();
            //if (this.fBD.ShowDialog() == DialogResult.OK)
            //{
            //    this.txtArchivoDestino.Text = this.fBD.SelectedPath.ToString();
            //}
            /*this.uisaveFileDialog = new SaveFileDialog();

            uisaveFileDialog.Title = "Selected Path";
            this.uisaveFileDialog.RestoreDirectory = true;
            this.uisaveFileDialog.FileName = "AISLA";
            if (uisaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtArchivoDestino.Text = uisaveFileDialog.FileName.Split(new string[] { "AISLA" }, StringSplitOptions.None)[0];
            }*/
            this.folderBrowserDialog1 = new FolderBrowserDialog();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtArchivoDestino.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
        
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
        
        }

        /// <summaty>
        /// block functions
        /// </summary>
        /// 
        protected void checkedDefaultEtabs()
        {
            for (int i = 0; i < etabsclbaislado.Items.Count; i++)
            {
                etabsclbaislado.SetItemChecked(i, true);
            }
            for (int i = 0; i < etabscblnosislado.Items.Count; i++)
            {
                etabscblnosislado.SetItemChecked(i, true);
            }
        }

        private void sismoVertical()
        {
            // Double delta = Convert.ToDouble(this.txtDelta.Value != null ? this.txtDelta.Value : 0);
            Double facZona = Convert.ToDouble((!string.IsNullOrEmpty(txtZonaZ.Value.ToString())) ? this.txtZonaZ.Value : 0);
            Double tipoSuelo = Convert.ToDouble((!string.IsNullOrEmpty(txtTipoSuelo.Value.ToString())) ? this.txtTipoSuelo.Value : 0);
            Double calc = (facZona * tipoSuelo * 0.2 * 2.5);
            this.lblSismoVertical.Text = Math.Round(calc, 2).ToString();
        }

        private void txtZonaZ_ValueChanged(object sender, EventArgs e)
        {
            sismoVertical();
        }

        private void txtTipoSuelo_ValueChanged(object sender, EventArgs e)
        {
            sismoVertical();
        }

        private void chckText()
        {
            Console.WriteLine("INSTAN CLICK");
            if (chkaislado.Checked)
            {
                txtaislado.Enabled = true;
            }else
            {
                txtaislado.Enabled = false;
            }

            if (chknoaislado.Checked)
            {
                txtnoaislado.Enabled = true;
            }else
            {
                txtnoaislado.Enabled = false;
            }
        }

        private void chknoaislado_CheckedChanged(object sender, EventArgs e)
        {
            chckText();
        }

        private void chkaislado_CheckedChanged(object sender, EventArgs e)
        {
            chckText();
        }

        private void ProcessFiles()
        {
            String path = this.txtArchivoFormato.Text;
            String fbase = this.txtArchivoBase.Text;
            String ext = fbase.Split(new char[] { '.' }).Last();
            bool valid = true;
            //Console.WriteLine(fbase);
            //Console.WriteLine(ext);
            BarraProgreso.status = 1;
            statusBar();
            #region process file if is extension for etabs
            if (ext.Equals("e2k"))
            {
                Console.WriteLine("Etabs Read");
                ReadFileEtabs etabs = new ReadFileEtabs(path);
                etabs.selectSheet("Hoja1");
                etabs.readETabs(this.chkaislado.Checked ? txtaislado.Text : "");
                //Thread.Sleep(3600);
                BarraProgreso.status = 2;
                statusBar();
                etabs.calcFormulas(Convert.ToDouble(txtDelta.Value), Convert.ToDouble(lblSismoVertical.Text), this.chknoaislado.Checked ? txtnoaislado.Text : "");
                //etabs.test();
                etabs.close();
                BarraProgreso.status = 3;
                statusBar();
                //Thread.Sleep(3600);
                // escritura de archivos
                WriteEtabs wa = new WriteEtabs(this.txtArchivoBase.Text);
                wa.destino = this.txtArchivoDestino.Text;
                Dictionary<string, bool> chksaislado = new Dictionary<string, bool>();
                for (int i = 0; i < etabsclbaislado.Items.Count; i++)
                {
                    chksaislado.Add(etabsclbaislado.Items[i].ToString(), etabsclbaislado.GetItemChecked(i));
                }
                wa.processe2kAislado(this.chkaislado.Checked ? txtaislado.Text : "Base", Convert.ToDouble(this.lblSismoVertical.Text), chksaislado);
                Dictionary<string, bool> chksnoaislado = new Dictionary<string, bool>();
                for (int i = 0; i < etabscblnosislado.Items.Count; i++)
                {
                    chksnoaislado.Add(etabscblnosislado.Items[i].ToString(), etabscblnosislado.GetItemChecked(i));
                }
                wa.processe2kNoAislado(this.chknoaislado.Checked ? this.txtnoaislado.Text : "story1", Convert.ToDouble(this.lblSismoVertical.Text), chksnoaislado);
            }
            #endregion
            #region Process if file is extension staad pro
            if (ext.Equals("std"))
            {
                if (checkClearSTD())
                {
                    if (MessageBox.Show(null, "Se va a eliminar el contenido de las etiquetas seleccionadas\r\nDesea Ud. continuar?", "ALERTA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        valid = writeSTD(path);
                    }
                    else
                    {
                        MessageBox.Show(this, "Se ha cancelado la operación", "INFORMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        valid = false;
                    }
                }
                else
                {
                    valid = writeSTD(path);
                }
            }
            #endregion
            // this.lblpasos.Text = "Completo!";
            #region reset progressbar and another controls
            
            if (valid)
            {
                BarraProgreso.status = 4;
                statusBar();
                MessageBox.Show(this, "Archivos creados correctamente!", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }else
            {
                BarraProgreso.status = 0;
                statusBar();
            }
            if (!BarraProgreso.tarea1.IsAlive)
            {
                BarraProgreso.tarea1.Abort();
            }
            BarraProgreso.tarea1.Join();
            #endregion
        }

        private void statusBar()
        {
            Console.WriteLine("ESTADO DE BAR PROGRESO {0}", BarraProgreso.status);
            switch (BarraProgreso.status)
            {
                case 0:
                #region  progressbar inital process
                    Invoke(new Action(() => this.lblpasos.Text = ""));
                    Invoke(new Action(() => this.lblcontador.Text = "0/4"));
                    Invoke(new Action(() => this.pgbarpasos.Style = ProgressBarStyle.Blocks));
                    Invoke(new Action(() => this.pgbarpasos.Value = 0));
                    Invoke(new Action(() => this.pgcontador.Value = 0));
                    Invoke(new Action(() => this.BtnProcesar.Enabled = true));
                    break;
                #endregion
                case 1:
                #region progressbar reader file
                    Invoke(new Action(() => this.lblpasos.Text = "Se inicio la lectura de archivo."));
                    Invoke(new Action(() => this.lblcontador.Text = "1/4"));
                    Invoke(new Action(() => this.pgbarpasos.Style = ProgressBarStyle.Marquee));
                    Invoke(new Action(() => this.pgbarpasos.MarqueeAnimationSpeed = 80));
                    Invoke(new Action(() => this.BtnProcesar.Enabled = false));
                    break;
                #endregion
                case 2:
                #region progressbar calculate of data reader
                    Invoke(new Action(() => this.lblpasos.Text = "Se inicio el calculo de datos."));
                    Invoke(new Action(() => this.lblcontador.Text = "3/4"));
                    Invoke(new Action(() => this.pgbarpasos.MarqueeAnimationSpeed = 30));
                    Invoke(new Action(() => this.pgcontador.Value = 33));
                    break;
                #endregion
                case 3:
                #region progressbar write date in the file
                    Invoke(new Action(() => lblpasos.Text = "Se inicio la escritura de archivos."));
                    Invoke(new Action(() => this.lblcontador.Text = "4/4"));
                    Invoke(new Action(() => this.pgbarpasos.MarqueeAnimationSpeed = 5));
                    Invoke(new Action(() => this.pgcontador.Value = 66));
                    break;
                #endregion
                case 4:
                #region progressbar status complete
                    Invoke(new Action(() => this.lblpasos.Text = "Completo!"));
                    Invoke(new Action(() => this.lblcontador.Text = "Completo!"));
                    Invoke(new Action(() => pgbarpasos.Style = ProgressBarStyle.Blocks));
                    Invoke(new Action(() => this.pgbarpasos.Value = 100));
                    Invoke(new Action(() => this.pgcontador.Value = 100));
                    Invoke(new Action(() => this.BtnProcesar.Enabled = true));
                    break;
                #endregion
                case 5:
                #region progressbar status vilidator
                    Invoke(new Action(() => this.lblpasos.Text = "Validando...!"));
                    Invoke(new Action(() => this.lblcontador.Text = "2/4"));
                    Invoke(new Action(() => pgbarpasos.Style = ProgressBarStyle.Blocks));
                    Invoke(new Action(() => this.pgbarpasos.MarqueeAnimationSpeed = 1));
                    Invoke(new Action(() => this.pgcontador.Value = 25));
                    Invoke(new Action(() => this.BtnProcesar.Enabled = true));
                    break;
                #endregion
            }
            Thread.Sleep(500);
        }

        private void txtDelta_ValueChanged(object sender, EventArgs e)
        {
            sismoVertical();
        }

        private void btnBaseNoAislado_Click(object sender, EventArgs e)
        {
            this.oFD = new OpenFileDialog();
            this.oFD.Filter = "Formato (*.std) | *.std";
            if (this.oFD.ShowDialog() == DialogResult.OK)
            {
                //this.txtBaseNoAislado.Text = this.oFD.FileName;
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void acercaTools_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void limpiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BarraProgreso.status = 0;
            statusBar();
            // this.btnBaseNoAislado.Enabled = false;
            this.txtArchivoBase.Text = "";
            // this.txtBaseNoAislado.Text = "";
            this.txtArchivoDestino.Text = "";
            this.txtArchivoFormato.Text = "";
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            statusReport();
            Thread t1 = new Thread(new ThreadStart(createRpt));
            BarraProgreso.tarea1 = t1;
            BarraProgreso.tarea1.IsBackground = true;
            BarraProgreso.tarea1.Start();
        }

        private void createRpt()
        {
            try
            {
                String fbase = this.txtArchivoBase.Text;
                String ext = fbase.Split(new char[] { '.' }).Last();
                Exportar nrpt = new Exportar(this.txtArchivoDestino.Text, ext);
                nrpt.LlenarDatos();
                statusReport(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void statusReport(int st = 0)
        {
            switch (st)
            {
                case 0:
                    Invoke(new Action(() => this.lblpasos.Text = "Creando Reporte"));
                    Invoke(new Action(() => this.lblcontador.Text = "1/1"));
                    Invoke(new Action(() => this.pgbarpasos.Style = ProgressBarStyle.Marquee));
                    Invoke(new Action(() => this.pgbarpasos.MarqueeAnimationSpeed = 10));
                    Invoke(new Action(() => this.BtnProcesar.Enabled = false));
                    Invoke(new Action(() => this.btnExportar.Enabled = false));
                    break;
                case 1:
                    Invoke(new Action(() => this.lblpasos.Text = "Completo!"));
                    Invoke(new Action(() => this.lblcontador.Text = "Completo!"));
                    Invoke(new Action(() => pgbarpasos.Style = ProgressBarStyle.Blocks));
                    Invoke(new Action(() => this.pgbarpasos.Value = 100));
                    Invoke(new Action(() => this.pgcontador.Value = 100));
                    Invoke(new Action(() => this.BtnProcesar.Enabled = true));
                    Invoke(new Action(() => this.btnExportar.Enabled = true));
                    MessageBox.Show(this, "Reporte creado Correctamente.", "Información!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        // verificar si el contenido del archivo se va a eliminar
        private Boolean checkClearSTD()
        {
            bool check = false;
            for (int i = 0; i < stdcblaislado.Items.Count; i++)
            {
                if (stdcblaislado.GetItemChecked(i))
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                for (int i = 0; i < this.stdcblnoaislado.Items.Count; i++)
                {
                    if (this.stdcblnoaislado.GetItemChecked(i))
                    {
                        check = true;
                        break;
                    }
                }
            }
            return check;
        }

        private Boolean writeSTD(String path)
        {
            bool status = false;
            Console.WriteLine("Staad PRO Read");
            ReadFileStaadPRO staad = new ReadFileStaadPRO();
            BarraProgreso.status = 2;
            statusBar();
            staad.participacion = Convert.ToDecimal(this.lblSismoVertical.Text);
            staad.delta = Convert.ToDecimal(this.txtDelta.Value);
            staad.path = path;
            staad.ReadStaadPro();
            if (!staad.error)
            {
                // staad.test();
                BarraProgreso.status = 3;
                statusBar();
                // Escribir Archivo
                WriteStaadPRO wsp = new WriteStaadPRO();
                wsp.path = this.txtArchivoBase.Text;
                wsp.destiny = this.txtArchivoDestino.Text;
                StringBuilder txt = wsp.readFile();
                wsp.initData();
                wsp.processAisladoSTD(txt);
                // NO AISLADO
                WriteStaadPRO wp = new WriteStaadPRO();
                wp.path = this.txtPathNotIsolated.Text;
                wp.destiny = this.txtArchivoDestino.Text;
                StringBuilder cad = wp.readFile();
                wp.initData();
                wp.processNoAisladoSTD(cad);
                status = true;
            }
            else
            {
                MessageBox.Show("Error unidad " + staad.raise, "ALERTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                status = !staad.error;
            }
            return status;
        }

        private void btnBaseNotIsolated_Click(object sender, EventArgs e)
        {
            this.oFD = new OpenFileDialog();
            this.oFD.Filter = "Importados (*.std) | *.std";
            if (this.oFD.ShowDialog() == DialogResult.OK)
            {
                this.txtPathNotIsolated.Text = this.oFD.FileName;
            }
        }
    }
}
