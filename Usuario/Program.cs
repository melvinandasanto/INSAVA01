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

            // Ruta al script de la base de datos
            string rutaScript = @"C:\Users\melvi\OneDrive\Documentos\Desktop\INSAVA01MELVINCLAROS\BasedeDatos";

            // Crear conexión temporal solo para pruebas
            var conexion = new ClaseConexion();

            if (!conexion.VerificarServidor())
            {
                MessageBox.Show("No se pudo conectar al servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!conexion.VerificarBaseDatos())
            {
                // Verificar que el script existe
                if (!Directory.Exists(rutaScript))
                {
                    MessageBox.Show("No se encontró el directorio con los scripts", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string rutaCompleta = Path.Combine(rutaScript, "SISTEMASEMILLA.sql");

                if (!File.Exists(rutaCompleta))
                {
                    MessageBox.Show("No se encontró el archivo de script SQL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
