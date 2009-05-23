namespace HeuristicLab.Visualization.Options {
  partial class ColorSelection {
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
      this.OptionsDialogSelectColorBtn = new System.Windows.Forms.Button();
      this.ColorPreviewTB = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // OptionsDialogSelectColorBtn
      // 
      this.OptionsDialogSelectColorBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.OptionsDialogSelectColorBtn.Location = new System.Drawing.Point(74, 2);
      this.OptionsDialogSelectColorBtn.Name = "OptionsDialogSelectColorBtn";
      this.OptionsDialogSelectColorBtn.Size = new System.Drawing.Size(50, 23);
      this.OptionsDialogSelectColorBtn.TabIndex = 9;
      this.OptionsDialogSelectColorBtn.Text = "Select";
      this.OptionsDialogSelectColorBtn.UseVisualStyleBackColor = true;
      this.OptionsDialogSelectColorBtn.Click += new System.EventHandler(this.OptionsDialogSelectColorBtn_Click);
      // 
      // ColorPreviewTB
      // 
      this.ColorPreviewTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ColorPreviewTB.Location = new System.Drawing.Point(3, 3);
      this.ColorPreviewTB.Name = "ColorPreviewTB";
      this.ColorPreviewTB.ReadOnly = true;
      this.ColorPreviewTB.Size = new System.Drawing.Size(64, 20);
      this.ColorPreviewTB.TabIndex = 8;
      // 
      // ColorSelection
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.OptionsDialogSelectColorBtn);
      this.Controls.Add(this.ColorPreviewTB);
      this.Name = "ColorSelection";
      this.Size = new System.Drawing.Size(126, 25);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button OptionsDialogSelectColorBtn;
    private System.Windows.Forms.TextBox ColorPreviewTB;
  }
}
