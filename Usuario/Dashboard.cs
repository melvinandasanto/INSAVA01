using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Usuario.Clases;

namespace Usuario
{
    public partial class Dashboard : UserControl
    {
        private readonly ClaseConexion _claseConexion;
        private readonly DashboardRepository _repo;

        public Dashboard()
        {
            InitializeComponent();

            _claseConexion = new ClaseConexion(); // usa los valores por defecto del constructor
            _repo = new DashboardRepository(_claseConexion);

            this.Load += Dashboard_Load;
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            RefrescarTodo();

            lbl1.BackColor = Color.ForestGreen;
            lbl1.ForeColor = Color.White;
            lbl2.BackColor = Color.IndianRed;
            lbl2.ForeColor = Color.White;
            lbl3.BackColor = Color.SteelBlue;
            lbl3.ForeColor = Color.White;
            lbl4.BackColor = Color.Goldenrod;
            lbl4.ForeColor = Color.White;
            lblKPI1Titulo.BackColor = Color.Transparent;
            lblKPI1Valor.BackColor = Color.Transparent;
            lblKPI2Titulo.BackColor = Color.Transparent;
            lblKPI2Valor.BackColor = Color.Transparent;
            lblKPI3Titulo.BackColor = Color.Transparent;
            lblKPI3Valor.BackColor = Color.Transparent;
            lblKPI4Titulo.BackColor = Color.Transparent;
            lblKPI4Valor.BackColor = Color.Transparent;

            IgualarColoresKPI();
        }

        private void RefrescarTodo()
        {
            CargarKPIs();
            CargarGraficos();
            CargarTablasInformativas();
            CargarProductosMasMenos();
            CargarEstadisticasLogin();
        }

        private void CargarKPIs()
        {
            try
            {
                lblKPI1Titulo.Text = "Total Ventas";
                lblKPI1Valor.Text = _repo.GetTotalVentas().ToString("C2");

                lblKPI2Titulo.Text = "Total Maquila";
                lblKPI2Valor.Text = _repo.GetTotalMaquila().ToString("C2");

                lblKPI3Titulo.Text = "Clientes Activos";
                lblKPI3Valor.Text = _repo.GetClientesActivos().ToString();

                lblKPI4Titulo.Text = "Productos con stock";
                lblKPI4Valor.Text = _repo.GetProductosConStock().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar KPIs: " + ex.Message);
            }
        }

        private void CargarProductosMasMenos()
        {
            try
            {
                // Producto más vendido en panel4
                DataTable dtMas = _repo.GetProductoMasVendido();
                if (dtMas != null && dtMas.Rows.Count > 0)
                {
                    var r = dtMas.Rows[0];
                    string nombre = r.Table.Columns.Contains("Nombre") && r["Nombre"] != DBNull.Value ? r["Nombre"].ToString() : "(sin nombre)";
                    decimal total = r.Table.Columns.Contains("TotalVendido") && r["TotalVendido"] != DBNull.Value ? Convert.ToDecimal(r["TotalVendido"]) : 0m;
                    label1.Text = $"{nombre}\nVendidas: {total}";
                }
                else
                {
                    label1.Text = "Sin datos de ventas";
                }

                label2.Text = "Producto más vendido";

                // Producto menos vendido en panel5
                DataTable dtMenos = _repo.GetProductoMenosVendido();
                if (dtMenos != null && dtMenos.Rows.Count > 0)
                {
                    var r = dtMenos.Rows[0];
                    string nombre = r.Table.Columns.Contains("Nombre") && r["Nombre"] != DBNull.Value ? r["Nombre"].ToString() : "(sin nombre)";
                    decimal total = r.Table.Columns.Contains("TotalVendido") && r["TotalVendido"] != DBNull.Value ? Convert.ToDecimal(r["TotalVendido"]) : 0m;
                    label8.Text = $"{nombre}\nVendidas: {total}";
                }
                else
                {
                    label8.Text = "Sin datos de ventas";
                }

                label4.Text = "Producto menos vendido";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos más/menos vendidos: " + ex.Message);
            }
        }

