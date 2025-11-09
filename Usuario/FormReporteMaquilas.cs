using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class FormReporteMaquilas : UserControl, IThemedReport
    {
        private readonly DashboardRepository repo;
        private DataTable datosActuales;
        private string nombreReporteActual = "MaquilasPendientes";

        public FormReporteMaquilas()
        {
            InitializeComponent();
            repo = new DashboardRepository(new ClaseConexion());

            // 🔹 Suscribirse al evento de cambio de tema global
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void FormReporteMaquilas_Load(object sender, EventArgs e)
        {
            CargarMaquilas();

            // 🔹 Aplica el tema actual del sistema al abrir
            ReporteHelper.AplicarTema(reportViewer1, ThemeManager.CurrentTheme);
            reportViewer1.RefreshReport();
        }

        private void CargarMaquilas()
        {
            try
            {
                datosActuales = repo.GetMaquilasPendientes();

                reportViewer1.LocalReport.DataSources.Clear();
                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", "ReporteMaquilasPendientes.rdlc");
                reportViewer1.LocalReport.ReportPath = ruta;

                ReportDataSource rds = new ReportDataSource("DataSetMaquilasPendientes", datosActuales);
                reportViewer1.LocalReport.DataSources.Add(rds);

                // 🔹 Aplica el tema actual también al RDLC
                ReporteHelper.AplicarTema(reportViewer1, ThemeManager.CurrentTheme);
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el reporte de maquilas: " + ex.Message);
            }
        }

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
                MessageBox.Show($"PDF exportado correctamente:\n{ruta}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar PDF: " + ex.Message);
            }
        }

        // 🔹 Se dispara automáticamente cuando cambia el tema global
        private void OnThemeChanged(Tema nuevoTema)
        {
            ApplyTheme(nuevoTema);
        }

        // 🔹 Implementación de la interfaz IThemedReport
        public void ApplyTheme(Tema tema)
        {
            try
            {
                // Solo aplica el parámetro si el reporte ya tiene una definición cargada
                if (reportViewer1.LocalReport != null &&
                    !string.IsNullOrEmpty(reportViewer1.LocalReport.ReportPath))
                {
                    ReporteHelper.AplicarTema(reportViewer1, tema);
                }

                // Colores para todos los demás controles (labels, botones, etc.)
                ReporteHelper.AplicarTemaControles(this, tema);

                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
            }
        }
    }
}

