using PdfiumViewer;
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

namespace Usuario
{
    public partial class UCMANUAL : UserControl
    {
        public UCMANUAL()
        {
            InitializeComponent();
            MostrarManual();
        }

        private void UCMANUAL_Load(object sender, EventArgs e)
        {

        }

        public void MostrarManual()
        {
            panel1.Controls.Clear();

            // Buscar el directorio ManualDeUsuario
            string rutaManualDir = BuscarDirectorioManual();

            if (rutaManualDir == null)
            {
                // No mostrar error silenciosamente si no encuentra el manual
                return;
            }

            string rutaPDF = Path.Combine(rutaManualDir, "ManualDeUsuario.pdf");

            // Verificar existencia del PDF
            if (!File.Exists(rutaPDF))
                return;

            // Crear visor PDF
            var pdfViewer = new PdfViewer
            {
                Dock = DockStyle.Fill
            };

            pdfViewer.Document = PdfDocument.Load(rutaPDF);

            panel1.Controls.Add(pdfViewer);
        }

        private static string BuscarDirectorioManual()
        {
            string inicio = Application.StartupPath;
            string current = inicio;

            // Buscar hacia arriba en la estructura de directorios (hasta 6 niveles)
            for (int i = 0; i < 6 && !string.IsNullOrEmpty(current); i++)
            {
                string candidato = Path.Combine(current, "ManualDeUsuario");
                if (Directory.Exists(candidato))
                {
                    return candidato;
                }

                var parent = Directory.GetParent(current);
                current = parent?.FullName;
            }

            // También revisar ruta relativa común desde proyecto (dos niveles arriba)
            try
            {
                string posible = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "ManualDeUsuario"));
                if (Directory.Exists(posible))
                {
                    return posible;
                }
            }
            catch { }

            return null;
        }
    }
}
