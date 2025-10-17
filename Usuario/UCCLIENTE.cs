using System;
using System.Data;
using System.Windows.Forms;
using Usuario.Clases;
using System.Collections.Generic;

namespace Usuario
{
    public partial class UCCLIENTE : UserControl
    {
        private ClaseCLIENTE cliente;
        private ClaseConexion clienteConexion;
        private Dictionary<string, object> valoresOriginales;
        private List<TextBox> CamposLetra;
        private List<ComboBox> CombosNumericos;
        private List<TextBox> CamposNumericos;

        public UCCLIENTE()
        {
            InitializeComponent();
            cliente = new ClaseCLIENTE();
            ClaseConexion conexion = new ClaseConexion();
            CargarFiltro();
            CargarClientes("Activos");
            ConfigurarEventosValidacion();

            txtId.TextChanged += ControlModificado;
            CBNumeroIdentidad.TextChanged += ControlModificado;
            txtPrimerNombre.TextChanged += ControlModificado;
            txtSegundoNombre.TextChanged += ControlModificado;
            txtPrimerApellido.TextChanged += ControlModificado;
            txtSegundoApellido.TextChanged += ControlModificado;
            txtTelefono.TextChanged += ControlModificado;
            checkactivo.CheckedChanged += ControlModificado;
            EstadoInicial();
        DiseñoGlobal.RegistrarUserControl(this);
        }

        private void FCLIENTE_Load(object sender, EventArgs e)
        {
            Permisos.AplicarPermisos(this);
            DiseñoGlobal.AplicarTamaño(this);
            CargarClientes(cboFiltroActivo.SelectedItem?.ToString() ?? "Activos");
            var menu = FindForm() as FMENU;
            if (menu != null)
                DiseñoGlobal.AplicarEstiloDataGridView(dataGridViewClientes, menu.temaActual);
            else
                DiseñoGlobal.AplicarEstiloDataGridView(dataGridViewClientes, Temas.Light);
            CamposLetra = new List<TextBox> { txtPrimerNombre, txtSegundoNombre, txtPrimerApellido, txtSegundoApellido };
            CamposNumericos = new List<TextBox> { txtTelefono };
            CombosNumericos = new List<ComboBox> { CBNumeroIdentidad };
            CamposLetra.ForEach(campo => campo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoLetras(ev));
            CamposNumericos.ForEach(campo => campo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoNumerico(ev));
            CombosNumericos.ForEach(combo => combo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoNumerico(ev));
        }

        private void CargarFiltro()
        {
            cboFiltroActivo.Items.Clear();
            cboFiltroActivo.Items.Add("Activos");
            cboFiltroActivo.Items.Add("Inactivos");
            cboFiltroActivo.Items.Add("Todos");
            cboFiltroActivo.SelectedIndex = 0; // Por defecto Activos
            cboFiltroActivo.SelectedIndexChanged += (s, e) =>
            {
                CargarClientes(cboFiltroActivo.SelectedItem.ToString());
            };
        }

        private void CargarClientes(string filtro)
        {
            DataTable dt = ClaseFiltroActivo.FiltrarTabla("CLIENTE", filtro);
            dataGridViewClientes.DataSource = dt;

            if (dataGridViewClientes.Columns.Contains("IDCliente"))
                dataGridViewClientes.Columns["IDCliente"].HeaderText = "ID Cliente";
            if (dataGridViewClientes.Columns.Contains("NumeroIdentidad"))
                dataGridViewClientes.Columns["NumeroIdentidad"].HeaderText = "NumeroIdentidad";
            if (dataGridViewClientes.Columns.Contains("PrimerNombre"))
                dataGridViewClientes.Columns["PrimerNombre"].HeaderText = "Primer Nombre";
            if (dataGridViewClientes.Columns.Contains("NombreCompleto"))
                dataGridViewClientes.Columns["NombreCompleto"].HeaderText = "Nombre Completo";

            dataGridViewClientes.ClearSelection();
            LimpiarCampos();
        }

