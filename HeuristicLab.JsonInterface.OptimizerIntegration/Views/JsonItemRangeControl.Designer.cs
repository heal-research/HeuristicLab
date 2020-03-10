namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class JsonItemRangeControl {
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.textBoxValueTo = new System.Windows.Forms.TextBox();
      this.textBoxValueFrom = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.numericRangeControl = new HeuristicLab.JsonInterface.OptimizerIntegration.NumericRangeControl();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.tableLayoutPanel1);
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(500, 68);
      this.groupBox1.TabIndex = 17;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Value";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.textBoxValueTo, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.textBoxValueFrom, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(494, 49);
      this.tableLayoutPanel1.TabIndex = 19;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label2.Location = new System.Drawing.Point(0, 0);
      this.label2.Margin = new System.Windows.Forms.Padding(0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 24);
      this.label2.TabIndex = 0;
      this.label2.Text = "From:";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // textBoxValueTo
      // 
      this.textBoxValueTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBoxValueTo.Location = new System.Drawing.Point(100, 24);
      this.textBoxValueTo.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxValueTo.Name = "textBoxValueTo";
      this.textBoxValueTo.Size = new System.Drawing.Size(394, 20);
      this.textBoxValueTo.TabIndex = 3;
      // 
      // textBoxValueFrom
      // 
      this.textBoxValueFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBoxValueFrom.Location = new System.Drawing.Point(100, 0);
      this.textBoxValueFrom.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxValueFrom.Name = "textBoxValueFrom";
      this.textBoxValueFrom.Size = new System.Drawing.Size(394, 20);
      this.textBoxValueFrom.TabIndex = 2;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label4.Location = new System.Drawing.Point(0, 24);
      this.label4.Margin = new System.Windows.Forms.Padding(0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(100, 25);
      this.label4.TabIndex = 1;
      this.label4.Text = "To:";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // numericRangeControl
      // 
      this.numericRangeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numericRangeControl.Location = new System.Drawing.Point(0, 71);
      this.numericRangeControl.Margin = new System.Windows.Forms.Padding(0);
      this.numericRangeControl.Name = "numericRangeControl";
      this.numericRangeControl.Size = new System.Drawing.Size(500, 74);
      this.numericRangeControl.TabIndex = 18;
      // 
      // JsonItemRangeControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.numericRangeControl);
      this.Controls.Add(this.groupBox1);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "JsonItemRangeControl";
      this.Size = new System.Drawing.Size(500, 147);
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox textBoxValueTo;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label2;
    private NumericRangeControl numericRangeControl;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.TextBox textBoxValueFrom;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}
