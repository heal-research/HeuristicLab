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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.textBoxValueTo = new System.Windows.Forms.TextBox();
      this.textBoxValueFrom = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.numericRangeControl = new HeuristicLab.JsonInterface.OptimizerIntegration.NumericRangeControl();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.textBoxValueTo);
      this.groupBox1.Controls.Add(this.textBoxValueFrom);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Location = new System.Drawing.Point(7, 101);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(487, 65);
      this.groupBox1.TabIndex = 17;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Value";
      // 
      // textBoxValueTo
      // 
      this.textBoxValueTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxValueTo.Location = new System.Drawing.Point(83, 39);
      this.textBoxValueTo.Name = "textBoxValueTo";
      this.textBoxValueTo.Size = new System.Drawing.Size(398, 20);
      this.textBoxValueTo.TabIndex = 3;
      // 
      // textBoxValueFrom
      // 
      this.textBoxValueFrom.Location = new System.Drawing.Point(83, 13);
      this.textBoxValueFrom.Name = "textBoxValueFrom";
      this.textBoxValueFrom.Size = new System.Drawing.Size(398, 20);
      this.textBoxValueFrom.TabIndex = 2;
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 42);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(23, 13);
      this.label4.TabIndex = 1;
      this.label4.Text = "To:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(33, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "From:";
      // 
      // numericRangeControl
      // 
      this.numericRangeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numericRangeControl.Location = new System.Drawing.Point(7, 172);
      this.numericRangeControl.Name = "numericRangeControl";
      this.numericRangeControl.Size = new System.Drawing.Size(487, 74);
      this.numericRangeControl.TabIndex = 18;
      // 
      // JsonItemRangeControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.numericRangeControl);
      this.Controls.Add(this.groupBox1);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Name = "JsonItemRangeControl";
      this.Size = new System.Drawing.Size(500, 252);
      this.Controls.SetChildIndex(this.groupBox1, 0);
      this.Controls.SetChildIndex(this.numericRangeControl, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox textBoxValueTo;
    private System.Windows.Forms.TextBox textBoxValueFrom;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label2;
    private NumericRangeControl numericRangeControl;
  }
}
