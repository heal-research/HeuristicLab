namespace HeuristicLab.JsonInterface.OptimizerIntegration.Views {
  partial class ValueLookupJsonItemControl {
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
      this.content = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // content
      // 
      this.content.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.content.Location = new System.Drawing.Point(0, 20);
      this.content.Margin = new System.Windows.Forms.Padding(0);
      this.content.Name = "content";
      this.content.Size = new System.Drawing.Size(651, 350);
      this.content.TabIndex = 14;
      // 
      // ValueLookupJsonItemControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.content);
      this.Name = "ValueLookupJsonItemControl";
      this.Size = new System.Drawing.Size(651, 370);
      this.Controls.SetChildIndex(this.content, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel content;
  }
}
