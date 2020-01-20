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
      this.textBoxAdd = new System.Windows.Forms.TextBox();
      this.tableOptions = new System.Windows.Forms.TableLayoutPanel();
      this.buttonAdd = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.comboBoxValues = new System.Windows.Forms.ComboBox();
      this.tableOptions.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // textBoxAdd
      // 
      this.textBoxAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxAdd.Location = new System.Drawing.Point(3, 3);
      this.textBoxAdd.Name = "textBoxAdd";
      this.textBoxAdd.Size = new System.Drawing.Size(420, 20);
      this.textBoxAdd.TabIndex = 10;
      // 
      // tableOptions
      // 
      this.tableOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tableOptions.AutoSize = true;
      this.tableOptions.ColumnCount = 2;
      this.tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 43F));
      this.tableOptions.Controls.Add(this.textBoxAdd, 0, 0);
      this.tableOptions.Controls.Add(this.buttonAdd, 1, 0);
      this.tableOptions.Location = new System.Drawing.Point(3, 3);
      this.tableOptions.Name = "tableOptions";
      this.tableOptions.RowCount = 1;
      this.tableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableOptions.Size = new System.Drawing.Size(469, 73);
      this.tableOptions.TabIndex = 12;
      // 
      // buttonAdd
      // 
      this.buttonAdd.Location = new System.Drawing.Point(446, 3);
      this.buttonAdd.Name = "buttonAdd";
      this.buttonAdd.Size = new System.Drawing.Size(20, 20);
      this.buttonAdd.TabIndex = 12;
      this.buttonAdd.Text = "+";
      this.buttonAdd.UseVisualStyleBackColor = true;
      this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.panel2);
      this.groupBox1.Location = new System.Drawing.Point(6, 107);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(488, 104);
      this.groupBox1.TabIndex = 14;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Range";
      // 
      // panel2
      // 
      this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel2.AutoScroll = true;
      this.panel2.Controls.Add(this.tableOptions);
      this.panel2.Location = new System.Drawing.Point(7, 20);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(475, 78);
      this.panel2.TabIndex = 0;
      // 
      // comboBoxValues
      // 
      this.comboBoxValues.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxValues.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxValues.FormattingEnabled = true;
      this.comboBoxValues.Location = new System.Drawing.Point(92, 81);
      this.comboBoxValues.Name = "comboBoxValues";
      this.comboBoxValues.Size = new System.Drawing.Size(402, 21);
      this.comboBoxValues.TabIndex = 15;
      // 
      // JsonItemValidValuesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.comboBoxValues);
      this.Controls.Add(this.groupBox1);
      this.ForeColor = System.Drawing.Color.Black;
      this.Name = "JsonItemValidValuesControl";
      this.Size = new System.Drawing.Size(500, 217);
      this.Load += new System.EventHandler(this.JsonItemValidValuesControl_Load);
      this.Controls.SetChildIndex(this.groupBox1, 0);
      this.Controls.SetChildIndex(this.comboBoxValues, 0);
      this.tableOptions.ResumeLayout(false);
      this.tableOptions.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.TextBox textBoxAdd;
    private System.Windows.Forms.Button buttonAdd;
    private System.Windows.Forms.TableLayoutPanel tableOptions;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.ComboBox comboBoxValues;
  }
}
