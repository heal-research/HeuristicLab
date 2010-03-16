using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.DeploymentService.AdminClient {
  class MainForm : DockingMainForm {
    private System.Windows.Forms.ToolStripProgressBar progressBar;

    public MainForm(Type type)
      : base(type) {
      InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      (new PluginEditor()).Show();
    }

    private void InitializeComponent() {
      this.SuspendLayout();

      progressBar = new System.Windows.Forms.ToolStripProgressBar();
      progressBar.MarqueeAnimationSpeed = 30;
      progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      progressBar.Visible = false;
      statusStrip.Items.Add(progressBar);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.ClientSize = new System.Drawing.Size(770, 550);
      this.Name = "MainForm";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public void ShowProgressBar() {
      progressBar.Visible = true;
    }

    public void HideProgressBar() {
      progressBar.Visible = false;
    }
  }
}
