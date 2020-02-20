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
      this.tableOptions = new System.Windows.Forms.TableLayoutPanel();
      this.comboBoxValues = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBoxRange = new System.Windows.Forms.GroupBox();
      this.groupBoxRange.SuspendLayout();
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
      this.tableOptions.Size = new System.Drawing.Size(482, 170);
      this.tableOptions.TabIndex = 12;
      // 
      // comboBoxValues
      // 
      this.comboBoxValues.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxValues.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxValues.FormattingEnabled = true;
      this.comboBoxValues.Location = new System.Drawing.Point(92, 101);
      this.comboBoxValues.Name = "comboBoxValues";
      this.comboBoxValues.Size = new System.Drawing.Size(402, 21);
      this.comboBoxValues.TabIndex = 15;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 104);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(34, 13);
      this.label2.TabIndex = 16;
      this.label2.Text = "Value";
      // 
      // groupBoxRange
      // 
      this.groupBoxRange.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBoxRange.Controls.Add(this.tableOptions);
      this.groupBoxRange.Location = new System.Drawing.Point(6, 128);
      this.groupBoxRange.Name = "groupBoxRange";
      this.groupBoxRange.Size = new System.Drawing.Size(488, 189);
      this.groupBoxRange.TabIndex = 17;
      this.groupBoxRange.TabStop = false;
      this.groupBoxRange.Text = "Range";
      // 
      // JsonItemValidValuesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBoxRange);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.comboBoxValues);
      this.ForeColor = System.Drawing.Color.Black;
      this.Name = "JsonItemValidValuesControl";
      this.Size = new System.Drawing.Size(500, 323);
      this.Controls.SetChildIndex(this.comboBoxValues, 0);
      this.Controls.SetChildIndex(this.label2, 0);
      this.Controls.SetChildIndex(this.groupBoxRange, 0);
      this.groupBoxRange.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.TableLayoutPanel tableOptions;
    private System.Windows.Forms.ComboBox comboBoxValues;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBoxRange;
  }
}
