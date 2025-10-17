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
            this.gpclie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvcliente)).BeginInit();
            this.SuspendLayout();
            // 
            // CBOperacion
            // 
            this.CBOperacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBOperacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CBOperacion.FormattingEnabled = true;
            this.CBOperacion.Items.AddRange(new object[] {
            "Clientes",
            "Facturas"});
            this.CBOperacion.Location = new System.Drawing.Point(848, 34);
            this.CBOperacion.Name = "CBOperacion";
            this.CBOperacion.Size = new System.Drawing.Size(274, 33);
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
            this.gpclie.Location = new System.Drawing.Point(3, 34);
            this.gpclie.Name = "gpclie";
            this.gpclie.Size = new System.Drawing.Size(692, 163);
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
            this.cmbclientes.Location = new System.Drawing.Point(15, 65);
            this.cmbclientes.Name = "cmbclientes";
            this.cmbclientes.Size = new System.Drawing.Size(664, 34);
            this.cmbclientes.TabIndex = 144;
            this.cmbclientes.SelectedIndexChanged += new System.EventHandler(this.cmbclientes_SelectedIndexChanged);
            // 
            // BtnBuscarcli
            // 
            this.BtnBuscarcli.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBuscarcli.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnBuscarcli.Location = new System.Drawing.Point(228, 102);
            this.BtnBuscarcli.Name = "BtnBuscarcli";
            this.BtnBuscarcli.Size = new System.Drawing.Size(448, 57);
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
            this.btnLimpiarcli.Location = new System.Drawing.Point(15, 102);
            this.btnLimpiarcli.Name = "btnLimpiarcli";
            this.btnLimpiarcli.Size = new System.Drawing.Size(207, 57);
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
            this.Lblcliente.Location = new System.Drawing.Point(14, 34);
            this.Lblcliente.Name = "Lblcliente";
            this.Lblcliente.Size = new System.Drawing.Size(195, 25);
            this.Lblcliente.TabIndex = 141;
            this.Lblcliente.Text = "Nombre del Cliente";
            // 
            // dgvcliente
            // 
            this.dgvcliente.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvcliente.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvcliente.Location = new System.Drawing.Point(21, 245);
            this.dgvcliente.Name = "dgvcliente";
            this.dgvcliente.RowHeadersWidth = 82;
            this.dgvcliente.RowTemplate.Height = 33;
            this.dgvcliente.Size = new System.Drawing.Size(1126, 546);
            this.dgvcliente.TabIndex = 11;
            this.dgvcliente.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvcliente_CellContentClick);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(452, 11);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(180, 32);
            this.label8.TabIndex = 382;
            this.label8.Text = "BUSCADOR ";
            // 
            // UCBUSCADOR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dgvcliente);
            this.Controls.Add(this.gpclie);
            this.Controls.Add(this.CBOperacion);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UCBUSCADOR";
            this.Size = new System.Drawing.Size(1150, 794);
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
    }
}
