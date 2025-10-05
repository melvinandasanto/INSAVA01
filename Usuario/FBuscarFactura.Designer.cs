namespace Usuario
{
    partial class FBuscarFactura
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FBuscarFactura));
            this.txtBuscarFactura = new System.Windows.Forms.TextBox();
            this.dgvFacturas = new System.Windows.Forms.DataGridView();
            this.lblFBC = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBuscarFactura
            // 
            this.txtBuscarFactura.Location = new System.Drawing.Point(40, 107);
            this.txtBuscarFactura.Margin = new System.Windows.Forms.Padding(1);
            this.txtBuscarFactura.Name = "txtBuscarFactura";
            this.txtBuscarFactura.Size = new System.Drawing.Size(717, 22);
            this.txtBuscarFactura.TabIndex = 0;
            // 
            // dgvFacturas
            // 
            this.dgvFacturas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvFacturas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFacturas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFacturas.Location = new System.Drawing.Point(39, 175);
            this.dgvFacturas.Margin = new System.Windows.Forms.Padding(1);
            this.dgvFacturas.Name = "dgvFacturas";
            this.dgvFacturas.RowHeadersWidth = 82;
            this.dgvFacturas.RowTemplate.Height = 33;
            this.dgvFacturas.Size = new System.Drawing.Size(719, 295);
            this.dgvFacturas.TabIndex = 1;
            // 
            // lblFBC
            // 
            this.lblFBC.AutoSize = true;
            this.lblFBC.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFBC.Location = new System.Drawing.Point(31, 25);
            this.lblFBC.Name = "lblFBC";
            this.lblFBC.Size = new System.Drawing.Size(311, 46);
            this.lblFBC.TabIndex = 4;
            this.lblFBC.Text = "Buscar Facturas";
            // 
            // FBuscarFactura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 599);
            this.Controls.Add(this.lblFBC);
            this.Controls.Add(this.dgvFacturas);
            this.Controls.Add(this.txtBuscarFactura);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "FBuscarFactura";
            this.Text = "FBuscarFactura";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBuscarFactura;
        private System.Windows.Forms.DataGridView dgvFacturas;
        private System.Windows.Forms.Label lblFBC;
    }
}