        private void ConfigurarEventosValidacion()
        {
            CBNumeroIdentidad.KeyPress += (s, e) => ClaseValidacion.ValidarCampoNumerico(e);
            txtPrimerNombre.KeyPress += (s, e) => ClaseValidacion.ValidarCampoLetras(e);
            txtSegundoNombre.KeyPress += (s, e) => ClaseValidacion.ValidarCampoLetras(e);
            txtPrimerApellido.KeyPress += (s, e) => ClaseValidacion.ValidarCampoLetras(e);
            txtSegundoApellido.KeyPress += (s, e) => ClaseValidacion.ValidarCampoLetras(e);
            txtTelefono.KeyPress += (s, e) => ClaseValidacion.ValidarCampoNumerico(e);
        }

        private void LimpiarCampos()
        {
            txtId.Text = "";
            CBNumeroIdentidad.Text = "";
            txtPrimerNombre.Text = "";
            txtSegundoNombre.Text = "";
            txtPrimerApellido.Text = "";
            txtSegundoApellido.Text = "";
            txtTelefono.Text = "";
            checkactivo.Checked = true;
            cliente.Reiniciar();
            EstadoInicial();
        }

        private void EstadoInicial()
        {
            txtId.Enabled = false;
            CBNumeroIdentidad.Enabled = true;
            txtPrimerNombre.Enabled = false;
            txtSegundoNombre.Enabled = false;
            txtPrimerApellido.Enabled = false;
            txtSegundoApellido.Enabled = false;
            txtTelefono.Enabled = false;
            checkactivo.Enabled = false;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnGuardar.Enabled = false;
            btnBuscar.Enabled = true;
            btnClean.Text = "Limpiar";
            valoresOriginales = null;
        }

        private void HabilitarCamposParaNuevoCliente()
        {
            CBNumeroIdentidad.Enabled = true;
            txtPrimerNombre.Enabled = true;
            txtSegundoNombre.Enabled = true;
            txtPrimerApellido.Enabled = true;
            txtSegundoApellido.Enabled = true;
            txtTelefono.Enabled = true;
            checkactivo.Enabled = true;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnGuardar.Enabled = true;
        }

        private void RestaurarValoresOriginales()
        {
            if (valoresOriginales == null)
                return;
            txtId.Text = valoresOriginales["txtId"].ToString();
            CBNumeroIdentidad.Text = valoresOriginales["CBNumeroIdentidad"].ToString();
            txtPrimerNombre.Text = valoresOriginales["txtPrimerNombre"].ToString();
            txtSegundoNombre.Text = valoresOriginales["txtSegundoNombre"].ToString();
            txtPrimerApellido.Text = valoresOriginales["txtPrimerApellido"].ToString();
            txtSegundoApellido.Text = valoresOriginales["txtSegundoApellido"].ToString();
            txtTelefono.Text = valoresOriginales["txtTelefono"].ToString();
            checkactivo.Checked = (bool)valoresOriginales["checkactivo"];
            btnEditar.Enabled = false;
            btnClean.Text = "Limpiar";
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
                CBNumeroIdentidad.Text != valoresOriginales["CBNumeroIdentidad"].ToString() ||
                txtPrimerNombre.Text != valoresOriginales["txtPrimerNombre"].ToString() ||
                txtSegundoNombre.Text != valoresOriginales["txtSegundoNombre"].ToString() ||
                txtPrimerApellido.Text != valoresOriginales["txtPrimerApellido"].ToString() ||
                txtSegundoApellido.Text != valoresOriginales["txtSegundoApellido"].ToString() ||
                txtTelefono.Text != valoresOriginales["txtTelefono"].ToString() ||
                checkactivo.Checked != (bool)valoresOriginales["checkactivo"];

            if (huboCambios)
            {
                btnEditar.Enabled = true;
                btnClean.Text = "Cancelar";
            }
            else
            {
                btnEditar.Enabled = false;
                btnClean.Text = "Limpiar";
            }
        }

        private void dataGridViewClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClientes.CurrentRow == null)
                return;

