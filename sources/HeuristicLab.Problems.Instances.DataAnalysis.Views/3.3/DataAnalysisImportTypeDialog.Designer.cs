#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.components = new System.ComponentModel.Container();
      this.ShuffleDataCheckbox = new System.Windows.Forms.CheckBox();
      this.OkButton = new System.Windows.Forms.Button();
      this.TrainingTestTrackBar = new System.Windows.Forms.TrackBar();
      this.TestLabel = new System.Windows.Forms.Label();
      this.TrainingLabel = new System.Windows.Forms.Label();
      this.CancelationButton = new System.Windows.Forms.Button();
      this.OpenFileButton = new System.Windows.Forms.Button();
      this.ProblemFileLabel = new System.Windows.Forms.Label();
      this.ProblemTextBox = new System.Windows.Forms.TextBox();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.DateTimeFormatComboBox = new System.Windows.Forms.ComboBox();
      this.DecimalSeparatorComboBox = new System.Windows.Forms.ComboBox();
      this.DateTimeFormatLabel = new System.Windows.Forms.Label();
      this.DecimalSeperatorLabel = new System.Windows.Forms.Label();
      this.SeparatorLabel = new System.Windows.Forms.Label();
      this.SeparatorComboBox = new System.Windows.Forms.ComboBox();
      this.CSVSettingsGroupBox = new System.Windows.Forms.GroupBox();
      this.DateTimeFormatInfoLabel = new System.Windows.Forms.Label();
      this.DecimalSeparatorInfoLabel = new System.Windows.Forms.Label();
      this.SeparatorInfoLabel = new System.Windows.Forms.Label();
      this.ProblemDataSettingsGroupBox = new System.Windows.Forms.GroupBox();
      this.ErrorTextBox = new System.Windows.Forms.TextBox();
      this.ShuffelInfoLabel = new System.Windows.Forms.Label();
      this.PreviewLabel = new System.Windows.Forms.Label();
      this.PreviewDatasetMatrix = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
      this.CheckboxColumnNames = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).BeginInit();
      this.CSVSettingsGroupBox.SuspendLayout();
      this.ProblemDataSettingsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // ShuffleDataCheckbox
      // 
      this.ShuffleDataCheckbox.AutoSize = true;
      this.ShuffleDataCheckbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.ShuffleDataCheckbox.Location = new System.Drawing.Point(9, 20);
      this.ShuffleDataCheckbox.Name = "ShuffleDataCheckbox";
      this.ShuffleDataCheckbox.Size = new System.Drawing.Size(91, 17);
      this.ShuffleDataCheckbox.TabIndex = 1;
      this.ShuffleDataCheckbox.Text = "Shuffle Data?";
      this.ShuffleDataCheckbox.UseVisualStyleBackColor = true;
      // 
      // OkButton
      // 
      this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OkButton.Enabled = false;
      this.OkButton.Location = new System.Drawing.Point(303, 422);
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
      this.TrainingTestTrackBar.Location = new System.Drawing.Point(6, 43);
      this.TrainingTestTrackBar.Maximum = 100;
      this.TrainingTestTrackBar.Minimum = 1;
      this.TrainingTestTrackBar.Name = "TrainingTestTrackBar";
      this.TrainingTestTrackBar.Size = new System.Drawing.Size(435, 45);
      this.TrainingTestTrackBar.TabIndex = 4;
      this.TrainingTestTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.TrainingTestTrackBar.Value = 66;
      this.TrainingTestTrackBar.ValueChanged += new System.EventHandler(this.TrainingTestTrackBar_ValueChanged);
      // 
      // TestLabel
      // 
      this.TestLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.TestLabel.AutoSize = true;
      this.TestLabel.Location = new System.Drawing.Point(303, 68);
      this.TestLabel.Name = "TestLabel";
      this.TestLabel.Size = new System.Drawing.Size(57, 13);
      this.TestLabel.TabIndex = 6;
      this.TestLabel.Text = "Test: 34 %";
      // 
      // TrainingLabel
      // 
      this.TrainingLabel.AutoSize = true;
      this.TrainingLabel.Location = new System.Drawing.Point(76, 68);
      this.TrainingLabel.Name = "TrainingLabel";
      this.TrainingLabel.Size = new System.Drawing.Size(74, 13);
      this.TrainingLabel.TabIndex = 5;
      this.TrainingLabel.Text = "Training: 66 %";
      // 
      // CancelationButton
      // 
      this.CancelationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.CancelationButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CancelationButton.Location = new System.Drawing.Point(384, 422);
      this.CancelationButton.Name = "CancelationButton";
      this.CancelationButton.Size = new System.Drawing.Size(75, 23);
      this.CancelationButton.TabIndex = 3;
      this.CancelationButton.Text = "&Cancel";
      this.CancelationButton.UseVisualStyleBackColor = true;
      // 
      // OpenFileButton
      // 
      this.OpenFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.OpenFileButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.OpenFileButton.Location = new System.Drawing.Point(429, 3);
      this.OpenFileButton.Name = "OpenFileButton";
      this.OpenFileButton.Size = new System.Drawing.Size(24, 24);
      this.OpenFileButton.TabIndex = 8;
      this.OpenFileButton.UseVisualStyleBackColor = true;
      this.OpenFileButton.Click += new System.EventHandler(this.OpenFileButtonClick);
      // 
      // ProblemFileLabel
      // 
      this.ProblemFileLabel.AutoSize = true;
      this.ProblemFileLabel.Location = new System.Drawing.Point(18, 9);
      this.ProblemFileLabel.Name = "ProblemFileLabel";
      this.ProblemFileLabel.Size = new System.Drawing.Size(67, 13);
      this.ProblemFileLabel.TabIndex = 6;
      this.ProblemFileLabel.Text = "Problem File:";
      // 
      // ProblemTextBox
      // 
      this.ProblemTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ProblemTextBox.Location = new System.Drawing.Point(91, 6);
      this.ProblemTextBox.Name = "ProblemTextBox";
      this.ProblemTextBox.ReadOnly = true;
      this.ProblemTextBox.Size = new System.Drawing.Size(332, 20);
      this.ProblemTextBox.TabIndex = 9;
      // 
      // openFileDialog
      // 
      this.openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
      // 
      // DateTimeFormatComboBox
      // 
      this.DateTimeFormatComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DateTimeFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.DateTimeFormatComboBox.Enabled = false;
      this.DateTimeFormatComboBox.FormattingEnabled = true;
      this.DateTimeFormatComboBox.Location = new System.Drawing.Point(111, 73);
      this.DateTimeFormatComboBox.Name = "DateTimeFormatComboBox";
      this.DateTimeFormatComboBox.Size = new System.Drawing.Size(300, 21);
      this.DateTimeFormatComboBox.TabIndex = 15;
      this.DateTimeFormatComboBox.SelectionChangeCommitted += new System.EventHandler(this.CSVFormatComboBoxSelectionChangeCommitted);
      // 
      // DecimalSeparatorComboBox
      // 
      this.DecimalSeparatorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DecimalSeparatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.DecimalSeparatorComboBox.Enabled = false;
      this.DecimalSeparatorComboBox.FormattingEnabled = true;
      this.DecimalSeparatorComboBox.Location = new System.Drawing.Point(111, 46);
      this.DecimalSeparatorComboBox.Name = "DecimalSeparatorComboBox";
      this.DecimalSeparatorComboBox.Size = new System.Drawing.Size(300, 21);
      this.DecimalSeparatorComboBox.TabIndex = 14;
      this.DecimalSeparatorComboBox.SelectionChangeCommitted += new System.EventHandler(this.CSVFormatComboBoxSelectionChangeCommitted);
      // 
      // DateTimeFormatLabel
      // 
      this.DateTimeFormatLabel.AutoSize = true;
      this.DateTimeFormatLabel.Location = new System.Drawing.Point(6, 76);
      this.DateTimeFormatLabel.Name = "DateTimeFormatLabel";
      this.DateTimeFormatLabel.Size = new System.Drawing.Size(91, 13);
      this.DateTimeFormatLabel.TabIndex = 13;
      this.DateTimeFormatLabel.Text = "DateTime Format:";
      // 
      // DecimalSeperatorLabel
      // 
      this.DecimalSeperatorLabel.AutoSize = true;
      this.DecimalSeperatorLabel.Location = new System.Drawing.Point(6, 49);
      this.DecimalSeperatorLabel.Name = "DecimalSeperatorLabel";
      this.DecimalSeperatorLabel.Size = new System.Drawing.Size(97, 13);
      this.DecimalSeperatorLabel.TabIndex = 12;
      this.DecimalSeperatorLabel.Text = "Decimal Separator:";
      // 
      // SeparatorLabel
      // 
      this.SeparatorLabel.AutoSize = true;
      this.SeparatorLabel.Location = new System.Drawing.Point(6, 22);
      this.SeparatorLabel.Name = "SeparatorLabel";
      this.SeparatorLabel.Size = new System.Drawing.Size(56, 13);
      this.SeparatorLabel.TabIndex = 11;
      this.SeparatorLabel.Text = "Separator:";
      // 
      // SeparatorComboBox
      // 
      this.SeparatorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.SeparatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.SeparatorComboBox.Enabled = false;
      this.SeparatorComboBox.FormattingEnabled = true;
      this.SeparatorComboBox.Location = new System.Drawing.Point(111, 19);
      this.SeparatorComboBox.Name = "SeparatorComboBox";
      this.SeparatorComboBox.Size = new System.Drawing.Size(300, 21);
      this.SeparatorComboBox.TabIndex = 10;
      this.SeparatorComboBox.SelectionChangeCommitted += new System.EventHandler(this.CSVFormatComboBoxSelectionChangeCommitted);
      // 
      // CSVSettingsGroupBox
      // 
      this.CSVSettingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.CSVSettingsGroupBox.Controls.Add(this.CheckboxColumnNames);
      this.CSVSettingsGroupBox.Controls.Add(this.DateTimeFormatInfoLabel);
      this.CSVSettingsGroupBox.Controls.Add(this.DecimalSeparatorInfoLabel);
      this.CSVSettingsGroupBox.Controls.Add(this.SeparatorInfoLabel);
      this.CSVSettingsGroupBox.Controls.Add(this.SeparatorComboBox);
      this.CSVSettingsGroupBox.Controls.Add(this.DateTimeFormatComboBox);
      this.CSVSettingsGroupBox.Controls.Add(this.SeparatorLabel);
      this.CSVSettingsGroupBox.Controls.Add(this.DecimalSeparatorComboBox);
      this.CSVSettingsGroupBox.Controls.Add(this.DecimalSeperatorLabel);
      this.CSVSettingsGroupBox.Controls.Add(this.DateTimeFormatLabel);
      this.CSVSettingsGroupBox.Location = new System.Drawing.Point(12, 32);
      this.CSVSettingsGroupBox.Name = "CSVSettingsGroupBox";
      this.CSVSettingsGroupBox.Size = new System.Drawing.Size(447, 126);
      this.CSVSettingsGroupBox.TabIndex = 16;
      this.CSVSettingsGroupBox.TabStop = false;
      this.CSVSettingsGroupBox.Text = "CSV Settings";
      // 
      // DateTimeFormatInfoLabel
      // 
      this.DateTimeFormatInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.DateTimeFormatInfoLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Information;
      this.DateTimeFormatInfoLabel.Location = new System.Drawing.Point(421, 76);
      this.DateTimeFormatInfoLabel.Name = "DateTimeFormatInfoLabel";
      this.DateTimeFormatInfoLabel.Size = new System.Drawing.Size(16, 16);
      this.DateTimeFormatInfoLabel.TabIndex = 23;
      this.DateTimeFormatInfoLabel.Tag = "Select the date time format used in the csv file";
      this.ToolTip.SetToolTip(this.DateTimeFormatInfoLabel, "Select the date time format used in the csv file");
      this.DateTimeFormatInfoLabel.DoubleClick += new System.EventHandler(this.ControlToolTip_DoubleClick);
      // 
      // DecimalSeparatorInfoLabel
      // 
      this.DecimalSeparatorInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.DecimalSeparatorInfoLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Information;
      this.DecimalSeparatorInfoLabel.Location = new System.Drawing.Point(421, 49);
      this.DecimalSeparatorInfoLabel.Name = "DecimalSeparatorInfoLabel";
      this.DecimalSeparatorInfoLabel.Size = new System.Drawing.Size(16, 16);
      this.DecimalSeparatorInfoLabel.TabIndex = 22;
      this.DecimalSeparatorInfoLabel.Tag = "Select the decimal separator used to for double values";
      this.ToolTip.SetToolTip(this.DecimalSeparatorInfoLabel, "Select the decimal separator used to for double values");
      this.DecimalSeparatorInfoLabel.DoubleClick += new System.EventHandler(this.ControlToolTip_DoubleClick);
      // 
      // SeparatorInfoLabel
      // 
      this.SeparatorInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.SeparatorInfoLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Information;
      this.SeparatorInfoLabel.Location = new System.Drawing.Point(421, 22);
      this.SeparatorInfoLabel.Name = "SeparatorInfoLabel";
      this.SeparatorInfoLabel.Size = new System.Drawing.Size(16, 16);
      this.SeparatorInfoLabel.TabIndex = 21;
      this.SeparatorInfoLabel.Tag = "Select the separator used to separate columns in the csv file.";
      this.ToolTip.SetToolTip(this.SeparatorInfoLabel, "Select the separator used to separate columns in the csv file.");
      this.SeparatorInfoLabel.DoubleClick += new System.EventHandler(this.ControlToolTip_DoubleClick);
      // 
      // ProblemDataSettingsGroupBox
      // 
      this.ProblemDataSettingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ProblemDataSettingsGroupBox.Controls.Add(this.ErrorTextBox);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.ShuffelInfoLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.PreviewLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.TestLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.PreviewDatasetMatrix);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.TrainingLabel);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.TrainingTestTrackBar);
      this.ProblemDataSettingsGroupBox.Controls.Add(this.ShuffleDataCheckbox);
      this.ProblemDataSettingsGroupBox.Location = new System.Drawing.Point(12, 164);
      this.ProblemDataSettingsGroupBox.Name = "ProblemDataSettingsGroupBox";
      this.ProblemDataSettingsGroupBox.Size = new System.Drawing.Size(447, 252);
      this.ProblemDataSettingsGroupBox.TabIndex = 17;
      this.ProblemDataSettingsGroupBox.TabStop = false;
      this.ProblemDataSettingsGroupBox.Text = "ProblemData Settings";
      // 
      // ErrorTextBox
      // 
      this.ErrorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
      this.ErrorTextBox.Location = new System.Drawing.Point(6, 15);
      this.ErrorTextBox.Multiline = true;
      this.ErrorTextBox.Name = "ErrorTextBox";
      this.ErrorTextBox.Size = new System.Drawing.Size(435, 73);
      this.ErrorTextBox.TabIndex = 0;
      this.ErrorTextBox.Text = "Open a CSV File";
      this.ErrorTextBox.Visible = false;
      // 
      // ShuffelInfoLabel
      // 
      this.ShuffelInfoLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Information;
      this.ShuffelInfoLabel.Location = new System.Drawing.Point(111, 20);
      this.ShuffelInfoLabel.Name = "ShuffelInfoLabel";
      this.ShuffelInfoLabel.Size = new System.Drawing.Size(16, 16);
      this.ShuffelInfoLabel.TabIndex = 8;
      this.ShuffelInfoLabel.Tag = "Check, if the imported data should be shuffled";
      this.ToolTip.SetToolTip(this.ShuffelInfoLabel, "Check, if the importat data should be shuffled");
      this.ShuffelInfoLabel.DoubleClick += new System.EventHandler(this.ControlToolTip_DoubleClick);
      // 
      // PreviewLabel
      // 
      this.PreviewLabel.AutoSize = true;
      this.PreviewLabel.Location = new System.Drawing.Point(9, 89);
      this.PreviewLabel.Name = "PreviewLabel";
      this.PreviewLabel.Size = new System.Drawing.Size(45, 13);
      this.PreviewLabel.TabIndex = 7;
      this.PreviewLabel.Text = "Preview";
      // 
      // PreviewDatasetMatrix
      // 
      this.PreviewDatasetMatrix.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.PreviewDatasetMatrix.Caption = "Dataset Preview";
      this.PreviewDatasetMatrix.Content = null;
      this.PreviewDatasetMatrix.Location = new System.Drawing.Point(6, 108);
      this.PreviewDatasetMatrix.Name = "PreviewDatasetMatrix";
      this.PreviewDatasetMatrix.ReadOnly = true;
      this.PreviewDatasetMatrix.ShowRowsAndColumnsTextBox = false;
      this.PreviewDatasetMatrix.ShowStatisticalInformation = false;
      this.PreviewDatasetMatrix.Size = new System.Drawing.Size(435, 138);
      this.PreviewDatasetMatrix.TabIndex = 0;
      // 
      // CheckboxColumnNames
      // 
      this.CheckboxColumnNames.AutoSize = true;
      this.CheckboxColumnNames.Location = new System.Drawing.Point(9, 103);
      this.CheckboxColumnNames.Name = "CheckboxColumnNames";
      this.CheckboxColumnNames.Size = new System.Drawing.Size(144, 17);
      this.CheckboxColumnNames.TabIndex = 24;
      this.CheckboxColumnNames.Text = "Column names in first line";
      this.CheckboxColumnNames.UseVisualStyleBackColor = true;
      this.CheckboxColumnNames.CheckedChanged += new System.EventHandler(this.CheckboxColumnNames_CheckedChanged);
      // 
      // DataAnalysisImportTypeDialog
      // 
      this.AcceptButton = this.OkButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(471, 457);
      this.Controls.Add(this.ProblemDataSettingsGroupBox);
      this.Controls.Add(this.CSVSettingsGroupBox);
      this.Controls.Add(this.ProblemTextBox);
      this.Controls.Add(this.OpenFileButton);
      this.Controls.Add(this.ProblemFileLabel);
      this.Controls.Add(this.CancelationButton);
      this.Controls.Add(this.OkButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DataAnalysisImportTypeDialog";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "CSV Import";
      ((System.ComponentModel.ISupportInitialize)(this.TrainingTestTrackBar)).EndInit();
      this.CSVSettingsGroupBox.ResumeLayout(false);
      this.CSVSettingsGroupBox.PerformLayout();
      this.ProblemDataSettingsGroupBox.ResumeLayout(false);
      this.ProblemDataSettingsGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.CheckBox ShuffleDataCheckbox;
    protected System.Windows.Forms.Button OkButton;
    protected System.Windows.Forms.TrackBar TrainingTestTrackBar;
    protected System.Windows.Forms.Label TestLabel;
    protected System.Windows.Forms.Label TrainingLabel;
    protected System.Windows.Forms.Button CancelationButton;
    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    protected System.Windows.Forms.Label ProblemFileLabel;
    protected System.Windows.Forms.Button OpenFileButton;
    protected System.Windows.Forms.TextBox ProblemTextBox;
    protected System.Windows.Forms.ComboBox DateTimeFormatComboBox;
    protected System.Windows.Forms.ComboBox DecimalSeparatorComboBox;
    protected System.Windows.Forms.Label DateTimeFormatLabel;
    protected System.Windows.Forms.Label DecimalSeperatorLabel;
    protected System.Windows.Forms.Label SeparatorLabel;
    protected System.Windows.Forms.ComboBox SeparatorComboBox;
    protected System.Windows.Forms.GroupBox CSVSettingsGroupBox;
    protected System.Windows.Forms.GroupBox ProblemDataSettingsGroupBox;
    protected System.Windows.Forms.TextBox ErrorTextBox;
    protected HeuristicLab.Data.Views.StringConvertibleMatrixView PreviewDatasetMatrix;
    protected System.Windows.Forms.Label PreviewLabel;
    protected System.Windows.Forms.Label SeparatorInfoLabel;
    protected System.Windows.Forms.Label DateTimeFormatInfoLabel;
    protected System.Windows.Forms.Label DecimalSeparatorInfoLabel;
    protected System.Windows.Forms.Label ShuffelInfoLabel;
    protected System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.CheckBox CheckboxColumnNames;
  }
}