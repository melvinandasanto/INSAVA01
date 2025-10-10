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
            // Asegura que el evento esté asignado
            this.NUPCantidad.ValueChanged += new System.EventHandler(this.ControlModificado);

        }

        private void UCProducto_Load(object sender, EventArgs e)
        {
            txtGerminacion.Enabled = false;
            CargarProductos();
            CargarProveedores();
            CargarCategorias();
            CargarComboFiltro();
            CargarProductosFiltro("Activos");
            // Eventos para detectar cambios en los campos
            CBCategoria.SelectedIndexChanged += ControlModificado;
            CBProducto.TextChanged += ControlModificado;
            NUPCantidad.ValueChanged += ControlModificado;
            txtPrecioUnitario.TextChanged += ControlModificado;
            txtGerminacion.TextChanged += ControlModificado;
            CBProveedor.SelectedIndexChanged += ControlModificado;
            checkactivo.CheckedChanged += ControlModificado;
            EstadoInicial();
            CamposDecimales = new List<TextBox>
            {
                txtPrecioUnitario,
                txtGerminacion
            };
            // Fix for the CS7036 error: Pass the required "txt" parameter to the ValidarCampoDecimal method.
            CamposDecimales.ForEach(campo => campo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoDecimal(ev, campo));
            
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

            producto.Categoria = CBCategoria.Text;
            producto.Nombre = CBProducto.Text;
            producto.Cantidad = NUPCantidad.Value;

            // Validación flexible para Precio Unitario (acepta entero o decimal)
            string textoPrecio = txtPrecioUnitario.Text.Trim();
            if (!decimal.TryParse(textoPrecio, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal precio))
            {
                MessageBox.Show("El precio unitario debe ser un número válido (entero o decimal).");
                return;
            }
            producto.PrecioUnitario = precio;

            // Validación y conversión del porcentaje de germinación
            if (CBCategoria.Text.Equals("Semilla", StringComparison.OrdinalIgnoreCase) ||
                CBCategoria.Text.Equals("Semilla Maquilada", StringComparison.OrdinalIgnoreCase))
            {
                string textoGerminacion = txtGerminacion.Text.Trim().Replace(',', '.');
                if (!decimal.TryParse(textoGerminacion, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal germ))
                {
                    MessageBox.Show("La germinación debe ser un número válido (ejemplo: 0.85).");
                    return;
                }
                if (germ < 0m || germ > 1m)
                {
                    MessageBox.Show("La germinación debe ser un número decimal entre 0 y 1 (por ejemplo, 0.85 para 85%).");
                    return;
                }
                producto.PorcentajeGerminacion = germ;
            }
            else
            {
                producto.PorcentajeGerminacion = null; // Si tu clase permite null, usa null
            }

            // Proveedor robusto
            object proveedorValue = CBProveedor.SelectedValue;
            int? idProveedor = null;
            if (proveedorValue is int)
                idProveedor = (int)proveedorValue;
            else if (proveedorValue is string && int.TryParse((string)proveedorValue, out int idParsed))
                idProveedor = idParsed;
            else if (proveedorValue is DataRowView drv && drv.Row["IDProveedor"] is int idFromRow)
                idProveedor = idFromRow;
            producto.IDProveedor = idProveedor;

            producto.Activo = checkactivo.Checked;

            try
            {
                if (producto.Guardar())
                {
                    MessageBox.Show("Producto guardado correctamente.");
                    CargarProductos();
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
                   MessageBox.Show("Error técnico: " + ex.Message);
               }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;

            producto.IDProducto = int.Parse(txtId.Text);
            producto.Categoria = CBCategoria.Text;
            producto.Nombre = CBProducto.Text;
            producto.Cantidad = NUPCantidad.Value;

            // Validación flexible para Precio Unitario (acepta entero o decimal)
            string textoPrecio = txtPrecioUnitario.Text.Trim();
            if (!decimal.TryParse(textoPrecio, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal precio))
            {
                MessageBox.Show("El precio unitario debe ser un número válido (entero o decimal).");
                return;
            }
            producto.PrecioUnitario = precio;

            // Validación y conversión del porcentaje de germinación
            if (CBCategoria.Text == "Semilla" || CBCategoria.Text == "Semilla maquilada")
            {
                string textoGerminacion = txtGerminacion.Text.Trim().Replace(',', '.');
                if (!decimal.TryParse(textoGerminacion, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal germ))
                {
                    MessageBox.Show("La germinación debe ser un número válido (ejemplo: 0.85).");
                    return;
                }
                if (germ < 0m || germ > 1m)
                {
                    MessageBox.Show("La germinación debe ser un número decimal entre 0 y 1 (por ejemplo, 0.85 para 85%).");
                    return;
                }
                producto.PorcentajeGerminacion = germ;

            }
            else // Producto
            {
                producto.PorcentajeGerminacion = null; // O NULL si tu clase lo permite
            }

            // Proveedor robusto
            object proveedorValue = CBProveedor.SelectedValue;
            int? idProveedor = null;
            if (proveedorValue is int)
                idProveedor = (int)proveedorValue;
            else if (proveedorValue is string && int.TryParse((string)proveedorValue, out int idParsed))
                idProveedor = idParsed;
            else if (proveedorValue is DataRowView drv && drv.Row["IDProveedor"] is int idFromRow)
                idProveedor = idFromRow;
            producto.IDProveedor = idProveedor;

            producto.Activo = checkactivo.Checked;

            try
            {
                bool resultado = producto.Editar();
                if (resultado)
                {
                    MessageBox.Show("Producto editado correctamente.");
                    CargarProductos();
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
                MessageBox.Show("Error técnico al editar el producto: " + ex.Message + (ex.InnerException != null ? "\n" + ex.InnerException.Message : ""));
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (btnEliminar.Text == "Cancelar")
            {
                RestaurarValoresOriginales();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text)) return;

            producto.IDProducto = int.Parse(txtId.Text);

            if (producto.Eliminar())
            {
                MessageBox.Show("Producto eliminado correctamente.");
                CargarProductos();
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

            // Buscar el producto por nombre
            var row = producto.ObtenerProductos().Select($"Nombre = '{nombreProducto.Replace("'", "''")}'");
            if (row.Length > 0)
            {
                // Producto encontrado
                var data = row[0];
                txtId.Text = data["IDProducto"].ToString();
                CBCategoria.Text = data["Categoria"].ToString();
                CBProducto.Text = data["Nombre"].ToString();
                NUPCantidad.Value = data["Cantidad"] != DBNull.Value ? Convert.ToDecimal(data["Cantidad"]) : 0;
                txtPrecioUnitario.Text = data["PrecioUnitario"].ToString();
                txtGerminacion.Text = data["PorcentajeGerminacion"].ToString();
                CBProveedor.SelectedValue = data["IDProveedor"] != DBNull.Value ? (int?)Convert.ToInt32(data["IDProveedor"]) : null;
                checkactivo.Checked = data["Activo"] != DBNull.Value ? Convert.ToBoolean(data["Activo"]) : false;

                // Guardar valores originales
                valoresOriginales = new Dictionary<string, object>
                {
                    { "txtId", txtId.Text },
                    { "CBCategoria", CBCategoria.Text },
                    { "CBProducto", CBProducto.Text },
                    { "NUPCantidad", NUPCantidad.Value },
                    { "txtPrecioUnitario", txtPrecioUnitario.Text },
                    { "txtGerminacion", txtGerminacion.Text },
                    { "CBProveedor", CBProveedor.SelectedValue },
                    { "checkactivo", checkactivo.Checked }
                };

                // Habilitar campos para edición (como en UCUSUARIO)
                CBCategoria.Enabled = true;
                CBProducto.Enabled = true;
                NUPCantidad.Enabled = true;
                txtPrecioUnitario.Enabled = true;
                txtGerminacion.Enabled = true;
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
                // Producto NO encontrado, habilitar para nuevo registro
                HabilitarCamposParaNuevoProducto();
                btnBuscar.Enabled = false;
                btnGuardar.Enabled = true;
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
                btnClean.Text = "Limpiar";
                MessageBox.Show("No existe un producto con este nombre. Puede crear uno nuevo.");
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
            CBProveedor.SelectedValue = valoresOriginales["CBProveedor"];
            checkactivo.Checked = (bool)valoresOriginales["checkactivo"];

            // Habilitar campos para seguir editando tras cancelar (como en UCUSUARIO)
            CBCategoria.Enabled = true;
            CBProducto.Enabled = true;
            NUPCantidad.Enabled = true;
            txtPrecioUnitario.Enabled = true;
            txtGerminacion.Enabled = true;
            CBProveedor.Enabled = true;
            checkactivo.Enabled = true;

            btnEditar.Enabled = false;
            btnEliminar.Text = "Eliminar";
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
            CBProveedor.SelectedIndex = -1;
            checkactivo.Checked = false;
            valoresOriginales = null;
        }

        private void EstadoInicial()
        {
            // Deshabilitar todos los campos
            CBCategoria.Enabled = false;
            CBProducto.Enabled = true;
            NUPCantidad.Enabled = false;
            txtPrecioUnitario.Enabled = false;
            txtGerminacion.Enabled = false;
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
            CBProveedor.Enabled = true;
            checkactivo.Enabled = true;
        }

        private void ControlModificado(object sender, EventArgs e)
        {
            if (valoresOriginales == null)
            {
                btnEditar.Enabled = false;
                btnEliminar.Text = "Eliminar";
                return;
            }

            bool huboCambios =
                txtId.Text != valoresOriginales["txtId"].ToString() ||
                CBCategoria.Text != valoresOriginales["CBCategoria"].ToString() ||
                CBProducto.Text != valoresOriginales["CBProducto"].ToString() ||
                NUPCantidad.Value != Convert.ToDecimal(valoresOriginales["NUPCantidad"]) ||
                txtPrecioUnitario.Text != valoresOriginales["txtPrecioUnitario"].ToString() ||
                txtGerminacion.Text != valoresOriginales["txtGerminacion"].ToString() ||
                (CBProveedor.SelectedValue == null ? "" : CBProveedor.SelectedValue.ToString()) !=
                    (valoresOriginales["CBProveedor"] == null ? "" : valoresOriginales["CBProveedor"].ToString()) ||
                checkactivo.Checked != (bool)valoresOriginales["checkactivo"];

            if (huboCambios)
            {
                btnEditar.Enabled = true;
                btnEliminar.Text = "Cancelar";
                btnEliminar.Enabled = true;
                btnGuardar.Enabled = false;
            }
            else
            {
                btnEditar.Enabled = false;
                btnEliminar.Text = "Eliminar";
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
                // Si quieres recargar la lista de proveedores después de agregar uno nuevo:
                CargarProveedores();
            }
        }

        private void CBCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            string categoria = CBCategoria.Text.Trim();
            if (categoria.Equals("Semilla", StringComparison.OrdinalIgnoreCase) ||
                categoria.Equals("Semilla Maquilada", StringComparison.OrdinalIgnoreCase))
            {
                txtGerminacion.Enabled = true;
                txtGerminacion.Text = ""; // Limpia el campo para evitar valores residuales
            }
            else
            {
                txtGerminacion.Enabled = false;
                txtGerminacion.Text = ""; // Limpia el campo
            }
        }
    }
}
