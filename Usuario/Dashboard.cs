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


        private void RefrescarTodo()
        {
            CargarKPIs();
            CargarGraficos();
            CargarTablasInformativas();
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
                    decimal total = Convert.ToDecimal(r["Total"]);
                    sVentas.Points.AddXY(mes, total);
                }

                chartVentasMensuales.Series.Add(sVentas);
                chartVentasMensuales.ChartAreas[0].AxisX.Title = "Mes";
                chartVentasMensuales.ChartAreas[0].AxisY.Title = "Total Ventas";
                chartVentasMensuales.ChartAreas[0].AxisX.Interval = 1;
                chartVentasMensuales.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
                chartVentasMensuales.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

                // === GRAFICO DE INGRESOS VS EGRESOS (LÍNEAS) ===
                var dtEgresos = _repo.GetEgresosPorMes();
                chartIngresosEgresos.Series.Clear();
                chartIngresosEgresos.Titles.Clear();
                chartIngresosEgresos.Titles.Add("Ingresos vs Egresos");
                chartIngresosEgresos.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                chartIngresosEgresos.BackColor = Color.FromArgb(245, 245, 245);
                chartIngresosEgresos.ChartAreas[0].BackColor = Color.WhiteSmoke;

                var sIng = new Series("Ingresos")
                {
                    ChartType = SeriesChartType.Area,
                    BorderWidth = 3,
                    Color = Color.FromArgb(120, Color.ForestGreen),
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 6,
                    IsValueShownAsLabel = true,
                    LabelForeColor = Color.ForestGreen
                };

                var sEgr = new Series("Egresos")
                {
                    ChartType = SeriesChartType.Area,
                    BorderWidth = 3,
                    Color = Color.FromArgb(120, Color.IndianRed),
                    MarkerStyle = MarkerStyle.Diamond,
                    MarkerSize = 6,
                    IsValueShownAsLabel = true,
                    LabelForeColor = Color.IndianRed
                };

                // Unificar por mes
                foreach (DataRow r in dtVentas.Rows)
                {
                    int mesNum = Convert.ToInt32(r["MesNum"]);
                    string mes = r["Mes"].ToString();
                    decimal totalVentas = Convert.ToDecimal(r["Total"]);

                    decimal totalEgresos = 0m;
                    foreach (DataRow er in dtEgresos.Rows)
                    {
                        if (Convert.ToInt32(er["MesNum"]) == mesNum)
                        {
                            totalEgresos = Convert.ToDecimal(er["TotalEgresos"]);
                            break;
                        }
                    }

                    sIng.Points.AddXY(mes, totalVentas);
                    sEgr.Points.AddXY(mes, totalEgresos);
                }

                chartIngresosEgresos.Series.Add(sIng);
                chartIngresosEgresos.Series.Add(sEgr);
                chartIngresosEgresos.ChartAreas[0].AxisX.Title = "Mes";
                chartIngresosEgresos.ChartAreas[0].AxisY.Title = "Cantidad";
                chartIngresosEgresos.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
                chartIngresosEgresos.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
                chartIngresosEgresos.ChartAreas[0].AxisX.Interval = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar gráficos: " + ex.Message);

            }
        }


        private void CargarTablasInformativas()
        {
            try
            {
                dgvUltimasTransacciones.DataSource = _repo.GetUltimasTransacciones(6);
                dgvBajoStock.DataSource = _repo.GetProductosBajoStock(10m);
                dgvMaquilasPendientes.DataSource = _repo.GetMaquilasPendientes();

                foreach (DataGridView dgv in new[] { dgvUltimasTransacciones, dgvBajoStock, dgvMaquilasPendientes })
                {
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgv.ReadOnly = true;
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                    dgv.BackgroundColor = Color.White;
                    dgv.DefaultCellStyle.BackColor = Color.White;
                    dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 255);
                    dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
                    dgv.BorderStyle = BorderStyle.FixedSingle;
                    dgv.GridColor = Color.Gainsboro;
                    dgv.RowHeadersVisible = false;
                }
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
