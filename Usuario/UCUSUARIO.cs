using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Usuario.Clases;

namespace Usuario
{
    public partial class UCUSUARIO : UserControl
    {
        private ClaseConexion conexion;
        private ClaseUSUARIO usuario;
        private ClaseROL rol;
        private List<TextBox> CamposLetra;
        private List<ComboBox> CamposNumericos;
        private Dictionary<string, object> valoresOriginales;

        public UCUSUARIO()
        {
            InitializeComponent();
            usuario = new ClaseUSUARIO();
            rol = new ClaseROL();
            conexion = new ClaseConexion();
            this.Load += UCSUARIO_Load;
            DiseñoGlobal.RegistrarUserControl(this);
        }

        private void UCSUARIO_Load(object sender, EventArgs e)
        {
            Permisos.AplicarPermisos(this);
            DiseñoGlobal.AplicarTamaño(this);
            // Aplica tema y formato de botones (añadido)
            DiseñoGlobal.AplicarTema(this, DiseñoGlobal.TemaLight);
            DiseñoGlobal.AplicarFormatoBotones(this, DiseñoGlobal.TemaLight);

            CargarIdentidades();
            CargarRoles();
            CargarFiltro();
            CargarUsuariosFiltro("Activos");
            var menu = FindForm() as FMENU;
            if (menu != null)
                DiseñoGlobal.AplicarEstiloDataGridView(dataGridUsuarios, menu.temaActual);
            else
                DiseñoGlobal.AplicarEstiloDataGridView(dataGridUsuarios, Temas.Light);
            CamposLetra = new List<TextBox> { txtPrimerNombre, txtSegundoNombre, txtPrimerApellido, txtSegundoApellido };
            CamposNumericos = new List<ComboBox> { CBRol };
            CamposLetra.ForEach(campo => campo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoLetras(ev));
            CamposNumericos.ForEach(campo => campo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoNumerico(ev));
            foreach (var txt in CamposLetra)
                txt.TextChanged += ControlModificado;
            txtClave.TextChanged += ControlModificado;
            CBNOIdentidad.SelectedIndexChanged += ControlModificado;
            CBRol.TextChanged += ControlModificado;
            checkactivo.CheckedChanged += ControlModificado;
            cboFiltroActivo.SelectedIndexChanged += cboFiltroActivo_SelectedIndexChanged;
            EstadoInicial();
        }

        private void CargarIdentidades()
        {
            DataTable dtUsuarios = usuario.ObtenerUsuarios();
            CBRol.DataSource = dtUsuarios;
            CBRol.DisplayMember = "NumeroIdentidad";
            CBRol.ValueMember = "NumeroIdentidad";
            CBRol.SelectedIndex = -1;
        }

        private void CargarRoles()
        {
            DataTable dtRoles = rol.ObtenerRoles();
            CBNOIdentidad.DataSource = dtRoles;
            CBNOIdentidad.DisplayMember = "NombreRol";
            CBNOIdentidad.ValueMember = "IDRol";
            CBNOIdentidad.SelectedIndex = -1;
        }

        private void CargarFiltro()
        {
            cboFiltroActivo.Items.Clear();
            cboFiltroActivo.Items.Add("Activos");
            cboFiltroActivo.Items.Add("Inactivos");
            cboFiltroActivo.Items.Add("Todos");
            cboFiltroActivo.SelectedIndex = 0; 
        }

        private void CargarUsuariosFiltro(string filtro)
        {
            DataTable dt = ClaseFiltroActivo.FiltrarTabla("USUARIO", filtro);
            dataGridUsuarios.DataSource = dt;

            if (dataGridUsuarios.Columns.Contains("IDUsuario"))
                dataGridUsuarios.Columns["IDUsuario"].HeaderText = "ID Usuario";
            if (dataGridUsuarios.Columns.Contains("NumeroIdentidad"))
                dataGridUsuarios.Columns["NumeroIdentidad"].HeaderText = "Número Identidad";
            if (dataGridUsuarios.Columns.Contains("NombreCompleto"))
                dataGridUsuarios.Columns["NombreCompleto"].HeaderText = "Nombre Completo";

            dataGridUsuarios.ClearSelection();
            LimpiarCampos();
        }

