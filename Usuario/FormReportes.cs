using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Usuario.Clases;

namespace Usuario
{
    public partial class FormReportes : UserControl, IThemedReport
    {
        private UserControl reporteActual = null;
        public static Tema temaActual = Temas.Light;

        public FormReportes()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(Tema tema)
        {
            DiseñoGlobal.AplicarTema(this, tema);

            if (reporteActual is IThemedReport themed)
                themed.ApplyTheme(tema);
        }

        private void CargarReporte<T>() where T : UserControl, IThemedReport, new()
        {
            panel2.Controls.Clear();

            var nuevo = new T();
            nuevo.Dock = DockStyle.Fill;
            panel2.Controls.Add(nuevo);
            reporteActual = nuevo;

            nuevo.ApplyTheme(ThemeManager.CurrentTheme);
        }

        public void ApplyTheme(Tema tema)
        {
            DiseñoGlobal.AplicarTema(this, tema);
            if (reporteActual is IThemedReport themed)
                themed.ApplyTheme(tema);
        }

        private void btnIngresos_Click(object sender, EventArgs e) => CargarReporte<FormReportesIngresos>();
        private void btnEgresos_Click(object sender, EventArgs e) => CargarReporte<FormReporteEgresos>();
        private void btnInventario_Click(object sender, EventArgs e) => CargarReporte<FormReporteInventario>();
        private void btnClientes_Click(object sender, EventArgs e) => CargarReporte<FormReporteClientes>();
        private void btnBajoStock_Click(object sender, EventArgs e) => CargarReporte<FormReporteBajoStock>();
        private void btnMaquilas_Click(object sender, EventArgs e) => CargarReporte<FormReporteMaquilas>();
        private void btnFinancieros_Click(object sender, EventArgs e) => CargarReporte<FormReporteFinancieros>();

        private void FormReportes_Load(object sender, EventArgs e)
        {
            btnBajoStock.BackColor = Color.AliceBlue;
            btnClientes.BackColor = Color.AliceBlue;
            btnEgresos.BackColor = Color.AliceBlue;
            btnFinancieros.BackColor = Color.AliceBlue;
            btnIngresos.BackColor = Color.AliceBlue;
            btnInventario.BackColor = Color.AliceBlue;
            btnMaquilas.BackColor = Color.AliceBlue;
        }
    }
}

