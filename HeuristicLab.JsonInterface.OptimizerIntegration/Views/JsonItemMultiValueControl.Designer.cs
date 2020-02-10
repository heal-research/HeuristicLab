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
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView
      // 
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Location = new System.Drawing.Point(9, 75);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.Size = new System.Drawing.Size(487, 171);
      this.dataGridView.TabIndex = 13;
      // 
      // numericRangeControl1
      // 
      this.numericRangeControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numericRangeControl1.Location = new System.Drawing.Point(9, 252);
      this.numericRangeControl1.Name = "numericRangeControl1";
      this.numericRangeControl1.Size = new System.Drawing.Size(487, 128);
      this.numericRangeControl1.TabIndex = 14;
      // 
      // JsonItemArrayValueControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.numericRangeControl1);
      this.Controls.Add(this.dataGridView);
      this.Name = "JsonItemArrayValueControl";
      this.Size = new System.Drawing.Size(502, 386);
      this.Controls.SetChildIndex(this.dataGridView, 0);
      this.Controls.SetChildIndex(this.numericRangeControl1, 0);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView;
    private NumericRangeControl numericRangeControl1;
  }
}
