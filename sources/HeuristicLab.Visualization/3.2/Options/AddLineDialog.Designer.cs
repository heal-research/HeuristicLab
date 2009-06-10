namespace HeuristicLab.Visualization.Options {
  partial class AddLineDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.btnAddLineDialogCancel = new System.Windows.Forms.Button();
      this.btnAddLineDialogOk = new System.Windows.Forms.Button();
      this.lblAddLineDialogLabel = new System.Windows.Forms.Label();
      this.lblAddLineDialogGeneralType = new System.Windows.Forms.Label();
      this.lblAddLineDialogSubtype = new System.Windows.Forms.Label();
      this.lblAddLineDialogYAxis = new System.Windows.Forms.Label();
      this.clbAddLineDialogAggregatorLines = new System.Windows.Forms.CheckedListBox();
      this.txtAddLineDialogLabel = new System.Windows.Forms.TextBox();
      this.cbAddLineDialogGeneralType = new System.Windows.Forms.ComboBox();
      this.cbAddLineDialogSubtype = new System.Windows.Forms.ComboBox();
      this.cbAddLineDialogYAxis = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // btnAddLineDialogCancel
      // 
      this.btnAddLineDialogCancel.Location = new System.Drawing.Point(113, 231);
      this.btnAddLineDialogCancel.Name = "btnAddLineDialogCancel";
      this.btnAddLineDialogCancel.Size = new System.Drawing.Size(75, 23);
      this.btnAddLineDialogCancel.TabIndex = 0;
      this.btnAddLineDialogCancel.Text = "Cancel";
      this.btnAddLineDialogCancel.UseVisualStyleBackColor = true;
      this.btnAddLineDialogCancel.Click += new System.EventHandler(this.btnAddLineDialogCancel_Click);
      // 
      // btnAddLineDialogOk
      // 
      this.btnAddLineDialogOk.Location = new System.Drawing.Point(195, 231);
      this.btnAddLineDialogOk.Name = "btnAddLineDialogOk";
      this.btnAddLineDialogOk.Size = new System.Drawing.Size(75, 23);
      this.btnAddLineDialogOk.TabIndex = 1;
      this.btnAddLineDialogOk.Text = "OK";
      this.btnAddLineDialogOk.UseVisualStyleBackColor = true;
      this.btnAddLineDialogOk.Click += new System.EventHandler(this.btnAddLineDialogOk_Click);
      // 
      // lblAddLineDialogLabel
      // 
      this.lblAddLineDialogLabel.AutoSize = true;
      this.lblAddLineDialogLabel.Location = new System.Drawing.Point(13, 13);
      this.lblAddLineDialogLabel.Name = "lblAddLineDialogLabel";
      this.lblAddLineDialogLabel.Size = new System.Drawing.Size(33, 13);
      this.lblAddLineDialogLabel.TabIndex = 2;
      this.lblAddLineDialogLabel.Text = "Label";
      // 
      // lblAddLineDialogGeneralType
      // 
      this.lblAddLineDialogGeneralType.AutoSize = true;
      this.lblAddLineDialogGeneralType.Location = new System.Drawing.Point(13, 43);
      this.lblAddLineDialogGeneralType.Name = "lblAddLineDialogGeneralType";
      this.lblAddLineDialogGeneralType.Size = new System.Drawing.Size(71, 13);
      this.lblAddLineDialogGeneralType.TabIndex = 3;
      this.lblAddLineDialogGeneralType.Text = "General Type";
      // 
      // lblAddLineDialogSubtype
      // 
      this.lblAddLineDialogSubtype.AutoSize = true;
      this.lblAddLineDialogSubtype.Location = new System.Drawing.Point(13, 74);
      this.lblAddLineDialogSubtype.Name = "lblAddLineDialogSubtype";
      this.lblAddLineDialogSubtype.Size = new System.Drawing.Size(46, 13);
      this.lblAddLineDialogSubtype.TabIndex = 4;
      this.lblAddLineDialogSubtype.Text = "Subtype";
      // 
      // lblAddLineDialogYAxis
      // 
      this.lblAddLineDialogYAxis.AutoSize = true;
      this.lblAddLineDialogYAxis.Location = new System.Drawing.Point(13, 106);
      this.lblAddLineDialogYAxis.Name = "lblAddLineDialogYAxis";
      this.lblAddLineDialogYAxis.Size = new System.Drawing.Size(33, 13);
      this.lblAddLineDialogYAxis.TabIndex = 5;
      this.lblAddLineDialogYAxis.Text = "YAxis";
      // 
      // clbAddLineDialogAggregatorLines
      // 
      this.clbAddLineDialogAggregatorLines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.clbAddLineDialogAggregatorLines.FormattingEnabled = true;
      this.clbAddLineDialogAggregatorLines.Location = new System.Drawing.Point(113, 137);
      this.clbAddLineDialogAggregatorLines.Name = "clbAddLineDialogAggregatorLines";
      this.clbAddLineDialogAggregatorLines.Size = new System.Drawing.Size(157, 79);
      this.clbAddLineDialogAggregatorLines.TabIndex = 6;
      // 
      // txtAddLineDialogLabel
      // 
      this.txtAddLineDialogLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtAddLineDialogLabel.Location = new System.Drawing.Point(113, 5);
      this.txtAddLineDialogLabel.Name = "txtAddLineDialogLabel";
      this.txtAddLineDialogLabel.Size = new System.Drawing.Size(157, 20);
      this.txtAddLineDialogLabel.TabIndex = 7;
      this.txtAddLineDialogLabel.Text = "New Line";
      // 
      // cbAddLineDialogGeneralType
      // 
      this.cbAddLineDialogGeneralType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbAddLineDialogGeneralType.FormattingEnabled = true;
      this.cbAddLineDialogGeneralType.Items.AddRange(new object[] {
            "DataLine",
            "Aggregator"});
      this.cbAddLineDialogGeneralType.Location = new System.Drawing.Point(113, 35);
      this.cbAddLineDialogGeneralType.Name = "cbAddLineDialogGeneralType";
      this.cbAddLineDialogGeneralType.Size = new System.Drawing.Size(157, 21);
      this.cbAddLineDialogGeneralType.TabIndex = 8;
      this.cbAddLineDialogGeneralType.SelectedIndexChanged += new System.EventHandler(this.cbAddLineDialogGeneralType_SelectedIndexChanged);
      // 
      // cbAddLineDialogSubtype
      // 
      this.cbAddLineDialogSubtype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbAddLineDialogSubtype.FormattingEnabled = true;
      this.cbAddLineDialogSubtype.Location = new System.Drawing.Point(113, 66);
      this.cbAddLineDialogSubtype.Name = "cbAddLineDialogSubtype";
      this.cbAddLineDialogSubtype.Size = new System.Drawing.Size(157, 21);
      this.cbAddLineDialogSubtype.TabIndex = 9;
      // 
      // cbAddLineDialogYAxis
      // 
      this.cbAddLineDialogYAxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cbAddLineDialogYAxis.FormattingEnabled = true;
      this.cbAddLineDialogYAxis.Location = new System.Drawing.Point(113, 97);
      this.cbAddLineDialogYAxis.Name = "cbAddLineDialogYAxis";
      this.cbAddLineDialogYAxis.Size = new System.Drawing.Size(157, 21);
      this.cbAddLineDialogYAxis.TabIndex = 10;
      // 
      // AddLineDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(292, 266);
      this.Controls.Add(this.cbAddLineDialogYAxis);
      this.Controls.Add(this.cbAddLineDialogSubtype);
      this.Controls.Add(this.cbAddLineDialogGeneralType);
      this.Controls.Add(this.txtAddLineDialogLabel);
      this.Controls.Add(this.clbAddLineDialogAggregatorLines);
      this.Controls.Add(this.lblAddLineDialogYAxis);
      this.Controls.Add(this.lblAddLineDialogSubtype);
      this.Controls.Add(this.lblAddLineDialogGeneralType);
      this.Controls.Add(this.lblAddLineDialogLabel);
      this.Controls.Add(this.btnAddLineDialogOk);
      this.Controls.Add(this.btnAddLineDialogCancel);
      this.Name = "AddLineDialog";
      this.Text = "AddLineDialog";
      this.Load += new System.EventHandler(this.AddLineDialog_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnAddLineDialogCancel;
    private System.Windows.Forms.Button btnAddLineDialogOk;
    private System.Windows.Forms.Label lblAddLineDialogLabel;
    private System.Windows.Forms.Label lblAddLineDialogGeneralType;
    private System.Windows.Forms.Label lblAddLineDialogSubtype;
    private System.Windows.Forms.Label lblAddLineDialogYAxis;
    private System.Windows.Forms.CheckedListBox clbAddLineDialogAggregatorLines;
    private System.Windows.Forms.TextBox txtAddLineDialogLabel;
    private System.Windows.Forms.ComboBox cbAddLineDialogGeneralType;
    private System.Windows.Forms.ComboBox cbAddLineDialogSubtype;
    private System.Windows.Forms.ComboBox cbAddLineDialogYAxis;
  }
}