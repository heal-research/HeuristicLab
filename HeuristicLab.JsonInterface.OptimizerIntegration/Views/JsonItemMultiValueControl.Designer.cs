namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class JsonItemMultiValueControl<T> {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.numericRangeControl1 = new HeuristicLab.JsonInterface.OptimizerIntegration.NumericRangeControl();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.textBoxColumns = new System.Windows.Forms.TextBox();
      this.textBoxRows = new System.Windows.Forms.TextBox();
      this.checkBoxColumns = new System.Windows.Forms.CheckBox();
      this.checkBoxRows = new System.Windows.Forms.CheckBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.AllowUserToResizeColumns = false;
      this.dataGridView.AllowUserToResizeRows = false;
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView.Location = new System.Drawing.Point(0, 44);
      this.dataGridView.Margin = new System.Windows.Forms.Padding(0);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.Size = new System.Drawing.Size(475, 172);
      this.dataGridView.TabIndex = 13;
      // 
      // numericRangeControl1
      // 
      this.numericRangeControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numericRangeControl1.Location = new System.Drawing.Point(0, 235);
      this.numericRangeControl1.Margin = new System.Windows.Forms.Padding(0);
      this.numericRangeControl1.Name = "numericRangeControl1";
      this.numericRangeControl1.Size = new System.Drawing.Size(481, 63);
      this.numericRangeControl1.TabIndex = 14;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel2);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(481, 235);
      this.groupBox1.TabIndex = 15;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Value";
      // 
      // textBoxColumns
      // 
      this.textBoxColumns.Dock = System.Windows.Forms.DockStyle.Fill;
      this.errorProvider.SetIconAlignment(this.textBoxColumns, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxColumns.Location = new System.Drawing.Point(100, 22);
      this.textBoxColumns.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxColumns.Name = "textBoxColumns";
      this.textBoxColumns.ReadOnly = true;
      this.textBoxColumns.Size = new System.Drawing.Size(375, 20);
      this.textBoxColumns.TabIndex = 17;
      this.textBoxColumns.Leave += new System.EventHandler(this.textBoxColumns_TextChanged);
      this.textBoxColumns.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxColumns_Validating);
      // 
      // textBoxRows
      // 
      this.textBoxRows.Dock = System.Windows.Forms.DockStyle.Fill;
      this.errorProvider.SetIconAlignment(this.textBoxRows, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxRows.Location = new System.Drawing.Point(100, 0);
      this.textBoxRows.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxRows.Name = "textBoxRows";
      this.textBoxRows.ReadOnly = true;
      this.textBoxRows.Size = new System.Drawing.Size(375, 20);
      this.textBoxRows.TabIndex = 16;
      this.textBoxRows.Leave += new System.EventHandler(this.textBoxRows_TextChanged);
      this.textBoxRows.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxRows_Validating);
      // 
      // checkBoxColumns
      // 
      this.checkBoxColumns.AutoSize = true;
      this.checkBoxColumns.Dock = System.Windows.Forms.DockStyle.Fill;
      this.checkBoxColumns.Location = new System.Drawing.Point(0, 22);
      this.checkBoxColumns.Margin = new System.Windows.Forms.Padding(0);
      this.checkBoxColumns.Name = "checkBoxColumns";
      this.checkBoxColumns.Size = new System.Drawing.Size(100, 22);
      this.checkBoxColumns.TabIndex = 15;
      this.checkBoxColumns.Text = "Columns:";
      this.checkBoxColumns.UseVisualStyleBackColor = true;
      // 
      // checkBoxRows
      // 
      this.checkBoxRows.AutoSize = true;
      this.checkBoxRows.Dock = System.Windows.Forms.DockStyle.Fill;
      this.checkBoxRows.Location = new System.Drawing.Point(0, 0);
      this.checkBoxRows.Margin = new System.Windows.Forms.Padding(0);
      this.checkBoxRows.Name = "checkBoxRows";
      this.checkBoxRows.Size = new System.Drawing.Size(100, 22);
      this.checkBoxRows.TabIndex = 14;
      this.checkBoxRows.Text = "Rows:";
      this.checkBoxRows.UseVisualStyleBackColor = true;
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.textBoxColumns, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxRows, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.textBoxRows, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.checkBoxColumns, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(475, 44);
      this.tableLayoutPanel1.TabIndex = 16;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.dataGridView, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(475, 216);
      this.tableLayoutPanel2.TabIndex = 17;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.numericRangeControl1, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(481, 298);
      this.tableLayoutPanel3.TabIndex = 16;
      // 
      // JsonItemMultiValueControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel3);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "JsonItemMultiValueControl";
      this.Size = new System.Drawing.Size(481, 298);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView;
    private NumericRangeControl numericRangeControl1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckBox checkBoxColumns;
    private System.Windows.Forms.CheckBox checkBoxRows;
    private System.Windows.Forms.TextBox textBoxRows;
    private System.Windows.Forms.TextBox textBoxColumns;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
  }
}
