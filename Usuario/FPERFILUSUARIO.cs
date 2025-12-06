using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuario.Clases;
using static Usuario.Login;

namespace Usuario
{
    public partial class FPERFILUSUARIO : Form
    {
        private bool mouseDentroDelFormulario = false;
        private readonly string nombreCompleto;
        private Tema temaActual;

        public FPERFILUSUARIO(string nombreCompleto, string correo, Tema tema)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.TopMost = true;
            this.Deactivate += FPerfilUsuario_Deactivate;

            this.nombreCompleto = nombreCompleto;
            this.temaActual = tema;

            ConfigurarInterfaz();
            AplicarTemaActual();

            this.MouseEnter += (s, e) => mouseDentroDelFormulario = true;
            this.MouseLeave += (s, e) => mouseDentroDelFormulario = false;
        }

        private void ConfigurarInterfaz()
        {
            this.Size = new Size(450, 230);
            this.Text = "Perfil de Usuario";

            // === Círculo con iniciales estilo botón de menú ===
            Panel panelIniciales = new Panel
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20)
            };
            panelIniciales.Paint += (s, pe) =>
            {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(102, 255, 153)))
                    pe.Graphics.FillEllipse(brush, 0, 0, panelIniciales.Width - 1, panelIniciales.Height - 1);

                using (Pen pen = new Pen(Color.White, 2))
                    pe.Graphics.DrawEllipse(pen, 1, 1, panelIniciales.Width - 3, panelIniciales.Height - 3);

                string iniciales = ObtenerIniciales(nombreCompleto);
                using (Font fuente = new Font("Segoe UI", 12, FontStyle.Bold))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    pe.Graphics.DrawString(iniciales, fuente, Brushes.White,
                        new RectangleF(0, 0, panelIniciales.Width, panelIniciales.Height), sf);
                }
            };

            // Nombre
            Label lblNombre = new Label
            {
                Text = $"Nombre: {nombreCompleto}",
                Location = new Point(100, 30),
                AutoSize = true
            };

            // Botón cerrar ventana
            Button btnCerrarVentana = new Button
            {
                Text = "Cerrar ventana",
                Size = new Size(130, 40),
                Location = new Point(80, 130)
            };
            btnCerrarVentana.Click += (s, e) => this.Close();

            // Botón cerrar sesión
            Button btnCerrarSesion = new Button
            {
                Text = "Cerrar sesión",
                Size = new Size(130, 40),
                Location = new Point(230, 130),
                BackColor = Color.LightCoral
            };
            btnCerrarSesion.Click += (s, e) =>
            {
                MessageBox.Show("Sesión cerrada correctamente.");

                var menu = Application.OpenForms.OfType<FMENU>().FirstOrDefault();
                if (menu != null)
                {
                    menu.motivoCierre = FMENU.MotivoCierre.CerrarSesion;
                    menu.Close();
                }

                this.Close();
            };

            this.Controls.Add(panelIniciales);
            this.Controls.Add(lblNombre);
            this.Controls.Add(btnCerrarVentana);
            this.Controls.Add(btnCerrarSesion);

            // === Borde dinámico según tema ===
            this.Paint += (s, pe) =>
            {
                Color colorMarco = (temaActual.Fondo == Color.White) ? Color.Black : Color.White;
                using (Pen pen = new Pen(colorMarco, 4))
                {
                    pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    pe.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };
        }

        private string ObtenerIniciales(string nombre)
        {
            var partes = nombre.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length >= 2)
                return partes[0][0].ToString().ToUpper() + partes[1][0].ToString().ToUpper();
            else if (partes.Length == 1 && partes[0].Length > 0)
                return partes[0][0].ToString().ToUpper();
            else
                return "?";
        }

        private void FPerfilUsuario_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void FPerfilUsuario_Deactivate(object sender, EventArgs e)
        {
            if (!mouseDentroDelFormulario)
                this.Close();
        }

        private void AplicarTemaActual()
        {
            DiseñoGlobal.AplicarTema(this, temaActual);
            DiseñoGlobal.AplicarFormatoBotones(this, temaActual);
        }

        public void CambiarTema(Tema nuevoTema)
        {
            this.temaActual = nuevoTema;
            AplicarTemaActual();
            this.Invalidate();  // repintar borde
        }

        public void AplicarTema(Color foreColor, Color backColor)
        {
            this.BackColor = backColor;
            foreach (Control ctrl in this.Controls)
            {
                ctrl.ForeColor = foreColor;
                ctrl.BackColor = backColor;
            }
        }
    }
}
