using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuario.Clases
{
    public class DashboardRepository
    {
        private readonly ClaseConexion _conexion;

        public DashboardRepository(ClaseConexion conexion)
        {
            _conexion = conexion ?? throw new ArgumentNullException(nameof(conexion));
        }

        private decimal GetDecimalScalar(string sql, SqlParameter[] parametros = null)
        {
            var dt = _conexion.Tabla(sql, parametros);
            if (dt.Rows.Count == 0) return 0m;
            var val = dt.Rows[0][0];
            if (val == DBNull.Value) return 0m;
            return Convert.ToDecimal(val);
        }

        private int GetIntScalar(string sql, SqlParameter[] parametros = null)
        {
            var dt = _conexion.Tabla(sql, parametros);
            if (dt.Rows.Count == 0) return 0;
            var val = dt.Rows[0][0];
            if (val == DBNull.Value) return 0;
            return Convert.ToInt32(val);
        }

        public decimal GetTotalVentas()
        {
            string sql = "SELECT ISNULL(SUM(TotalVenta),0) FROM VENTA_PRODUCTO WHERE Activo = 1";
            return GetDecimalScalar(sql);
        }

        public decimal GetTotalMaquila()
        {
            string sql = "SELECT ISNULL(SUM(CantidadMaquilada * PrecioPorUnidad),0) FROM MAQUILA_SEMILLA WHERE Activo = 1";
            return GetDecimalScalar(sql);
        }

        public int GetClientesActivos()
        {
            string sql = "SELECT COUNT(*) FROM CLIENTE WHERE Activo = 1";
            return GetIntScalar(sql);
        }

        public int GetProductosConStock()
        {
            string sql = "SELECT COUNT(*) FROM PRODUCTO WHERE Activo = 1 AND Cantidad > 0";
            return GetIntScalar(sql);
        }

        public DataTable GetProductosBajoStock(decimal umbral = 10m)
        {
            string sql = "SELECT IDProducto, Nombre, Cantidad FROM PRODUCTO WHERE Cantidad < @umbral AND Activo = 1";
            var p = new SqlParameter[] { new SqlParameter("@umbral", umbral) };
            return _conexion.Tabla(sql, p);
        }

        // --- Gráficos ---
        public DataTable GetVentasPorMes(int anio = 0) 
        {
            string sql = @"
            SELECT MONTH(T.FechaEntrada) AS MesNum, DATENAME(MONTH, T.FechaEntrada) AS Mes, SUM(VP.TotalVenta) AS Total
            FROM VENTA_PRODUCTO VP
            INNER JOIN TRANSACCION T ON T.IDTransaccion = VP.IDTransaccion
            {0}
            GROUP BY MONTH(T.FechaEntrada), DATENAME(MONTH, T.FechaEntrada)
            ORDER BY MesNum";
            string where = "";
            if (anio > 0)
                where = "WHERE YEAR(T.FechaEntrada) = " + anio;
            sql = string.Format(sql, where);
            return _conexion.Tabla(sql);
        }

        public DataTable GetEgresosPorMes(int anio = 0)
        {
            string sql = @"
            SELECT MONTH(M.FechaMovimiento) AS MesNum, DATENAME(MONTH, M.FechaMovimiento) AS Mes, SUM(M.CantidadMovida) AS TotalEgresos
            FROM MOVIMIENTO_PRODUCTO M
            INNER JOIN TIPO_MOVIMIENTO TM ON M.IDTipoMovimiento = TM.IDTipoMovimiento
            WHERE TM.NombreMovimiento = 'Egreso'
            {0}
            GROUP BY MONTH(M.FechaMovimiento), DATENAME(MONTH, M.FechaMovimiento)
            ORDER BY MesNum";
            string where = "";
            if (anio > 0)
                where = "AND YEAR(M.FechaMovimiento) = " + anio;
            sql = string.Format(sql, where);
            return _conexion.Tabla(sql);
        }

        public DataTable GetUltimasTransacciones(int top = 5)
        {
            string sql = $@"
            SELECT TOP({top}) T.IDTransaccion, T.IDCliente, CONCAT(C.PrimerNombre, ' ', C.PrimerApellido) AS Cliente,
                   T.FechaEntrada, T.FechaSalida, TT.NombreTipo
            FROM TRANSACCION T
            LEFT JOIN CLIENTE C ON C.IDCliente = T.IDCliente
            LEFT JOIN TIPO_TRANSACCION TT ON TT.IDTipoTransaccion = T.IDTipoTransaccion
            ORDER BY T.FechaEntrada DESC";
            return _conexion.Tabla(sql);
        }

        public DataTable GetMaquilasPendientes()
        {
            string sql = "SELECT IDMaquila, IDTransaccion, IDProducto, CantidadMaquilada, FechaInicio, FechaEntrega FROM MAQUILA_SEMILLA WHERE Activo = 1 AND FechaEntrega >= CAST(GETDATE() AS DATE)";
            return _conexion.Tabla(sql);
        }

        public DataTable GetIngresosPorRango(DateTime desde, DateTime hasta)
        {
            string sql = @"
            SELECT VP.IDVentaProducto, VP.IDTransaccion, VP.IDProducto, P.Nombre AS Producto, VP.CantidadVendida, VP.TotalVenta, T.FechaEntrada, C.PrimerNombre, C.PrimerApellido
            FROM VENTA_PRODUCTO VP
            INNER JOIN TRANSACCION T ON T.IDTransaccion = VP.IDTransaccion
            LEFT JOIN PRODUCTO P ON P.IDProducto = VP.IDProducto
            LEFT JOIN CLIENTE C ON C.IDCliente = T.IDCliente
            WHERE T.FechaEntrada BETWEEN @desde AND @hasta
            ORDER BY T.FechaEntrada";
            var p = new SqlParameter[] {
            new SqlParameter("@desde", desde),
            new SqlParameter("@hasta", hasta)
        };
            return _conexion.Tabla(sql, p);
        }

        // Reporte: Egresos por rango de fechas (usa MOVIMIENTO_PRODUCTO con tipo 'Egreso')
        public DataTable GetEgresosPorRango(DateTime desde, DateTime hasta)
        {
            string sql = @"
        SELECT M.IDMovimiento, P.Nombre AS Producto, TM.NombreMovimiento, M.CantidadMovida, M.Descripcion, M.FechaMovimiento
        FROM MOVIMIENTO_PRODUCTO M
        LEFT JOIN PRODUCTO P ON P.IDProducto = M.IDProducto
        LEFT JOIN TIPO_MOVIMIENTO TM ON TM.IDTipoMovimiento = M.IDTipoMovimiento
        WHERE TM.NombreMovimiento = 'Egreso' AND M.FechaMovimiento BETWEEN @desde AND @hasta
        ORDER BY M.FechaMovimiento";
            var p = new SqlParameter[] {
        new SqlParameter("@desde", desde),
        new SqlParameter("@hasta", hasta)
    };
            return _conexion.Tabla(sql, p);
        }

        // Reporte: Inventario actual (productos)
        public DataTable GetInventario()
        {
            string sql = "SELECT IDProducto, Categoria, Nombre, Cantidad, PrecioUnitario, Activo FROM PRODUCTO ORDER BY Nombre";
            return _conexion.Tabla(sql);
        }

        // Reporte: Clientes
        public DataTable GetClientes()
        {
            string sql = "SELECT IDCliente, NumeroIdentidad, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, NumTel, Activo FROM CLIENTE ORDER BY PrimerNombre, PrimerApellido";
            return _conexion.Tabla(sql);
        }

        // Reporte: Estados financieros por rango (Ingresos totales, Egresos totales y Balance)
        public DataTable GetEstadosFinancierosPorRango(DateTime desde, DateTime hasta)
        {
            string sql = @"
        -- Ingresos totales
        SELECT 'Ingresos' AS Tipo, ISNULL(SUM(VP.TotalVenta),0) AS Monto
        FROM VENTA_PRODUCTO VP
        INNER JOIN TRANSACCION T ON T.IDTransaccion = VP.IDTransaccion
        WHERE T.FechaEntrada BETWEEN @desde AND @hasta
        UNION ALL
        -- Egresos totales (movimientos tipo Egreso)
        SELECT 'Egresos' AS Tipo, ISNULL(SUM(M.CantidadMovida),0) AS Monto
        FROM MOVIMIENTO_PRODUCTO M
        INNER JOIN TIPO_MOVIMIENTO TM ON TM.IDTipoMovimiento = M.IDTipoMovimiento
        WHERE TM.NombreMovimiento = 'Egreso' AND M.FechaMovimiento BETWEEN @desde AND @hasta";
            var p = new SqlParameter[] {
        new SqlParameter("@desde", desde),
        new SqlParameter("@hasta", hasta)
    };
            return _conexion.Tabla(sql, p);
        }

    }
}
