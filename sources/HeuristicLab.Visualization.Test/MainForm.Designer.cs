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
      this.legendButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // canvasUI
      // 
      this.canvasUI.Location = new System.Drawing.Point(12, 29);
      this.canvasUI.MouseEventListener = null;
      this.canvasUI.Name = "canvasUI";
      this.canvasUI.Size = new System.Drawing.Size(800, 571);
      this.canvasUI.TabIndex = 3;
      this.canvasUI.Text = "canvasUI";
      this.canvasUI.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvasUI_MouseDown);
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
      // legendButton
      // 
      this.legendButton.Location = new System.Drawing.Point(12, 606);
      this.legendButton.Name = "legendButton";
      this.legendButton.Size = new System.Drawing.Size(75, 23);
      this.legendButton.TabIndex = 5;
      this.legendButton.Text = "Legend";
      this.legendButton.UseVisualStyleBackColor = true;
      this.legendButton.Click += new System.EventHandler(this.legendButton_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(822, 637);
      this.Controls.Add(this.legendButton);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.canvasUI);
      this.Name = "MainForm";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Visualization.CanvasUI canvasUI;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button legendButton;
  }
}

