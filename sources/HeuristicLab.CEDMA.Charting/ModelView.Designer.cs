namespace HeuristicLab.CEDMA.Charting {
  partial class ModelView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.lowerSplitContainer = new System.Windows.Forms.SplitContainer();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.lowerSplitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.lowerSplitContainer);
      this.splitContainer.Size = new System.Drawing.Size(450, 459);
      this.splitContainer.SplitterDistance = 217;
      this.splitContainer.TabIndex = 0;
      // 
      // lowerSplitContainer
      // 
      this.lowerSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lowerSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.lowerSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.lowerSplitContainer.Name = "lowerSplitContainer";
      this.lowerSplitContainer.Size = new System.Drawing.Size(450, 238);
      this.lowerSplitContainer.SplitterDistance = 232;
      this.lowerSplitContainer.TabIndex = 0;
      // 
      // ModelView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "ModelView";
      this.Size = new System.Drawing.Size(450, 459);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.lowerSplitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.SplitContainer lowerSplitContainer;

  }
}
