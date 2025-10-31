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
            this.label8 = new System.Windows.Forms.Label();
            this.cboFiltroActivo = new System.Windows.Forms.ComboBox();
            this.gpclie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvcliente)).BeginInit();
            this.SuspendLayout();
            // 
            // CBOperacion
            // 
            this.CBOperacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBOperacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CBOperacion.FormattingEnabled = true;
            this.CBOperacion.Location = new System.Drawing.Point(566, 22);
            this.CBOperacion.Margin = new System.Windows.Forms.Padding(2);
            this.CBOperacion.Name = "CBOperacion";
            this.CBOperacion.Size = new System.Drawing.Size(184, 24);
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
            this.gpclie.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpclie.Location = new System.Drawing.Point(2, 22);
            this.gpclie.Margin = new System.Windows.Forms.Padding(2);
            this.gpclie.Name = "gpclie";
            this.gpclie.Padding = new System.Windows.Forms.Padding(2);
            this.gpclie.Size = new System.Drawing.Size(462, 106);
            this.gpclie.TabIndex = 10;
            this.gpclie.TabStop = false;
            this.gpclie.Text = "Descripcion del cliente";
            // 
            // cmbclientes
            // 
            this.cmbclientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbclientes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbclientes.FormattingEnabled = true;
            this.cmbclientes.Location = new System.Drawing.Point(10, 42);
            this.cmbclientes.Margin = new System.Windows.Forms.Padding(2);
            this.cmbclientes.Name = "cmbclientes";
            this.cmbclientes.Size = new System.Drawing.Size(444, 25);
            this.cmbclientes.TabIndex = 144;
            this.cmbclientes.SelectedIndexChanged += new System.EventHandler(this.cmbclientes_SelectedIndexChanged);
            // 
            // BtnBuscarcli
            // 
            this.BtnBuscarcli.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBuscarcli.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnBuscarcli.Location = new System.Drawing.Point(152, 67);
            this.BtnBuscarcli.Margin = new System.Windows.Forms.Padding(2);
            this.BtnBuscarcli.Name = "BtnBuscarcli";
            this.BtnBuscarcli.Size = new System.Drawing.Size(298, 37);
            this.BtnBuscarcli.TabIndex = 143;
            this.BtnBuscarcli.Text = "Buscar Cliente";
            this.BtnBuscarcli.UseVisualStyleBackColor = true;
            this.BtnBuscarcli.Click += new System.EventHandler(this.BtnBuscarcli_Click);
            // 
            // btnLimpiarcli
            // 
            this.btnLimpiarcli.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLimpiarcli.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiarcli.Location = new System.Drawing.Point(10, 67);
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
            this.Lblcliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lblcliente.Location = new System.Drawing.Point(10, 22);
            this.Lblcliente.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Lblcliente.Name = "Lblcliente";
            this.Lblcliente.Size = new System.Drawing.Size(128, 17);
            this.Lblcliente.TabIndex = 141;
            this.Lblcliente.Text = "Nombre del Cliente";
            // 
            // dgvcliente
            // 
            this.dgvcliente.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvcliente.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvcliente.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvcliente.Location = new System.Drawing.Point(14, 159);
            this.dgvcliente.Margin = new System.Windows.Forms.Padding(2);
            this.dgvcliente.Name = "dgvcliente";
            this.dgvcliente.RowHeadersWidth = 82;
            this.dgvcliente.RowTemplate.Height = 33;
            this.dgvcliente.Size = new System.Drawing.Size(750, 355);
            this.dgvcliente.TabIndex = 11;
            this.dgvcliente.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvcliente_CellContentClick);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(302, 7);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 24);
            this.label8.TabIndex = 382;
            this.label8.Text = "BUSCADOR ";
            // 
            // cboFiltroActivo
            // 
            this.cboFiltroActivo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFiltroActivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboFiltroActivo.FormattingEnabled = true;
            this.cboFiltroActivo.Location = new System.Drawing.Point(1133, 187);
            this.cboFiltroActivo.Margin = new System.Windows.Forms.Padding(4);
            this.cboFiltroActivo.Name = "cboFiltroActivo";
            this.cboFiltroActivo.Size = new System.Drawing.Size(202, 24);
            this.cboFiltroActivo.TabIndex = 383;
            this.cboFiltroActivo.SelectedIndexChanged += new System.EventHandler(this.cboFiltroActivo_SelectedIndexChanged);
            // 
            // UCBUSCADOR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboFiltroActivo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dgvcliente);
            this.Controls.Add(this.gpclie);
            this.Controls.Add(this.CBOperacion);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "UCBUSCADOR";
            this.Size = new System.Drawing.Size(766, 516);
            this.gpclie.ResumeLayout(false);
            this.gpclie.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvcliente)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CBOperacion;
        private System.Windows.Forms.GroupBox gpclie;
        private System.Windows.Forms.ComboBox cmbclientes;
        private System.Windows.Forms.Button BtnBuscarcli;
        private System.Windows.Forms.Button btnLimpiarcli;
        private System.Windows.Forms.Label Lblcliente;
        private System.Windows.Forms.DataGridView dgvcliente;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cboFiltroActivo;
    }
}
