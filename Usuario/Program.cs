using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuario;
using Usuario.Clases;

namespace Usuario
{
    internal static class UsuarioProgram
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Crear conexión temporal solo para pruebas
            var conexion = new ClaseConexion("MARCELAPACHECO\\MSSQLSERVER05", "SISTEMASEMILLA");

            if (!conexion.VerificarServidor())
            {
                MessageBox.Show("No se pudo conectar al servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!conexion.VerificarBaseDatos())
            {
                var respuesta = MessageBox.Show("La base de datos no existe. ¿Deseas crearla?", "No existe", MessageBoxButtons.YesNo);
                if (respuesta == DialogResult.Yes)
                {
                    try { conexion.CrearBaseDatosSiNoExiste(rutaScript); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); return; }
                }
                else return;
            }
            Application.Run(new FormInicio());
        }
    }
}
