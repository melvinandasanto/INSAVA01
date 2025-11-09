using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class FormReportesIngresos : UserControl, IThemedReport
    {
        private readonly DashboardRepository repo;
        private DataTable datosActuales;
        public FormReportesIngresos()
        {
            InitializeComponent();
            repo = new DashboardRepository(new ClaseConexion());
        }

        private void FormReportesIngresos_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime desde = DateTime.Now.AddMonths(-1);
                DateTime hasta = DateTime.Now;

                datosActuales = repo.GetIngresosPorRango(desde, hasta);

                reportViewer1.LocalReport.DataSources.Clear();
                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reportes", "ReporteIngresos.rdlc");
                reportViewer1.LocalReport.ReportPath = ruta;

                ReportDataSource rds = new ReportDataSource("DataSetIngresos", datosActuales);
                reportViewer1.LocalReport.DataSources.Add(rds);

                string themeValue = (ThemeManager.CurrentTheme == Temas.Dark) ? "Dark" : "Light";
                ReportParameter pTheme = new ReportParameter("Theme", themeValue);
                reportViewer1.LocalReport.SetParameters(new ReportParameter[] { pTheme });

                ReporteHelper.AplicarTema(reportViewer1, ThemeManager.CurrentTheme);
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el reporte: " + ex.Message);
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

                string nombreArchivo = $"ReporteIngresos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
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
