using HeuristicLab.Visualization.Drawing;

namespace HeuristicLab.Visualization.Test {
  partial class LegendForm {
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
      this.canvasUI = new CanvasUI();
      this.SuspendLayout();
      // 
      // canvasUI
      // 
      this.canvasUI.Location = new System.Drawing.Point(12, 12);
      this.canvasUI.Name = "canvasUI";
      this.canvasUI.Size = new System.Drawing.Size(260, 240);
      this.canvasUI.TabIndex = 0;
      this.canvasUI.Text = "canvasUI";
      // 
      // LegendForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 264);
      this.Controls.Add(this.canvasUI);
      this.Name = "LegendForm";
      this.Text = "LegendForm";
      this.ResumeLayout(false);

    }

    #endregion

    private CanvasUI canvasUI;
  }
}