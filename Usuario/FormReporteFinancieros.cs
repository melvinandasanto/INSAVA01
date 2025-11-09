using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class FormReporteFinancieros : UserControl, IThemedReport
    {
        private readonly DashboardRepository repo;
        private DataTable datosActuales;
        private string nombreReporteActual = "EstadosFinancieros";

        public FormReporteFinancieros()
        {
            InitializeComponent();
            repo = new DashboardRepository(new ClaseConexion());

            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void FormReporteFinancieros_Load(object sender, EventArgs e)
        {
            ReporteHelper.AplicarTemaControles(this, ThemeManager.CurrentTheme);
        }

        // Botón Generar Reporte
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime desde = dtDesde.Value.Date;
                DateTime hasta = dtHasta.Value.Date;

                datosActuales = repo.GetEstadosFinancierosPorRango(desde, hasta);

                reportViewer1.LocalReport.DataSources.Clear();

                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", "ReporteEstadosFinancieros.rdlc");
                if (!File.Exists(ruta))
                {
                    MessageBox.Show($"No se encontró el reporte en:\n{ruta}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                reportViewer1.LocalReport.ReportPath = ruta;

                ReportDataSource rds = new ReportDataSource("DataSetEstadosFinancieros", datosActuales);
                reportViewer1.LocalReport.DataSources.Add(rds);

                ReporteHelper.AplicarTema(reportViewer1, ThemeManager.CurrentTheme);

                reportViewer1.RefreshReport();

                //  Aplica también el tema a los controles del formulario
                ReporteHelper.AplicarTemaControles(this, ThemeManager.CurrentTheme);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el reporte financiero: " + ex.Message);
            }
        }

        //  Exportar PDF
        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (datosActuales == null || datosActuales.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.");
                    return;
                }

                string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportesPDF");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string nombreArchivo = $"{nombreReporteActual}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string ruta = Path.Combine(carpeta, nombreArchivo);

                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;

                byte[] bytes = reportViewer1.LocalReport.Render(
                    "PDF", null, out mimeType, out encoding, out filenameExtension,
                    out streamids, out warnings
                );

                File.WriteAllBytes(ruta, bytes);
                MessageBox.Show($"PDF exportado con éxito:\n{ruta}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar PDF: " + ex.Message);
            }
        }

        // Evento global de cambio de tema
        private void OnThemeChanged(Tema nuevoTema)
        {
            ApplyTheme(nuevoTema);
        }

        // Implementación de IThemedReport
        public void ApplyTheme(Tema tema)
        {
            try
            {
                // Solo aplica el tema si ya hay un RDLC cargado
                if (reportViewer1.LocalReport != null &&
                    !string.IsNullOrEmpty(reportViewer1.LocalReport.ReportPath))
                {
                    ReporteHelper.AplicarTema(reportViewer1, tema);
                    reportViewer1.RefreshReport();
                }

                // Fondo y texto de botones, labels, etc.
                ReporteHelper.AplicarTemaControles(this, tema);
            }
            catch
            {
                // Evita error si aún no hay RDLC cargado
            }
        }
    }
}


