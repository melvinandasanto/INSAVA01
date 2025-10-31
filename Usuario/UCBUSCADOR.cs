using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class UCBUSCADOR : UserControl
    {
        private ClaseConexion _conexionDB;

        public UCBUSCADOR()
        {
            InitializeComponent();
            _conexionDB = new ClaseConexion();
            DiseñoGlobal.RegistrarUserControl(this);
            cboFiltroActivo.DropDownStyle = ComboBoxStyle.DropDownList;
            CargarFiltro();

            // Cargar directamente las opciones del combo
            CBOperacion.Items.AddRange(new object[] { "Clientes", "Facturas", "Inventario", "Maquilas", "Movimientos" });
            CBOperacion.SelectedIndex = 0;
        }


        private void UCBUSCADOR_Load(object sender, EventArgs e)
        {
            CBOperacion.Items.Clear();

            CBOperacion.Items.Add("Clientes");
            CBOperacion.Items.Add("Facturas");
            CBOperacion.Items.Add("Inventario");
            CBOperacion.Items.Add("Maquilas");
            CBOperacion.Items.Add("Movimientos");

            if (CBOperacion.Items.Count > 0)
            {
                CBOperacion.SelectedIndex = 0;
            }

            CBOperacion_SelectedIndexChanged(null, null);
        }



        private void CargarClientesCombo()
        {
            try
            {
                DataTable dtClientes = _conexionDB.Tabla("SELECT IDCliente, NombreCompleto FROM VISTAFCLIENTE", null);
                cmbclientes.DataSource = dtClientes;
                cmbclientes.DisplayMember = "NombreCompleto";
                cmbclientes.ValueMember = "IDCliente";
                cmbclientes.SelectedIndex = -1; // Ningún cliente seleccionado por defecto
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message);
            }
        }

        private void CargarOperaciones()
        {
            CBOperacion.Items.Clear();
            CBOperacion.Items.Add("Nombre o Teléfono");
            CBOperacion.Items.Add("ID Cliente");
            CBOperacion.SelectedIndex = 0;
        }

        private void cmbclientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbclientes.SelectedIndex >= 0)
            {
                string idCliente = cmbclientes.SelectedValue.ToString();
                BuscarClientePorID(idCliente);
            }
        }

        private void BtnBuscarcli_Click(object sender, EventArgs e)
        {
            if (CBOperacion.SelectedItem == null) return;
            if (cboFiltroActivo.SelectedItem == null) cboFiltroActivo.SelectedIndex = 2; // asegurar 'Todos' si es nulo

            string searchTerm = cmbclientes.Text?.Trim() ?? "";
            if (searchTerm.StartsWith("Escribe") || searchTerm.StartsWith("Buscar") || searchTerm.StartsWith("Reporte"))
                searchTerm = "";

            DataTable dt = new DataTable();
            List<SqlParameter> parametros = new List<SqlParameter>();
            string estadoFiltro = cboFiltroActivo.SelectedItem.ToString();
            string query = "";

            try
            {
                string operacion = CBOperacion.SelectedItem.ToString();

                if (operacion == "Clientes")
                {
                    // Usamos la vista VISTAFCLIENTE (que en tu script sí contiene IDCliente, NombreCompleto, NumeroIdentidad, Telefono, Activo)
                    query = "SELECT IDCliente, NumeroIdentidad AS Identidad, NombreCompleto, Telefono, Activo FROM VISTAFCLIENTE";

                    List<string> condiciones = new List<string>();

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        condiciones.Add("(NombreCompleto LIKE @busqueda OR NumeroIdentidad LIKE @busqueda OR Telefono LIKE @busqueda)");
                        parametros.Add(new SqlParameter("@busqueda", SqlDbType.NVarChar) { Value = "%" + searchTerm + "%" });
                    }

                    if (estadoFiltro != "Todos")
                    {
                        // En VISTAFCLIENTE la columna se llama Activo (bit)
                        condiciones.Add("Activo = @activo");
                        parametros.Add(new SqlParameter("@activo", SqlDbType.Bit) { Value = estadoFiltro == "Activos" });
                    }

                    if (condiciones.Count > 0)
                        query += " WHERE " + string.Join(" AND ", condiciones);
                }
                else if (operacion == "Facturas")
                {
                    query = "SELECT NumeroFactura, FechaEntrada, Cliente, MontoTotal, TipoTransaccion, MetodoPago FROM VISTA_BUSCADOR_FACTURAS_RESUMEN";
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " WHERE Cliente LIKE @busqueda OR NumeroFactura LIKE @busqueda";
                        parametros.Add(new SqlParameter("@busqueda", SqlDbType.NVarChar) { Value = "%" + searchTerm + "%" });
                    }
                }
                else if (operacion == "Inventario")
                {
                    query = "SELECT Producto, Categoria, StockActual, PrecioUnitario, ValorInventario FROM VISTA_INVENTARIO_VALORIZADO";
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " WHERE Producto LIKE @busqueda OR Categoria LIKE @busqueda";
                        parametros.Add(new SqlParameter("@busqueda", SqlDbType.NVarChar) { Value = "%" + searchTerm + "%" });
                    }
                }
                else if (operacion == "Maquilas")
                {
                    // Usa la vista que sí existe y tiene la columna EstadoMaquila
                    query = "SELECT NumeroMaquila, Cliente, OrigenSemilla, Producto, CantidadMaquilada, MontoMaquila, FechaEntrega, EstadoMaquila FROM VISTA_MAQUILA_ESTADO_DETALLE";
                    List<string> condiciones = new List<string>();

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        condiciones.Add("(NumeroMaquila LIKE @busqueda OR Cliente LIKE @busqueda OR Producto LIKE @busqueda)");
                        parametros.Add(new SqlParameter("@busqueda", SqlDbType.NVarChar) { Value = "%" + searchTerm + "%" });
                    }

                    if (estadoFiltro != "Todos")
                    {
                        // 🔹 Mapeo del filtro
                        // "Activos" → EstadoMaquila = 'Activa'
                        // "Inactivos" → EstadoMaquila = 'Desactivada manualmente'
                        string estadoBuscado = estadoFiltro == "Activos" ? "Activa" : "Desactivada manualmente";
                        condiciones.Add("EstadoMaquila = @estado");
                        parametros.Add(new SqlParameter("@estado", SqlDbType.NVarChar) { Value = estadoBuscado });
                    }

                    if (condiciones.Count > 0)
                        query += " WHERE " + string.Join(" AND ", condiciones);
                }
                else if (operacion == "Movimientos")
                {
                    query = "SELECT FechaMovimiento, Producto, TipoMovimiento, CantidadMovida, Descripcion, TransaccionRef FROM VISTA_MOVIMIENTOS_POR_PRODUCTO_AUDITORIA";
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " WHERE Producto LIKE @busqueda OR TipoMovimiento LIKE @busqueda OR Descripcion LIKE @busqueda";
                        parametros.Add(new SqlParameter("@busqueda", SqlDbType.NVarChar) { Value = "%" + searchTerm + "%" });
                    }

                    // Nota: la vista de movimientos en tu script no expone un campo 'Estado'. Si quieres filtrar por Activo,
                    // deberías modificar la vista VISTA_MOVIMIENTOS_POR_PRODUCTO_AUDITORIA para incluir m.Activo o similar.
                }
                else
                {
                    MessageBox.Show("Seleccione una operación válida o configure su búsqueda.");
                    return;
                }

                dt = _conexionDB.Tabla(query, parametros.ToArray());
                dgvcliente.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar la búsqueda: {ex.Message}");
            }
        }


        private void btnLimpiarcli_Click(object sender, EventArgs e)
        {
            cmbclientes.Text = string.Empty;
            dgvcliente.DataSource = null;
        }


        private void CBOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Limpiar resultados anteriores
            cmbclientes.Text = string.Empty;
            dgvcliente.DataSource = null;
            cmbclientes.DataSource = null;

            if (CBOperacion.SelectedItem == null) return;

            string operacion = CBOperacion.SelectedItem.ToString();

            // Por defecto dejamos el filtro habilitado solo si aplica (Clientes, Maquilas, Movimientos)
            cboFiltroActivo.SelectedIndex = Math.Min(Math.Max(cboFiltroActivo.SelectedIndex, 0), cboFiltroActivo.Items.Count - 1);

            switch (operacion)
            {
                case "Clientes":
                    cmbclientes.Text = "Escribe Nombre, Identidad o Teléfono...";
                    cboFiltroActivo.Enabled = true;
                    break;

                case "Facturas":
                    cmbclientes.Text = "Escribe Número de Factura o Nombre del Cliente...";
                    cboFiltroActivo.Enabled = false;
                    break;

                case "Inventario":
                    cmbclientes.Text = "Buscar Producto o Categoría (Semilla, Producto)...";
                    cboFiltroActivo.Enabled = false;
                    break;

                case "Maquilas":
                    cmbclientes.Text = "Buscar por Maquila #, Cliente o Producto...";
                    cboFiltroActivo.Enabled = true; // tu vista de maquilas tiene Estado/EstadoMaquila si quieres mapear
                    break;

                case "Movimientos":
                    cmbclientes.Text = "Buscar por Producto, Tipo de Movimiento o Descripción...";
                    cboFiltroActivo.Enabled = true; // si quieres filtrar por estado, revisa que vista devuelva campo
                    break;

                default:
                    cmbclientes.Text = string.Empty;
                    cboFiltroActivo.Enabled = false;
                    break;
            }
        }


        private void BuscarClientePorID(string idCliente)
        {
            if (string.IsNullOrWhiteSpace(idCliente))
            {
                dgvcliente.DataSource = null;
                return;
            }

            string query = "SELECT * FROM VISTAFCLIENTE WHERE IDCliente = @id";
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id", SqlDbType.NVarChar) { Value = idCliente }
            };

            try
            {
                DataTable dt = _conexionDB.Tabla(query, parametros.ToArray());
                dgvcliente.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente: " + ex.Message);
            }
        }

        private void BuscarClientePorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                dgvcliente.DataSource = null;
                return;
            }

            string query = "SELECT * FROM VISTAFCLIENTE WHERE NombreCompleto LIKE @nombre";
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@nombre", SqlDbType.NVarChar) { Value = "%" + nombre + "%" }
            };

            try
            {
                DataTable dt = _conexionDB.Tabla(query, parametros.ToArray());
                dgvcliente.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente: " + ex.Message);
            }
        }

        private void dgvcliente_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvcliente.Rows.Count > 0)
            {
                string idCliente = dgvcliente.Rows[e.RowIndex].Cells["IDCliente"].Value.ToString();
                MessageBox.Show("Cliente seleccionado: " + idCliente);
            }
        }
        private void CargarSugerencias()
        {
            var dtClientes = _conexionDB.Tabla("SELECT NombreCompleto FROM VISTAFCLIENTE");
            AutoCompleteStringCollection coleccion = new AutoCompleteStringCollection();

            foreach (DataRow row in dtClientes.Rows)
                coleccion.Add(row["NombreCompleto"].ToString());

            cmbclientes.AutoCompleteCustomSource = coleccion;
        }


        public void IniciarModoClientes(bool soloClientesYFacturas = false)
        {
            CBOperacion.Items.Clear();

            if (soloClientesYFacturas)
            {
                CBOperacion.Items.Add("Clientes");
                CBOperacion.Items.Add("Facturas");
            }
            else
            {
                CBOperacion.Items.Add("Clientes");
                CBOperacion.Items.Add("Facturas");
                CBOperacion.Items.Add("Inventario");
                CBOperacion.Items.Add("Maquilas");
                CBOperacion.Items.Add("Movimientos");
            }

            CBOperacion.SelectedIndex = 0;
            cmbclientes.Text = string.Empty;
            dgvcliente.DataSource = null;
        }



        private void cboFiltroActivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si hay texto en el buscador, ejecutar la búsqueda con el nuevo filtro
            if (!string.IsNullOrEmpty(cmbclientes.Text.Trim()))
            {
                BtnBuscarcli_Click(sender, e);
            }
        }

        private void CargarFiltro()
        {
            cboFiltroActivo.Items.Clear();
            cboFiltroActivo.Items.Add("Activos");
            cboFiltroActivo.Items.Add("Inactivos");
            cboFiltroActivo.Items.Add("Todos");
            cboFiltroActivo.SelectedIndex = 0;
        }

    }
}

