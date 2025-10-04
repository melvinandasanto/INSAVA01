namespace Usuario
{
    partial class UCBUSCADOR
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.CBOperacion = new System.Windows.Forms.ComboBox();
            this.gpclie = new System.Windows.Forms.GroupBox();
            this.cmbclientes = new System.Windows.Forms.ComboBox();
            this.BtnBuscarcli = new System.Windows.Forms.Button();
            this.btnLimpiarcli = new System.Windows.Forms.Button();
            this.Lblcliente = new System.Windows.Forms.Label();
            this.dgvcliente = new System.Windows.Forms.DataGridView();
            this.gpclie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvcliente)).BeginInit();
            this.SuspendLayout();
            // 
            // CBOperacion
            // 
            this.CBOperacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBOperacion.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CBOperacion.FormattingEnabled = true;
            this.CBOperacion.Items.AddRange(new object[] {
            "Clientes",
            "Facturas"});
            this.CBOperacion.Location = new System.Drawing.Point(565, 22);
            this.CBOperacion.Margin = new System.Windows.Forms.Padding(2);
            this.CBOperacion.Name = "CBOperacion";
            this.CBOperacion.Size = new System.Drawing.Size(184, 23);
            this.CBOperacion.TabIndex = 9;
            this.CBOperacion.SelectedIndexChanged += new System.EventHandler(this.CBOperacion_SelectedIndexChanged);
            // 
            // gpclie
            // 
            this.gpclie.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpclie.Controls.Add(this.cmbclientes);
            this.gpclie.Controls.Add(this.BtnBuscarcli);
            this.gpclie.Controls.Add(this.btnLimpiarcli);
            this.gpclie.Controls.Add(this.Lblcliente);
            this.gpclie.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpclie.Location = new System.Drawing.Point(2, 22);
            this.gpclie.Margin = new System.Windows.Forms.Padding(2);
            this.gpclie.Name = "gpclie";
            this.gpclie.Padding = new System.Windows.Forms.Padding(2);
            this.gpclie.Size = new System.Drawing.Size(461, 106);
            this.gpclie.TabIndex = 10;
            this.gpclie.TabStop = false;
            this.gpclie.Text = "Descripcion del cliente";
            // 
            // cmbclientes
            // 
            this.cmbclientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbclientes.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbclientes.FormattingEnabled = true;
            this.cmbclientes.Location = new System.Drawing.Point(10, 42);
            this.cmbclientes.Margin = new System.Windows.Forms.Padding(2);
            this.cmbclientes.Name = "cmbclientes";
            this.cmbclientes.Size = new System.Drawing.Size(444, 24);
            this.cmbclientes.TabIndex = 144;
            this.cmbclientes.SelectedIndexChanged += new System.EventHandler(this.cmbclientes_SelectedIndexChanged);
            // 
            // BtnBuscarcli
            // 
            this.BtnBuscarcli.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBuscarcli.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnBuscarcli.Location = new System.Drawing.Point(152, 66);
            this.BtnBuscarcli.Margin = new System.Windows.Forms.Padding(2);
            this.BtnBuscarcli.Name = "BtnBuscarcli";
            this.BtnBuscarcli.Size = new System.Drawing.Size(299, 37);
            this.BtnBuscarcli.TabIndex = 143;
            this.BtnBuscarcli.Text = "Buscar Cliente";
            this.BtnBuscarcli.UseVisualStyleBackColor = true;
            this.BtnBuscarcli.Click += new System.EventHandler(this.BtnBuscarcli_Click);
            // 
            // btnLimpiarcli
            // 
            this.btnLimpiarcli.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLimpiarcli.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiarcli.Location = new System.Drawing.Point(10, 66);
            this.btnLimpiarcli.Margin = new System.Windows.Forms.Padding(2);
            this.btnLimpiarcli.Name = "btnLimpiarcli";
            this.btnLimpiarcli.Size = new System.Drawing.Size(138, 37);
            this.btnLimpiarcli.TabIndex = 142;
            this.btnLimpiarcli.Text = "Limpiar";
            this.btnLimpiarcli.UseVisualStyleBackColor = true;
            this.btnLimpiarcli.Click += new System.EventHandler(this.btnLimpiarcli_Click);
            // 
            // Lblcliente
            // 
            this.Lblcliente.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Lblcliente.AutoSize = true;
            this.Lblcliente.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lblcliente.Location = new System.Drawing.Point(9, 22);
            this.Lblcliente.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lblcliente.Name = "Lblcliente";
            this.Lblcliente.Size = new System.Drawing.Size(137, 16);
            this.Lblcliente.TabIndex = 141;
            this.Lblcliente.Text = "Nombre del Cliente";
            // 
            // dgvcliente
            // 
            this.dgvcliente.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvcliente.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvcliente.Location = new System.Drawing.Point(14, 159);
            this.dgvcliente.Margin = new System.Windows.Forms.Padding(2);
            this.dgvcliente.Name = "dgvcliente";
            this.dgvcliente.RowHeadersWidth = 82;
            this.dgvcliente.RowTemplate.Height = 33;
            this.dgvcliente.Size = new System.Drawing.Size(751, 355);
            this.dgvcliente.TabIndex = 11;
            this.dgvcliente.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvcliente_CellContentClick);
            // 
            // UCBUSCADOR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvcliente);
            this.Controls.Add(this.gpclie);
            this.Controls.Add(this.CBOperacion);
            this.Name = "UCBUSCADOR";
            this.Size = new System.Drawing.Size(767, 516);
            this.gpclie.ResumeLayout(false);
            this.gpclie.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvcliente)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CBOperacion;
        private System.Windows.Forms.GroupBox gpclie;
        private System.Windows.Forms.ComboBox cmbclientes;
        private System.Windows.Forms.Button BtnBuscarcli;
        private System.Windows.Forms.Button btnLimpiarcli;
        private System.Windows.Forms.Label Lblcliente;
        private System.Windows.Forms.DataGridView dgvcliente;
    }
}
