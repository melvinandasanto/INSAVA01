using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    /// <summary>
    /// Proporciona métodos para la conexión y operaciones con la base de datos SQL Server.
    /// Incluye utilidades para ejecutar consultas, procedimientos y manejar transacciones.
    /// </summary>
    public class ClaseConexion
    {
        internal SqlConnection _conexion; 
        private string _host;
        private string _nombreDB;

        /// <summary>
        /// Inicializa una nueva instancia de ClaseConexion.
        /// </summary>
        public ClaseConexion(string host = "MARCELAPACHECO\\MSSQLSERVER05", string nombreDB = "SISTEMASEMILLA")
        {
            _host = host;
            _nombreDB = nombreDB;
            _conexion = new SqlConnection($"Server={_host};Database={_nombreDB};Integrated Security=True;Encrypt=False;");
        }


        /// <summary>
        /// Abre la conexión a la base de datos.
        /// </summary>
        public void Conectar()
        {
            try
            {
                _conexion.ConnectionString = $"Server={_host};Database={_nombreDB};Integrated Security=True;Encrypt=False;";
                _conexion.Open();
            }
            catch (Exception er)
            {
                MessageBox.Show("Error al conectar a la base de datos: " + er.Message);
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL sin parámetros.
        /// </summary>
        public bool EjecutarSQL(string sql)
        {
            try
            {
                Conectar();
                SqlCommand comando = _conexion.CreateCommand();
                comando.CommandText = sql;
                comando.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                    _conexion.Close();
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL con parámetros.
        /// </summary>
        public bool EjecutarSQL(string sql, SqlParameter[] parametros)
        {
            try
            {
                Conectar();
                using (SqlCommand comando = _conexion.CreateCommand())
                {
                    comando.CommandText = sql;
                    if (parametros != null)
                        comando.Parameters.AddRange(parametros);
                    comando.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                    _conexion.Close();
            }
        }

        /// <summary>
        /// Obtiene una tabla de resultados de una consulta SQL sin parámetros.
        /// </summary>
        public DataTable Tabla(string sql)
        {
            DataTable _t = new DataTable();
            try
            {
                if (_conexion.State != ConnectionState.Open)
                    Conectar();

                using (SqlCommand cmd = _conexion.CreateCommand())
                {
                    cmd.CommandText = sql;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(_t);
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show("Error al consultar la base de datos: " + e.Message);
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                    _conexion.Close();
            }
            return _t;
        }

        /// <summary>
        /// Obtiene una tabla de resultados de una consulta SQL con parámetros.
        /// </summary>
        public DataTable Tabla(string sql, SqlParameter[] parametros)
        {
            DataTable _t = new DataTable();
            try
            {
                if (_conexion.State != ConnectionState.Open)
                    Conectar();

                using (SqlCommand cmd = _conexion.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(_t);
                    }
                }
            }
            catch (SqlException e)
            {
                MessageBox.Show("Error al consultar la base de datos: " + e.Message);
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                    _conexion.Close();
            }
            return _t;
        }

        /// <summary>
        /// Ejecuta una consulta SQL y retorna el número de filas afectadas.
        /// </summary>
        public int EjecutarSQLConFilas(string sql, SqlParameter[] parametros)
        {
            try
            {
                Conectar();

                using (SqlCommand comando = _conexion.CreateCommand())
                {
                    comando.CommandText = sql;
                    if (parametros != null)
                        comando.Parameters.AddRange(parametros);

                    int filas = comando.ExecuteNonQuery();
                    return filas;
                }
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                    _conexion.Close();
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado y retorna un DataTable.
        /// </summary>
        public DataTable EjecutarProcedimiento(string nombreProcedimiento, SqlParameter[] parametros = null)
        {
            DataTable dt = new DataTable();
            try
            {
                Conectar();
                using (SqlCommand cmd = new SqlCommand(nombreProcedimiento, _conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar procedimiento: " + ex.Message);
            }
            finally
            {
                if (_conexion.State == ConnectionState.Open)
                    _conexion.Close();
            }
            return dt;
        }

        /// <summary>
        /// Inicia una transacción SQL.
        /// </summary>
        public SqlTransaction IniciarTransaccion()
        {
            if (_conexion.State != ConnectionState.Open)
                Conectar();
            return _conexion.BeginTransaction();
        }

        public bool VerificarServidor()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection($"Server={_host};Integrated Security=True;Encrypt=False;"))
                {
                    cn.Open();
                    return true;
                }
            }
            catch
            {
                MessageBox.Show(" No se pudo conectar al servidor SQL. El programa se cerrará.",
                                "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return false;
            }
        }

        public bool VerificarBaseDatos()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = _host,
                    IntegratedSecurity = true,
                    Encrypt = false,
                    ConnectTimeout = 5,
                    InitialCatalog = "master"
                };

                using (var cn = new SqlConnection(builder.ConnectionString))
                using (var cmd = cn.CreateCommand())
                {
                    cn.Open();
                    cmd.CommandText = "SELECT database_id FROM sys.databases WHERE name = @name";
                    cmd.Parameters.AddWithValue("@name", _nombreDB);
                    object result = cmd.ExecuteScalar();
                    return result != null;
                }
            }
            catch
            {
                throw; // Dejar que la capa de UI decida qué mostrar o hacer
            }
        }

        // Crear base desde script manejando GO
        public void CrearBaseDatosSiNoExiste(string rutaScript)
        {
            string script = File.ReadAllText(rutaScript);
            // Separar por líneas que contengan solo GO (ignorar mayúsculas/minúsculas)
            var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = _host,
                IntegratedSecurity = true,
                Encrypt = false,
                ConnectTimeout = 30,
                InitialCatalog = "master"
            };

            using (var cn = new SqlConnection(builder.ConnectionString))
            {
                cn.Open();
                foreach (var batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch)) continue;
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = batch;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}