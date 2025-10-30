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
            
            // Configurar el combo de filtro
            cboFiltroActivo.DropDownStyle = ComboBoxStyle.DropDownList; // Forzar selección de lista
            CargarFiltro(); // Cargar las opciones inicialmente
        }

        private void UCBUSCADOR_Load(object sender, EventArgs e)
        {
            CBOperacion.Items.Clear();
            CBOperacion.Items.Add("Clientes");
            CBOperacion.Items.Add("Facturas");
            CBOperacion.SelectedIndex = 0; // Selecciona Clientes por defecto

            // Asegurar que el filtro esté inicializado
            if (cboFiltroActivo.Items.Count == 0)
            {
                CargarFiltro();
            }
            
            // Texto inicial en el ComboBox de búsqueda
            cmbclientes.Text = "Escriba el nombre o ID del cliente...";
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
            string searchTerm = cmbclientes.Text?.Trim() ?? "";
            // Si el texto es guía, considerarlo como búsqueda vacía
            if (searchTerm == "Escriba el nombre o ID del cliente..." ||
                searchTerm == "Escribe el nombre o ID del cliente" ||
                searchTerm == "Escribe número de factura o cliente")
            {
                searchTerm = "";
            }

            DataTable dt = new DataTable();
            List<SqlParameter> parametros = new List<SqlParameter>();
            string estadoFiltro = cboFiltroActivo.SelectedItem?.ToString() ?? "Activos";
            string query;

            if (CBOperacion.SelectedItem.ToString() == "Clientes")
            {
                query = "SELECT * FROM VISTAFCLIENTE";
                
                // Construir WHERE según búsqueda y filtro
                List<string> condiciones = new List<string>();
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    condiciones.Add("(NombreCompleto LIKE @busqueda OR NumeroIdentidad LIKE @busqueda)");
                    parametros.Add(new SqlParameter("@busqueda", SqlDbType.NVarChar) { Value = "%" + searchTerm + "%" });
                }

                if (estadoFiltro != "Todos")
                {
                    condiciones.Add("Activo = @activo");
                    parametros.Add(new SqlParameter("@activo", SqlDbType.Bit) { Value = estadoFiltro == "Activos" });
                }

                if (condiciones.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", condiciones);
                }
            }
            else if (CBOperacion.SelectedItem.ToString() == "Facturas")
            {
                query = "SELECT * FROM VISTAFACTURA";
                
                // Construir WHERE según búsqueda y filtro
                List<string> condiciones = new List<string>();
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    condiciones.Add("(NombreCompletoCliente LIKE @busqueda OR NumeroFactura LIKE @busqueda)");
                    parametros.Add(new SqlParameter("@busqueda", SqlDbType.NVarChar) { Value = "%" + searchTerm + "%" });
                }

                if (estadoFiltro != "Todos")
                {
                    condiciones.Add("Activo = @activo");
                    parametros.Add(new SqlParameter("@activo", SqlDbType.Bit) { Value = estadoFiltro == "Activos" });
                }

                if (condiciones.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", condiciones);
                }
            }
            else
            {
                MessageBox.Show("Seleccione una operación válida");
                return;
            }

            try
            {
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
            // Limpia la búsqueda y el DataGridView al cambiar la opción
            cmbclientes.Text = "";
            dgvcliente.DataSource = null;

            // Opcional: puedes cambiar el texto que se muestra en la ComboBox como guía
            if (CBOperacion.SelectedItem.ToString() == "Clientes")
            {
                cmbclientes.Items.Clear();
                cmbclientes.Items.Add("Escribe el nombre o ID del cliente"); // guía temporal
            }
            else if (CBOperacion.SelectedItem.ToString() == "Facturas")
            {
                cmbclientes.Items.Clear();
                cmbclientes.Items.Add("Escribe número de factura o cliente"); // guía temporal
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


        public void IniciarModoClientes()
        {
            CBOperacion.Items.Clear();
            CBOperacion.Items.Add("Clientes");
            CBOperacion.Items.Add("Facturas");
            CBOperacion.SelectedIndex = 0; // Selecciona Clientes por defecto

            cmbclientes.Text = string.Empty; // limpia el ComboBox
            dgvcliente.DataSource = null;    // limpia el DataGridView
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

