using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Usuario.Clases;
using System.Globalization;

namespace Usuario
{
    /// <summary>
    /// Representa un producto (semilla o normal) y proporciona utilidades para manipular productos.
    /// </summary>
    public class ClasePRODUCTO
    {
        private int _idProducto;
        private string _categoria; // "Semilla" o "Producto"
        private string _nombre;
        private decimal _cantidad;
        private decimal _precioUnitario;
        private decimal? _porcentajeGerminacion;
        private decimal? _precioMaquila;
        private int? _idProveedor;
        private bool _activo;

        private CRUD crud;

        public ClasePRODUCTO()
        {
            Reiniciar();
            var conexion = new ClaseConexion();
            crud = new CRUD("PRODUCTO", conexion);
        }

        public int IDProducto
        {
            get => _idProducto;
            set => _idProducto = value;
        }

        public string Categoria
        {
            get => _categoria;
            set => _categoria = value;
        }

        public string Nombre
        {
            get => _nombre;
            set => _nombre = value;
        }

        public decimal Cantidad
        {
            get => _cantidad;
            set => _cantidad = value;
        }

        public decimal PrecioUnitario
        {
            get => _precioUnitario;
            set => _precioUnitario = value;
        }

        public decimal? PorcentajeGerminacion
        {
            get => _porcentajeGerminacion;
            set => _porcentajeGerminacion = value;
        }

        public decimal? PrecioMaquila
        {
            get => _precioMaquila;
            set => _precioMaquila = value;
        }

        public int? IDProveedor
        {
            get => _idProveedor;
            set => _idProveedor = value;
        }

        public bool Activo
        {
            get => _activo;
            set => _activo = value;
        }

        public void Reiniciar()
        {
            _idProducto = 0;
            _categoria = "Producto";
            _nombre = string.Empty;
            _cantidad = 0;
            _precioUnitario = 0;
            _porcentajeGerminacion = null;
            _precioMaquila = null;
            _idProveedor = null;
            _activo = true;
        }

