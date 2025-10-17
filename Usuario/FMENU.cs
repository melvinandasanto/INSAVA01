using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class FMENU : Form
    {
        public enum MotivoCierre { Ninguno, CerrarSesion, CerrarApp } // <-- public
        public MotivoCierre motivoCierre = MotivoCierre.Ninguno;

        public Tema temaActual = Temas.Light;
        private bool cerrandoApp = false;

        public string UsuarioActual { get; set; } // <-- Propiedad agregada

        // Declaración de los UserControls
        private UCUSUARIO ucusuario = null;
        private UCVENTA ucFVENTA = null;
        private UCCLIENTE uccliente = null;
        private UCBUSCADOR ucBuscadorClientes = null;
        private Usuario.UCProducto ucproducto = null;

        // Campo para guardar la referencia al perfil abierto
        private FPERFILUSUARIO perfilAbierto = null;

        public FMENU()
        {
            InitializeComponent();
        }

        private void FMENU_Load(object sender, EventArgs e)
        {
            DiseñoGlobal.AplicarFormatoBotones(this, temaActual);
            DiseñoGlobal.AplicarTema(this, temaActual);
            DiseñoGlobal.AplicarTinteToolStripButtons(this, temaActual.ForeColor);

            WindowState = FormWindowState.Maximized;

            // Aplica permisos del menú
            Permisos.AplicarPermisosMenu(this);

            AgregarBotonPerfilUsuario();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DiseñoGlobal.RegistrarFormulario(this);
        }

        private void tsbtnCambiarTema_Click(object sender, EventArgs e)
        {
            temaActual = (temaActual == Temas.Light) ? Temas.Dark : Temas.Light;
            DiseñoGlobal.AplicarTema(this, temaActual);
            DiseñoGlobal.AplicarTinteToolStripButtons(this, temaActual.ForeColor);
            DiseñoGlobal.AplicarFormatoBotones(this, temaActual);

            // Cambia el tema del perfil si está abierto
            if (perfilAbierto != null && !perfilAbierto.IsDisposed)
                perfilAbierto.CambiarTema(temaActual);

            foreach (Control ctrl in panelContenedor.Controls)
            {
                DiseñoGlobal.AplicarTema(ctrl, temaActual);
                foreach (Control child in ctrl.Controls)
                {
                    if (child is DataGridView dgvChild)
                        DiseñoGlobal.AplicarEstiloDataGridView(dgvChild, temaActual);
                }
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            motivoCierre = MotivoCierre.CerrarApp;
            this.Close();
        }

        private void Minimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Maximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = (this.WindowState == FormWindowState.Normal)
                ? FormWindowState.Maximized
                : FormWindowState.Normal;
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private void toolStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        public void CerrarSesion()
        {
            motivoCierre = MotivoCierre.CerrarSesion;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (cerrandoApp)
                return;

            // --- Caso: cerrar sesión ---
            if (motivoCierre == MotivoCierre.CerrarSesion)
            {
                // Abrir directamente el login sin preguntar
                new Login().Show();
                motivoCierre = MotivoCierre.Ninguno;
                return; // No sigue al flujo de confirmar salida
            }

            // --- Caso: cerrar aplicación desde la X ---
            if (motivoCierre == MotivoCierre.CerrarApp)
            {
                var result = MessageBox.Show(
                    "¿Desea cerrar la aplicación completamente?",
                    "Confirmar salida",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    cerrandoApp = true; // Marca que ya se está cerrando
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true; // Cancelar cierre, no abrir login
                }
                motivoCierre = MotivoCierre.Ninguno;
                return;
            }

            // --- Cierre por otros motivos (ej: Alt+F4 directo) ---
            var confirmacion = MessageBox.Show(
                "¿Desea cerrar la aplicación completamente?",
                "Confirmar salida",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                cerrandoApp = true;
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }


        // === BOTÓN PERFIL DE USUARIO (GLOBO) ===
        private void AgregarBotonPerfilUsuario()
        {
            // Evitar duplicados
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item.Tag != null && item.Tag.ToString() == "BotonPerfil")
                    return;
            }

            string iniciales = ObtenerIniciales(SesionUsuario.NombreCompleto);

            PictureBox pbPerfil = new PictureBox
            {
                Size = new Size(40, 40),
                Margin = new Padding(4),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            Bitmap bmp = new Bitmap(40, 40);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                Brush fondo = new SolidBrush(Color.LightGreen);
                g.FillEllipse(fondo, 0, 0, 39, 39);
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(iniciales, new Font("Segoe UI", 12, FontStyle.Bold),
                             Brushes.White, new RectangleF(0, 0, 40, 40), sf);
            }

            pbPerfil.Image = bmp;

            pbPerfil.Click += (s, e) =>
            {
                if (perfilAbierto == null || perfilAbierto.IsDisposed)
                {
                    perfilAbierto = new FPERFILUSUARIO(SesionUsuario.NombreCompleto, SesionUsuario.Correo, temaActual); // <-- Agrega temaActual
                    perfilAbierto.StartPosition = FormStartPosition.Manual;
                    perfilAbierto.Location = ObtenerPosicionDebajoDelPerfil(pbPerfil);
                    perfilAbierto.Show();
                }
                else
                {
                    perfilAbierto.BringToFront();
                }
            };

            ToolStripControlHost host = new ToolStripControlHost(pbPerfil)
            {
                Alignment = ToolStripItemAlignment.Right,
                Tag = "BotonPerfil"
            };

            toolStrip1.Items.Add(host);
        }

        private Point ObtenerPosicionDebajoDelPerfil(Control control)
        {
            Point punto = control.PointToScreen(Point.Empty);

            // Tamaño estimado del formulario de perfil
            int anchoFormulario = 450;
            int altoFormulario = 230;

            // Posición inicial (debajo del botón)
            int x = punto.X + control.Width - anchoFormulario;
            int y = punto.Y + control.Height;

            // Asegurar que no se salga a la izquierda
            if (x < 0) x = 0;
            // Asegurar que no se salga por la derecha
            if (x + anchoFormulario > Screen.PrimaryScreen.WorkingArea.Width)
                x = Screen.PrimaryScreen.WorkingArea.Width - anchoFormulario;

            // Asegurar que no se salga por abajo
            if (y + altoFormulario > Screen.PrimaryScreen.WorkingArea.Height)
                y = punto.Y - altoFormulario; // abre hacia arriba si no hay espacio abajo

            return new Point(x, y);
        }


        private string ObtenerIniciales(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "?";
            var partes = nombre.Split(' ');
            return (partes.Length >= 2)
                ? partes[0][0].ToString().ToUpper() + partes[1][0].ToString().ToUpper()
                : nombre[0].ToString().ToUpper();
        }

        // === MÉTODOS PARA QUE EL DISEÑADOR NO FALLE ===
        private void LlamaUsuarios_Click(object sender, EventArgs e)
        {
            if (ucusuario != null && panelContenedor.Controls.Contains(ucusuario))
            {
                panelContenedor.Controls.Remove(ucusuario);
                ucusuario.Dispose();
                ucusuario = null;
            }
            else
            {
                AbrirUserControlExclusivo(ref ucusuario, panelContenedor);
            }
        }

        private void LlamaVentas_Click(object sender, EventArgs e)
        {
            if (ucFVENTA != null && panelContenedor.Controls.Contains(ucFVENTA))
            {
                panelContenedor.Controls.Remove(ucFVENTA);
                ucFVENTA.Dispose();
                ucFVENTA = null;
            }
            else
            {
                AbrirUserControlExclusivo(ref ucFVENTA, panelContenedor);
            }
        }

        private void LlamaClientes_Click(object sender, EventArgs e)
        {
            if (uccliente != null && panelContenedor.Controls.Contains(uccliente))
            {
                panelContenedor.Controls.Remove(uccliente);
                uccliente.Dispose();
                uccliente = null;
            }
            else
            {
                AbrirUserControlExclusivo(ref uccliente, panelContenedor);
            }
        }

        private void LlamaInventario_Click(object sender, EventArgs e)
        {
            if (ucproducto != null && panelContenedor.Controls.Contains(ucproducto))
            {
                panelContenedor.Controls.Remove(ucproducto);
                ucproducto.Dispose();
                ucproducto = null;
            }
            else
            {
                AbrirUserControlExclusivo(ref ucproducto, panelContenedor);
            }
        }

      

        private void panelContenedor_Paint(object sender, PaintEventArgs e)
        {
            // evento vacío
        }

        // Método genérico para abrir un UserControl exclusivo
        private void AbrirUserControlExclusivo<T>(ref T instanciaControl, Panel contenedor) where T : UserControl, new()
        {
            // Cierra y elimina todos los controles existentes en el panel
            foreach (Control ctrl in contenedor.Controls.OfType<UserControl>().ToList())
            {
                contenedor.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
            instanciaControl = new T();
            DiseñoGlobal.AplicarTema(instanciaControl, temaActual);
            instanciaControl.Dock = DockStyle.Fill;
            contenedor.Controls.Add(instanciaControl);

            // Aplica el tema a todos los DataGridView hijos del nuevo UserControl
            foreach (Control child in instanciaControl.Controls)
            {
                if (child is DataGridView dgv)
                    DiseñoGlobal.AplicarEstiloDataGridView(dgv, temaActual);
            }
        }

        private void btnFacturas_Click(object sender, EventArgs e)
        {
            var formBuscarFacturas = new FBuscarFactura();
            formBuscarFacturas.ShowDialog();
        }

        private void LlamaBuscador_Click(object sender, EventArgs e)
        {
            // Si el UserControl ya está abierto, lo cerramos
            if (ucBuscadorClientes != null && panelContenedor.Controls.Contains(ucBuscadorClientes))
            {
                panelContenedor.Controls.Remove(ucBuscadorClientes);
                ucBuscadorClientes.Dispose();
                ucBuscadorClientes = null;
            }
            else
            {
                // Si no está abierto, lo abrimos
                AbrirUserControlExclusivo<UCBUSCADOR>(ref ucBuscadorClientes, panelContenedor);
                ucBuscadorClientes.IniciarModoClientes(); // inicializa en modo Clientes
            }
        }


    }
}
