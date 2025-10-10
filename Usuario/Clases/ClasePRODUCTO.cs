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
        private string _variedad;
        private string _descripcion;
        private decimal _cantidad;
        private decimal _precioUnitario;
        private decimal? _porcentajeGerminacion;
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

        public string Variedad
        {
            get => _variedad;
            set => _variedad = value;
        }

        public string Descripcion
        {
            get => _descripcion;
            set => _descripcion = value;
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

        public decimal? PorcentajeGerminacion { get; set; }

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
            _variedad = string.Empty;
            _descripcion = string.Empty;
            _cantidad = 0;
            _precioUnitario = 0;
            _porcentajeGerminacion = 0;
            _idProveedor = null;
            _activo = true;
        }

        public bool Guardar()
        {
            var campos = new Dictionary<string, object>
            {
                ["Categoria"] = _categoria,
                ["Nombre"] = _nombre,
                ["Variedad"] = string.IsNullOrEmpty(_variedad) ? (object)DBNull.Value : _variedad,
                ["Descripcion"] = string.IsNullOrEmpty(_descripcion) ? (object)DBNull.Value : _descripcion,
                ["Cantidad"] = _cantidad,
                ["PrecioUnitario"] = _precioUnitario,
                ["PorcentajeGerminacion"] = _categoria == "Semilla" ? (object)_porcentajeGerminacion : DBNull.Value,
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
                ["Variedad"] = string.IsNullOrEmpty(_variedad) ? (object)DBNull.Value : _variedad,
                ["Descripcion"] = string.IsNullOrEmpty(_descripcion) ? (object)DBNull.Value : _descripcion,
                ["Cantidad"] = _cantidad,
                ["PrecioUnitario"] = _precioUnitario,
                ["PorcentajeGerminacion"] = _categoria == "Semilla" ? (object)_porcentajeGerminacion : DBNull.Value,
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
                _variedad = row["Variedad"] == DBNull.Value ? "" : row["Variedad"].ToString();
                _descripcion = row["Descripcion"] == DBNull.Value ? "" : row["Descripcion"].ToString();
                _cantidad = Convert.ToDecimal(row["Cantidad"]);
                _precioUnitario = Convert.ToDecimal(row["PrecioUnitario"]);
                _porcentajeGerminacion = row["PorcentajeGerminacion"] == DBNull.Value ? 0 : Convert.ToDecimal(row["PorcentajeGerminacion"]);
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
                condiciones.Add("Categoria = '" + categoria + "'");

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
                condiciones.Add("Categoria = '" + categoria + "'");

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
            return !string.IsNullOrWhiteSpace(Nombre) && Cantidad >= 0 && PrecioUnitario >= 0;
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