        private void CargarEstadisticasLogin()
        {
            try
            {
                int totalLogins = _repo.GetTotalLoginsSistema();
                int totalFallidos = _repo.GetTotalIntentosFallidos();

                // Mostrar estadísticas como etiquetas (opcional, en la UI del designer)
                // Aquí simplemente cargamos el DataGridView
                var dtUsuarios = _repo.GetLoginsPorUsuario();
                dataGridViewInicioSesion.DataSource = dtUsuarios;

                if (dataGridViewInicioSesion.Columns.Contains("usuario"))
                    dataGridViewInicioSesion.Columns["usuario"].HeaderText = "Usuario";
                if (dataGridViewInicioSesion.Columns.Contains("TotalLogins"))
                    dataGridViewInicioSesion.Columns["TotalLogins"].HeaderText = "Total";
                if (dataGridViewInicioSesion.Columns.Contains("LoginsExitosos"))
                    dataGridViewInicioSesion.Columns["LoginsExitosos"].HeaderText = "Exitosos";
                if (dataGridViewInicioSesion.Columns.Contains("LoginsFallidos"))
                    dataGridViewInicioSesion.Columns["LoginsFallidos"].HeaderText = "Fallidos";
                if (dataGridViewInicioSesion.Columns.Contains("UltimoAcceso"))
                {
                    dataGridViewInicioSesion.Columns["UltimoAcceso"].HeaderText = "Último Acceso";
                    dataGridViewInicioSesion.Columns["UltimoAcceso"].DefaultCellStyle.Format = "g";
                }

                dataGridViewInicioSesion.ReadOnly = true;
                dataGridViewInicioSesion.AllowUserToAddRows = false;
                dataGridViewInicioSesion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridViewInicioSesion.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estadísticas de login: " + ex.Message);
            }
        }

        private void CargarGraficos()
        {
            try
            {
                // === GRAFICO DE VENTAS MENSUALES (BARRAS) ===
                var dtVentas = _repo.GetVentasPorMes();

                chartVentasMensuales.Series.Clear();
                chartVentasMensuales.Titles.Clear();
                chartVentasMensuales.Titles.Add("Ventas Mensuales");
                chartVentasMensuales.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                chartVentasMensuales.BackColor = Color.FromArgb(245, 245, 245); // gris muy claro
                chartVentasMensuales.ChartAreas[0].BackColor = Color.WhiteSmoke;
                chartVentasMensuales.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
                chartVentasMensuales.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;

                var sVentas = new Series("Ventas")
                {
                    ChartType = SeriesChartType.Column,
                    XValueType = ChartValueType.String,
                    Color = Color.SteelBlue,
                    BorderWidth = 2,
                    IsValueShownAsLabel = true,
                    LabelForeColor = Color.Black
                };

                foreach (DataRow r in dtVentas.Rows)
                {
                    string mes = r["Mes"].ToString();
                    decimal total = 0m;
                    decimal.TryParse(r["Total"].ToString(), out total);
                    sVentas.Points.AddXY(mes, total);
                }

                chartVentasMensuales.Series.Add(sVentas);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar gráfico de ventas: " + ex.Message);
            }
        }

        private void CargarTablasInformativas()
        {
            try
            {
                var dtUltimas = _repo.GetUltimasTransacciones(6);
                dgvUltimasTransacciones.DataSource = dtUltimas;

                var dtMaquilas = _repo.GetMaquilasPendientes();
                dgvMaquilasPendientes.DataSource = dtMaquilas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tablas informativas: " + ex.Message);
            }
        }

        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            RefrescarTodo();
        }

        private void BtnActualizar_Click_1(object sender, EventArgs e)
        {

        }

        private void IgualarColoresKPI()
        {
            // Sin transparencia: heredan el color del lbl de fondo (lbl1-4)
            lblKPI1Titulo.BackColor = lbl1.BackColor;
            lblKPI1Valor.BackColor = lbl1.BackColor;

            lblKPI2Titulo.BackColor = lbl2.BackColor;
            lblKPI2Valor.BackColor = lbl2.BackColor;

            lblKPI3Titulo.BackColor = lbl3.BackColor;
            lblKPI3Valor.BackColor = lbl3.BackColor;

            lblKPI4Titulo.BackColor = lbl4.BackColor;
            lblKPI4Valor.BackColor = lbl4.BackColor;

            // Texto en blanco o negro según el fondo (opcional)
            foreach (var lbl in new[] { lblKPI1Titulo, lblKPI1Valor, lblKPI2Titulo, lblKPI2Valor, lblKPI3Titulo, lblKPI3Valor, lblKPI4Titulo, lblKPI4Valor })
            {
                lbl.ForeColor = Color.White;
                lbl.BorderStyle = BorderStyle.None;
            }
        }

        public void colore()
        {
            lbl1.BackColor = Color.ForestGreen;
            lbl1.ForeColor = Color.White;
            lbl2.BackColor = Color.IndianRed;
            lbl2.ForeColor = Color.White;
            lbl3.BackColor = Color.SteelBlue;
            lbl3.ForeColor = Color.White;
            lbl4.BackColor = Color.Goldenrod;
            lbl4.ForeColor = Color.White;
            lblKPI1Titulo.BackColor = Color.Transparent;
            lblKPI1Valor.BackColor = Color.Transparent;
            lblKPI2Titulo.BackColor = Color.Transparent;
            lblKPI2Valor.BackColor = Color.Transparent;
            lblKPI3Titulo.BackColor = Color.Transparent;
            lblKPI3Valor.BackColor = Color.Transparent;
            lblKPI4Titulo.BackColor = Color.Transparent;
            lblKPI4Valor.BackColor = Color.Transparent;

            IgualarColoresKPI();
        }

