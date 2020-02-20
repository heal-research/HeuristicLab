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
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.numericRangeControl1 = new HeuristicLab.JsonInterface.OptimizerIntegration.NumericRangeControl();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.textBoxRows = new System.Windows.Forms.TextBox();
      this.checkBoxColumns = new System.Windows.Forms.CheckBox();
      this.checkBoxRows = new System.Windows.Forms.CheckBox();
      this.textBoxColumns = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.AllowUserToResizeColumns = false;
      this.dataGridView.AllowUserToResizeRows = false;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Location = new System.Drawing.Point(6, 67);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.Size = new System.Drawing.Size(473, 192);
      this.dataGridView.TabIndex = 13;
      // 
      // numericRangeControl1
      // 
      this.numericRangeControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numericRangeControl1.Location = new System.Drawing.Point(9, 372);
      this.numericRangeControl1.Name = "numericRangeControl1";
      this.numericRangeControl1.Size = new System.Drawing.Size(487, 71);
      this.numericRangeControl1.TabIndex = 14;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.textBoxColumns);
      this.groupBox1.Controls.Add(this.textBoxRows);
      this.groupBox1.Controls.Add(this.checkBoxColumns);
      this.groupBox1.Controls.Add(this.checkBoxRows);
      this.groupBox1.Controls.Add(this.dataGridView);
      this.groupBox1.Location = new System.Drawing.Point(9, 101);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(485, 265);
      this.groupBox1.TabIndex = 15;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Value";
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      // 
      // textBoxRows
      // 
      this.textBoxRows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.textBoxRows, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxRows.Location = new System.Drawing.Point(83, 17);
      this.textBoxRows.Name = "textBoxRows";
      this.textBoxRows.ReadOnly = true;
      this.textBoxRows.Size = new System.Drawing.Size(396, 20);
      this.textBoxRows.TabIndex = 16;
      this.textBoxRows.TextChanged += new System.EventHandler(this.textBoxRows_TextChanged);
      this.textBoxRows.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxRows_Validating);
      // 
      // checkBoxColumns
      // 
      this.checkBoxColumns.AutoSize = true;
      this.checkBoxColumns.Location = new System.Drawing.Point(7, 43);
      this.checkBoxColumns.Name = "checkBoxColumns";
      this.checkBoxColumns.Size = new System.Drawing.Size(69, 17);
      this.checkBoxColumns.TabIndex = 15;
      this.checkBoxColumns.Text = "Columns:";
      this.checkBoxColumns.UseVisualStyleBackColor = true;
      // 
      // checkBoxRows
      // 
      this.checkBoxRows.AutoSize = true;
      this.checkBoxRows.Location = new System.Drawing.Point(7, 20);
      this.checkBoxRows.Name = "checkBoxRows";
      this.checkBoxRows.Size = new System.Drawing.Size(56, 17);
      this.checkBoxRows.TabIndex = 14;
      this.checkBoxRows.Text = "Rows:";
      this.checkBoxRows.UseVisualStyleBackColor = true;
      // 
      // textBoxColumns
      // 
      this.textBoxColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.textBoxColumns, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxColumns.Location = new System.Drawing.Point(83, 43);
      this.textBoxColumns.Name = "textBoxColumns";
      this.textBoxColumns.ReadOnly = true;
      this.textBoxColumns.Size = new System.Drawing.Size(396, 20);
      this.textBoxColumns.TabIndex = 17;
      this.textBoxColumns.TextChanged += new System.EventHandler(this.textBoxColumns_TextChanged);
      this.textBoxColumns.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxColumns_Validating);
      // 
      // JsonItemMultiValueControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.numericRangeControl1);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Name = "JsonItemMultiValueControl";
      this.Size = new System.Drawing.Size(502, 449);
      this.Controls.SetChildIndex(this.numericRangeControl1, 0);
      this.Controls.SetChildIndex(this.groupBox1, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView;
    private NumericRangeControl numericRangeControl1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckBox checkBoxColumns;
    private System.Windows.Forms.CheckBox checkBoxRows;
    private System.Windows.Forms.TextBox textBoxRows;
    private System.Windows.Forms.TextBox textBoxColumns;
  }
}
