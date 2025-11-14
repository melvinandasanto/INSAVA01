using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Usuario.Clases
{
    /// <summary>
    /// Maneja la autenticación de usuarios y registra cada intento en la bitácora.
    /// Ahora registra el número de identidad y el estado (Activo) del usuario.
    /// </summary>
    public class ClaseLogin
    {
        private ClaseConexion _conexion;
        private ClaseBitacora _bitacora;

        public ClaseLogin()
        {
            _conexion = new ClaseConexion();
            _bitacora = new ClaseBitacora();
        }

        /// <summary>
        /// Valida las credenciales del usuario y registra el intento en la bitácora.
        /// </summary>
        public bool ValidarUsuario(string numeroIdentidad, string clave, out ClaseUSUARIO usuarioDatos)
        {
            usuarioDatos = null;
            string statusUsuario = "INACTIVO";

            try
            {
                string sql = @"
                    SELECT IDUsuario, NumeroIdentidad, PrimerNombre, SegundoNombre, PrimerApellido, 
                           SegundoApellido, Clave, IDRol, Activo 
                    FROM USUARIO 
                    WHERE NumeroIdentidad = @numeroIdentidad";

                SqlParameter[] parametros = new SqlParameter[]
                {
                    new SqlParameter("@numeroIdentidad", numeroIdentidad)
                };

                DataTable dt = _conexion.Tabla(sql, parametros);

                if (dt == null || dt.Rows.Count == 0)
                {
                    // Usuario no existe -> registrar intento con identidad proporcionada (fallback)
                    _bitacora.RegistrarAcceso(numeroIdentidad, statusUsuario, false);
                    return false;
                }

                DataRow row = dt.Rows[0];
                bool activo = Convert.ToBoolean(row["Activo"]);
                string claveAlmacenada = row["Clave"].ToString();

                statusUsuario = activo ? "ACTIVO" : "INACTIVO";

                // contraseña incorrecta
                if (claveAlmacenada != clave)
                {
                    _bitacora.RegistrarAcceso(numeroIdentidad, statusUsuario, false);
                    return false;
                }

                // usuario inactivo -> no permitir acceso
                if (!activo)
                {
                    _bitacora.RegistrarAcceso(numeroIdentidad, statusUsuario, false);
                    return false;
                }

                // login exitoso
                usuarioDatos = new ClaseUSUARIO
                {
                    IDUsuario = Convert.ToInt32(row["IDUsuario"]),
                    NumeroIdentidad = row["NumeroIdentidad"].ToString(),
                    PrimerNombre = row["PrimerNombre"].ToString(),
                    SegundoNombre = row["SegundoNombre"].ToString(),
                    PrimerApellido = row["PrimerApellido"].ToString(),
                    SegundoApellido = row["SegundoApellido"].ToString(),
                    Clave = claveAlmacenada,
                    IDRol = Convert.ToInt32(row["IDRol"]),
                    Activo = activo
                };

                _bitacora.RegistrarAcceso(numeroIdentidad, statusUsuario, true);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en validación de usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _bitacora.RegistrarAcceso(numeroIdentidad, "INACTIVO", false);
                return false;
            }
        }

        public ClaseUSUARIO ObtenerUsuarioPorIdentidad(string numeroIdentidad)
        {
            try
            {
                string sql = @"
                    SELECT IDUsuario, NumeroIdentidad, PrimerNombre, SegundoNombre, PrimerApellido, 
                           SegundoApellido, Clave, IDRol, Activo 
                    FROM USUARIO 
                    WHERE NumeroIdentidad = @numeroIdentidad";

                SqlParameter[] parametros = new SqlParameter[]
                {
                    new SqlParameter("@numeroIdentidad", numeroIdentidad)
                };

                DataTable dt = _conexion.Tabla(sql, parametros);

                if (dt == null || dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];
                return new ClaseUSUARIO
                {
                    IDUsuario = Convert.ToInt32(row["IDUsuario"]),
                    NumeroIdentidad = row["NumeroIdentidad"].ToString(),
                    PrimerNombre = row["PrimerNombre"].ToString(),
                    SegundoNombre = row["SegundoNombre"].ToString(),
                    PrimerApellido = row["PrimerApellido"].ToString(),
                    SegundoApellido = row["SegundoApellido"].ToString(),
                    Clave = row["Clave"].ToString(),
                    IDRol = Convert.ToInt32(row["IDRol"]),
                    Activo = Convert.ToBoolean(row["Activo"])
                };
            }
            catch
            {
                return null;
            }
        }
    }
}