        private void lblKPI3Titulo_Click(object sender, EventArgs e)
        {

        }

        public void CambiarTema(Tema tema)
        {
            this.BackColor = tema.Fondo;

            // === Aplicar tema a charts ===
            foreach (var chart in new[] { chartVentasMensuales, chartIngresosEgresos })
            {
                chart.BackColor = tema.Fondo;
                chart.ChartAreas[0].BackColor = tema.Fondo;

                // Ejes
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = tema.ForeColor;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = tema.ForeColor;
                chart.ChartAreas[0].AxisX.LineColor = tema.ForeColor;
                chart.ChartAreas[0].AxisY.LineColor = tema.ForeColor;
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;

                // Series
                foreach (var series in chart.Series)
                    series.LabelForeColor = tema.ForeColor;

                // Leyenda
                if (chart.Legends.Count > 0)
                {
                    chart.Legends[0].ForeColor = tema.ForeColor;
                    chart.Legends[0].BackColor = tema.Fondo;
                }

                // Títulos
                foreach (var t in chart.Titles)
                    t.ForeColor = tema.ForeColor;
            }

            // === Aplicar tema a labels y paneles con borde ===
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.ForeColor = tema.ForeColor;
                    if (tema == Temas.Dark)
                    {
                        lbl.BorderStyle = BorderStyle.FixedSingle;
                        lbl.BackColor = tema.Fondo;
                    }
                    else
                    {
                        lbl.BorderStyle = BorderStyle.None;
                    }
                }
                else if (ctrl is Panel pnl)
                {
                    if (tema == Temas.Dark)
                    {
                        pnl.BackColor = tema.Fondo;
                        pnl.BorderStyle = BorderStyle.FixedSingle;
                    }
                    else
                    {
                        pnl.BackColor = tema.Fondo;
                        pnl.BorderStyle = BorderStyle.None;
                    }
                }
            }

            AplicarTemaRecursivo(this, tema);
            lblKPI1Titulo.BackColor = lbl1.BackColor;
            lblKPI1Valor.BackColor = lbl1.BackColor;

            lblKPI2Titulo.BackColor = lbl2.BackColor;
            lblKPI2Valor.BackColor = lbl2.BackColor;

            lblKPI3Titulo.BackColor = lbl3.BackColor;
            lblKPI3Valor.BackColor = lbl3.BackColor;

            lblKPI4Titulo.BackColor = lbl4.BackColor;
            lblKPI4Valor.BackColor = lbl4.BackColor;
        }

        private void AplicarTemaRecursivo(Control parent, Tema tema)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label lbl)
                {
                    if (lbl.Name.StartsWith("lblKPI"))
                    {
                        lbl.BackColor = Color.Transparent;
                        lbl.ForeColor = tema.ForeColor;
                        continue; 
                    }

                    lbl.ForeColor = tema.ForeColor;

                    if (tema == Temas.Dark)
                    {
                        lbl.BorderStyle = BorderStyle.FixedSingle;
                        lbl.BackColor = tema.Fondo;
                    }
                    else
                    {
                        lbl.BorderStyle = BorderStyle.None;
                        lbl.BackColor = tema.Fondo;
                    }

                    // Colores específicos para lbl1-4
                    switch (lbl.Name)
                    {
                        case "lbl1":
                            lbl.BackColor = Color.ForestGreen;
                            lbl.ForeColor = Color.White;
                            break;
                        case "lbl2":
                            lbl.BackColor = Color.IndianRed;
                            lbl.ForeColor = Color.White;
                            break;
                        case "lbl3":
                            lbl.BackColor = Color.SteelBlue;
                            lbl.ForeColor = Color.White;
                            break;
                        case "lbl4":
                            lbl.BackColor = Color.Goldenrod;
                            lbl.ForeColor = Color.White;
                            break;
                    }
                }
                else if (ctrl is Panel pnl)
                {
                    pnl.BackColor = tema.Fondo;
                    pnl.BorderStyle = (tema == Temas.Dark)
                        ? BorderStyle.FixedSingle
                        : BorderStyle.None;
                }

                // Recursión si tiene hijos
                if (ctrl.HasChildren)
                    AplicarTemaRecursivo(ctrl, tema);
            }
        }
    }
}
