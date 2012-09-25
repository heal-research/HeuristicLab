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
      this.ShuffleDataCheckbox = new System.Windows.Forms.CheckBox();
      this.OkButton = new System.Windows.Forms.Button();
      this.TrainingTestTrackBar = new System.Windows.Forms.TrackBar();
      this.TrainingTestGroupBox = new System.Windows.Forms.GroupBox();
      this.TestLabel = new System.Windows.Forms.Label();
      this.TrainingLabel = new System.Windows.Forms.Label();
      this.CancelButton = new System.Windows.Forms.Button();
      this.openFileButton = new System.Windows.Forms.Button();
      this.ProblemFileLabel = new System.Windows.Forms.Label();
      this.ProblemTextBox = new System.Windows.Forms.TextBox();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).BeginInit();
      this.TrainingTestGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // ShuffleDataCheckbox
      // 
      this.ShuffleDataCheckbox.AutoSize = true;
      this.ShuffleDataCheckbox.Location = new System.Drawing.Point(12, 38);
      this.ShuffleDataCheckbox.Name = "ShuffleDataCheckbox";
      this.ShuffleDataCheckbox.Size = new System.Drawing.Size(91, 17);
      this.ShuffleDataCheckbox.TabIndex = 1;
      this.ShuffleDataCheckbox.Text = "Shuffle Data?";
      this.ShuffleDataCheckbox.UseVisualStyleBackColor = true;
      // 
      // OkButton
      // 
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OkButton.Enabled = false;
      this.OkButton.Location = new System.Drawing.Point(61, 147);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(75, 23);
      this.OkButton.TabIndex = 2;
      this.OkButton.Text = "&Ok";
      this.OkButton.UseVisualStyleBackColor = true;
      // 
      // TrainingTestTrackBar
      // 
      this.TrainingTestTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.TrainingTestTrackBar.Location = new System.Drawing.Point(6, 19);
      this.TrainingTestTrackBar.Maximum = 100;
      this.TrainingTestTrackBar.Minimum = 1;
      this.TrainingTestTrackBar.Name = "TrainingTestTrackBar";
      this.TrainingTestTrackBar.Size = new System.Drawing.Size(303, 45);
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
      this.TrainingTestGroupBox.Location = new System.Drawing.Point(12, 61);
      this.TrainingTestGroupBox.Name = "TrainingTestGroupBox";
      this.TrainingTestGroupBox.Size = new System.Drawing.Size(315, 80);
      this.TrainingTestGroupBox.TabIndex = 5;
      this.TrainingTestGroupBox.TabStop = false;
      this.TrainingTestGroupBox.Text = "Training/Test";
      // 
      // TestLabel
      // 
      this.TestLabel.AutoSize = true;
      this.TestLabel.Location = new System.Drawing.Point(188, 51);
      this.TestLabel.Name = "TestLabel";
      this.TestLabel.Size = new System.Drawing.Size(57, 13);
      this.TestLabel.TabIndex = 6;
      this.TestLabel.Text = "Test: 34 %";
      // 
      // TrainingLabel
      // 
      this.TrainingLabel.AutoSize = true;
      this.TrainingLabel.Location = new System.Drawing.Point(50, 51);
      this.TrainingLabel.Name = "TrainingLabel";
      this.TrainingLabel.Size = new System.Drawing.Size(74, 13);
      this.TrainingLabel.TabIndex = 5;
      this.TrainingLabel.Text = "Training: 66 %";
      // 
      // CancelButton
      // 
      this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CancelButton.Location = new System.Drawing.Point(182, 147);
      this.CancelButton.Name = "CancelButton";
      this.CancelButton.Size = new System.Drawing.Size(75, 23);
      this.CancelButton.TabIndex = 3;
      this.CancelButton.Text = "&Cancel";
      this.CancelButton.UseVisualStyleBackColor = true;
      // 
      // openFileButton
      // 
      this.openFileButton.Location = new System.Drawing.Point(303, 12);
      this.openFileButton.Name = "openFileButton";
      this.openFileButton.Size = new System.Drawing.Size(24, 24);
      this.openFileButton.TabIndex = 8;
      this.openFileButton.UseVisualStyleBackColor = true;
      this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
      // 
      // ProblemFileLabel
      // 
      this.ProblemFileLabel.AutoSize = true;
      this.ProblemFileLabel.Location = new System.Drawing.Point(12, 15);
      this.ProblemFileLabel.Name = "ProblemFileLabel";
      this.ProblemFileLabel.Size = new System.Drawing.Size(67, 13);
      this.ProblemFileLabel.TabIndex = 6;
      this.ProblemFileLabel.Text = "Problem File:";
      // 
      // ProblemTextBox
      // 
      this.ProblemTextBox.Location = new System.Drawing.Point(85, 12);
      this.ProblemTextBox.Name = "ProblemTextBox";
      this.ProblemTextBox.ReadOnly = true;
      this.ProblemTextBox.Size = new System.Drawing.Size(212, 20);
      this.ProblemTextBox.TabIndex = 9;
      // 
      // openFileDialog
      // 
      this.openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
      // 
      // DataAnalysisImportTypeDialog
      // 
      this.AcceptButton = this.OkButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(339, 179);
      this.ControlBox = false;
      this.Controls.Add(this.ProblemTextBox);
      this.Controls.Add(this.openFileButton);
      this.Controls.Add(this.ProblemFileLabel);
      this.Controls.Add(this.CancelButton);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.ShuffleDataCheckbox);
      this.Controls.Add(this.TrainingTestGroupBox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DataAnalysisImportTypeDialog";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "CSV Import";
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).EndInit();
      this.TrainingTestGroupBox.ResumeLayout(false);
      this.TrainingTestGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.CheckBox ShuffleDataCheckbox;
    protected System.Windows.Forms.Button OkButton;
    protected System.Windows.Forms.TrackBar TrainingTestTrackBar;
    protected System.Windows.Forms.GroupBox TrainingTestGroupBox;
    protected System.Windows.Forms.Label TestLabel;
    protected System.Windows.Forms.Label TrainingLabel;
    protected System.Windows.Forms.Button CancelButton;
    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    protected System.Windows.Forms.Label ProblemFileLabel;
    protected System.Windows.Forms.Button openFileButton;
    protected System.Windows.Forms.TextBox ProblemTextBox;

  }
}