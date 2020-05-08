namespace HeuristicLab.Problems.TravelingSalesman.Views {
  partial class ITSPDataView {
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
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.SuspendLayout();
      // 
      // viewHost
      // 
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(0, 0);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(582, 404);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = false;
      this.viewHost.ViewType = null;
      // 
      // ITSPDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.viewHost);
      this.Name = "ITSPDataView";
      this.Size = new System.Drawing.Size(582, 404);
      this.ResumeLayout(false);

    }

    #endregion

    private MainForm.WindowsForms.ViewHost viewHost;
  }
}
