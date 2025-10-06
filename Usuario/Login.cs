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

            var usuario = new ClaseUSUARIO();
            if (usuario.Autenticar(numeroIdentidad, clave) && usuario.Activo)
            {
                // Guardar datos de sesión
                SesionUsuario.IDUsuario = usuario.IDUsuario;
                SesionUsuario.NombreCompleto = $"{usuario.PrimerNombre} {usuario.PrimerApellido}";

                var rol = new ClaseROL();
                if (rol.BuscarPorId(usuario.IDRol))
                {
                    MessageBox.Show($"Bienvenido, {usuario.PrimerNombre}! Rol: {rol.NombreRol}");
                    SesionUsuario.RolNombre = rol.NombreRol; // <--- Agrega esto
                }
                else
                {
                    MessageBox.Show($"Bienvenido, {usuario.PrimerNombre}! (Rol no encontrado)");
                    SesionUsuario.RolNombre = ""; // <--- Para evitar null
                }

                // Abrir el menú principal
                var menu = new FMENU();
                menu.Show();
                this.Hide();
            }
            else
            {
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
