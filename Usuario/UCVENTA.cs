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
        private DataGridView dgvMaquila;
        private List<TextBox> CamposDecimales;

        public UCVENTA()
        {
            InitializeComponent();
            conexion = new ClaseConexion();
            DiseñoGlobal.RegistrarUserControl(this);
        
         // Inicializa el DataGridView de maquila
           dgvMaquila = new DataGridView();
            dgvMaquila.Name = "dgvMaquila";
            dgvMaquila.Visible = false;
            dgvMaquila.Anchor = dgvPedido.Anchor;
            dgvMaquila.Size = dgvPedido.Size;
            dgvMaquila.Location = dgvPedido.Location;
            panel3.Controls.Add(dgvMaquila);

            // Selecciona por defecto "Producto/Semilla" si existe en los ítems
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
            ConfigurarDataGridViewMaquila();
            DiseñoGlobal.AplicarEstiloDataGridView(dgvPedido, Temas.Light); // SIEMPRE inicia en Light
            DiseñoGlobal.AplicarEstiloDataGridView(dgvMaquila, Temas.Light);
            CBOperacion.SelectedIndexChanged += CBOperacion_SelectedIndexChanged;
            dgvMaquila.CellClick += DgvMaquila_CellClick;
            dgvMaquila.CellEndEdit += dgvMaquila_CellEndEdit;
            CamposDecimales = new List<TextBox>
            {
                txtPrecio,
                txtStockoporcent,
                txtMaquila
            };
            CamposDecimales.ForEach(campo => {
                // Validación para solo permitir caracteres válidos de decimal
                campo.KeyPress += (s, ev) => {
                    ClaseValidacion.ValidarCampoDecimal(ev, campo);
                };
                // Validación para no permitir valores negativos
                campo.Validating += (s, ev) => {
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
                DiseñoGlobal.AplicarEstiloDataGridView(dgvMaquila, menu.temaActual);
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

        private void ConfigurarDataGridViewMaquila()
        {
            dgvMaquila.Columns.Clear();
            dgvMaquila.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMaquila.Columns.Add("IDProducto", "IDProducto");
            dgvMaquila.Columns["IDProducto"].Visible = false;
            dgvMaquila.Columns.Add("Producto", "Producto");
            dgvMaquila.Columns.Add("PorcentajeGerminacion", "Germinación");
            dgvMaquila.Columns.Add("Cantidad", "Cantidad");
            dgvMaquila.Columns.Add("SemillasRealistas", "Plantulas");
            dgvMaquila.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgvMaquila.Columns.Add("Total", "Total");
            DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn
            {
                HeaderText = "Eliminar",
                Name = "Eliminar",
                Text = "X",
                UseColumnTextForButtonValue = true
            };
            dgvMaquila.Columns.Add(btnEliminar);
            dgvMaquila.AllowUserToAddRows = false;
            dgvMaquila.ReadOnly = false;
            dgvMaquila.Columns["Cantidad"].ReadOnly = false;
        }

        private void CargarProductosCombo()
        {
            string sql;
            // Validar el modo de operación
            if (CBOperacion.SelectedItem != null && CBOperacion.SelectedItem.ToString() == "Maquila")
            {
                sql = "SELECT IDProducto, Producto, Categoria, PrecioUnitario FROM VISTAPRODUCTOS WHERE Categoria IN ('Semilla', 'Semilla maquilada')";
            }
            else
            {
                // Incluye todas las categorías relevantes, incluyendo 'Semilla maquilada'
                sql = "SELECT IDProducto, Producto, Categoria, PrecioUnitario FROM VISTAPRODUCTOS";
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
                dgvPedido.Visible = true;
                dgvMaquila.Visible = false;
                OcultarControlesMaquila(); // <-- Agrega esta línea
                CargarProductosCombo();
                rbStock.Checked = false;
                radioButton1.Checked = false;
                txtStockoporcent.ReadOnly = true;
                txtStockoporcent.Enabled = true;
                txtPrecio.ReadOnly = true;
            }
            else if (operacion == "Maquila")
            {
                dgvPedido.Visible = false;
                dgvMaquila.Visible = true;
                rbStock.Visible = true;
                radioButton1.Visible = true;
                CargarSemillasCombo();
                rbStock.Checked = true; // Selecciona por defecto "Stock"
                // Forzar evento CheckedChanged para aplicar lógica
                RbStock_CheckedChanged(rbStock, EventArgs.Empty);
                MostrarControlesMaquila();
            }
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
            if (radioButton1.Checked)
            {
                txtStockoporcent.ReadOnly = false;
                txtStockoporcent.Enabled = true;
                txtPrecio.ReadOnly = true;
                txtPrecio.Visible = false;
                lblPrecio.Visible = false;
                if (OperacionEsMaquila())
                {
                    lblestock.Text = "%";
                    txtStockoporcent.Text = "";
                }
            }
        }

        private void CargarSemillasCombo()
        {
            string sql = "SELECT IDProducto, Producto, Categoria, PrecioUnitario FROM VISTAPRODUCTOS WHERE Categoria IN ('Semilla', 'Semilla maquilada')";
            productosOriginal = conexion.Tabla(sql);
            cmbProducto.DataSource = productosOriginal.Copy();
            cmbProducto.DisplayMember = "Producto";
            cmbProducto.ValueMember = "IDProducto";
            cmbProducto.SelectedIndex = -1;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string nombre = cmbProducto.Text.Trim();

            bool esMaquila = CBOperacion.SelectedItem != null && CBOperacion.SelectedItem.ToString() == "Maquila";
            bool esMaquilaCliente = esMaquila && radioButton1.Checked;
            bool esMaquilaStock = esMaquila && rbStock.Checked;

            // Si hay selección válida, busca normalmente
            if (cmbProducto.Items.Count > 0 && cmbProducto.SelectedIndex >= 0 && cmbProducto.SelectedValue != null)
            {
                int idProducto = Convert.ToInt32(cmbProducto.SelectedValue);
                string sql = "SELECT Cantidad AS Stock, PrecioUnitario, PorcentajeGerminacion FROM PRODUCTO WHERE IDProducto=@id";
                SqlParameter[] parametros = { new SqlParameter("@id", idProducto) };
                DataTable dt = conexion.Tabla(sql, parametros);

                if (dt.Rows.Count > 0)
                {
                    if (esMaquilaCliente)
                    {
                        lblestock.Text = "%";
                        txtStockoporcent.ReadOnly = false;
                        txtStockoporcent.Enabled = true;
                        txtPrecio.Visible = false;
                        lblPrecio.Visible = false;
                        txtStockoporcent.Text = dt.Rows[0]["PorcentajeGerminacion"].ToString();
                        nudCantidad.Maximum = 1000000; // Sin límite real para maquila cliente
                        nudCantidad.Value = 0;
                        return;
                    }
                    if (esMaquilaStock)
                    {
                        lblestock.Text = "Stock";
                        txtStockoporcent.ReadOnly = true;
                        txtStockoporcent.Enabled = true;
                        txtStockoporcent.Text = dt.Rows[0]["Stock"].ToString();
                        txtPrecio.Visible = true;
                        lblPrecio.Visible = true;
                        txtPrecio.Text = dt.Rows[0]["PrecioUnitario"].ToString();
                        txtMaquila.Enabled = true;

                        // Asignar el máximo del NumericUpDown según el stock
                        decimal stockDisponible = 0;
                        decimal.TryParse(dt.Rows[0]["Stock"].ToString().Replace('.', ','), out stockDisponible);
                        nudCantidad.Maximum = stockDisponible > 0 ? stockDisponible : 1;
                        nudCantidad.Value = 0;
                        return;
                    }
                    // Producto/Semilla
                    lblestock.Text = "Stock";
                    txtStockoporcent.Text = dt.Rows[0]["Stock"].ToString();
                    txtStockoporcent.ReadOnly = true;
                    txtStockoporcent.Enabled = false;
                    txtPrecio.Visible = true;
                    lblPrecio.Visible = true;
                    txtPrecio.Text = dt.Rows[0]["PrecioUnitario"].ToString();
                    txtMaquila.Enabled = esMaquila;

                    // Asignar el máximo del NumericUpDown según el stock
                    decimal stockDisponible2 = 0;
                    decimal.TryParse(dt.Rows[0]["Stock"].ToString().Replace('.', ','), out stockDisponible2);
                    nudCantidad.Maximum = stockDisponible2 > 0 ? stockDisponible2 : 1;
                    nudCantidad.Value = 0;
                    return;
                }
            }

            // Lógica para crear producto nuevo solo aplica en maquila/cliente
            if (esMaquilaCliente && !string.IsNullOrWhiteSpace(nombre))
            {
                lblestock.Text = "%";
                txtStockoporcent.ReadOnly = false;
                txtStockoporcent.Enabled = true;
                txtStockoporcent.Text = "";

                lblPrecio.Visible = false;
                txtPrecio.Visible = false;

                txtMaquila.Enabled = true;
                txtMaquila.ReadOnly = false;
                txtMaquila.Text = "";

                nudCantidad.Maximum = 1000000; // Sin límite real para maquila cliente
                nudCantidad.Value = 0;
                return;
            }

            MessageBox.Show("Producto no encontrado o seleccione un producto válido.");
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cmbProducto.SelectedIndex = -1;
            txtStockoporcent.Clear();
            txtPrecio.Clear();
            nudCantidad.Value = 0;
        }

        private void ActualizarTotalPedidos()
        {
            int totalPedidos = dgvPedido.Rows.Count + dgvMaquila.Rows.Count;
            txtCantidadPedido.Text = totalPedidos.ToString();
        }

        private void ActualizarTotalesGenerales()
        {
            // Total de venta: suma de subtotales y totales de ambos grids
            decimal totalVenta = 0;
            foreach (DataGridViewRow row in dgvPedido.Rows)
            {
                totalVenta += Convert.ToDecimal(row.Cells["Subtotal"].Value);
            }
            foreach (DataGridViewRow row in dgvMaquila.Rows)
            {
                decimal total = (row.Cells["Total"].Value == DBNull.Value || row.Cells["Total"].Value == null)
                    ? 0
                    : Convert.ToDecimal(row.Cells["Total"].Value);
                totalVenta += total;
            }

            // Total de pedidos: suma de filas de ambos grids
            int totalPedidos = dgvPedido.Rows.Count + dgvMaquila.Rows.Count;

            txtCantidadPedido.Text = totalPedidos.ToString();
            txtTotalPagar.Text = totalVenta.ToString("N2");
        }

        private void ActualizarTotales()
        {
            ActualizarTotalesGenerales();
        }

        private void ActualizarTotalesMaquila()
        {
            ActualizarTotalesGenerales();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            txtMaquila.Enabled = true;

            string nombreProducto = cmbProducto.Text.Trim();
            string categoria = "";
            decimal precioUnitario = 0;
            decimal porcentajeGerminacion = 1;
            int cantidad = (int)nudCantidad.Value;

            bool esMaquila = CBOperacion.SelectedItem != null && CBOperacion.SelectedItem.ToString() == "Maquila";
            bool esStock = !esMaquila || (esMaquila && rbStock.Checked);

            // --- VALIDACIÓN DE STOCK ---
            if (esStock)
            {
                decimal stockDisponible = 0;
                decimal.TryParse(txtStockoporcent.Text.Replace('.', ','), out stockDisponible);
                if (cantidad > stockDisponible)
                {
                    MessageBox.Show("No puede agregar una cantidad mayor al stock disponible.");
                    return;
                }
            }
            // --- FIN VALIDACIÓN DE STOCK ---

            DataRowView row = cmbProducto.SelectedItem as DataRowView;
            bool productoExiste = (row != null);
            decimal precioMaquila = 0;

            if (esMaquila && radioButton1.Checked) // Maquila Cliente
            {
                // --- Validación de porcentaje ---
                string textoPorcentaje = txtStockoporcent.Text.Trim().Replace('.', ',');
                if (!decimal.TryParse(textoPorcentaje, out porcentajeGerminacion) || porcentajeGerminacion <= 0)
                {
                    MessageBox.Show("Ingrese un porcentaje de germinación válido (mayor a 0).");
                    return;
                }
                if (porcentajeGerminacion > 1) porcentajeGerminacion = porcentajeGerminacion / 100;

                if (!decimal.TryParse(txtMaquila.Text.Replace('.', ','), out precioMaquila) || precioMaquila < 0)
                {
                    MessageBox.Show("Ingrese un precio de maquila válido.");
                    return;
                }
                int semillasRealistas = (int)Math.Round(cantidad * porcentajeGerminacion);
                decimal totalMaquila = cantidad * precioMaquila;
                dgvMaquila.Rows.Add(
                    DBNull.Value, // no hay IDProducto en maquila cliente
                    nombreProducto,
                    porcentajeGerminacion,
                    cantidad,
                    semillasRealistas,
                    precioMaquila,
                    totalMaquila
                );
                ActualizarTotalesMaquila();
            }
            else if (esMaquila && productoExiste)
            {
                int idProducto = Convert.ToInt32(row["IDProducto"]);
                categoria = row["Categoria"].ToString();
                decimal.TryParse(row["PrecioUnitario"].ToString(), out precioUnitario);

                // Calcula porcentaje germinación
                string textoPorcentaje = txtStockoporcent.Text.Trim().Replace('.', ',');
                if (!decimal.TryParse(textoPorcentaje, out porcentajeGerminacion) || porcentajeGerminacion <= 0)
                    porcentajeGerminacion = 1;
                if (porcentajeGerminacion > 1) porcentajeGerminacion = porcentajeGerminacion / 100;

                int semillasRealistas = (int)Math.Round(cantidad * porcentajeGerminacion);
                decimal.TryParse(txtMaquila.Text.Replace('.', ','), out precioMaquila);
                if (precioMaquila < 0) precioMaquila = 0;

                decimal totalVenta = cantidad * precioUnitario;
                decimal totalMaquila = cantidad * precioMaquila;

                dgvMaquila.Rows.Add(idProducto, nombreProducto + " (Maquila)", porcentajeGerminacion, cantidad, semillasRealistas, precioMaquila, totalMaquila);
                ActualizarTotalesMaquila();
            }
            else
            {
                // Venta normal
                if (row != null)
                {
                    int idProducto = Convert.ToInt32(row["IDProducto"]);
                    categoria = row["Categoria"].ToString();
                    decimal.TryParse(row["PrecioUnitario"].ToString(), out precioUnitario);

                    decimal subtotal = cantidad * precioUnitario;

                    // Aquí guardamos el IDProducto real en la fila
                    dgvPedido.Rows.Add(idProducto, nombreProducto, categoria, cantidad, precioUnitario, subtotal);
                    ActualizarTotales();
                }
                else
                {
                    MessageBox.Show("Seleccione un producto válido.");
                }
            }

            nudCantidad.Value = 0;
            txtMaquila.Text = "";
        }



        private void DgvPedido_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvPedido.Columns["Cantidad"].Index)
            {
                // Validar y actualizar subtotal
                DataGridViewRow fila = dgvPedido.Rows[e.RowIndex];
                int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                decimal precioUnitario = Convert.ToDecimal(fila.Cells["PrecioUnitario"].Value);
                fila.Cells["Subtotal"].Value = cantidad * precioUnitario;

                // Actualizar totales
                ActualizarTotales();
            }
        }

        private void dgvMaquila_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvMaquila.Columns["Cantidad"].Index)
            {
                // Validar y actualizar total
                DataGridViewRow fila = dgvMaquila.Rows[e.RowIndex];
                int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                decimal precioUnitario = Convert.ToDecimal(fila.Cells["PrecioUnitario"].Value);
                decimal porcentajeGerminacion = Convert.ToDecimal(fila.Cells["PorcentajeGerminacion"].Value);
                int semillasRealistas = (int)Math.Round(cantidad * porcentajeGerminacion);
                fila.Cells["SemillasRealistas"].Value = semillasRealistas;
                fila.Cells["Total"].Value = cantidad * precioUnitario;

                // Actualizar totales
                ActualizarTotalesMaquila();
            }
        }

        private void DgvMaquila_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvMaquila.Columns["Eliminar"].Index)
            {
                // Eliminar fila de maquila
                dgvMaquila.Rows.RemoveAt(e.RowIndex);
                ActualizarTotalesMaquila();
            }
        }

        private void DgvPedido_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvPedido.Columns["Eliminar"].Index)
            {
                // Eliminar fila de pedido
                dgvPedido.Rows.RemoveAt(e.RowIndex);
                ActualizarTotales();
            }
        }

        private void btnCancelarPedidos_Click(object sender, EventArgs e)
        {
            // Limpia el DataGridView de pedidos
            dgvPedido.Rows.Clear();
            // Limpia el DataGridView de maquila
            dgvMaquila.Rows.Clear();
            // Limpia los totales
            txtCantidadPedido.Text = "0";
            txtTotalPagar.Text = "0.00";
        }

        private void CmbProducto_KeyUp(object sender, KeyEventArgs e)
        {
            if (productosOriginal == null || productosOriginal.Rows.Count == 0)
                return;

            string textoActual = cmbProducto.Text; // Guarda el texto actual
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

            cmbProducto.Text = textoActual; // Restaura el texto
            cmbProducto.SelectionStart = posCursor; // Restaura la posición del cursor
            cmbProducto.DroppedDown = true;
        }

        // Método para ocultar controles de maquila
        private void OcultarControlesMaquila()
        {
            lblMaquila.Visible = false;
            txtMaquila.Visible = false;
            radioButton1.Visible = false;
            rbStock.Visible = false;
        }

        // Método para mostrar controles de maquila
        private void MostrarControlesMaquila()
        {
            lblMaquila.Visible = true;
            txtMaquila.Visible = true;
            txtMaquila.ReadOnly = false; // <-- Asegura que siempre sea editable
            radioButton1.Visible = true;
            rbStock.Visible = true;
            rbStock.Checked = true;         // Activa por defecto el de stock
            radioButton1.Checked = false;   // Desactiva el otro
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            txtMaquila.Enabled = true; // Siempre habilitado
            // Si quieres limpiar el campo al seleccionar, puedes dejar:
            // txtMaquila.Text = "";
        }
        // Devuelve true si la operación seleccionada es "Maquila"
        private bool OperacionEsMaquila()
        {
            return CBOperacion.SelectedItem != null && CBOperacion.SelectedItem.ToString() == "Maquila";
        }

        // Devuelve un DataTable con el detalle de los pedidos (solo productos, no maquila)
        public DataTable ObtenerDetallePedidos()
        {
            DataTable detalle = new DataTable();
            detalle.Columns.Add("IDProducto", typeof(int));
            detalle.Columns.Add("Producto", typeof(string));
            detalle.Columns.Add("Cantidad", typeof(decimal));
            detalle.Columns.Add("PrecioUnitario", typeof(decimal));
            detalle.Columns.Add("Subtotal", typeof(decimal));
            detalle.Columns.Add("TipoOperacion", typeof(string));

            // --- Pedidos normales (Venta) ---
            foreach (DataGridViewRow row in dgvPedido.Rows)
            {
                if (row.IsNewRow) continue;

                int idProducto = row.Cells["IDProducto"].Value != null && row.Cells["IDProducto"].Value != DBNull.Value
                    ? Convert.ToInt32(row.Cells["IDProducto"].Value)
                    : 0;

                detalle.Rows.Add(
                    idProducto,
                    row.Cells["Producto"].Value?.ToString() ?? "",
                    row.Cells["Cantidad"].Value != null ? Convert.ToDecimal(row.Cells["Cantidad"].Value) : 0,
                    row.Cells["PrecioUnitario"].Value != null ? Convert.ToDecimal(row.Cells["PrecioUnitario"].Value) : 0,
                    row.Cells["Subtotal"].Value != null ? Convert.ToDecimal(row.Cells["Subtotal"].Value) : 0,
                    "Venta"
                );
            }

            // --- Pedidos de maquila ---
            foreach (DataGridViewRow row in dgvMaquila.Rows)
            {
                if (row.IsNewRow) continue;

                int idProducto = row.Cells["IDProducto"].Value != null && row.Cells["IDProducto"].Value != DBNull.Value
                    ? Convert.ToInt32(row.Cells["IDProducto"].Value)
                    : 0;

                string producto = row.Cells["Producto"].Value?.ToString() ?? "";
                decimal precioUnitario = row.Cells["PrecioUnitario"].Value != null ? Convert.ToDecimal(row.Cells["PrecioUnitario"].Value) : 0;
                decimal total = row.Cells["Total"].Value != null ? Convert.ToDecimal(row.Cells["Total"].Value) : 0;
                decimal cantidad = row.Cells["Cantidad"].Value != null ? Convert.ToDecimal(row.Cells["Cantidad"].Value) : 0;

                string tipoOperacion = producto.Contains("(Maquila)") ? "Maquila" : "Venta";

                detalle.Rows.Add(idProducto, producto, cantidad, precioUnitario, total, tipoOperacion);
            }

            return detalle;
        }


        // Limpia los pedidos y actualiza los totales
        public void LimpiarPedidos()
        {
            dgvPedido.Rows.Clear();
            ActualizarTotales();
        }



        // Métodos de configuración de DataGridView, carga de productos, búsqueda, agregar productos, etc...
        // (se dejan tal cual ya los tenías)

        // =======================
        // NUEVO: Botón Cobrar
        // =======================
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (dgvPedido.Rows.Count == 0 && dgvMaquila.Rows.Count == 0)
            {
                MessageBox.Show("No hay productos o maquilas para cobrar.");
                return;
            }

            DataTable detalle = ObtenerDetallePedidos();

            string cliente = "CONSUMIDOR FINAL";
            int numeroOrden = ObtenerNumeroOrden();

            FACTURA frmFactura = new FACTURA(numeroOrden, cliente, detalle); // <-- ahora sin usuario
            if (frmFactura.ShowDialog() == DialogResult.OK)
            {
                LimpiarPedidos();
                dgvMaquila.Rows.Clear();
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

       
    }
}