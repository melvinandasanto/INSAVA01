using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class Login : Form
    {
        private Tema temaActual = Temas.Light;
        private Image imagenOriginalLogo;
        private List<PictureBox> imagensimple = new List<PictureBox>();
        private Dictionary<PictureBox, Image> imagenesOriginales = new Dictionary<PictureBox, Image>();
        private ClasePin pinLogin = new ClasePin(4);

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            imagensimple = new List<PictureBox> { pbUsuario, pbContra };
            txtContrasena.PasswordChar = '*';
            DiseñoGlobal.AplicarTema(this, temaActual);
            DiseñoGlobal.AplicarFormatoBotones(this, temaActual);
            DiseñoGlobal.AplicarTintePictureBoxLista(imagensimple, temaActual.ForeColor, imagenesOriginales);
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            ClaseValidacion.ValidarCampoNumerico(e);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string numeroIdentidad = txtUsuario.Text.Trim();
            string clave = txtContrasena.Text.Trim();

            // Usar el servicio de autenticación que registra en la bitácora
            var loginService = new ClaseLogin();
            ClaseUSUARIO usuarioDatos;
            bool esValido = loginService.ValidarUsuario(numeroIdentidad, clave, out usuarioDatos);

            if (esValido && usuarioDatos != null)
            {
                // Guardar datos de sesión (usa los datos devueltos por ClaseLogin)
                SesionUsuario.IDUsuario = usuarioDatos.IDUsuario;
                SesionUsuario.NombreCompleto = $"{usuarioDatos.PrimerNombre} {usuarioDatos.PrimerApellido}";

                var rol = new ClaseROL();
                if (rol.BuscarPorId(usuarioDatos.IDRol))
                {
                    SesionUsuario.RolNombre = rol.NombreRol;
                    MessageBox.Show($"Bienvenido, {usuarioDatos.PrimerNombre}! Rol: {rol.NombreRol}");
                }
                else
                {
                    SesionUsuario.RolNombre = string.Empty;
                    MessageBox.Show($"Bienvenido, {usuarioDatos.PrimerNombre}! (Rol no encontrado)");
                }

                // Abrir el menú principal
                var menu = new FMENU();
                menu.Show();
                this.Hide();
            }
            else
            {
                // Intento fallido: ClaseLogin ya registró en la bitácora.
                // Mantener la lógica de bloqueo por PIN/contador existente.
                pinLogin.ValidarPinLogin(clave, this, temaActual);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public static SqlConnection ObtenerConexion()
        {
            string cadena = "Data Source=DESKTOP-VPTL2O8\\SQLEXPRESS;Initial Catalog=SISTEMASEMILLA;Integrated Security=True;Encrypt=False";
            SqlConnection conexion = new SqlConnection(cadena);
            try
            {
                conexion.Open();
                return conexion;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar a la base de datos: " + ex.Message);
                return null;
            }
        }

        private void lblCambiarTema_Click(object sender, EventArgs e)
        {
            temaActual = (temaActual == Temas.Light) ? Temas.Dark : Temas.Light;
            DiseñoGlobal.AplicarTema(this, temaActual);
            DiseñoGlobal.AplicarFormatoBotones(this, temaActual);
            DiseñoGlobal.AplicarTintePictureBoxLista(imagensimple, temaActual.ForeColor, imagenesOriginales);
        }

        private void AbrirOlvidoContraseña()
        {
            FOLVIDOCONTRA recuperarcontra = new FOLVIDOCONTRA(temaActual);
            recuperarcontra.FormClosed += (s, e) => this.Show();
            this.Hide();
            recuperarcontra.Show();
        }

        private void lblolvidocontra_Click(object sender, EventArgs e)
        {
            AbrirOlvidoContraseña();
        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
