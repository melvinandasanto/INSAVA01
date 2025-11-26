using System;
using System.IO;
using System.Windows.Forms;

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

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string rutaScriptDir = Path.Combine(userProfile, "source", "repos", "INSAVA01", "BasedeDatos");
            string rutaCompleta = Path.Combine(rutaScriptDir, "SISTEMASEMILLA.sql");

            var conexion = new ClaseConexion(); // usa "." o .\SQLEXPRESS según tu instalación

            if (!conexion.VerificarServidor())
            {
                MessageBox.Show("No se pudo conectar al servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!conexion.VerificarBaseDatos())
            {
                if (!Directory.Exists(rutaScriptDir))
                {
                    MessageBox.Show($"No se encontró el directorio de scripts:\n{rutaScriptDir}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!File.Exists(rutaCompleta))
                {
                    MessageBox.Show($"No se encontró el archivo SQL:\n{rutaCompleta}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var respuesta = MessageBox.Show("La base de datos no existe. ¿Deseas crearla?", "No existe", MessageBoxButtons.YesNo);
                if (respuesta == DialogResult.Yes)
                {
                    try
                    {
                        conexion.CrearBaseDatosSiNoExiste(rutaCompleta);
                        MessageBox.Show("Base de datos creada exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al crear la base de datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else return;
            }

            Application.Run(new Login());

        }
    }
}
