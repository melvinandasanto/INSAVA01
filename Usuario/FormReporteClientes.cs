using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class FormReporteClientes : UserControl, IThemedReport
    {
        private readonly DashboardRepository repo;
        private DataTable datosActuales;
        private string nombreReporteActual = "Clientes";

        public FormReporteClientes()
        {
            InitializeComponent();
            repo = new DashboardRepository(new ClaseConexion());

            // 🔹 Escuchar el cambio global de tema
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void FormReporteClientes_Load(object sender, EventArgs e)
        {
            CargarClientes();
        }

        private void CargarClientes()
        {
            try
            {
                datosActuales = repo.GetClientes();

                reportViewer1.LocalReport.DataSources.Clear();
                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", "ReporteClientes.rdlc");
                reportViewer1.LocalReport.ReportPath = ruta;

                ReportDataSource rds = new ReportDataSource("DataSetClientes", datosActuales);
                reportViewer1.LocalReport.DataSources.Add(rds);

                // 🔹 Aplica el tema actual (color + parámetro RDLC)
                ReporteHelper.AplicarTema(reportViewer1, ThemeManager.CurrentTheme);

                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el reporte de clientes: " + ex.Message);
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
                MessageBox.Show($"PDF exportado con éxito:\n{ruta}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar PDF: " + ex.Message);
            }
        }

        // 🔹 Este método se ejecuta automáticamente cuando cambia el tema
        private void OnThemeChanged(Tema nuevoTema)
        {
            ApplyTheme(nuevoTema);
        }

        // 🔹 Implementación requerida por IThemedReport
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
