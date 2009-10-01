namespace HeuristicLab.MainForm {
  partial class MainFormBase {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFormBase));
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.toolStrip = new System.Windows.Forms.ToolStrip();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
      this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip
      // 
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(624, 24);
      this.menuStrip.TabIndex = 0;
      this.menuStrip.Text = "menuStrip";
      // 
      // toolStrip
      // 
      this.toolStrip.Location = new System.Drawing.Point(0, 24);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(624, 25);
      this.toolStrip.TabIndex = 1;
      // 
      // statusStrip
      // 
      this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel});
      this.statusStrip.Location = new System.Drawing.Point(0, 390);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(624, 22);
      this.statusStrip.TabIndex = 2;
      this.statusStrip.Text = "statusStrip";
      // 
      // toolStripProgressBar
      // 
      this.toolStripProgressBar.Name = "toolStripProgressBar";
      this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
      this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.toolStripProgressBar.Visible = false;
      // 
      // toolStripStatusLabel
      // 
      this.toolStripStatusLabel.Name = "toolStripStatusLabel";
      this.toolStripStatusLabel.Size = new System.Drawing.Size(112, 17);
      this.toolStripStatusLabel.Text = "toolStripStatusLabel";
      // 
      // MainFormBase
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(624, 412);
      this.Controls.Add(this.statusStrip);
      this.Controls.Add(this.toolStrip);
      this.Controls.Add(this.menuStrip);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip;
      this.Name = "MainFormBase";
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip toolStrip;
    private System.Windows.Forms.StatusStrip statusStrip;
    protected System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
  }
}