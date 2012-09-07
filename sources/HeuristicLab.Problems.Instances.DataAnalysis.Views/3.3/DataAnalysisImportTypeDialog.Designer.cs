namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  partial class DataAnalysisImportTypeDialog {
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
      this.ShuffelDataCheckbox = new System.Windows.Forms.CheckBox();
      this.OkButton = new System.Windows.Forms.Button();
      this.CancelButton = new System.Windows.Forms.Button();
      this.TrainingTestTrackBar = new System.Windows.Forms.TrackBar();
      this.TrainingTestGroupBox = new System.Windows.Forms.GroupBox();
      this.TestLabel = new System.Windows.Forms.Label();
      this.TrainingLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).BeginInit();
      this.TrainingTestGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // ShuffelDataCheckbox
      // 
      this.ShuffelDataCheckbox.AutoSize = true;
      this.ShuffelDataCheckbox.Location = new System.Drawing.Point(18, 12);
      this.ShuffelDataCheckbox.Name = "ShuffelDataCheckbox";
      this.ShuffelDataCheckbox.Size = new System.Drawing.Size(91, 17);
      this.ShuffelDataCheckbox.TabIndex = 1;
      this.ShuffelDataCheckbox.Text = "Shuffel Data?";
      this.ShuffelDataCheckbox.UseVisualStyleBackColor = true;
      // 
      // OkButton
      // 
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OkButton.Location = new System.Drawing.Point(18, 121);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(75, 23);
      this.OkButton.TabIndex = 2;
      this.OkButton.Text = "&Ok";
      this.OkButton.UseVisualStyleBackColor = true;
      // 
      // CancelButton
      // 
      this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CancelButton.Location = new System.Drawing.Point(142, 121);
      this.CancelButton.Name = "CancelButton";
      this.CancelButton.Size = new System.Drawing.Size(75, 23);
      this.CancelButton.TabIndex = 3;
      this.CancelButton.Text = "&Cancel";
      this.CancelButton.UseVisualStyleBackColor = true;
      // 
      // TrainingTestTrackBar
      // 
      this.TrainingTestTrackBar.Location = new System.Drawing.Point(6, 19);
      this.TrainingTestTrackBar.Maximum = 100;
      this.TrainingTestTrackBar.Minimum = 1;
      this.TrainingTestTrackBar.Name = "TrainingTestTrackBar";
      this.TrainingTestTrackBar.Size = new System.Drawing.Size(210, 45);
      this.TrainingTestTrackBar.TabIndex = 4;
      this.TrainingTestTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.TrainingTestTrackBar.Value = 66;
      this.TrainingTestTrackBar.ValueChanged += new System.EventHandler(this.TrainingTestTrackBar_ValueChanged);
      // 
      // TrainingTestGroupBox
      // 
      this.TrainingTestGroupBox.Controls.Add(this.TestLabel);
      this.TrainingTestGroupBox.Controls.Add(this.TrainingLabel);
      this.TrainingTestGroupBox.Controls.Add(this.TrainingTestTrackBar);
      this.TrainingTestGroupBox.Location = new System.Drawing.Point(4, 35);
      this.TrainingTestGroupBox.Name = "TrainingTestGroupBox";
      this.TrainingTestGroupBox.Size = new System.Drawing.Size(222, 80);
      this.TrainingTestGroupBox.TabIndex = 5;
      this.TrainingTestGroupBox.TabStop = false;
      this.TrainingTestGroupBox.Text = "Training/Test";
      // 
      // TestLabel
      // 
      this.TestLabel.AutoSize = true;
      this.TestLabel.Location = new System.Drawing.Point(108, 51);
      this.TestLabel.Name = "TestLabel";
      this.TestLabel.Size = new System.Drawing.Size(57, 13);
      this.TestLabel.TabIndex = 6;
      this.TestLabel.Text = "Test: 34 %";
      // 
      // TrainingLabel
      // 
      this.TrainingLabel.AutoSize = true;
      this.TrainingLabel.Location = new System.Drawing.Point(11, 51);
      this.TrainingLabel.Name = "TrainingLabel";
      this.TrainingLabel.Size = new System.Drawing.Size(74, 13);
      this.TrainingLabel.TabIndex = 5;
      this.TrainingLabel.Text = "Training: 66 %";
      // 
      // DataAnalysisImportTypeDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(229, 151);
      this.Controls.Add(this.CancelButton);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.ShuffelDataCheckbox);
      this.Controls.Add(this.TrainingTestGroupBox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "DataAnalysisImportTypeDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "DataAnalysisImportTypeDialog";
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).EndInit();
      this.TrainingTestGroupBox.ResumeLayout(false);
      this.TrainingTestGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.CheckBox ShuffelDataCheckbox;
    protected System.Windows.Forms.Button OkButton;
    protected System.Windows.Forms.Button CancelButton;
    private System.Windows.Forms.TrackBar TrainingTestTrackBar;
    private System.Windows.Forms.GroupBox TrainingTestGroupBox;
    private System.Windows.Forms.Label TestLabel;
    private System.Windows.Forms.Label TrainingLabel;

  }
}