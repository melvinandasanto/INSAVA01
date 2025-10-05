namespace Usuario
{
    partial class FACTURA
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FACTURA));
            this.BtnNuevoCliente = new System.Windows.Forms.Button();
            this.cmbMetodoPago = new System.Windows.Forms.ComboBox();
            this.lblClienteInfo = new System.Windows.Forms.Label();
            this.lblTotalPagar = new System.Windows.Forms.Label();
            this.dgvFactura = new System.Windows.Forms.DataGridView();
            this.cmbClientes = new System.Windows.Forms.ComboBox();
            this.btnImprimirFactura = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGuardarVenta = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFactura)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnNuevoCliente
            // 
            this.BtnNuevoCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnNuevoCliente.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnNuevoCliente.Location = new System.Drawing.Point(669, 52);
            this.BtnNuevoCliente.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BtnNuevoCliente.Name = "BtnNuevoCliente";
            this.BtnNuevoCliente.Size = new System.Drawing.Size(133, 26);
            this.BtnNuevoCliente.TabIndex = 10;
            this.BtnNuevoCliente.Text = "Nuevo Cliente";
            this.BtnNuevoCliente.UseVisualStyleBackColor = true;
            this.BtnNuevoCliente.Click += new System.EventHandler(this.btnNuevoCliente_Click);
            // 
            // cmbMetodoPago
            // 
            this.cmbMetodoPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMetodoPago.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMetodoPago.FormattingEnabled = true;
            this.cmbMetodoPago.Location = new System.Drawing.Point(659, 154);
            this.cmbMetodoPago.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbMetodoPago.Name = "cmbMetodoPago";
            this.cmbMetodoPago.Size = new System.Drawing.Size(252, 26);
            this.cmbMetodoPago.TabIndex = 9;
            // 
            // lblClienteInfo
            // 
            this.lblClienteInfo.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClienteInfo.Location = new System.Drawing.Point(15, 27);
            this.lblClienteInfo.Name = "lblClienteInfo";
            this.lblClienteInfo.Size = new System.Drawing.Size(563, 128);
            this.lblClienteInfo.TabIndex = 8;
            this.lblClienteInfo.Text = "label2";
            // 
            // lblTotalPagar
            // 
            this.lblTotalPagar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblTotalPagar.Font = new System.Drawing.Font("Arial Rounded MT Bold", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPagar.Location = new System.Drawing.Point(8, 485);
            this.lblTotalPagar.Name = "lblTotalPagar";
            this.lblTotalPagar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblTotalPagar.Size = new System.Drawing.Size(675, 70);
            this.lblTotalPagar.TabIndex = 7;
            this.lblTotalPagar.Text = "label1";
            // 
            // dgvFactura
            // 
            this.dgvFactura.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.dgvFactura.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFactura.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFactura.Location = new System.Drawing.Point(13, 196);
            this.dgvFactura.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvFactura.Name = "dgvFactura";
            this.dgvFactura.RowHeadersWidth = 82;
            this.dgvFactura.RowTemplate.Height = 33;
            this.dgvFactura.Size = new System.Drawing.Size(901, 236);
            this.dgvFactura.TabIndex = 6;
            // 
            // cmbClientes
            // 
            this.cmbClientes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbClientes.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbClientes.FormattingEnabled = true;
            this.cmbClientes.Location = new System.Drawing.Point(669, 18);
            this.cmbClientes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbClientes.Name = "cmbClientes";
            this.cmbClientes.Size = new System.Drawing.Size(252, 26);
            this.cmbClientes.TabIndex = 15;
            // 
            // btnImprimirFactura
            // 
            this.btnImprimirFactura.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImprimirFactura.Location = new System.Drawing.Point(804, 514);
            this.btnImprimirFactura.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnImprimirFactura.Name = "btnImprimirFactura";
            this.btnImprimirFactura.Size = new System.Drawing.Size(105, 42);
            this.btnImprimirFactura.TabIndex = 16;
            this.btnImprimirFactura.Text = "Imprimir";
            this.btnImprimirFactura.UseVisualStyleBackColor = true;
            this.btnImprimirFactura.Click += new System.EventHandler(this.btnImprimirFactura_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(757, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "Metodo De Pago";
            // 
            // btnGuardarVenta
            // 
            this.btnGuardarVenta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardarVenta.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarVenta.Location = new System.Drawing.Point(804, 458);
            this.btnGuardarVenta.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGuardarVenta.Name = "btnGuardarVenta";
            this.btnGuardarVenta.Size = new System.Drawing.Size(105, 50);
            this.btnGuardarVenta.TabIndex = 18;
            this.btnGuardarVenta.Text = "Guardar Venta";
            this.btnGuardarVenta.UseVisualStyleBackColor = true;
            this.btnGuardarVenta.Click += new System.EventHandler(this.btnGuardarVenta_Click);
            // 
            // FACTURA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 561);
            this.Controls.Add(this.btnGuardarVenta);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnImprimirFactura);
            this.Controls.Add(this.cmbClientes);
            this.Controls.Add(this.BtnNuevoCliente);
            this.Controls.Add(this.cmbMetodoPago);
            this.Controls.Add(this.lblClienteInfo);
            this.Controls.Add(this.lblTotalPagar);
            this.Controls.Add(this.dgvFactura);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "FACTURA";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FACTURA";
            this.Click += new System.EventHandler(this.FACTURA_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFactura)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnNuevoCliente;
        private System.Windows.Forms.ComboBox cmbMetodoPago;
        private System.Windows.Forms.Label lblClienteInfo;
        private System.Windows.Forms.Label lblTotalPagar;
        private System.Windows.Forms.DataGridView dgvFactura;
        private System.Windows.Forms.ComboBox cmbClientes;
        private System.Windows.Forms.Button btnImprimirFactura;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGuardarVenta;
    }
}       