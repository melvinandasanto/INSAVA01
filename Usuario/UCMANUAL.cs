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

            // Obtener el perfil del usuario actual
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Construir ruta completa al PDF
            string rutaPDF = Path.Combine(userProfile, "source", "repos", "INSAVA01", "ManualDeUsuario", "ManualDeUsuario.pdf");

            // Verificar existencia silenciosamente
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


    }
}
