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
      this.textBoxFrom = new System.Windows.Forms.TextBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.checkBoxTo = new System.Windows.Forms.CheckBox();
      this.textBoxTo = new System.Windows.Forms.TextBox();
      this.checkBoxFrom = new System.Windows.Forms.CheckBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // textBoxFrom
      // 
      this.textBoxFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.textBoxFrom, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.textBoxFrom.Location = new System.Drawing.Point(87, 16);
      this.textBoxFrom.Name = "textBoxFrom";
      this.textBoxFrom.ReadOnly = true;
      this.textBoxFrom.Size = new System.Drawing.Size(407, 20);
      this.textBoxFrom.TabIndex = 2;
      this.textBoxFrom.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxFrom_Validating);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.checkBoxTo);
      this.groupBox2.Controls.Add(this.textBoxTo);
      this.groupBox2.Controls.Add(this.checkBoxFrom);
      this.groupBox2.Controls.Add(this.textBoxFrom);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(0, 0);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(500, 75);
      this.groupBox2.TabIndex = 19;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Range";
      // 
      // checkBoxTo
      // 
      this.checkBoxTo.AutoSize = true;
      this.checkBoxTo.Location = new System.Drawing.Point(9, 45);
      this.checkBoxTo.Name = "checkBoxTo";
      this.checkBoxTo.Size = new System.Drawing.Size(42, 17);
      this.checkBoxTo.TabIndex = 7;
      this.checkBoxTo.Text = "To:";
      this.checkBoxTo.UseVisualStyleBackColor = true;
      // 
      // textBoxTo
      // 
      this.textBoxTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.textBoxTo, System.Windows.Forms.ErrorIconAlignment.TopLeft);
      this.textBoxTo.Location = new System.Drawing.Point(87, 42);
      this.textBoxTo.Name = "textBoxTo";
      this.textBoxTo.ReadOnly = true;
      this.textBoxTo.Size = new System.Drawing.Size(407, 20);
      this.textBoxTo.TabIndex = 6;
      this.textBoxTo.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxTo_Validating);
      // 
      // checkBoxFrom
      // 
      this.checkBoxFrom.AutoSize = true;
      this.checkBoxFrom.Location = new System.Drawing.Point(9, 19);
      this.checkBoxFrom.Name = "checkBoxFrom";
      this.checkBoxFrom.Size = new System.Drawing.Size(52, 17);
      this.checkBoxFrom.TabIndex = 4;
      this.checkBoxFrom.Text = "From:";
      this.checkBoxFrom.UseVisualStyleBackColor = true;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // NumericRangeControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox2);
      this.Name = "NumericRangeControl";
      this.Size = new System.Drawing.Size(500, 75);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.TextBox textBoxFrom;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.CheckBox checkBoxTo;
    private System.Windows.Forms.TextBox textBoxTo;
    private System.Windows.Forms.CheckBox checkBoxFrom;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}
