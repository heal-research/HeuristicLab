namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class JsonItemValueControl {
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
      this.textBoxValue = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.numericRangeControl1 = new HeuristicLab.JsonInterface.OptimizerIntegration.NumericRangeControl();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // textBoxValue
      // 
      this.textBoxValue.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBoxValue.Location = new System.Drawing.Point(100, 0);
      this.textBoxValue.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxValue.Name = "textBoxValue";
      this.textBoxValue.Size = new System.Drawing.Size(577, 20);
      this.textBoxValue.TabIndex = 14;
      this.textBoxValue.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxValue_Validating);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label2.Location = new System.Drawing.Point(0, 0);
      this.label2.Margin = new System.Windows.Forms.Padding(0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 22);
      this.label2.TabIndex = 15;
      this.label2.Text = "Value";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // numericRangeControl1
      // 
      this.numericRangeControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.numericRangeControl1.Location = new System.Drawing.Point(0, 22);
      this.numericRangeControl1.Margin = new System.Windows.Forms.Padding(0);
      this.numericRangeControl1.Name = "numericRangeControl1";
      this.numericRangeControl1.Size = new System.Drawing.Size(677, 66);
      this.numericRangeControl1.TabIndex = 16;
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
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.textBoxValue, 1, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(677, 22);
      this.tableLayoutPanel1.TabIndex = 17;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.numericRangeControl1, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(677, 88);
      this.tableLayoutPanel2.TabIndex = 18;
      // 
      // JsonItemValueControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel2);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "JsonItemValueControl";
      this.Size = new System.Drawing.Size(677, 88);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox textBoxValue;
    private System.Windows.Forms.Label label2;
    private NumericRangeControl numericRangeControl1;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
  }
}
