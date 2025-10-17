using System;
using System.Data;
using System.Windows.Forms;
using Usuario.Clases;
using System.Collections.Generic;

namespace Usuario
{
    public partial class FPROVEEDOR : Form
    {
        private ClasePROVEEDOR proveedor;
        private ClaseConexion conexion;
        private List<TextBox> CamposLetra;
        private List<TextBox> CamposNumericos;

        public FPROVEEDOR()
        {
            InitializeComponent();
            proveedor = new ClasePROVEEDOR();
            this.Load += PROVEEDOR_Load;

            btnClean.Click += btnClean_Click;
        }

        private void PROVEEDOR_Load(object sender, EventArgs e)
        {
            RefrescarGrid();
            var menu = FindForm() as FMENU;
            if (menu != null)
                DiseñoGlobal.AplicarEstiloDataGridView(DGPROVEEDOR, menu.temaActual);
            else
                DiseñoGlobal.AplicarEstiloDataGridView(DGPROVEEDOR, Temas.Dark);
            CargarProveedoresCombo();
            CamposLetra = new List<TextBox> { txtNombreProveedor};
            CamposNumericos = new List<TextBox> { txtTelefonoProveedor };
            CamposLetra.ForEach(campo => campo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoLetras(ev));
            CamposNumericos.ForEach(campo => campo.KeyPress += (s, ev) => ClaseValidacion.ValidarCampoNumerico(ev));
            cboFiltroActivo.Items.Clear();
            cboFiltroActivo.Items.Add("Todos");
            cboFiltroActivo.Items.Add("Activos");
            cboFiltroActivo.Items.Add("Inactivos");
            cboFiltroActivo.SelectedIndex = 0;
        }

        private void CargarProveedoresCombo()
        {
            DataTable dt = proveedor.ObtenerProveedores(true);
            comboIDProveedor.DataSource = dt;
            comboIDProveedor.DisplayMember = "IDProveedor";
            comboIDProveedor.ValueMember = "IDProveedor";
            comboIDProveedor.SelectedIndex = -1;
        }

        private void RefrescarGrid()
        {
            proveedor.CargarProveedores(DGPROVEEDOR);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(comboIDProveedor.Text.Trim(), out int idProveedor))
            {
                if (proveedor.BuscarPorId(idProveedor))
                {
                    txtNombreProveedor.Text = proveedor.NombreProveedor;
                    txtTelefonoProveedor.Text = proveedor.TelefonoProveedor;
                    checkactivo.Checked = proveedor.Activo;
                }
                else
                {
                    MessageBox.Show("Proveedor no encontrado.");
                }
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor.");
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(comboIDProveedor.Text.Trim(), out int idProveedor))
            {
                if (string.IsNullOrWhiteSpace(txtNombreProveedor.Text))
                {
                    MessageBox.Show("Por favor, ingresa el nombre del proveedor.");
                    return;
                }

                proveedor.IDProveedor = idProveedor;
                proveedor.NombreProveedor = txtNombreProveedor.Text.Trim();
                proveedor.TelefonoProveedor = txtTelefonoProveedor.Text.Trim();
                proveedor.Activo = checkactivo.Checked;

                try
                {
                    if (proveedor.Editar())
                    {
                        MessageBox.Show("Proveedor editado correctamente.");
                        RefrescarGrid();
                        CargarProveedoresCombo();
                    }
                    else
                    {
                        MessageBox.Show("Error al editar el proveedor.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(comboIDProveedor.Text.Trim(), out int idProveedor))
            {
                EliminarProveedor(idProveedor);
                RefrescarGrid();
                CargarProveedoresCombo();
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor.");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreProveedor.Text))
            {
                MessageBox.Show("Por favor, ingresa el nombre del proveedor.");
                return;
            }

            proveedor.NombreProveedor = txtNombreProveedor.Text.Trim();
            proveedor.TelefonoProveedor = txtTelefonoProveedor.Text.Trim();
            proveedor.Activo = checkactivo.Checked;

            try
            {
                if (proveedor.Guardar())
                {
                    MessageBox.Show("Proveedor guardado correctamente.");
                    RefrescarGrid();
                    CargarProveedoresCombo();
                }
                else
                {
                    MessageBox.Show("Error al guardar el proveedor.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            comboIDProveedor.SelectedIndex = -1;
            txtNombreProveedor.Text = "";
            txtTelefonoProveedor.Text = "";
            checkactivo.Checked = false;
        }

        private void EliminarProveedor(int idProveedor)
        {
            var conexion = new ClaseConexion();
            var borrador = new ClaseBorrado(conexion);

            var dependencias = new List<string>
            {
                // Verifica si el proveedor tiene productos asociados
                "SELECT COUNT(*) FROM INVENTARIO_PRODUCTO WHERE IdProveedor = @id"
            };

            // Instancia de ClasePROVEEDOR cargada con el proveedor a eliminar
            var proveedorEntidad = new ClasePROVEEDOR();
            if (!proveedorEntidad.BuscarPorId(idProveedor))
            {
                MessageBox.Show("No se encontró el proveedor.");
                return;
            }

            bool exito = borrador.EliminarSegunReglasEntidad(
                proveedorEntidad, // Instancia de ClasePROVEEDOR
                idProveedor,
                "Activo",
                dependencias
            );

            if (exito)
                MessageBox.Show("Eliminación realizada correctamente.");
        }

        private void cboFiltroActivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filtro = cboFiltroActivo.SelectedItem.ToString();
            string tabla = "PROVEEDOR";

            DataTable datos = ClaseFiltroActivo.FiltrarTabla(tabla, filtro);
            DGPROVEEDOR.DataSource = datos;

            // Opcional: personalizar encabezados
            if (DGPROVEEDOR.Columns.Contains("id"))
                DGPROVEEDOR.Columns["id"].HeaderText = "ID Cliente";

            if (DGPROVEEDOR.Columns.Contains("RTN"))
                DGPROVEEDOR.Columns["RTN"].HeaderText = "RTN";

            if (DGPROVEEDOR.Columns.Contains("PrimerNombre"))
                DGPROVEEDOR.Columns["PrimerNombre"].HeaderText = "Primer Nombre";
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}