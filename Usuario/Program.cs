using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuario;
using Usuario.Clases;
using System.IO;

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

            // Definir la ruta del script SQL
            string rutaScript = Path.Combine(Application.StartupPath, "Scripts", "CreateDatabase.sql");

            // Crear conexión temporal solo para pruebas
            var conexion = new ClaseConexion("MARCELAPACHECO\\MSSQLSERVER05", "SISTEMASEMILLA");

            if (!conexion.VerificarServidor())
            {
                MessageBox.Show("No se pudo conectar al servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!conexion.VerificarBaseDatos())
            {
                if (!File.Exists(rutaScript))
                {
                    MessageBox.Show("No se encontró el script de creación de la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var respuesta = MessageBox.Show("La base de datos no existe. ¿Deseas crearla?", "No existe", MessageBoxButtons.YesNo);
                if (respuesta == DialogResult.Yes)
                {
                    try 
                    { 
                        conexion.CrearBaseDatosSiNoExiste(rutaScript);
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
            Application.Run(new FormInicio());
        }
    }
}