            var fila = dataGridViewClientes.CurrentRow;
            cliente.Id = Convert.ToInt32(fila.Cells["IDCliente"].Value);
            cliente.NumeroIdentidad = fila.Cells["NumeroIdentidad"].Value?.ToString();
            cliente.PrimerNombre = fila.Cells["PrimerNombre"].Value?.ToString();
            cliente.SegundoNombre = fila.Cells["SegundoNombre"].Value?.ToString() ?? "";
            cliente.PrimerApellido = fila.Cells["PrimerApellido"].Value?.ToString();
            cliente.SegundoApellido = fila.Cells["SegundoApellido"].Value?.ToString() ?? "";
            cliente.NumTel = fila.Cells["NumTel"].Value?.ToString();
            cliente.Activo = Convert.ToBoolean(fila.Cells["Activo"].Value);

            // Mostrar en controles
            txtId.Text = cliente.Id.ToString();
            CBNumeroIdentidad.Text = cliente.NumeroIdentidad;
            txtPrimerNombre.Text = cliente.PrimerNombre;
            txtSegundoNombre.Text = cliente.SegundoNombre;
            txtPrimerApellido.Text = cliente.PrimerApellido;
            txtSegundoApellido.Text = cliente.SegundoApellido;
            txtTelefono.Text = cliente.NumTel;
            checkactivo.Checked = cliente.Activo;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Ejecutar validación antes de guardar
            if (!ValidarCampos())
                return;

            // Asignar valores a cliente
            cliente.NumeroIdentidad = CBNumeroIdentidad.Text.Trim();
            cliente.PrimerNombre = txtPrimerNombre.Text.Trim();
            cliente.SegundoNombre = txtSegundoNombre.Text.Trim();
            cliente.PrimerApellido = txtPrimerApellido.Text.Trim();
            cliente.SegundoApellido = txtSegundoApellido.Text.Trim();
            cliente.NumTel = txtTelefono.Text.Trim();
            cliente.Activo = checkactivo.Checked;

