using System;
using System.Data;
using System.Data.SqlClient;

namespace Usuario.Clases
{
    public class ClaseMaquila
    {
        private readonly ClaseConexion conexion;

        public ClaseMaquila()
        {
            conexion = new ClaseConexion();
        }

        // Registrar nueva maquila
        public bool RegistrarMaquila(int idTransaccion, string origenSemilla, int? idProducto, 
            int cantidadMaquilada, decimal precioPorUnidad)
        {
            try
            {
                string sql = @"INSERT INTO MAQUILA_SEMILLA 
                    (IDTransaccion, OrigenSemilla, IDProducto, CantidadMaquilada, 
                     PrecioPorUnidad, FechaInicio, FechaEntrega, Estado)
                VALUES 
                    (@IDTransaccion, @OrigenSemilla, @IDProducto, @CantidadMaquilada,
                     @PrecioPorUnidad, GETDATE(), DATEADD(DAY, 30, GETDATE()), 1)";

                SqlParameter[] parametros = {
                    new SqlParameter("@IDTransaccion", idTransaccion),
                    new SqlParameter("@OrigenSemilla", origenSemilla),
                    new SqlParameter("@IDProducto", (object)idProducto ?? DBNull.Value),
                    new SqlParameter("@CantidadMaquilada", cantidadMaquilada),
                    new SqlParameter("@PrecioPorUnidad", precioPorUnidad)
                };

                return conexion.EjecutarSQL(sql, parametros);
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error al registrar maquila: {ex.Message}");
                return false;
            }
        }

        // Actualizar estados de maquilas vencidas
        public bool ActualizarEstadosMaquila()
        {
            try
            {
                string sql = @"UPDATE MAQUILA_SEMILLA 
                             SET Estado = 0 
                             WHERE FechaEntrega < GETDATE() 
                             AND Estado = 1";

                return conexion.EjecutarSQL(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar estados: {ex.Message}");
                return false;
            }
        }

        // Obtener maquilas activas
        public DataTable ObtenerMaquilasActivas()
        {
            string sql = @"SELECT 
                            MS.*, 
                            P.Nombre AS NombreProducto,
                            DATEDIFF(DAY, GETDATE(), MS.FechaEntrega) as DiasRestantes
                          FROM MAQUILA_SEMILLA MS
                          LEFT JOIN PRODUCTO P ON MS.IDProducto = P.IDProducto
                          WHERE MS.Estado = 1
                          ORDER BY MS.FechaEntrega";

            return conexion.Tabla(sql);
        }
    }
}