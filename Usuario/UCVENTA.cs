using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class UCVENTA : UserControl
    {
        private ClaseConexion conexion;
        private DataTable productosOriginal;
        private List<TextBox> CamposDecimales;

        // Agregar campo privado para saber si el producto fue leído desde la BD
        private bool productoLeido = false;

        public UCVENTA()
        {
            InitializeComponent();
            conexion = new ClaseConexion();
            DiseñoGlobal.RegistrarUserControl(this);

            // Ocultar radio buttons inicialmente
            rbStock.Visible = false;
            radioButton1.Visible = false;

            // Selecciona por defecto "Producto/Semilla" si existe
            if (CBOperacion.Items.Contains("Producto/Semilla"))
                CBOperacion.SelectedItem = "Producto/Semilla";
            else if (CBOperacion.Items.Count > 0)
                CBOperacion.SelectedIndex = 0;
        }

        private void FVENTA_Load(object sender, EventArgs e)
        {
            Permisos.AplicarPermisos(this);
            DiseñoGlobal.AplicarTamaño(this);
            ConfigurarDataGridView();
            DiseñoGlobal.AplicarEstiloDataGridView(dgvPedido, Temas.Light); // SIEMPRE inicia en Light
            CBOperacion.SelectedIndexChanged += CBOperacion_SelectedIndexChanged;
            CamposDecimales = new List<TextBox>
                {
                    txtPrecio,
                    txtStockoporcent,
                    txtMaquila
                };
            CamposDecimales.ForEach(campo =>
            {
                // Validación para solo permitir caracteres válidos de decimal
                campo.KeyPress += (s, ev) =>
                {
                    ClaseValidacion.ValidarCampoDecimal(ev, campo);
                };
                // Validación para no permitir valores negativos
                campo.Validating += (s, ev) =>
                {
                    decimal valor;
                    if (decimal.TryParse(campo.Text.Replace('.', ','), out valor))
                    {
                        if (ClaseValidacion.EsNegativo(valor))
                        {
                            MessageBox.Show("No se permiten valores negativos.");
                            campo.Focus();
                            ev.Cancel = true;
                        }
                    }
                };
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            rbStock.CheckedChanged += RbStock_CheckedChanged;
            radioButton1.CheckedChanged += RadioButton1_CheckedChanged;
            var menu = this.FindForm() as FMENU;
            if (menu != null)
            {
                DiseñoGlobal.AplicarEstiloDataGridView(dgvPedido, menu.temaActual);
            }
        }

        private void ConfigurarDataGridView()
        {
            dgvPedido.Columns.Clear();

            dgvPedido.Columns.Add("IDProducto", "IDProducto");
            dgvPedido.Columns["IDProducto"].Visible = false;

            dgvPedido.Columns.Add("Producto", "Producto");
            dgvPedido.Columns.Add("Categoria", "Categoria");
            dgvPedido.Columns.Add("Cantidad", "Cantidad");
            dgvPedido.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgvPedido.Columns.Add("Subtotal", "Subtotal");

            DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn
            {
                HeaderText = "Eliminar",
                Name = "Eliminar",
                Text = "X",
                UseColumnTextForButtonValue = true
            };
            dgvPedido.Columns.Add(btnEliminar);

            dgvPedido.AllowUserToAddRows = false;
            dgvPedido.ReadOnly = false;
            dgvPedido.Columns["Cantidad"].ReadOnly = false;
        }

        private void CargarProductosCombo()
        {
            string sql;
            // Validar el modo de operación
            if (CBOperacion.SelectedItem != null && CBOperacion.SelectedItem.ToString() == "Maquila")
            {
                sql = "SELECT IDProducto, Producto, Categoria, PrecioUnitario, PrecioMaquila FROM VISTAPRODUCTOS WHERE Categoria IN ('Semilla', 'Semilla maquilada')";
            }
            else
            {
                // Incluye todas las categorías relevantes, incluyendo 'Semilla maquilada'
                sql = "SELECT IDProducto, Producto, Categoria, PrecioUnitario, PrecioMaquila FROM VISTAPRODUCTOS";
            }
            productosOriginal = conexion.Tabla(sql);

            cmbProducto.DataSource = productosOriginal.Copy();
            cmbProducto.DisplayMember = "Producto";
            cmbProducto.ValueMember = "IDProducto";
            cmbProducto.SelectedIndex = -1;
        }

        private void CBOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBOperacion.SelectedItem == null)
                return;

            string operacion = CBOperacion.SelectedItem.ToString();
            if (operacion == "Producto/Semilla")
            {
                // Ocultar controles de maquila
                rbStock.Visible = false;
                radioButton1.Visible = false;
                OcultarControlesMaquila();
                CargarProductosCombo();
                
                // Restablecer campos
                txtStockoporcent.ReadOnly = true;
                txtStockoporcent.Enabled = true;
                txtPrecio.ReadOnly = true;
                txtPrecio.Visible = true;
                lblPrecio.Visible = true;
            }
            else if (operacion == "Maquila")
            {
                // Mostrar controles de maquila
                rbStock.Visible = true;
                radioButton1.Visible = true;
                rbStock.Checked = true; // Seleccionar Stock por defecto
                
                MostrarControlesMaquila();
                CargarSemillasCombo();
            }

            // Mantener compatibilidad con el evento existente si lo hay
            UpdateControlsForClienteRadio();
        }

        private void RbStock_CheckedChanged(object sender, EventArgs e)
        {
            if (rbStock.Checked)
            {
                txtStockoporcent.ReadOnly = true;
                txtStockoporcent.Enabled = true;
                txtPrecio.ReadOnly = false;
                txtPrecio.Visible = true;
                lblPrecio.Visible = true;
                if (OperacionEsMaquila())
                {
                    lblestock.Text = "Stock";
                }
            }
        }
        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsForClienteRadio();
        }

        private void CargarSemillasCombo()
        {
            string sql = "SELECT IDProducto, Producto, Categoria, PrecioUnitario, PrecioMaquila FROM VISTAPRODUCTOS WHERE Categoria IN ('Semilla', 'Semilla maquilada')";
            productosOriginal = conexion.Tabla(sql);
            cmbProducto.DataSource = productosOriginal.Copy();
            cmbProducto.DisplayMember = "Producto";
            cmbProducto.ValueMember = "IDProducto";
            cmbProducto.SelectedIndex = -1;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string nombre = cmbProducto.Text.Trim();
            bool esMaquila = CBOperacion.SelectedItem?.ToString() == "Maquila";
            bool esMaquilaCliente = esMaquila && radioButton1.Checked;

            if (cmbProducto.SelectedValue != null)
            {
                // Producto existente en BD
                int idProducto = Convert.ToInt32(cmbProducto.SelectedValue);
                string sql = "SELECT Cantidad AS Stock, PrecioUnitario, PorcentajeGerminacion, PrecioMaquila FROM PRODUCTO WHERE IDProducto=@id";
                SqlParameter[] parametros = { new SqlParameter("@id", idProducto) };
                DataTable dt = conexion.Tabla(sql, parametros);

                if (dt.Rows.Count > 0)
                {
                    // Producto encontrado - bloquear campos
                    txtPrecio.ReadOnly = true;
                    txtMaquila.ReadOnly = true;
                    
                    if (esMaquilaCliente)
                    {
                        txtStockoporcent.ReadOnly = false;
                        txtPrecio.Visible = true; // Mostrar pero no usar
                        lblPrecio.Visible = true;
                    }
                    else
                    {
                        txtStockoporcent.ReadOnly = true;
                    }

                    // Llenar datos...
                    CargarDatosProducto(dt.Rows[0]);
                }
            }
            else if (esMaquilaCliente)
            {
                // Nuevo producto del cliente
                txtPrecio.Visible = false;
                lblPrecio.Visible = false;
                txtStockoporcent.ReadOnly = false;
                txtMaquila.ReadOnly = false;
                txtStockoporcent.Text = "";
                lblestock.Text = "%";
                nudCantidad.Maximum = 1000000;
            }
        }

        // 6. Método auxiliar para cargar datos del producto
        private void CargarDatosProducto(DataRow row)
        {
            string precioMaquilaText = row.Table.Columns.Contains("PrecioMaquila") && 
                                      row["PrecioMaquila"] != DBNull.Value
                                      ? row["PrecioMaquila"].ToString()
                                      : "";

            txtStockoporcent.Text = row["Stock"].ToString();
            txtPrecio.Text = row["PrecioUnitario"].ToString();
            txtMaquila.Text = precioMaquilaText;
            
            decimal stock = 0;
            decimal.TryParse(row["Stock"].ToString(), out stock);
            nudCantidad.Maximum = stock > 0 ? stock : 1;
            nudCantidad.Value = 0;

            productoLeido = true; // Marcar como leído
            UpdateControlsForClienteRadio(); // asegurar estado correcto de UI
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string nombreProducto = cmbProducto.Text.Trim();
            int cantidad = (int)nudCantidad.Value;
            bool esMaquila = CBOperacion.SelectedItem?.ToString() == "Maquila";

            if (string.IsNullOrWhiteSpace(nombreProducto) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese un producto válido y una cantidad mayor a 0.");
                return;
            }

            DataRowView row = cmbProducto.SelectedItem as DataRowView;
            if (row != null)
            {
                int idProducto = Convert.ToInt32(row["IDProducto"]);
                string categoria = row["Categoria"].ToString();

                decimal precio = 0m;
                if (esMaquila)
                {
                    decimal.TryParse(txtMaquila.Text.Replace('.', ','), out precio);
                }
                else
                {
                    decimal.TryParse(row["PrecioUnitario"].ToString().Replace('.', ','), out precio);
                }

                decimal subtotal = cantidad * precio;
                string nombreMostrar = esMaquila ? $"Maquila {nombreProducto}" : nombreProducto;

                // Añadir fila creando la fila y asignando celdas por nombre (más robusto)
                int newIndex = dgvPedido.Rows.Add();
                var nuevaFila = dgvPedido.Rows[newIndex];
                nuevaFila.Cells["IDProducto"].Value = idProducto;
                nuevaFila.Cells["Producto"].Value = nombreMostrar;
                nuevaFila.Cells["Categoria"].Value = categoria;
                nuevaFila.Cells["Cantidad"].Value = cantidad;
                nuevaFila.Cells["PrecioUnitario"].Value = precio;
                nuevaFila.Cells["Subtotal"].Value = subtotal;
            }
            else if (esMaquila) // Nuevo producto de maquila traído por cliente
            {
                if (!decimal.TryParse(txtMaquila.Text.Replace('.', ','), out decimal precioMaquila))
                {
                    MessageBox.Show("Ingrese un precio de maquila válido.");
                    return;
                }

                decimal subtotal = cantidad * precioMaquila;
                int newIndex = dgvPedido.Rows.Add();
                var nuevaFila = dgvPedido.Rows[newIndex];
                nuevaFila.Cells["IDProducto"].Value = DBNull.Value;
                nuevaFila.Cells["Producto"].Value = $"Maquila {nombreProducto}";
                nuevaFila.Cells["Categoria"].Value = "Semilla maquilada";
                nuevaFila.Cells["Cantidad"].Value = cantidad;
                nuevaFila.Cells["PrecioUnitario"].Value = precioMaquila;
                nuevaFila.Cells["Subtotal"].Value = subtotal;
            }

            ActualizarTotales();
            LimpiarCampos();
        }

        private void MostrarControlesMaquila()
        {
            lblMaquila.Visible = true;
            txtMaquila.Visible = true;
            
            // Si hay producto seleccionado, verificar si existe en BD
            if (cmbProducto.SelectedItem is DataRowView row)
            {
                txtMaquila.ReadOnly = true; // Producto existente
                if (row.Row.Table.Columns.Contains("PrecioMaquila") && 
                    row["PrecioMaquila"] != DBNull.Value)
                {
                    txtMaquila.Text = row["PrecioMaquila"].ToString();
                }
            }
            else
            {
                txtMaquila.ReadOnly = false; // Nuevo producto
                txtMaquila.Text = "";
            }
        }

        private void OcultarControlesMaquila()
        {
            lblMaquila.Visible = false;
            txtMaquila.Visible = false;
        }

        // Devuelve true si la operación seleccionada es "Maquila"
        private bool OperacionEsMaquila()
        {
            return CBOperacion.SelectedItem != null && CBOperacion.SelectedItem.ToString() == "Maquila";
        }

        // Devuelve un DataTable con el detalle de los pedidos (ya no depende de una columna 'Tipo' en el grid)
        public DataTable ObtenerDetallePedidos()
        {
            DataTable detalle = new DataTable();
            detalle.Columns.Add("IDProducto", typeof(int));
            detalle.Columns.Add("Producto", typeof(string));
            detalle.Columns.Add("Cantidad", typeof(decimal));
            detalle.Columns.Add("PrecioUnitario", typeof(decimal));
            detalle.Columns.Add("Subtotal", typeof(decimal));
            detalle.Columns.Add("TipoOperacion", typeof(string));

            foreach (DataGridViewRow row in dgvPedido.Rows)
            {
                if (row.IsNewRow) continue;

                int idProducto = row.Cells["IDProducto"].Value != null && row.Cells["IDProducto"].Value != DBNull.Value
                    ? Convert.ToInt32(row.Cells["IDProducto"].Value)
                    : 0;

                string producto = row.Cells["Producto"].Value?.ToString() ?? "";

                decimal precioUnitario = 0m;
                decimal.TryParse(row.Cells["PrecioUnitario"].Value?.ToString().Replace('.', ','), out precioUnitario);

                decimal subtotal = 0m;
                decimal.TryParse(row.Cells["Subtotal"].Value?.ToString().Replace('.', ','), out subtotal);

                decimal cantidad = 0m;
                decimal.TryParse(row.Cells["Cantidad"].Value?.ToString().Replace('.', ','), out cantidad);

                // Inferir tipo desde el texto del producto
                string tipoOperacion = (producto.StartsWith("Maquila ", StringComparison.OrdinalIgnoreCase) ||
                                        producto.Contains("(Maquila)")) ? "Maquila" : "Venta";

                detalle.Rows.Add(idProducto, producto, cantidad, precioUnitario, subtotal, tipoOperacion);
            }

            return detalle;
        }

        // Limpia los pedidos y actualiza los totales
        public void LimpiarPedidos()
        {
            dgvPedido.Rows.Clear();
            ActualizarTotales();
        }

        // =====================
        // Totales
        // =====================
        // Hago público para que otros formularios puedan invocarlo si es necesario.
        public void ActualizarTotales()
        {
            ActualizarTotalesGenerales();
        }

        private void ActualizarTotalesGenerales()
        {
            decimal totalVenta = 0m;
            foreach (DataGridViewRow row in dgvPedido.Rows)
            {
                if (row.IsNewRow) continue;

                decimal subtotal = 0m;
                var val = row.Cells["Subtotal"].Value;
                if (val != null && val != DBNull.Value)
                    decimal.TryParse(val.ToString().Replace('.', ','), out subtotal);

                totalVenta += subtotal;
            }

            int totalPedidos = dgvPedido.Rows.Count;
            txtCantidadPedido.Text = totalPedidos.ToString();
            txtTotalPagar.Text = totalVenta.ToString("N2");
        }

        // =====================
        // NUEVO: Botón Cobrar
        // =====================
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (dgvPedido.Rows.Count == 0)
            {
                MessageBox.Show("No hay productos o maquilas para cobrar.");
                return;
            }

            DataTable detalle = ObtenerDetallePedidos();
            string cliente = "CONSUMIDOR FINAL";
            int numeroOrden = ObtenerNumeroOrden();

            FACTURA frmFactura = new FACTURA(numeroOrden, cliente, detalle);
            if (frmFactura.ShowDialog() == DialogResult.OK)
            {
                // Registrar maquilas si existen
                var maquila = new ClaseMaquila();
                foreach (DataGridViewRow row in dgvPedido.Rows)
                {
                    if (row.Cells["Producto"].Value?.ToString().StartsWith("Maquila ") == true)
                    {
                        int? idProducto = row.Cells["IDProducto"].Value as int?;
                        int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                        decimal precio = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value);
                        
                        maquila.RegistrarMaquila(
                            numeroOrden,
                            idProducto.HasValue ? "Inventario" : "Cliente",
                            idProducto,
                            cantidad,
                            precio
                        );
                    }
                }

                LimpiarPedidos();
                ActualizarTotales();
            }
        }

        // Devuelve el siguiente número de orden correlativo para la factura
        private int ObtenerNumeroOrden()
        {
            string sql = "SELECT ISNULL(MAX(IDTransaccion), 0) FROM TRANSACCION";
            DataTable dt = conexion.Tabla(sql);
            int ultimoNumero = 0;
            if (dt.Rows.Count > 0)
                int.TryParse(dt.Rows[0][0].ToString(), out ultimoNumero);

            return ultimoNumero + 1;
        }

        private void LimpiarCampos()
        {
            cmbProducto.SelectedIndex = -1;
            txtStockoporcent.Clear();
            txtPrecio.Clear();
            txtMaquila.Clear();
            nudCantidad.Value = 0;

            // Si está en modo maquila y es cliente, también limpiar campos específicos
            if (OperacionEsMaquila() && radioButton1.Checked)
            {
                txtStockoporcent.ReadOnly = false;
                txtStockoporcent.Enabled = true;
                txtPrecio.Visible = false;
                lblPrecio.Visible = false;
            }

            productoLeido = false; // ya no es un producto leído de BD
            UpdateControlsForClienteRadio();
        }

        // Event handler añadido: eliminar fila al pulsar el botón "Eliminar"
        private void DgvPedido_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Asegurarse de que existe la columna "Eliminar"
            if (dgvPedido.Columns.Contains("Eliminar") && e.ColumnIndex == dgvPedido.Columns["Eliminar"].Index)
            {
                dgvPedido.Rows.RemoveAt(e.RowIndex);
                ActualizarTotales();
            }
        }

        // Event handler añadido: actualizar subtotal cuando se edita la cantidad
        private void DgvPedido_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Asegurarse de que existe la columna "Cantidad"
            if (dgvPedido.Columns.Contains("Cantidad") && e.ColumnIndex == dgvPedido.Columns["Cantidad"].Index)
            {
                var fila = dgvPedido.Rows[e.RowIndex];
                try
                {
                    decimal cantidad = 0m;
                    decimal precioUnitario = 0m;

                    if (fila.Cells["Cantidad"].Value != null && fila.Cells["Cantidad"].Value != DBNull.Value)
                        decimal.TryParse(fila.Cells["Cantidad"].Value.ToString().Replace('.', ','), out cantidad);

                    if (fila.Cells["PrecioUnitario"].Value != null && fila.Cells["PrecioUnitario"].Value != DBNull.Value)
                        decimal.TryParse(fila.Cells["PrecioUnitario"].Value.ToString().Replace('.', ','), out precioUnitario);

                    fila.Cells["Subtotal"].Value = cantidad * precioUnitario;
                }
                catch
                {
                    fila.Cells["Subtotal"].Value = 0m;
                }

                ActualizarTotales();
            }
        }

        private void CmbProducto_KeyUp(object sender, KeyEventArgs e)
        {
            if (productosOriginal == null || productosOriginal.Rows.Count == 0)
                return;

            string textoActual = cmbProducto.Text;
            int posCursor = cmbProducto.SelectionStart;

            string filtro = textoActual.Trim().ToLower();
            if (string.IsNullOrEmpty(filtro))
            {
                cmbProducto.DataSource = productosOriginal.Copy();
            }
            else
            {
                var rows = productosOriginal.AsEnumerable()
                    .Where(r => r.Field<string>("Producto").ToLower().Contains(filtro));
                if (rows.Any())
                {
                    cmbProducto.DataSource = rows.CopyToDataTable();
                }
                else
                {
                    cmbProducto.DataSource = productosOriginal.Clone();
                }
            }
            cmbProducto.DisplayMember = "Producto";
            cmbProducto.ValueMember = "IDProducto";

            cmbProducto.Text = textoActual;
            cmbProducto.SelectionStart = posCursor;
            cmbProducto.DroppedDown = true;

            productoLeido = false;
            UpdateControlsForClienteRadio();
        }

        private void btnCancelarPedidos_Click(object sender, EventArgs e)
        {
            // Limpia las filas del pedido y actualiza los totales
            LimpiarPedidos();

            // Asegura valores por defecto en los campos de totales
            txtCantidadPedido.Text = "0";
            txtTotalPagar.Text = "0.00";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        // Llamar a este método desde los manejadores relevantes para mantener UI consistente
        private void UpdateControlsForClienteRadio()
        {
            bool operacionMaquila = CBOperacion?.SelectedItem != null &&
                                    CBOperacion.SelectedItem.ToString().IndexOf("Maquila", StringComparison.OrdinalIgnoreCase) >= 0;

            if (radioButton1.Checked && operacionMaquila && !productoLeido)
            {
                // Nuevo producto en modo Maquila+Cliente -> ocultar precio y mostrar %
                txtPrecio.Visible = false;
                lblPrecio.Visible = false;
                lblestock.Text = "%";
            }
            else
            {
                // Resto de casos -> estado normal
                txtPrecio.Visible = true;
                lblPrecio.Visible = true;
                lblestock.Text = "Stock";
            }
        }
    }
}