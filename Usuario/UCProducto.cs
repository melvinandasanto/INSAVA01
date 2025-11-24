using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Usuario.Clases;
using System.Globalization;

namespace Usuario
{
    public partial class UCProducto : UserControl
    {
        private ClasePRODUCTO producto;
        private Dictionary<string, object> valoresOriginales;
        private List<TextBox> CamposDecimales;

        public UCProducto()
        {
            InitializeComponent();
            producto = new ClasePRODUCTO();
            this.Load += UCProducto_Load;
            // Diseño global
            DiseñoGlobal.RegistrarUserControl(this);
        }

        private void UCProducto_Load(object sender, EventArgs e)
        {
            txtGerminacion.Enabled = false;
            txtPrecioMaquila.Enabled = false;

            // Cargar listas y grid usando el filtro por defecto
            CargarProveedores();
            CargarCategorias();
            CargarComboFiltro();
            // Usar el filtro actual para llenar el grid
            CargarProductosFiltro(cboFiltroActivo.SelectedItem?.ToString() ?? "Activos");

            // Asegurar que el combo de productos esté poblado
            CBProducto.DataSource = producto.ObtenerProductos();
            CBProducto.DisplayMember = "Nombre";
            CBProducto.ValueMember = "IDProducto";
            CBProducto.SelectedIndex = -1;

            // Eventos para detectar cambios en los campos
            CBCategoria.SelectedIndexChanged += ControlModificado;
            CBProducto.TextChanged += ControlModificado;
            NUPCantidad.ValueChanged += ControlModificado;
            txtPrecioUnitario.TextChanged += ControlModificado;
            txtGerminacion.TextChanged += ControlModificado;
            txtPrecioMaquila.TextChanged += ControlModificado;
            CBProveedor.SelectedIndexChanged += ControlModificado;
            checkactivo.CheckedChanged += ControlModificado;

            EstadoInicial();

            CamposDecimales = new List<TextBox>
            {
                txtPrecioUnitario,
                txtGerminacion,
                txtPrecioMaquila
            };
            CamposDecimales.ForEach(campo => campo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoDecimal(ev, campo));

            // Manejador para habilitar campos dependientes de categoría
            CBCategoria.SelectedIndexChanged += CBCategoria_SelectedIndexChanged;
        }

        private void CargarProductos()
        {
            producto.CargarProductos(DGProducto);
            CBProducto.DataSource = producto.ObtenerProductos();
            CBProducto.DisplayMember = "Nombre";
            CBProducto.ValueMember = "IDProducto";
            CBProducto.SelectedIndex = -1;
        }

        private void CargarProveedores()
        {
            var proveedor = new ClasePROVEEDOR();
            CBProveedor.DataSource = proveedor.ObtenerProveedores();
            CBProveedor.DisplayMember = "NombreProveedor";
            CBProveedor.ValueMember = "IDProveedor";
            CBProveedor.SelectedIndex = -1;
        }

        private void CargarCategorias()
        {
            CBCategoria.Items.Clear();
            CBCategoria.Items.Add("Semilla");
            CBCategoria.Items.Add("Semilla Maquilada");
            CBCategoria.Items.Add("Producto");
            CBCategoria.SelectedIndex = -1;
        }

        private void CargarComboFiltro()
        {
            cboFiltroActivo.Items.Clear();
            cboFiltroActivo.Items.Add("Activos");
            cboFiltroActivo.Items.Add("Inactivos");
            cboFiltroActivo.Items.Add("Todos");
            cboFiltroActivo.SelectedIndex = 0;
            cboFiltroActivo.SelectedIndexChanged += (s, e) =>
            {
                CargarProductosFiltro(cboFiltroActivo.SelectedItem.ToString());
            };
        }

        private void CargarProductosFiltro(string filtro)
        {
            DataTable datos = ClaseFiltroActivo.FiltrarTabla("PRODUCTO", filtro);
            DGProducto.DataSource = datos;

            if (DGProducto.Columns.Contains("IDProducto"))
                DGProducto.Columns["IDProducto"].HeaderText = "ID Producto";
            if (DGProducto.Columns.Contains("Nombre"))
                DGProducto.Columns["Nombre"].HeaderText = "Nombre Producto";
            if (DGProducto.Columns.Contains("Categoria"))
                DGProducto.Columns["Categoria"].HeaderText = "Categoría";

            DGProducto.ClearSelection();
            LimpiarCampos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            producto.Categoria = CBCategoria.Text?.Trim();
            producto.Nombre = CBProducto.Text?.Trim();
            producto.Cantidad = NUPCantidad.Value;

            string textoPrecio = txtPrecioUnitario.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(textoPrecio, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal precio))
            {
                MessageBox.Show("El precio unitario debe ser un número válido (entero o decimal).");
                return;
            }
            producto.PrecioUnitario = Math.Round(precio, 2);

