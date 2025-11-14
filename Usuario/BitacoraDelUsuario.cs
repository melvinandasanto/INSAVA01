using System;
using System.Data;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class BitacoraDelUsuario : Form
    {
        private ClaseBitacora _bitacora;

        public BitacoraDelUsuario()
        {
            InitializeComponent();
            _bitacora = new ClaseBitacora();

            // Asegurar que el evento Load se dispare y cargue los datos
            this.Load += Bitacora_del_Usuario_Load;
        }

        private void Bitacora_del_Usuario_Load(object sender, EventArgs e)
        {
            CargarDatos();
            ConfigurarDataGridView();
        }

        private void CargarDatos()
        {
            try
            {
                DataTable dt = _bitacora.ObtenerTodosLosRegistros();

                if (dt == null)
                {
                    MessageBox.Show("La consulta devolvió null. Verifica la conexión y la existencia de la tabla.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dataGridView1.DataSource = null;
                    return;
                }

                dataGridView1.DataSource = dt;
                AjustarColumnasDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        private void AjustarColumnasDataGrid()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                if (dataGridView1.Columns.Contains("id"))
                    dataGridView1.Columns["id"].Visible = false;

                if (dataGridView1.Columns.Contains("usuario"))
                    dataGridView1.Columns["usuario"].HeaderText = "Numero Identidad";

                if (dataGridView1.Columns.Contains("fecha_hora"))
                {
                    dataGridView1.Columns["fecha_hora"].HeaderText = "Fecha y Hora";
                    dataGridView1.Columns["fecha_hora"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                }

                if (dataGridView1.Columns.Contains("status_usuario"))
                    dataGridView1.Columns["status_usuario"].HeaderText = "Estado";

                if (dataGridView1.Columns.Contains("intento_exitoso"))
                    dataGridView1.Columns["intento_exitoso"].HeaderText = "Exitoso";
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // opcional
        }

        public void BuscarPorUsuario(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                CargarDatos();
                return;
            }

            DataTable dt = _bitacora.ObtenerPorUsuario(texto);
            dataGridView1.DataSource = dt;
            AjustarColumnasDataGrid();
        }
    }
}