            // Validar duplicado si es nuevo
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                if (cliente.ExistePorNumeroIdentidad(cliente.NumeroIdentidad))
                {
                    MessageBox.Show("Ya existe un cliente con este número de identidad. Use el botón Buscar para editarlo.");
                    return;
                }
            }

            // Guardar o editar
            bool resultado = string.IsNullOrWhiteSpace(txtId.Text)
                ? cliente.Guardar()
                : cliente.Editar();

            if (resultado)
            {
                MessageBox.Show(string.IsNullOrWhiteSpace(txtId.Text)
                    ? "Cliente guardado correctamente."
                    : "Cliente actualizado correctamente.");
                CargarClientes(cboFiltroActivo.SelectedItem.ToString());
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Error al guardar el cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombreCompleto = $"{txtPrimerNombre.Text} {txtPrimerApellido.Text}";

            if (MessageBox.Show($"¿Seguro que desea eliminar al cliente {nombreCompleto}?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cliente.Id = Convert.ToInt32(txtId.Text);
                if (cliente.Eliminar())
                {
                    MessageBox.Show("Cliente eliminado correctamente.");
                    CargarClientes(cboFiltroActivo.SelectedItem.ToString());
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al eliminar cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                dataGridViewClientes.ClearSelection();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            string numeroIdentidad = CBNumeroIdentidad.Text.Trim();

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente para editar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(numeroIdentidad))
            {
                MessageBox.Show("Debe ingresar el número de identidad.");
                return;
            }

            if (numeroIdentidad.Length != 13)
            {
                MessageBox.Show("El número de identidad debe tener exactamente 13 dígitos.");
                return;
            }

            cliente.Id = Convert.ToInt32(txtId.Text);
            cliente.NumeroIdentidad = numeroIdentidad;
            cliente.PrimerNombre = txtPrimerNombre.Text.Trim();
            cliente.SegundoNombre = txtSegundoNombre.Text.Trim();
            cliente.PrimerApellido = txtPrimerApellido.Text.Trim();
            cliente.SegundoApellido = txtSegundoApellido.Text.Trim();
            cliente.NumTel = txtTelefono.Text.Trim();
            cliente.Activo = checkactivo.Checked;

            if (cliente.Editar())
            {
                MessageBox.Show("Cliente actualizado correctamente.");
                CargarClientes(cboFiltroActivo.SelectedItem.ToString());
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Error al actualizar cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string numeroIdentidad = CBNumeroIdentidad.Text.Trim();

            if (string.IsNullOrWhiteSpace(numeroIdentidad))
            {
                MessageBox.Show("Debe ingresar el número de identidad para buscar.");
                return;
            }

            if (numeroIdentidad.Length != 13)
            {
                MessageBox.Show("El número de identidad debe tener exactamente 13 dígitos.");
                return;
            }

            var clienteEncontrado = cliente.BuscarPorNumeroIdentidad(numeroIdentidad);

            if (clienteEncontrado != null)
            {
                txtId.Text = clienteEncontrado.Id.ToString();
                CBNumeroIdentidad.Text = clienteEncontrado.NumeroIdentidad;
                txtPrimerNombre.Text = clienteEncontrado.PrimerNombre;
                txtSegundoNombre.Text = clienteEncontrado.SegundoNombre;
                txtPrimerApellido.Text = clienteEncontrado.PrimerApellido;
                txtSegundoApellido.Text = clienteEncontrado.SegundoApellido;
                txtTelefono.Text = clienteEncontrado.NumTel;
                checkactivo.Checked = clienteEncontrado.Activo;

                txtPrimerNombre.Enabled = true;
                txtSegundoNombre.Enabled = true;
                txtPrimerApellido.Enabled = true;
                txtSegundoApellido.Enabled = true;
                txtTelefono.Enabled = true;
                checkactivo.Enabled = true;

                btnEditar.Enabled = false;
                btnEliminar.Enabled = true;
                btnBuscar.Enabled = false;
                btnGuardar.Enabled = false;
                btnClean.Text = "Limpiar";

                MessageBox.Show("Cliente encontrado. Puede eliminarlo o modificar sus datos.");

                valoresOriginales = new Dictionary<string, object>
                {
                    { "txtId", txtId.Text },
                    { "CBNumeroIdentidad", CBNumeroIdentidad.Text },
                    { "txtPrimerNombre", txtPrimerNombre.Text },
                    { "txtSegundoNombre", txtSegundoNombre.Text },
                    { "txtPrimerApellido", txtPrimerApellido.Text },
                    { "txtSegundoApellido", txtSegundoApellido.Text },
                    { "txtTelefono", txtTelefono.Text },
                    { "checkactivo", checkactivo.Checked }
                };
            }
            else
            {
                HabilitarCamposParaNuevoCliente();
                btnBuscar.Enabled = false;
                btnGuardar.Enabled = true;

                MessageBox.Show("No existe un cliente con este número de identidad. Puede crear uno nuevo.");
            }
        }

        private void cboFiltroActivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filtro = cboFiltroActivo.SelectedItem.ToString();
            DataTable datos = ClaseFiltroActivo.FiltrarTabla("CLIENTE", filtro);
            dataGridViewClientes.DataSource = datos;

            if (dataGridViewClientes.Columns.Contains("IDCliente"))
                dataGridViewClientes.Columns["IDCliente"].HeaderText = "ID Cliente";
            if (dataGridViewClientes.Columns.Contains("PrimerNombre"))
                dataGridViewClientes.Columns["PrimerNombre"].HeaderText = "Primer Nombre";
            if (dataGridViewClientes.Columns.Contains("NombreCompleto"))
                dataGridViewClientes.Columns["NombreCompleto"].HeaderText = "Nombre Completo";
        }
    
    private bool ValidarCampos()
        {
            // Número de identidad
            if (string.IsNullOrWhiteSpace(CBNumeroIdentidad.Text))
            {
                MessageBox.Show("Debe ingresar el número de identidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CBNumeroIdentidad.Focus();
                return false;
            }
            if (CBNumeroIdentidad.Text.Length != 13)
            {
                MessageBox.Show("El número de identidad debe tener exactamente 13 dígitos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CBNumeroIdentidad.Focus();
                return false;
            }

            // Primer nombre
            if (string.IsNullOrWhiteSpace(txtPrimerNombre.Text))
            {
                MessageBox.Show("Debe ingresar el primer nombre.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrimerNombre.Focus();
                return false;
            }

            // Primer apellido
            if (string.IsNullOrWhiteSpace(txtPrimerApellido.Text))
            {
                MessageBox.Show("Debe ingresar el primer apellido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrimerApellido.Focus();
                return false;
            }

         

            // Teléfono
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Debe ingresar el número de teléfono.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return false;
            }

            // Si todo está bien
            return true;
        }

    }
}