            bool esSemilla = string.Equals(producto.Categoria, "Semilla", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(producto.Categoria, "Semilla Maquilada", StringComparison.OrdinalIgnoreCase);

            if (esSemilla)
            {
                string textoGerminacion = txtGerminacion.Text?.Trim();
                if (string.IsNullOrWhiteSpace(textoGerminacion))
                {
                    MessageBox.Show("Debe ingresar el porcentaje de germinación para productos de la categoría Semilla.");
                    return;
                }
                textoGerminacion = textoGerminacion.Replace(',', '.');
                if (!decimal.TryParse(textoGerminacion, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal germ))
                {
                    MessageBox.Show("La germinación debe ser un número válido (ejemplo: 0.85 o 85).");
                    return;
                }
                if (germ > 1m) germ = germ / 100m;
                germ = Math.Round(germ, 2);
                if (germ < 0m || germ > 1m)
                {
                    MessageBox.Show("La germinación debe estar entre 0 y 1 (por ejemplo 0.85 para 85%).");
                    return;
                }
                producto.PorcentajeGerminacion = germ;

                string textoPrecioMaquila = txtPrecioMaquila.Text?.Trim();
                textoPrecioMaquila = textoPrecioMaquila?.Replace(',', '.');
                if (string.IsNullOrWhiteSpace(textoPrecioMaquila) ||
                    !decimal.TryParse(textoPrecioMaquila, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal pmq))
                {
                    MessageBox.Show("Debe ingresar el precio de maquila para productos de la categoría Semilla.");
                    return;
                }
                producto.PrecioMaquila = Math.Round(pmq, 2);
            }
            else
            {
                producto.PorcentajeGerminacion = null;
                producto.PrecioMaquila = null;
            }

            object proveedorValue = CBProveedor.SelectedValue;
            int? idProveedor = null;
            if (proveedorValue == null || proveedorValue == DBNull.Value)
            {
                idProveedor = null;
            }
            else if (proveedorValue is int intVal)
            {
                idProveedor = intVal;
            }
            else if (proveedorValue is long longVal)
            {
                idProveedor = (int)longVal;
            }
            else if (proveedorValue is string s && int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
            {
                idProveedor = parsed;
            }
            else if (proveedorValue is DataRowView drv && drv.Row.Table.Columns.Contains("IDProveedor"))
            {
                object rowVal = drv.Row["IDProveedor"];
                if (rowVal != DBNull.Value && int.TryParse(rowVal.ToString(), out int rv)) idProveedor = rv;
            }
            producto.IDProveedor = idProveedor;

            producto.Activo = checkactivo.Checked;

            try
            {
                bool ok = producto.Guardar();
                if (ok)
                {
                    MessageBox.Show("Producto guardado correctamente.");
                    CargarProductosFiltro(cboFiltroActivo.SelectedItem?.ToString() ?? "Activos");
                    EstadoInicial();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al guardar el producto. Revisa los datos y restricciones de la base de datos.");
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? "\n" + ex.InnerException.Message : string.Empty;
                MessageBox.Show("Error técnico al guardar el producto: " + ex.Message + inner);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("ID de producto inválido.");
                return;
            }

            if (!int.TryParse(txtId.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int idProd))
            {
                MessageBox.Show("ID de producto inválido.");
                return;
            }
            producto.IDProducto = idProd;
            producto.Categoria = CBCategoria.Text?.Trim();
            producto.Nombre = CBProducto.Text?.Trim();
            producto.Cantidad = NUPCantidad.Value;

            string textoPrecio = txtPrecioUnitario.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(textoPrecio, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal precio))
            {
                MessageBox.Show("El precio unitario debe ser un número válido (entero o decimal).");
                return;
            }
            producto.PrecioUnitario = Math.Round(precio, 2);

            bool esSemilla = string.Equals(producto.Categoria, "Semilla", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(producto.Categoria, "Semilla Maquilada", StringComparison.OrdinalIgnoreCase);

            if (esSemilla)
            {
                string textoGerminacion = txtGerminacion.Text?.Trim();
                if (string.IsNullOrWhiteSpace(textoGerminacion))
                {
                    MessageBox.Show("Debe ingresar el porcentaje de germinación para productos de la categoría Semilla.");
                    return;
                }
                textoGerminacion = textoGerminacion.Replace(',', '.');
                if (!decimal.TryParse(textoGerminacion, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal germ))
                {
                    MessageBox.Show("La germinación debe ser un número válido (ejemplo: 0.85 o 85).");
                    return;
                }
                if (germ > 1m) germ = germ / 100m;
                germ = Math.Round(germ, 2);
                if (germ < 0m || germ > 1m)
                {
                    MessageBox.Show("La germinación debe estar entre 0 y 1 (por ejemplo 0.85 para 85%).");
                    return;
                }
                producto.PorcentajeGerminacion = germ;

                string textoPrecioMaquila = txtPrecioMaquila.Text?.Trim();
                textoPrecioMaquila = textoPrecioMaquila?.Replace(',', '.');
                if (string.IsNullOrWhiteSpace(textoPrecioMaquila) ||
                    !decimal.TryParse(textoPrecioMaquila, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal pmq))
                {
                    MessageBox.Show("Debe ingresar el precio de maquila para productos de la categoría Semilla.");
                    return;
                }
                producto.PrecioMaquila = Math.Round(pmq, 2);
            }
            else
            {
                producto.PorcentajeGerminacion = null;
                producto.PrecioMaquila = null;
            }

            object proveedorValue = CBProveedor.SelectedValue;
            int? idProveedor = null;
            if (proveedorValue == null || proveedorValue == DBNull.Value)
            {
                idProveedor = null;
            }
            else if (proveedorValue is int intVal)
            {
                idProveedor = intVal;
            }
            else if (proveedorValue is long longVal)
            {
                idProveedor = (int)longVal;
            }
            else if (proveedorValue is string s && int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
            {
                idProveedor = parsed;
            }
            else if (proveedorValue is DataRowView drv && drv.Row.Table.Columns.Contains("IDProveedor"))
            {
                object rowVal = drv.Row["IDProveedor"];
                if (rowVal != DBNull.Value && int.TryParse(rowVal.ToString(), out int rv)) idProveedor = rv;
            }
            producto.IDProveedor = idProveedor;

            producto.Activo = checkactivo.Checked;

            try
            {
                bool resultado = producto.Editar();
                if (resultado)
                {
                    MessageBox.Show("Producto editado correctamente.");
                    CargarProductosFiltro(cboFiltroActivo.SelectedItem?.ToString() ?? "Activos");
                    btnEditar.Enabled = false;
                    btnClean.Text = "Limpiar";
                    valoresOriginales = null;
                }
                else
                {
                    MessageBox.Show("La edición no se realizó. Revisa que todos los datos sean válidos y que el producto exista.");
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null ? "\n" + ex.InnerException.Message : string.Empty;
                MessageBox.Show("Error técnico al editar el producto: " + ex.Message + inner);
            }
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Si el usuario estaba en modo editar y quería cancelar, se usa btnClean para esa acción
            if (btnClean.Text == "Cancelar")
            {
                RestaurarValoresOriginales();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text)) return;

            if (!int.TryParse(txtId.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int idProd))
            {
                MessageBox.Show("ID de producto inválido.");
                return;
            }

            producto.IDProducto = idProd;

            if (producto.Eliminar())
            {
                MessageBox.Show("Producto eliminado correctamente.");
                CargarProductosFiltro(cboFiltroActivo.SelectedItem?.ToString() ?? "Activos");
                LimpiarCampos();
                EstadoInicial();
            }
            else
            {
                MessageBox.Show("Error al eliminar el producto.");
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string nombreProducto = CBProducto.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombreProducto))
            {
                MessageBox.Show("Debe ingresar el nombre del producto para buscar.");
                return;
            }

            string filtroActual = cboFiltroActivo.SelectedItem?.ToString() ?? "Activos";
            DataTable datosConFiltro = ClaseFiltroActivo.FiltrarTabla("PRODUCTO", filtroActual);
            var rows = datosConFiltro.Select($"Nombre = '{nombreProducto.Replace("'", "''")}'");

            if (rows.Length > 0)
            {
                var data = rows[0];
                txtId.Text = data["IDProducto"].ToString();
                CBCategoria.Text = data["Categoria"].ToString();
                CBProducto.Text = data["Nombre"].ToString();
                NUPCantidad.Value = data["Cantidad"] != DBNull.Value ? Convert.ToDecimal(data["Cantidad"]) : 0;
                txtPrecioUnitario.Text = data["PrecioUnitario"].ToString();
                txtGerminacion.Text = data["PorcentajeGerminacion"].ToString();
                txtPrecioMaquila.Text = data.Table.Columns.Contains("PrecioMaquila") && data["PrecioMaquila"] != DBNull.Value ? data["PrecioMaquila"].ToString() : "";
                CBProveedor.SelectedValue = data["IDProveedor"] != DBNull.Value ? (int?)Convert.ToInt32(data["IDProveedor"]) : null;
                checkactivo.Checked = data["Activo"] != DBNull.Value ? Convert.ToBoolean(data["Activo"]) : false;

                valoresOriginales = new Dictionary<string, object>
                {
                    { "txtId", txtId.Text },
                    { "CBCategoria", CBCategoria.Text },
                    { "CBProducto", CBProducto.Text },
                    { "NUPCantidad", NUPCantidad.Value },
                    { "txtPrecioUnitario", txtPrecioUnitario.Text },
                    { "txtGerminacion", txtGerminacion.Text },
                    { "txtPrecioMaquila", txtPrecioMaquila.Text },
                    { "CBProveedor", CBProveedor.SelectedValue },
                    { "checkactivo", checkactivo.Checked }
                };

                CBCategoria.Enabled = true;
                CBProducto.Enabled = true;
                NUPCantidad.Enabled = true;
                txtPrecioUnitario.Enabled = true;
                txtGerminacion.Enabled = true;
                txtPrecioMaquila.Enabled = true;
                CBProveedor.Enabled = true;
                checkactivo.Enabled = true;

                btnBuscar.Enabled = false;
                btnEliminar.Enabled = true;
                btnEditar.Enabled = false;
                btnGuardar.Enabled = false;
                btnClean.Text = "Limpiar";
            }
            else
            {
                MessageBox.Show($"No existe un producto con este nombre en el filtro '{filtroActual}'. Puede crear uno nuevo o cambiar el filtro.");
                HabilitarCamposParaNuevoProducto();
                btnBuscar.Enabled = false;
                btnGuardar.Enabled = true;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
                btnClean.Text = "Limpiar";
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            if (btnClean.Text == "Cancelar")
            {
                RestaurarValoresOriginales();
            }
            else
            {
                LimpiarCampos();
                EstadoInicial();
            }
        }

        private void RestaurarValoresOriginales()
        {
            if (valoresOriginales == null)
                return;

            txtId.Text = valoresOriginales["txtId"].ToString();
            CBCategoria.Text = valoresOriginales["CBCategoria"].ToString();
            CBProducto.Text = valoresOriginales["CBProducto"].ToString();
            NUPCantidad.Value = (decimal)valoresOriginales["NUPCantidad"];
            txtPrecioUnitario.Text = valoresOriginales["txtPrecioUnitario"].ToString();
            txtGerminacion.Text = valoresOriginales["txtGerminacion"].ToString();
            txtPrecioMaquila.Text = valoresOriginales["txtPrecioMaquila"].ToString();

            // Restaurar proveedor robustamente
            object provVal = valoresOriginales.ContainsKey("CBProveedor") ? valoresOriginales["CBProveedor"] : null;
            if (provVal == null || provVal == DBNull.Value)
            {
                CBProveedor.SelectedIndex = -1;
            }
            else
            {
                try
                {
                    CBProveedor.SelectedValue = provVal;
                }
                catch
                {
                    CBProveedor.SelectedIndex = -1;
                }
            }

            checkactivo.Checked = (bool)valoresOriginales["checkactivo"];

            CBCategoria.Enabled = true;
            CBProducto.Enabled = true;
            NUPCantidad.Enabled = true;
            txtPrecioUnitario.Enabled = true;
            txtGerminacion.Enabled = true;
            txtPrecioMaquila.Enabled = true;
            CBProveedor.Enabled = true;
            checkactivo.Enabled = true;

            btnEditar.Enabled = false;
            btnClean.Text = "Limpiar";
            btnEliminar.Enabled = true;
            btnGuardar.Enabled = false;
            btnBuscar.Enabled = false;
        }

        private void LimpiarCampos()
        {
            txtId.Text = "";
            CBCategoria.SelectedIndex = -1;
            CBProducto.SelectedIndex = -1;
            NUPCantidad.Value = 0;
            txtPrecioUnitario.Text = "";
            txtGerminacion.Text = "";
            txtPrecioMaquila.Text = "";
            CBProveedor.SelectedIndex = -1;
            checkactivo.Checked = false;
            valoresOriginales = null;
        }

        private void EstadoInicial()
        {
            CBCategoria.Enabled = false;
            CBProducto.Enabled = true;
            NUPCantidad.Enabled = false;
            txtPrecioUnitario.Enabled = false;
            txtGerminacion.Enabled = false;
            txtPrecioMaquila.Enabled = false;
            CBProveedor.Enabled = false;
            checkactivo.Enabled = false;

            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnGuardar.Enabled = false;
            btnBuscar.Enabled = true;
            btnClean.Text = "Limpiar";
            valoresOriginales = null;
        }

        private void HabilitarCamposParaNuevoProducto()
        {
            CBCategoria.Enabled = true;
            CBProducto.Enabled = true;
            NUPCantidad.Enabled = true;
            txtPrecioUnitario.Enabled = true;
            txtGerminacion.Enabled = true;
            txtPrecioMaquila.Enabled = true;
            CBProveedor.Enabled = true;
            checkactivo.Enabled = true;
        }

        private void ControlModificado(object sender, EventArgs e)
        {
            if (valoresOriginales == null)
            {
                btnEditar.Enabled = false;
                btnClean.Text = "Limpiar";
                return;
            }

            bool huboCambios =
                txtId.Text != valoresOriginales["txtId"].ToString() ||
                CBCategoria.Text != valoresOriginales["CBCategoria"].ToString() ||
                CBProducto.Text != valoresOriginales["CBProducto"].ToString() ||
                NUPCantidad.Value != Convert.ToDecimal(valoresOriginales["NUPCantidad"]) ||
                txtPrecioUnitario.Text != valoresOriginales["txtPrecioUnitario"].ToString() ||
                txtGerminacion.Text != valoresOriginales["txtGerminacion"].ToString() ||
                txtPrecioMaquila.Text != valoresOriginales["txtPrecioMaquila"].ToString() ||
                (CBProveedor.SelectedValue == null ? "" : CBProveedor.SelectedValue.ToString()) !=
                    (valoresOriginales["CBProveedor"] == null ? "" : valoresOriginales["CBProveedor"].ToString()) ||
                checkactivo.Checked != (bool)valoresOriginales["checkactivo"];

            if (huboCambios)
            {
                btnEditar.Enabled = true;
                btnClean.Text = "Cancelar";
                btnEliminar.Enabled = true;
                btnGuardar.Enabled = false;
            }
            else
            {
                btnEditar.Enabled = false;
                btnClean.Text = "Limpiar";
                btnEliminar.Enabled = true;
                btnGuardar.Enabled = false;
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(CBProducto.Text))
            {
                MessageBox.Show("Debe seleccionar o ingresar el nombre del producto.");
                return false;
            }
            if (CBCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar una categoría.");
                return false;
            }
            if (CBProveedor.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un proveedor.");
                return false;
            }
            return true;
        }

        private void cboFiltroActivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFiltroActivo.SelectedItem != null)
                CargarProductosFiltro(cboFiltroActivo.SelectedItem.ToString());
        }

        private void aggprovj_Click(object sender, EventArgs e)
        {
            using (var formProveedor = new FPROVEEDOR())
            {
                formProveedor.ShowDialog();
                CargarProveedores();
            }
        }

        private void CBCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            string categoria = CBCategoria.Text?.Trim() ?? string.Empty;
            if (categoria.Equals("Semilla", StringComparison.OrdinalIgnoreCase) ||
                categoria.Equals("Semilla Maquilada", StringComparison.OrdinalIgnoreCase))
            {
                txtGerminacion.Enabled = true;
                txtGerminacion.Text = "";
                txtPrecioMaquila.Enabled = true;
                txtPrecioMaquila.Text = "";
            }
            else
            {
                txtGerminacion.Enabled = false;
                txtGerminacion.Text = "";
                txtPrecioMaquila.Enabled = false;
                txtPrecioMaquila.Text = "";
            }
        }

        private void UCProducto_Load_1(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