        public bool Guardar()
        {
            var campos = new Dictionary<string, object>
            {
                ["Categoria"] = _categoria,
                ["Nombre"] = _nombre,
                ["Cantidad"] = _cantidad,
                ["PrecioUnitario"] = _precioUnitario,
                ["PorcentajeGerminacion"] = (string.Equals(_categoria, "Semilla", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(_categoria, "Semilla Maquilada", StringComparison.OrdinalIgnoreCase))
                             ? (_porcentajeGerminacion.HasValue ? (object)_porcentajeGerminacion.Value : (object)DBNull.Value)
                             : (object)DBNull.Value,
                ["PrecioMaquila"] = (string.Equals(_categoria, "Semilla", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(_categoria, "Semilla Maquilada", StringComparison.OrdinalIgnoreCase))
                             ? (_precioMaquila.HasValue ? (object)_precioMaquila.Value : (object)DBNull.Value)
                             : (object)DBNull.Value,
                ["IDProveedor"] = _idProveedor.HasValue ? (object)_idProveedor.Value : DBNull.Value,
                ["Activo"] = _activo ? 1 : 0
            };

            return crud.Insertar(campos);
        }

        public bool Editar()
        {
            if (_idProducto == 0)
                throw new InvalidOperationException("Debe especificar el ID del producto para editar.");

            var campos = new Dictionary<string, object>
            {
                ["Categoria"] = _categoria,
                ["Nombre"] = _nombre,
                ["Cantidad"] = _cantidad,
                ["PrecioUnitario"] = _precioUnitario,
                ["PorcentajeGerminacion"] = (string.Equals(_categoria, "Semilla", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(_categoria, "Semilla Maquilada", StringComparison.OrdinalIgnoreCase))
                             ? (_porcentajeGerminacion.HasValue ? (object)_porcentajeGerminacion.Value : (object)DBNull.Value)
                             : (object)DBNull.Value,
                ["PrecioMaquila"] = (string.Equals(_categoria, "Semilla", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(_categoria, "Semilla Maquilada", StringComparison.OrdinalIgnoreCase))
                             ? (_precioMaquila.HasValue ? (object)_precioMaquila.Value : (object)DBNull.Value)
                             : (object)DBNull.Value,
                ["IDProveedor"] = _idProveedor.HasValue ? (object)_idProveedor.Value : DBNull.Value,
                ["Activo"] = _activo ? 1 : 0
            };

            return crud.Editar("IDProducto", _idProducto, campos);
        }

        public bool Eliminar()
        {
            if (_idProducto == 0)
                throw new InvalidOperationException("Debe especificar el ID del producto para eliminar.");

            var campos = new Dictionary<string, object>
            {
                ["Activo"] = 0
            };

            return crud.Editar("IDProducto", _idProducto, campos);
        }

        public bool BuscarPorId(int id)
        {
            var row = crud.BuscarRegistroPorId("IDProducto", id);
            if (row != null)
            {
                _idProducto = Convert.ToInt32(row["IDProducto"]);
                _categoria = row["Categoria"].ToString();
                _nombre = row["Nombre"].ToString();
                _cantidad = Convert.ToDecimal(row["Cantidad"]);
                _precioUnitario = Convert.ToDecimal(row["PrecioUnitario"]);
                _porcentajeGerminacion = row["PorcentajeGerminacion"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["PorcentajeGerminacion"]);
                _precioMaquila = row.Table.Columns.Contains("PrecioMaquila") && row["PrecioMaquila"] != DBNull.Value
                                 ? (decimal?)Convert.ToDecimal(row["PrecioMaquila"])
                                 : (decimal?)null;
                _idProveedor = row["IDProveedor"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["IDProveedor"]);
                _activo = Convert.ToBoolean(row["Activo"]);
                return true;
            }
            return false;
        }

        public void CargarProductos(DataGridView grid, string categoria = null, bool soloActivos = true)
        {
            var conexion = new ClaseConexion();
            string consulta = "SELECT * FROM PRODUCTO";
            List<string> condiciones = new List<string>();

            if (soloActivos)
                condiciones.Add("Activo = 1");
            if (!string.IsNullOrEmpty(categoria))
                condiciones.Add("Categoria = @categoria");

            if (condiciones.Count > 0)
                consulta += " WHERE " + string.Join(" AND ", condiciones);

            var dt = conexion.Tabla(consulta);
            grid.DataSource = dt;

            if (grid.Columns.Contains("IDProducto"))
                grid.Columns["IDProducto"].HeaderText = "ID Producto";
        }

        public DataTable ObtenerProductos(string categoria = null, bool soloActivos = true)
        {
            var conexion = new ClaseConexion();
            string consulta = "SELECT * FROM PRODUCTO";
            List<string> condiciones = new List<string>();

            if (soloActivos)
                condiciones.Add("Activo = 1");
            if (!string.IsNullOrEmpty(categoria))
                condiciones.Add("Categoria = @categoria");

            if (condiciones.Count > 0)
                consulta += " WHERE " + string.Join(" AND ", condiciones);

            return conexion.Tabla(consulta);
        }

        /// <summary>
        /// Crea una copia superficial del producto.
        /// </summary>
        public ClasePRODUCTO Clonar()
        {
            return (ClasePRODUCTO)this.MemberwiseClone();
        }

        /// <summary>
        /// Compara este producto con otro por ID.
        /// </summary>
        public bool EsIgual(ClasePRODUCTO otro)
        {
            if (otro == null) return false;
            return this.IDProducto == otro.IDProducto;
        }

        /// <summary>
        /// Valida si el producto tiene los datos mínimos requeridos.
        /// </summary>
        public bool EsValido()
        {
            bool baseValido = !string.IsNullOrWhiteSpace(Nombre) && Cantidad >= 0 && PrecioUnitario >= 0;
            bool requiereGerm = string.Equals(Categoria, "Semilla", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(Categoria, "Semilla Maquilada", StringComparison.OrdinalIgnoreCase);
            if (requiereGerm)
            {
                if (!PorcentajeGerminacion.HasValue) return false;
                if (!PrecioMaquila.HasValue) return false;
            }
            return baseValido;
        }

        /// <summary>
        /// Obtiene una lista de productos filtrados por nombre.
        /// </summary>
        public static List<ClasePRODUCTO> FiltrarPorNombre(List<ClasePRODUCTO> productos, string nombre)
        {
            return productos?.FindAll(p => p.Nombre?.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0) ?? new List<ClasePRODUCTO>();
        }
    }
}