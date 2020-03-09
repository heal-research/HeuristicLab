namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class JsonItemValidValuesControl {
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
      this.tableOptions = new System.Windows.Forms.TableLayoutPanel();
      this.comboBoxValues = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBoxRange = new System.Windows.Forms.GroupBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBoxRange.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableOptions
      // 
      this.tableOptions.AutoScroll = true;
      this.tableOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.tableOptions.ColumnCount = 2;
      this.tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableOptions.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableOptions.Location = new System.Drawing.Point(3, 16);
      this.tableOptions.Name = "tableOptions";
      this.tableOptions.RowCount = 1;
      this.tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableOptions.Size = new System.Drawing.Size(494, 217);
      this.tableOptions.TabIndex = 12;
      // 
      // comboBoxValues
      // 
      this.comboBoxValues.Dock = System.Windows.Forms.DockStyle.Fill;
      this.comboBoxValues.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxValues.FormattingEnabled = true;
      this.comboBoxValues.Location = new System.Drawing.Point(100, 0);
      this.comboBoxValues.Margin = new System.Windows.Forms.Padding(0);
      this.comboBoxValues.Name = "comboBoxValues";
      this.comboBoxValues.Size = new System.Drawing.Size(400, 21);
      this.comboBoxValues.TabIndex = 15;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label2.Location = new System.Drawing.Point(0, 0);
      this.label2.Margin = new System.Windows.Forms.Padding(0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 22);
      this.label2.TabIndex = 16;
      this.label2.Text = "Value";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // groupBoxRange
      // 
      this.groupBoxRange.Controls.Add(this.tableOptions);
      this.groupBoxRange.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBoxRange.Location = new System.Drawing.Point(0, 22);
      this.groupBoxRange.Margin = new System.Windows.Forms.Padding(0);
      this.groupBoxRange.Name = "groupBoxRange";
      this.groupBoxRange.Size = new System.Drawing.Size(500, 236);
      this.groupBoxRange.TabIndex = 17;
      this.groupBoxRange.TabStop = false;
      this.groupBoxRange.Text = "Range";
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBoxRange, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 258);
      this.tableLayoutPanel1.TabIndex = 18;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.comboBoxValues, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(500, 22);
      this.tableLayoutPanel2.TabIndex = 19;
      // 
      // JsonItemValidValuesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.ForeColor = System.Drawing.Color.Black;
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "JsonItemValidValuesControl";
      this.Size = new System.Drawing.Size(500, 258);
      this.groupBoxRange.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.TableLayoutPanel tableOptions;
    private System.Windows.Forms.ComboBox comboBoxValues;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBoxRange;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}
