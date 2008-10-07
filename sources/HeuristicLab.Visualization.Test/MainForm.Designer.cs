namespace HeuristicLab.Visualization.Test {
  partial class MainForm {
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
      this.canvasUI = new HeuristicLab.Visualization.CanvasUI();
      this.label2 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // canvasUI
      // 
      this.canvasUI.Location = new System.Drawing.Point(12, 29);
      this.canvasUI.Name = "canvasUI";
      this.canvasUI.Size = new System.Drawing.Size(800, 600);
      this.canvasUI.TabIndex = 3;
      this.canvasUI.Text = "canvasUI";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 13);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(43, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Canvas";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(822, 637);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.canvasUI);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Visualization.CanvasUI canvasUI;
    private System.Windows.Forms.Label label2;
  }
}

