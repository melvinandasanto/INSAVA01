using Microsoft.Reporting.WinForms;
using System.Drawing;
using System.Windows.Forms;

namespace Usuario.Clases
{
    public static class ReporteHelper
    {
        public static void AplicarTema(ReportViewer viewer, Tema tema)
        {
            // 🔹 Pasa el parámetro de tema al RDLC
            string themeValue = (tema == Temas.Dark) ? "Dark" : "Light";
            viewer.LocalReport.SetParameters(new ReportParameter("Theme", themeValue));

            // 🔹 Cambia apariencia del ReportViewer
            viewer.BackColor = tema.Fondo;
            viewer.ForeColor = tema.ForeColor;
        }

        // 🔹 Nuevo: aplicar el tema a los demás controles (botones, labels, etc.)
        public static void AplicarTemaControles(Control parent, Tema tema)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.BackColor = tema.Fondo;
                    lbl.ForeColor = tema.ForeColor;
                }
                else if (ctrl is Button btn)
                {
                    btn.BackColor = tema.BtnColor;
                    btn.ForeColor = tema.BtnForeColor;
                }
                else if (ctrl is Panel pnl)
                {
                    pnl.BackColor = tema.Fondo;
                }
                else if (ctrl is TextBox txt)
                {
                    txt.BackColor = tema.TxtBoxColor;
                    txt.ForeColor = tema.TxtBoxForeColor;
                }

                // 🔁 Llamada recursiva para aplicar a hijos anidados
                if (ctrl.HasChildren)
                    AplicarTemaControles(ctrl, tema);
            }
        }
    }
}


