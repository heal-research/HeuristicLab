#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  partial class RegressionImportDialog {
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
      // OkButton
      // 
      this.OkButton.Location = new System.Drawing.Point(303, 421);
      // 
      // CancellationButton
      // 
      this.CancellationButton.Location = new System.Drawing.Point(384, 421);
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
      this.TargetVariableInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.TargetVariableInfoLabel.Location = new System.Drawing.Point(421, 22);
      this.TargetVariableInfoLabel.Name = "Target Variable Info";
      this.TargetVariableInfoLabel.Size = new System.Drawing.Size(16, 16);
      this.TargetVariableInfoLabel.TabIndex = 21;
      this.TargetVariableInfoLabel.Tag = "Select the target variable of the csv file.";
      this.TargetVariableInfoLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Information;
      this.TargetVariableInfoLabel.DoubleClick += new System.EventHandler(this.ControlToolTip_DoubleClick);
      this.ToolTip.SetToolTip(this.TargetVariableInfoLabel, (string)this.TargetVariableInfoLabel.Tag);
      // 
      // RegressionImportDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(471, 456);
      this.Name = "RegressionImportDialog";
      this.Text = "Regression CSV Import";
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).EndInit();
      this.CSVSettingsGroupBox.ResumeLayout(false);
      this.CSVSettingsGroupBox.PerformLayout();
      this.ProblemDataSettingsGroupBox.ResumeLayout(false);
      this.ProblemDataSettingsGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.ComboBox TargetVariableComboBox;
    protected System.Windows.Forms.Label TargetVariableLabel;
    protected System.Windows.Forms.Label TargetVariableInfoLabel;


  }
}