namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  partial class ClassificationImportTypeDialog {
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
      this.TargetVariableComboBox = new System.Windows.Forms.ComboBox();
      this.TargetVariableLabel = new System.Windows.Forms.Label();
      this.TargetVariableInfoLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).BeginInit();
      this.CSVSettingsGroupBox.SuspendLayout();
      this.ProblemDataSettingsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // ProblemDataSettingsGroupBox
      // 
      this.ProblemDataSettingsGroupBox.Controls.Add(this.TargetVariableInfoLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.TargetVariableLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.TargetVariableComboBox);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.PreviewLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.ShuffelInfoLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TargetVariableComboBox, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.ShuffleDataCheckbox, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TargetVariableLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TargetVariableInfoLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TrainingTestTrackBar, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TrainingLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.PreviewDatasetMatrix, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.TestLabel, 0);
      this.ProblemDataSettingsGroupBox.Controls.SetChildIndex(this.ErrorTextBox, 0);
      // 
      // TargetVariableComboBox
      // 
      this.TargetVariableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.TargetVariableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.TargetVariableComboBox.FormattingEnabled = true;
      this.TargetVariableComboBox.Location = new System.Drawing.Point(230, 19);
      this.TargetVariableComboBox.Name = "TargetVariableComboBox";
      this.TargetVariableComboBox.Size = new System.Drawing.Size(181, 21);
      this.TargetVariableComboBox.TabIndex = 10;
      // 
      // TargetVariableLabel
      // 
      this.TargetVariableLabel.AutoSize = true;
      this.TargetVariableLabel.Location = new System.Drawing.Point(142, 22);
      this.TargetVariableLabel.Name = "TargetVariableLabel";
      this.TargetVariableLabel.Size = new System.Drawing.Size(82, 13);
      this.TargetVariableLabel.TabIndex = 20;
      this.TargetVariableLabel.Text = "Target Variable:";
      // 
      // TargetVariableinfoLabel
      // 
      this.TargetVariableInfoLabel.Location = new System.Drawing.Point(421, 21);
      this.TargetVariableInfoLabel.Name = "Target Variable Info";
      this.TargetVariableInfoLabel.Size = new System.Drawing.Size(16, 16);
      this.TargetVariableInfoLabel.TabIndex = 21;
      this.TargetVariableInfoLabel.Tag = "Select the target variable of the csv file.";
      this.TargetVariableInfoLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Information;
      this.TargetVariableInfoLabel.DoubleClick += new System.EventHandler(this.ControlToolTip_DoubleClick);
      // 
      // ClassificationImportTypeDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(471, 442);
      this.Name = "ClassificationImportTypeDialog";
      this.Text = "Classification CSV Import";
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).EndInit();
      this.CSVSettingsGroupBox.ResumeLayout(false);
      this.CSVSettingsGroupBox.PerformLayout();
      this.ProblemDataSettingsGroupBox.ResumeLayout(false);
      this.ProblemDataSettingsGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox TargetVariableComboBox;
    private System.Windows.Forms.Label TargetVariableLabel;
    protected System.Windows.Forms.Label TargetVariableInfoLabel;


  }
}