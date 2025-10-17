using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Usuario.Clases;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Usuario
{
    public partial class FACTURA : Form
    {
        private ClaseConexion conexion;
        private DataTable _detalleFactura;
        private string _usuario;
        private int _numeroOrden;
        private string _cliente;

        public FACTURA(int numeroOrden, string cliente, DataTable detalle)
        {
            InitializeComponent();
            conexion = new ClaseConexion();
            DiseñoGlobal.RegistrarFormulario(this);
            _cliente = cliente;
            _detalleFactura = detalle;
            _numeroOrden = numeroOrden;

            // Tomar el usuario actual desde la sesión
            _usuario = SesionUsuario.NombreCompleto;

            CargarClientes();
            CargarMetodosPago();
            MostrarDatosFactura();
            CargarDetalle();
            ActualizarTotalFactura();
        }

        private void FACTURA_Load(object sender, EventArgs e)
        {
            Permisos.AplicarPermisos(this);
            var menu = this.Owner as FMENU;
            var tema = menu != null ? menu.temaActual : Temas.Light;
            AplicarTema(tema);
        }

        private void CargarClientes()
        {
            DataTable dt = conexion.Tabla("SELECT IDCliente, (PrimerNombre + ' ' + ISNULL(SegundoNombre,'') + ' ' + PrimerApellido + ' ' + ISNULL(SegundoApellido,'')) AS NombreCompleto FROM CLIENTE WHERE Activo = 1");
            cmbClientes.DataSource = dt;
            cmbClientes.DisplayMember = "NombreCompleto";
            cmbClientes.ValueMember = "IDCliente";
        }

        private void CargarMetodosPago()
        {
            DataTable dt = conexion.Tabla("SELECT * FROM METODO_PAGO");
            cmbMetodoPago.DataSource = dt;
            cmbMetodoPago.DisplayMember = "NombreMetodo";
            cmbMetodoPago.ValueMember = "IDMetodoPago";
        }

        private void MostrarDatosFactura()
        {
            lblClienteInfo.Text = $"Cliente: {_cliente}\nUsuario: {_usuario}\nN° Orden: {_numeroOrden}";
        }

        private void CargarDetalle()
        {
            dgvFactura.Columns.Clear();
            dgvFactura.Columns.Add("Producto", "Producto");
            dgvFactura.Columns.Add("Cantidad", "Cantidad");
            dgvFactura.Columns.Add("PrecioUnitario", "Precio");
            dgvFactura.Columns.Add("Total", "Total");

            foreach (DataRow row in _detalleFactura.Rows)
            {
                dgvFactura.Rows.Add(row["Producto"], row["Cantidad"], row["PrecioUnitario"], row["Subtotal"]);
            }
        }

        private void ActualizarTotalFactura()
        {
            decimal totalFactura = 0;
            foreach (DataGridViewRow row in dgvFactura.Rows)
            {
                if (row.IsNewRow) continue;
                totalFactura += Convert.ToDecimal(row.Cells["Total"].Value ?? 0);
            }
            lblTotalPagar.Text = $"Total a pagar: L {totalFactura:N2}";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnNuevoCliente_Click(object sender, EventArgs e)
        {
            using (Form frm = new Form())
            {
                UCCLIENTE clienteControl = new UCCLIENTE();
                clienteControl.Dock = DockStyle.Fill;
                frm.Controls.Add(clienteControl);
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Size = new Size(800, 600);
                frm.ShowDialog();
                CargarClientes();
            }
        }

        public void AplicarTema(Tema tema)
        {
            DiseñoGlobal.AplicarTema(this, tema);
            DiseñoGlobal.AplicarEstiloDataGridView(dgvFactura, tema);
        }

        private void btnImprimirFactura_Click(object sender, EventArgs e)
        {
            btnImprimirFactura.Enabled = false;
            try
            {
                string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Factura_{_numeroOrden}.pdf");
                Document doc = new Document(PageSize.A4, 50, 50, 25, 25);
                using (FileStream stream = new FileStream(ruta, FileMode.Create))
                {
                    PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                    doc.Open();

                    // ===== RUTAS DE IMAGENES =====
                    string rutaCarpeta = @"C:\Users\melvi\OneDrive\Desktop\insava6\aqui esta\Recursos";
                    string nombreLogo = "MEMBRETE INSAVA.png";
                    string rutaMembrete = Path.Combine(rutaCarpeta, nombreLogo);

                    // ===== FONDO (MEMBRETE) =====
                    if (File.Exists(rutaMembrete))
                    {
                        iTextSharp.text.Image fondo = iTextSharp.text.Image.GetInstance(rutaMembrete);
                        fondo.SetAbsolutePosition(0, 0);
                        fondo.ScaleToFit(PageSize.A4.Width, PageSize.A4.Height);
                        PdfContentByte canvas = writer.DirectContentUnder;
                        canvas.AddImage(fondo);
                    }
                    else
                    {
                        MessageBox.Show("El membrete no se encuentra en la ruta especificada.");
                    }

                    // ===== ESPACIADO PARA NO SOLAPAR =====
                    doc.Add(new Paragraph("\n\n\n\n\n\n"));

                    // ===== DATOS DE FACTURA =====
                    doc.Add(new Paragraph($"Número de Factura: {_numeroOrden}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                    doc.Add(new Paragraph($"Cliente: {_cliente}", FontFactory.GetFont(FontFactory.HELVETICA, 12)));
                    doc.Add(new Paragraph($"Usuario: {_usuario}", FontFactory.GetFont(FontFactory.HELVETICA, 12)));
                    doc.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy}\n\n"));

                    // ===== TABLA DETALLE =====
                    PdfPTable tabla = new PdfPTable(4);
                    tabla.WidthPercentage = 100;
                    tabla.SetWidths(new float[] { 50f, 15f, 15f, 20f });
                    tabla.AddCell("Producto");
                    tabla.AddCell("Cantidad");
                    tabla.AddCell("Precio");
                    tabla.AddCell("Total");

                    foreach (DataGridViewRow row in dgvFactura.Rows)
                    {
                        if (row.IsNewRow) continue;
                        tabla.AddCell(row.Cells["Producto"].Value?.ToString() ?? "");
                        tabla.AddCell(row.Cells["Cantidad"].Value?.ToString() ?? "0");
                        tabla.AddCell(row.Cells["PrecioUnitario"].Value?.ToString() ?? "0");
                        tabla.AddCell(row.Cells["Total"].Value?.ToString() ?? "0");
                    }
                    doc.Add(tabla);

                    doc.Add(new Paragraph($"\n{lblTotalPagar.Text}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));

                    doc.Close();
                    stream.Close();
                }
                MessageBox.Show($"Factura guardada en: {ruta}", "PDF Generado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(ruta);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el PDF: {ex.Message}");
            }
            finally
            {
                btnImprimirFactura.Enabled = true;
            }
        }


        private void btnGuardarVenta_Click(object sender, EventArgs e)
        {
            try
            {
                int idCliente = Convert.ToInt32(cmbClientes.SelectedValue);
                int idMetodoPago = Convert.ToInt32(cmbMetodoPago.SelectedValue);
                ServicioVenta.RegistrarVenta(_detalleFactura, idCliente, idMetodoPago, conexion);

                MessageBox.Show("Venta registrada con éxito.");
                btnImprimirFactura.Enabled = true;
                btnGuardarVenta.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar la venta: {ex.Message}");
            }
        }

    }
}
