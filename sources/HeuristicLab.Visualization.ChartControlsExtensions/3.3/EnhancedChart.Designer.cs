namespace HeuristicLab.Visualization.ChartControlsExtensions {
  partial class EnhancedChart {
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
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copyImageToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.contextMenuStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
      this.SuspendLayout();
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveImageToolStripMenuItem,
            this.copyImageToClipboardToolStripMenuItem});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(208, 48);
      // 
      // saveImageToolStripMenuItem
      // 
      this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
      this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
      this.saveImageToolStripMenuItem.Text = "Save Image";
      this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
      // 
      // copyImageToClipboardToolStripMenuItem
      // 
      this.copyImageToClipboardToolStripMenuItem.Name = "copyImageToClipboardToolStripMenuItem";
      this.copyImageToClipboardToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
      this.copyImageToClipboardToolStripMenuItem.Text = "Copy Image to Clipboard";
      this.copyImageToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyImageToClipboardToolStripMenuItem_Click);
      this.contextMenuStrip.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem copyImageToClipboardToolStripMenuItem;
  }
}
