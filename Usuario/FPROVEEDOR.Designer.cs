namespace Usuario
{
    partial class FPROVEEDOR
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

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FPROVEEDOR));
            this.DGPROVEEDOR = new System.Windows.Forms.DataGridView();
            this.txtNombreProveedor = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.checkactivo = new System.Windows.Forms.CheckBox();
            this.btnClean = new System.Windows.Forms.Button();
            this.comboIDProveedor = new System.Windows.Forms.ComboBox();
            this.txtTelefonoProveedor = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboFiltroActivo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGPROVEEDOR)).BeginInit();
            this.SuspendLayout();
            // 
            // DGPROVEEDOR
            // 
            this.DGPROVEEDOR.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.DGPROVEEDOR.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGPROVEEDOR.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.DGPROVEEDOR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGPROVEEDOR.Location = new System.Drawing.Point(11, 349);
            this.DGPROVEEDOR.Name = "DGPROVEEDOR";
            this.DGPROVEEDOR.RowHeadersWidth = 82;
            this.DGPROVEEDOR.RowTemplate.Height = 33;
            this.DGPROVEEDOR.Size = new System.Drawing.Size(923, 159);
            this.DGPROVEEDOR.TabIndex = 54;
            // 
            // txtNombreProveedor
            // 
            this.txtNombreProveedor.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtNombreProveedor.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombreProveedor.Location = new System.Drawing.Point(23, 132);
            this.txtNombreProveedor.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombreProveedor.Name = "txtNombreProveedor";
            this.txtNombreProveedor.Size = new System.Drawing.Size(420, 39);
            this.txtNombreProveedor.TabIndex = 49;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 104);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 23);
            this.label3.TabIndex = 56;
            this.label3.Text = "Nombre Proveedor";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnEliminar.AutoSize = true;
            this.btnEliminar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LimeGreen;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.Location = new System.Drawing.Point(407, 195);
            this.btnEliminar.Margin = new System.Windows.Forms.Padding(4);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(191, 53);
            this.btnEliminar.TabIndex = 52;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnEditar.AutoSize = true;
            this.btnEditar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LimeGreen;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditar.Location = new System.Drawing.Point(205, 195);
            this.btnEditar.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(193, 53);
            this.btnEditar.TabIndex = 51;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnGuardar.AutoSize = true;
            this.btnGuardar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LimeGreen;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Location = new System.Drawing.Point(605, 195);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(196, 53);
            this.btnGuardar.TabIndex = 53;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnBuscar.AutoSize = true;
            this.btnBuscar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LimeGreen;
            this.btnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(7, 195);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(191, 53);
            this.btnBuscar.TabIndex = 50;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 23);
            this.label2.TabIndex = 55;
            this.label2.Text = "ID Cliente";
            // 
            // checkactivo
            // 
            this.checkactivo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.checkactivo.AutoSize = true;
            this.checkactivo.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkactivo.Location = new System.Drawing.Point(492, 138);
            this.checkactivo.Margin = new System.Windows.Forms.Padding(4);
            this.checkactivo.Name = "checkactivo";
            this.checkactivo.Size = new System.Drawing.Size(102, 30);
            this.checkactivo.TabIndex = 242;
            this.checkactivo.Text = "Activo";
            this.checkactivo.UseVisualStyleBackColor = true;
            // 
            // btnClean
            // 
            this.btnClean.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnClean.AutoSize = true;
            this.btnClean.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LimeGreen;
            this.btnClean.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClean.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClean.Location = new System.Drawing.Point(809, 195);
            this.btnClean.Margin = new System.Windows.Forms.Padding(4);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(121, 53);
            this.btnClean.TabIndex = 246;
            this.btnClean.Text = "Limpiar";
            this.btnClean.UseVisualStyleBackColor = true;
            // 
            // comboIDProveedor
            // 
            this.comboIDProveedor.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.comboIDProveedor.FormattingEnabled = true;
            this.comboIDProveedor.Location = new System.Drawing.Point(20, 52);
            this.comboIDProveedor.Margin = new System.Windows.Forms.Padding(4);
            this.comboIDProveedor.Name = "comboIDProveedor";
            this.comboIDProveedor.Size = new System.Drawing.Size(421, 24);
            this.comboIDProveedor.TabIndex = 248;
            // 
            // txtTelefonoProveedor
            // 
            this.txtTelefonoProveedor.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtTelefonoProveedor.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTelefonoProveedor.Location = new System.Drawing.Point(492, 52);
            this.txtTelefonoProveedor.Margin = new System.Windows.Forms.Padding(4);
            this.txtTelefonoProveedor.Name = "txtTelefonoProveedor";
            this.txtTelefonoProveedor.Size = new System.Drawing.Size(420, 39);
            this.txtTelefonoProveedor.TabIndex = 249;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(487, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 23);
            this.label1.TabIndex = 250;
            this.label1.Text = "Telefono Proveedor";
            // 
            // cboFiltroActivo
            // 
            this.cboFiltroActivo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cboFiltroActivo.FormattingEnabled = true;
            this.cboFiltroActivo.Location = new System.Drawing.Point(850, 286);
            this.cboFiltroActivo.Margin = new System.Windows.Forms.Padding(2);
            this.cboFiltroActivo.Name = "cboFiltroActivo";
            this.cboFiltroActivo.Size = new System.Drawing.Size(82, 24);
            this.cboFiltroActivo.TabIndex = 251;
            this.cboFiltroActivo.SelectedIndexChanged += new System.EventHandler(this.cboFiltroActivo_SelectedIndexChanged);
            // 
            // FPROVEEDOR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 523);
            this.Controls.Add(this.cboFiltroActivo);
            this.Controls.Add(this.txtTelefonoProveedor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboIDProveedor);
            this.Controls.Add(this.btnClean);
            this.Controls.Add(this.checkactivo);
            this.Controls.Add(this.DGPROVEEDOR);
            this.Controls.Add(this.txtNombreProveedor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FPROVEEDOR";
            this.Text = "PROVEEDOR";
            this.Load += new System.EventHandler(this.PROVEEDOR_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DGPROVEEDOR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DGPROVEEDOR;
        private System.Windows.Forms.TextBox txtNombreProveedor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkactivo;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.ComboBox comboIDProveedor;
        private System.Windows.Forms.TextBox txtTelefonoProveedor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboFiltroActivo;
    }
}