        private void CargarUsuarios()
        {
            usuario.CargarUsuarios(dataGridUsuarios);
            DiseñoGlobal.AplicarEstiloDataGridView(dataGridUsuarios, Temas.Dark); // Aplica fuente y tema cada vez que se cargan datos
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Ejecutar validación antes de guardar
            if (!ValidarCampos())
                return;

            usuario.NumeroIdentidad = CBRol.Text.Trim();

            // Validar duplicado
            if (txtId.Text == "")
            {
                if (usuario.ExistePorNumeroIdentidad(usuario.NumeroIdentidad))
                {
                    MessageBox.Show("Ya existe un usuario con este número de identidad. Use el botón Buscar para editarlo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Asignar datos
            usuario.PrimerNombre = txtPrimerNombre.Text.Trim();
            usuario.SegundoNombre = txtSegundoNombre.Text.Trim();
            usuario.PrimerApellido = txtPrimerApellido.Text.Trim();
            usuario.SegundoApellido = txtSegundoApellido.Text.Trim();
            usuario.Clave = txtClave.Text.Trim();
            usuario.IDRol = Convert.ToInt32(CBNOIdentidad.SelectedValue);
            usuario.Activo = checkactivo.Checked;

            // Guardar
            if (usuario.Guardar())
            {
                MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarIdentidades();
                CargarUsuarios();
                EstadoInicial();
                btnClean_Click(null, null);
            }
            else
            {
                MessageBox.Show("Error al guardar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            string numeroIdentidad = CBRol.Text.Trim();
            var usuarioExistente = usuario.BuscarPorNumeroIdentidad(numeroIdentidad);
            if (usuarioExistente == null)
            {
                MessageBox.Show("No se encontró el usuario.");
                return;
            }

            usuario.IDUsuario = usuarioExistente.IDUsuario;
            usuario.NumeroIdentidad = numeroIdentidad;
            usuario.PrimerNombre = txtPrimerNombre.Text.Trim();
            usuario.SegundoNombre = txtSegundoNombre.Text.Trim();
            usuario.PrimerApellido = txtPrimerApellido.Text.Trim();
            usuario.SegundoApellido = txtSegundoApellido.Text.Trim();
            usuario.Clave = txtClave.Text.Trim();
            usuario.IDRol = Convert.ToInt32(CBNOIdentidad.SelectedValue);
            usuario.Activo = checkactivo.Checked;

            if (usuario.Editar())
            {
                MessageBox.Show("Usuario editado correctamente.");
                CargarIdentidades();
                CargarUsuarios();
                btnEditar.Enabled = false;
                btnClean.Text = "Limpiar";
            }
            else
            {
                MessageBox.Show("Error al editar el usuario.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string numeroIdentidad = CBRol.Text.Trim();
            var usuarioExistente = usuario.BuscarPorNumeroIdentidad(numeroIdentidad);
            if (usuarioExistente == null)
            {
                MessageBox.Show("No se encontró el usuario.");
                return;
            }

            EliminarUsuario(usuarioExistente.IDUsuario);
            CargarIdentidades();
            CargarUsuarios();
            btnClean_Click(null, null);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string numeroIdentidad = CBRol.Text.Trim();

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

            var usuarioEncontrado = usuario.BuscarPorNumeroIdentidad(numeroIdentidad);

            if (usuarioEncontrado != null)
            {
                txtPrimerNombre.Text = usuarioEncontrado.PrimerNombre;
                txtSegundoNombre.Text = usuarioEncontrado.SegundoNombre;
                txtPrimerApellido.Text = usuarioEncontrado.PrimerApellido;
                txtSegundoApellido.Text = usuarioEncontrado.SegundoApellido;
                txtClave.Text = usuarioEncontrado.Clave;
                CBNOIdentidad.SelectedValue = usuarioEncontrado.IDRol;
                checkactivo.Checked = usuarioEncontrado.Activo;
                txtId.Text = usuarioEncontrado.IDUsuario.ToString();
                CBRol.Text = usuarioEncontrado.NumeroIdentidad;

                txtPrimerNombre.Enabled = true;
                txtSegundoNombre.Enabled = true;
                txtPrimerApellido.Enabled = true;
                txtSegundoApellido.Enabled = true;
                txtClave.Enabled = true;
                CBNOIdentidad.Enabled = true;
                checkactivo.Enabled = true;

                btnEditar.Enabled = false;
                // Chequeo de rol antes de habilitar Eliminar
                if (SesionUsuario.RolNombre == "Administrador")
                    btnEliminar.Enabled = true;
                else
                    btnEliminar.Enabled = false;

                btnBuscar.Enabled = false;
                btnGuardar.Enabled = false;
                btnClean.Text = "Limpiar";

                MessageBox.Show("Usuario encontrado. Puede eliminarlo o modificar sus datos.");

                valoresOriginales = new Dictionary<string, object>
                {
                    { "txtId", txtId.Text },
                    { "CBRol", CBRol.Text },
                    { "txtPrimerNombre", txtPrimerNombre.Text },
                    { "txtSegundoNombre", txtSegundoNombre.Text },
                    { "txtPrimerApellido", txtPrimerApellido.Text },
                    { "txtSegundoApellido", txtSegundoApellido.Text },
                    { "txtClave", txtClave.Text },
                    { "CBNOIdentidad", CBNOIdentidad.SelectedValue },
                    { "checkactivo", checkactivo.Checked }
                };
            }
            else
            {
                HabilitarCamposParaNuevoUsuario();
                btnBuscar.Enabled = false;
                btnGuardar.Enabled = true;

                MessageBox.Show("No existe un usuario con este número de identidad. Puede crear uno nuevo.");
            }
        }

        private void EliminarUsuario(int idUsuario)
        {
            var conexion = new ClaseConexion();
            var borrador = new ClaseBorrado(conexion);

            var dependencias = new List<string>();

            var usuarioEntidad = new ClaseUSUARIO();

            if (!usuarioEntidad.BuscarPorId(idUsuario))
            {
                MessageBox.Show("No se encontró el usuario.");
                return;
            }

            bool exito = borrador.EliminarSegunReglasEntidad(
                usuarioEntidad,
                idUsuario,
                "Activo",
                dependencias
            );

            if (exito)
                MessageBox.Show("Eliminación realizada correctamente.");
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
            CBRol.Text = valoresOriginales["CBRol"].ToString();
            txtPrimerNombre.Text = valoresOriginales["txtPrimerNombre"].ToString();
            txtSegundoNombre.Text = valoresOriginales["txtSegundoNombre"].ToString();
            txtPrimerApellido.Text = valoresOriginales["txtPrimerApellido"].ToString();
            txtSegundoApellido.Text = valoresOriginales["txtSegundoApellido"].ToString();
            txtClave.Text = valoresOriginales["txtClave"].ToString();
            CBNOIdentidad.SelectedValue = valoresOriginales["CBNOIdentidad"];
            checkactivo.Checked = (bool)valoresOriginales["checkactivo"];

            btnEditar.Enabled = false;
            btnClean.Text = "Limpiar";
        }

        private void LimpiarCampos()
        {
            txtId.Text = "";
            CBRol.Text = "";
            txtPrimerNombre.Text = "";
            txtSegundoNombre.Text = "";
            txtPrimerApellido.Text = "";
            txtSegundoApellido.Text = "";
            txtClave.Text = "";
            CBNOIdentidad.SelectedIndex = -1;
            checkactivo.Checked = false;

            valoresOriginales = null;
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
                CBRol.Text != valoresOriginales["CBRol"].ToString() ||
                txtPrimerNombre.Text != valoresOriginales["txtPrimerNombre"].ToString() ||
                txtSegundoNombre.Text != valoresOriginales["txtSegundoNombre"].ToString() ||
                txtPrimerApellido.Text != valoresOriginales["txtPrimerApellido"].ToString() ||
                txtSegundoApellido.Text != valoresOriginales["txtSegundoApellido"].ToString() ||
                txtClave.Text != valoresOriginales["txtClave"].ToString() ||
                (CBNOIdentidad.SelectedValue == null ? "" : CBNOIdentidad.SelectedValue.ToString()) !=
                    (valoresOriginales["CBNOIdentidad"] == null ? "" : valoresOriginales["CBNOIdentidad"].ToString()) ||
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

        private void cboFiltroActivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFiltroActivo.SelectedItem != null)
                CargarUsuariosFiltro(cboFiltroActivo.SelectedItem.ToString());
        }

        private void EstadoInicial()
        {
            txtPrimerNombre.Enabled = false;
            txtSegundoNombre.Enabled = false;
            txtPrimerApellido.Enabled = false;
            txtSegundoApellido.Enabled = false;
            txtClave.Enabled = false;
            CBNOIdentidad.Enabled = false;
            checkactivo.Enabled = false;

            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnGuardar.Enabled = false;

            CBRol.Enabled = true;
            btnBuscar.Enabled = true;
            btnClean.Text = "Limpiar";
            valoresOriginales = null;
        }


        private void HabilitarCamposParaNuevoUsuario()
        {
            txtPrimerNombre.Enabled = true;
            txtSegundoNombre.Enabled = true;
            txtPrimerApellido.Enabled = true;
            txtSegundoApellido.Enabled = true;
            txtClave.Enabled = true;
            CBNOIdentidad.Enabled = true;
            checkactivo.Enabled = true;

            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnGuardar.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }


    
        
    private bool ValidarCampos()
        {
            // Validar Número de Identidad
            if (string.IsNullOrWhiteSpace(CBRol.Text))
            {
                MessageBox.Show("Debe ingresar el número de identidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CBRol.Focus();
                return false;
            }

            if (CBRol.Text.Length != 13)
            {
                MessageBox.Show("El número de identidad debe tener exactamente 13 dígitos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CBRol.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrimerNombre.Text))
            {
                MessageBox.Show("Debe ingresar el primer nombre.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrimerNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrimerApellido.Text))
            {
                MessageBox.Show("Debe ingresar el primer apellido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrimerApellido.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MessageBox.Show("Debe ingresar una clave.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClave.Focus();
                return false;
            }

            if (CBNOIdentidad.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un rol.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CBNOIdentidad.Focus();
                return false;
            }

            return true;
        }

    } // 🔹 Esta llave es la que cierra toda la clase UCUSUARIO

}

