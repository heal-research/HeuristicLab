namespace HeuristicLab.CEDMA.Server {
  partial class ExecuterView {
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
      this.jobsList = new System.Windows.Forms.ListBox();
      this.maxJobsLabel = new System.Windows.Forms.Label();
      this.maxActiveJobs = new System.Windows.Forms.NumericUpDown();
      this.finishedLabel = new System.Windows.Forms.Label();
      this.finishedTextBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.maxActiveJobs)).BeginInit();
      this.SuspendLayout();
      // 
      // jobsList
      // 
      this.jobsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.jobsList.FormattingEnabled = true;
      this.jobsList.Location = new System.Drawing.Point(3, 55);
      this.jobsList.Name = "jobsList";
      this.jobsList.Size = new System.Drawing.Size(327, 238);
      this.jobsList.TabIndex = 0;
      // 
      // maxJobsLabel
      // 
      this.maxJobsLabel.AutoSize = true;
      this.maxJobsLabel.Location = new System.Drawing.Point(5, 5);
      this.maxJobsLabel.Name = "maxJobsLabel";
      this.maxJobsLabel.Size = new System.Drawing.Size(87, 13);
      this.maxJobsLabel.TabIndex = 1;
      this.maxJobsLabel.Text = "Max. active jobs:";
      // 
      // maxActiveJobs
      // 
      this.maxActiveJobs.Location = new System.Drawing.Point(117, 3);
      this.maxActiveJobs.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.maxActiveJobs.Name = "maxActiveJobs";
      this.maxActiveJobs.Size = new System.Drawing.Size(120, 20);
      this.maxActiveJobs.TabIndex = 2;
      this.maxActiveJobs.ValueChanged += new System.EventHandler(this.maxActiveJobs_ValueChanged);
      // 
      // finishedLabel
      // 
      this.finishedLabel.AutoSize = true;
      this.finishedLabel.Location = new System.Drawing.Point(5, 32);
      this.finishedLabel.Name = "finishedLabel";
      this.finishedLabel.Size = new System.Drawing.Size(106, 13);
      this.finishedLabel.TabIndex = 3;
      this.finishedLabel.Text = "Stored models (new):";
      // 
      // finishedTextBox
      // 
      this.finishedTextBox.Location = new System.Drawing.Point(117, 29);
      this.finishedTextBox.Name = "finishedTextBox";
      this.finishedTextBox.ReadOnly = true;
      this.finishedTextBox.Size = new System.Drawing.Size(100, 20);
      this.finishedTextBox.TabIndex = 4;
      // 
      // ExecuterView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.finishedTextBox);
      this.Controls.Add(this.finishedLabel);
      this.Controls.Add(this.maxActiveJobs);
      this.Controls.Add(this.maxJobsLabel);
      this.Controls.Add(this.jobsList);
      this.Name = "ExecuterView";
      this.Size = new System.Drawing.Size(333, 294);
      ((System.ComponentModel.ISupportInitialize)(this.maxActiveJobs)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox jobsList;
    private System.Windows.Forms.Label maxJobsLabel;
    private System.Windows.Forms.NumericUpDown maxActiveJobs;
    private System.Windows.Forms.Label finishedLabel;
    private System.Windows.Forms.TextBox finishedTextBox;
  }
}
