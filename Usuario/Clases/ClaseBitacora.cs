using System;
using System.Data;
using System.Data.SqlClient;

namespace Usuario.Clases
{
    /// <summary>
    /// Clase para manejar operaciones con la tabla bitacora_accesos.
    /// Almacena en la columna 'usuario' el número de identidad del usuario que intenta ingresar.
    /// </summary>
    public class ClaseBitacora
    {
        private ClaseConexion _conexion;

        public ClaseBitacora()
        {
            _conexion = new ClaseConexion();
        }

        public DataTable ObtenerTodosLosRegistros()
        {
            string sql = "SELECT id, usuario, fecha_hora, status_usuario, intento_exitoso FROM bitacora_accesos ORDER BY fecha_hora DESC";
            return _conexion.Tabla(sql);
        }

        public DataTable ObtenerPorUsuario(string texto)
        {
            string sql = "SELECT id, usuario, fecha_hora, status_usuario, intento_exitoso FROM bitacora_accesos WHERE usuario LIKE @usuario ORDER BY fecha_hora DESC";
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@usuario", "%" + texto + "%")
            };
            return _conexion.Tabla(sql, parametros);
        }

        public DataTable ObtenerPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            string sql = "SELECT id, usuario, fecha_hora, status_usuario, intento_exitoso FROM bitacora_accesos WHERE fecha_hora >= @fechaInicio AND fecha_hora < DATEADD(DAY, 1, @fechaFin) ORDER BY fecha_hora DESC";
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@fechaInicio", fechaInicio),
                new SqlParameter("@fechaFin", fechaFin)
            };
            return _conexion.Tabla(sql, parametros);
        }

        public DataTable ObtenerPorStatus(string statusUsuario)
        {
            string sql = "SELECT id, usuario, fecha_hora, status_usuario, intento_exitoso FROM bitacora_accesos WHERE status_usuario = @status ORDER BY fecha_hora DESC";
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@status", statusUsuario)
            };
            return _conexion.Tabla(sql, parametros);
        }

        /// <summary>
        /// Registra un nuevo acceso en la bitácora.
        /// Ahora 'usuario' almacena el número de identidad que intentó ingresar.
        /// </summary>
        public bool RegistrarAcceso(string numeroIdentidad, string statusUsuario, bool intentoExitoso)
        {
            string sql = "INSERT INTO bitacora_accesos (usuario, fecha_hora, status_usuario, intento_exitoso) VALUES (@usuario, GETDATE(), @status, @intentoExitoso)";
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@usuario", numeroIdentidad ?? string.Empty),
                new SqlParameter("@status", statusUsuario ?? string.Empty),
                new SqlParameter("@intentoExitoso", SqlDbType.Bit) { Value = intentoExitoso }
            };

            int filas = _conexion.EjecutarSQLConFilas(sql, parametros);
            return filas > 0;
        }

        public DataTable ObtenerIntentosFallidos()
        {
            string sql = "SELECT id, usuario, fecha_hora, status_usuario, intento_exitoso FROM bitacora_accesos WHERE intento_exitoso = 0 ORDER BY fecha_hora DESC";
            return _conexion.Tabla(sql);
        }

        public DataTable ObtenerUltimosAccesosPorUsuario(string nombreUsuario, int cantidad = 10)
        {
            string sql = $"SELECT TOP {cantidad} id, usuario, fecha_hora, status_usuario, intento_exitoso FROM bitacora_accesos WHERE usuario = @usuario ORDER BY fecha_hora DESC";
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@usuario", nombreUsuario)
            };
            return _conexion.Tabla(sql, parametros);
        }

        public DataTable ObtenerUltimoAcceso(string nombreUsuario)
        {
            string sql = "SELECT TOP 1 id, usuario, fecha_hora, status_usuario, intento_exitoso FROM bitacora_accesos WHERE usuario = @usuario ORDER BY fecha_hora DESC";
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@usuario", nombreUsuario)
            };
            return _conexion.Tabla(sql, parametros);
        }

        public int EliminarRegistrosAntiguos(int diasAntiguos)
        {
            string sql = "DELETE FROM bitacora_accesos WHERE fecha_hora < DATEADD(DAY, -@dias, CAST(GETDATE() AS DATE))";
            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@dias", diasAntiguos)
            };
            return _conexion.EjecutarSQLConFilas(sql, parametros);
        }

        public DataTable ObtenerEstadisticasPorUsuario()
        {
            string sql = @"
                SELECT usuario, 
                       COUNT(*) as total_accesos, 
                       SUM(CASE WHEN intento_exitoso = 1 THEN 1 ELSE 0 END) as accesos_exitosos
                FROM bitacora_accesos
                GROUP BY usuario
                ORDER BY total_accesos DESC";
            return _conexion.Tabla(sql);
        }
    }
}