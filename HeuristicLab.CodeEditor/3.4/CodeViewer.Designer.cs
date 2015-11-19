namespace HeuristicLab.CodeEditor {
  partial class CodeViewer {
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
      this.okButton = new System.Windows.Forms.Button();
      this.elementHost = new System.Windows.Forms.Integration.ElementHost();
      this.avalonEditWrapper = new HeuristicLab.CodeEditor.AvalonEditWrapper();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.Location = new System.Drawing.Point(647, 426);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // elementHost
      // 
      this.elementHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.elementHost.Location = new System.Drawing.Point(12, 12);
      this.elementHost.Name = "elementHost";
      this.elementHost.Size = new System.Drawing.Size(710, 408);
      this.elementHost.TabIndex = 1;
      this.elementHost.Child = this.avalonEditWrapper;
      // 
      // CodeViewer
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(734, 461);
      this.Controls.Add(this.elementHost);
      this.Controls.Add(this.okButton);
      this.Icon = HeuristicLab.Common.Resources.HeuristicLab.Icon;
      this.Name = "CodeViewer";
      this.Text = "CodeViewer";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Integration.ElementHost elementHost;
    private AvalonEditWrapper avalonEditWrapper;
  }
}