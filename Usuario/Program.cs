using System;
using System.IO;
using System.Windows.Forms;

namespace Usuario
{
    internal static class UsuarioProgram
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string rutaScriptDir = BuscarDirectorioBasedeDatos();
            string rutaCompleta = Path.Combine(rutaScriptDir ?? Application.StartupPath, "SISTEMASEMILLA.sql");

            var conexion = new ClaseConexion();

            if (!conexion.VerificarServidor())
            {
                MessageBox.Show("No se pudo conectar al servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!conexion.VerificarBaseDatos())
            {
                if (rutaScriptDir == null || !Directory.Exists(rutaScriptDir))
                {
                    MessageBox.Show($"No se encontró el directorio de scripts.\nRutas buscadas (revisar que la carpeta 'BasedeDatos' esté incluida en la instalación):\n{ObtenerRutasInspeccionadas()}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private static string BuscarDirectorioBasedeDatos()
        {
            string inicio = Application.StartupPath;
            var inspeccionadas = new System.Text.StringBuilder();
            string current = inicio;
            for (int i = 0; i < 6 && !string.IsNullOrEmpty(current); i++)
            {
                string candidato = Path.Combine(current, "BasedeDatos");
                inspeccionadas.AppendLine(candidato);
                if (Directory.Exists(candidato))
                {
                    RutasInspeccionadas = inspeccionadas.ToString();
                    return candidato;
                }

                var parent = Directory.GetParent(current);
                current = parent?.FullName;
            }

            // También revisar ruta relativa común desde proyecto (dos niveles arriba)
            try
            {
                string posible = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "BasedeDatos"));
                inspeccionadas.AppendLine(posible);
                if (Directory.Exists(posible))
                {
                    RutasInspeccionadas = inspeccionadas.ToString();
                    return posible;
                }
            }
            catch { }

            RutasInspeccionadas = inspeccionadas.ToString();
            return null;
        }

        private static string RutasInspeccionadas = string.Empty;

        private static string ObtenerRutasInspeccionadas()
        {
            if (string.IsNullOrWhiteSpace(RutasInspeccionadas))
                return Application.StartupPath;
            return RutasInspeccionadas;
        }
    }
}
