namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class NumericRangeControl {
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
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.textBoxTo = new System.Windows.Forms.TextBox();
      this.textBoxFrom = new System.Windows.Forms.TextBox();
      this.checkBoxFrom = new System.Windows.Forms.CheckBox();
      this.checkBoxTo = new System.Windows.Forms.CheckBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.tableLayoutPanel2);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(0, 0);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(526, 63);
      this.groupBox2.TabIndex = 19;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Range";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.textBoxTo, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.textBoxFrom, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.checkBoxFrom, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.checkBoxTo, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(520, 44);
      this.tableLayoutPanel2.TabIndex = 22;
      // 
      // textBoxTo
      // 
      this.textBoxTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.errorProvider.SetIconAlignment(this.textBoxTo, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxTo.Location = new System.Drawing.Point(100, 22);
      this.textBoxTo.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxTo.Name = "textBoxTo";
      this.textBoxTo.ReadOnly = true;
      this.textBoxTo.Size = new System.Drawing.Size(420, 20);
      this.textBoxTo.TabIndex = 6;
      this.textBoxTo.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxTo_Validating);
      // 
      // textBoxFrom
      // 
      this.textBoxFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.errorProvider.SetIconAlignment(this.textBoxFrom, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxFrom.Location = new System.Drawing.Point(100, 0);
      this.textBoxFrom.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxFrom.Name = "textBoxFrom";
      this.textBoxFrom.ReadOnly = true;
      this.textBoxFrom.Size = new System.Drawing.Size(420, 20);
      this.textBoxFrom.TabIndex = 2;
      this.textBoxFrom.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxFrom_Validating);
      // 
      // checkBoxFrom
      // 
      this.checkBoxFrom.AutoSize = true;
      this.checkBoxFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.checkBoxFrom.Location = new System.Drawing.Point(0, 0);
      this.checkBoxFrom.Margin = new System.Windows.Forms.Padding(0);
      this.checkBoxFrom.Name = "checkBoxFrom";
      this.checkBoxFrom.Size = new System.Drawing.Size(100, 22);
      this.checkBoxFrom.TabIndex = 4;
      this.checkBoxFrom.Text = "From:";
      this.checkBoxFrom.UseVisualStyleBackColor = true;
      // 
      // checkBoxTo
      // 
      this.checkBoxTo.AutoSize = true;
      this.checkBoxTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.checkBoxTo.Location = new System.Drawing.Point(0, 22);
      this.checkBoxTo.Margin = new System.Windows.Forms.Padding(0);
      this.checkBoxTo.Name = "checkBoxTo";
      this.checkBoxTo.Size = new System.Drawing.Size(100, 22);
      this.checkBoxTo.TabIndex = 7;
      this.checkBoxTo.Text = "To:";
      this.checkBoxTo.UseVisualStyleBackColor = true;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(526, 63);
      this.tableLayoutPanel1.TabIndex = 23;
      // 
      // NumericRangeControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "NumericRangeControl";
      this.Size = new System.Drawing.Size(526, 63);
      this.groupBox2.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.CheckBox checkBoxTo;
    private System.Windows.Forms.TextBox textBoxTo;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.CheckBox checkBoxFrom;
    private System.Windows.Forms.TextBox textBoxFrom;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}
