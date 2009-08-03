#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using System.Windows.Forms;
namespace HeuristicLab.CEDMA.Server {
  partial class ProblemView {
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
      this.importButton = new System.Windows.Forms.Button();
      this.trainingSamplesStartTextBox = new System.Windows.Forms.TextBox();
      this.trainingLabel = new System.Windows.Forms.Label();
      this.trainingSamplesEndTextBox = new System.Windows.Forms.TextBox();
      this.validationSamplesEndTextBox = new System.Windows.Forms.TextBox();
      this.validationSamplesStartTextBox = new System.Windows.Forms.TextBox();
      this.validationLabel = new System.Windows.Forms.Label();
      this.testSamplesEndTextBox = new System.Windows.Forms.TextBox();
      this.testSamplesStartTextBox = new System.Windows.Forms.TextBox();
      this.testLabel = new System.Windows.Forms.Label();
      this.autoregressiveCheckBox = new System.Windows.Forms.CheckBox();
      this.partitioningGroupBox = new System.Windows.Forms.GroupBox();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.classificationRadioButton = new System.Windows.Forms.RadioButton();
      this.regressionRadioButton = new System.Windows.Forms.RadioButton();
      this.timeSeriesRadioButton = new System.Windows.Forms.RadioButton();
      this.minTimeOffsetLabel = new System.Windows.Forms.Label();
      this.minTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.learningTaskGroupBox = new System.Windows.Forms.GroupBox();
      this.autoregressiveLabel = new System.Windows.Forms.Label();
      this.maxTimeOffsetLabel = new System.Windows.Forms.Label();
      this.maxTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.datasetView1 = new HeuristicLab.DataAnalysis.DatasetView();
      this.partitioningGroupBox.SuspendLayout();
      this.learningTaskGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // importButton
      // 
      this.importButton.Location = new System.Drawing.Point(3, 3);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(75, 23);
      this.importButton.TabIndex = 0;
      this.importButton.Text = "Import";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // trainingSamplesStartTextBox
      // 
      this.trainingSamplesStartTextBox.Location = new System.Drawing.Point(119, 17);
      this.trainingSamplesStartTextBox.Name = "trainingSamplesStartTextBox";
      this.trainingSamplesStartTextBox.Size = new System.Drawing.Size(96, 20);
      this.trainingSamplesStartTextBox.TabIndex = 4;
      this.trainingSamplesStartTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.trainingSamplesStartTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // trainingLabel
      // 
      this.trainingLabel.AutoSize = true;
      this.trainingLabel.Location = new System.Drawing.Point(13, 20);
      this.trainingLabel.Name = "trainingLabel";
      this.trainingLabel.Size = new System.Drawing.Size(89, 13);
      this.trainingLabel.TabIndex = 3;
      this.trainingLabel.Text = "Training samples:";
      // 
      // trainingSamplesEndTextBox
      // 
      this.trainingSamplesEndTextBox.Location = new System.Drawing.Point(221, 17);
      this.trainingSamplesEndTextBox.Name = "trainingSamplesEndTextBox";
      this.trainingSamplesEndTextBox.Size = new System.Drawing.Size(96, 20);
      this.trainingSamplesEndTextBox.TabIndex = 10;
      this.trainingSamplesEndTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.trainingSamplesEndTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // validationSamplesEndTextBox
      // 
      this.validationSamplesEndTextBox.Location = new System.Drawing.Point(221, 43);
      this.validationSamplesEndTextBox.Name = "validationSamplesEndTextBox";
      this.validationSamplesEndTextBox.Size = new System.Drawing.Size(96, 20);
      this.validationSamplesEndTextBox.TabIndex = 13;
      this.validationSamplesEndTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.validationSamplesEndTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // validationSamplesStartTextBox
      // 
      this.validationSamplesStartTextBox.Location = new System.Drawing.Point(119, 43);
      this.validationSamplesStartTextBox.Name = "validationSamplesStartTextBox";
      this.validationSamplesStartTextBox.Size = new System.Drawing.Size(96, 20);
      this.validationSamplesStartTextBox.TabIndex = 12;
      this.validationSamplesStartTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.validationSamplesStartTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // validationLabel
      // 
      this.validationLabel.AutoSize = true;
      this.validationLabel.Location = new System.Drawing.Point(5, 46);
      this.validationLabel.Name = "validationLabel";
      this.validationLabel.Size = new System.Drawing.Size(97, 13);
      this.validationLabel.TabIndex = 11;
      this.validationLabel.Text = "Validation samples:";
      // 
      // testSamplesEndTextBox
      // 
      this.testSamplesEndTextBox.Location = new System.Drawing.Point(221, 68);
      this.testSamplesEndTextBox.Name = "testSamplesEndTextBox";
      this.testSamplesEndTextBox.Size = new System.Drawing.Size(96, 20);
      this.testSamplesEndTextBox.TabIndex = 16;
      this.testSamplesEndTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.testSamplesEndTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // testSamplesStartTextBox
      // 
      this.testSamplesStartTextBox.Location = new System.Drawing.Point(119, 68);
      this.testSamplesStartTextBox.Name = "testSamplesStartTextBox";
      this.testSamplesStartTextBox.Size = new System.Drawing.Size(96, 20);
      this.testSamplesStartTextBox.TabIndex = 15;
      this.testSamplesStartTextBox.Validated += new System.EventHandler(this.samplesTextBox_Validated);
      this.testSamplesStartTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.samplesTextBox_Validating);
      // 
      // testLabel
      // 
      this.testLabel.AutoSize = true;
      this.testLabel.Location = new System.Drawing.Point(30, 71);
      this.testLabel.Name = "testLabel";
      this.testLabel.Size = new System.Drawing.Size(72, 13);
      this.testLabel.TabIndex = 14;
      this.testLabel.Text = "Test samples:";
      // 
      // autoregressiveCheckBox
      // 
      this.autoregressiveCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.autoregressiveCheckBox.AutoSize = true;
      this.autoregressiveCheckBox.Enabled = false;
      this.autoregressiveCheckBox.Location = new System.Drawing.Point(131, 83);
      this.autoregressiveCheckBox.Name = "autoregressiveCheckBox";
      this.autoregressiveCheckBox.Size = new System.Drawing.Size(15, 14);
      this.autoregressiveCheckBox.TabIndex = 22;
      this.autoregressiveCheckBox.UseVisualStyleBackColor = true;
      this.autoregressiveCheckBox.CheckedChanged += new System.EventHandler(this.autoregressiveCheckBox_CheckedChanged);
      // 
      // partitioningGroupBox
      // 
      this.partitioningGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.partitioningGroupBox.Controls.Add(this.validationSamplesEndTextBox);
      this.partitioningGroupBox.Controls.Add(this.trainingLabel);
      this.partitioningGroupBox.Controls.Add(this.trainingSamplesStartTextBox);
      this.partitioningGroupBox.Controls.Add(this.trainingSamplesEndTextBox);
      this.partitioningGroupBox.Controls.Add(this.validationLabel);
      this.partitioningGroupBox.Controls.Add(this.testSamplesEndTextBox);
      this.partitioningGroupBox.Controls.Add(this.validationSamplesStartTextBox);
      this.partitioningGroupBox.Controls.Add(this.testSamplesStartTextBox);
      this.partitioningGroupBox.Controls.Add(this.testLabel);
      this.partitioningGroupBox.Location = new System.Drawing.Point(3, 452);
      this.partitioningGroupBox.Name = "partitioningGroupBox";
      this.partitioningGroupBox.Size = new System.Drawing.Size(326, 100);
      this.partitioningGroupBox.TabIndex = 25;
      this.partitioningGroupBox.TabStop = false;
      this.partitioningGroupBox.Text = "Data set partitions:";
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "txt";
      this.openFileDialog.FileName = "txt";
      this.openFileDialog.Filter = "Text files|*.txt|All files|*.*";
      this.openFileDialog.Title = "Import data set from file";
      // 
      // classificationRadioButton
      // 
      this.classificationRadioButton.AutoSize = true;
      this.classificationRadioButton.Checked = true;
      this.classificationRadioButton.Location = new System.Drawing.Point(6, 16);
      this.classificationRadioButton.Name = "classificationRadioButton";
      this.classificationRadioButton.Size = new System.Drawing.Size(86, 17);
      this.classificationRadioButton.TabIndex = 28;
      this.classificationRadioButton.TabStop = true;
      this.classificationRadioButton.Text = "Classification";
      this.classificationRadioButton.UseVisualStyleBackColor = true;
      this.classificationRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // regressionRadioButton
      // 
      this.regressionRadioButton.AutoSize = true;
      this.regressionRadioButton.Location = new System.Drawing.Point(6, 39);
      this.regressionRadioButton.Name = "regressionRadioButton";
      this.regressionRadioButton.Size = new System.Drawing.Size(78, 17);
      this.regressionRadioButton.TabIndex = 29;
      this.regressionRadioButton.Text = "Regression";
      this.regressionRadioButton.UseVisualStyleBackColor = true;
      this.regressionRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // timeSeriesRadioButton
      // 
      this.timeSeriesRadioButton.AutoSize = true;
      this.timeSeriesRadioButton.Location = new System.Drawing.Point(6, 62);
      this.timeSeriesRadioButton.Name = "timeSeriesRadioButton";
      this.timeSeriesRadioButton.Size = new System.Drawing.Size(119, 17);
      this.timeSeriesRadioButton.TabIndex = 30;
      this.timeSeriesRadioButton.Text = "Time series forecast";
      this.timeSeriesRadioButton.UseVisualStyleBackColor = true;
      this.timeSeriesRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // minTimeOffsetLabel
      // 
      this.minTimeOffsetLabel.AutoSize = true;
      this.minTimeOffsetLabel.Enabled = false;
      this.minTimeOffsetLabel.Location = new System.Drawing.Point(38, 106);
      this.minTimeOffsetLabel.Name = "minTimeOffsetLabel";
      this.minTimeOffsetLabel.Size = new System.Drawing.Size(87, 13);
      this.minTimeOffsetLabel.TabIndex = 31;
      this.minTimeOffsetLabel.Text = "Min. Time Offset:";
      // 
      // minTimeOffsetTextBox
      // 
      this.minTimeOffsetTextBox.Enabled = false;
      this.minTimeOffsetTextBox.Location = new System.Drawing.Point(131, 103);
      this.minTimeOffsetTextBox.Name = "minTimeOffsetTextBox";
      this.minTimeOffsetTextBox.Size = new System.Drawing.Size(96, 20);
      this.minTimeOffsetTextBox.TabIndex = 32;
      this.minTimeOffsetTextBox.Validated += new System.EventHandler(this.timeOffsetTextBox_Validated);
      this.minTimeOffsetTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.timeOffsetTextBox_Validating);
      // 
      // learningTaskGroupBox
      // 
      this.learningTaskGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.learningTaskGroupBox.Controls.Add(this.autoregressiveLabel);
      this.learningTaskGroupBox.Controls.Add(this.classificationRadioButton);
      this.learningTaskGroupBox.Controls.Add(this.minTimeOffsetLabel);
      this.learningTaskGroupBox.Controls.Add(this.minTimeOffsetTextBox);
      this.learningTaskGroupBox.Controls.Add(this.maxTimeOffsetLabel);
      this.learningTaskGroupBox.Controls.Add(this.regressionRadioButton);
      this.learningTaskGroupBox.Controls.Add(this.maxTimeOffsetTextBox);
      this.learningTaskGroupBox.Controls.Add(this.timeSeriesRadioButton);
      this.learningTaskGroupBox.Controls.Add(this.autoregressiveCheckBox);
      this.learningTaskGroupBox.Location = new System.Drawing.Point(335, 452);
      this.learningTaskGroupBox.Name = "learningTaskGroupBox";
      this.learningTaskGroupBox.Size = new System.Drawing.Size(326, 163);
      this.learningTaskGroupBox.TabIndex = 35;
      this.learningTaskGroupBox.TabStop = false;
      this.learningTaskGroupBox.Text = "Learning task";
      // 
      // autoregressiveLabel
      // 
      this.autoregressiveLabel.AutoSize = true;
      this.autoregressiveLabel.Enabled = false;
      this.autoregressiveLabel.Location = new System.Drawing.Point(45, 83);
      this.autoregressiveLabel.Name = "autoregressiveLabel";
      this.autoregressiveLabel.Size = new System.Drawing.Size(80, 13);
      this.autoregressiveLabel.TabIndex = 35;
      this.autoregressiveLabel.Text = "Autoregressive:";
      // 
      // maxTimeOffsetLabel
      // 
      this.maxTimeOffsetLabel.AutoSize = true;
      this.maxTimeOffsetLabel.Enabled = false;
      this.maxTimeOffsetLabel.Location = new System.Drawing.Point(35, 132);
      this.maxTimeOffsetLabel.Name = "maxTimeOffsetLabel";
      this.maxTimeOffsetLabel.Size = new System.Drawing.Size(90, 13);
      this.maxTimeOffsetLabel.TabIndex = 33;
      this.maxTimeOffsetLabel.Text = "Max. Time Offset:";
      // 
      // maxTimeOffsetTextBox
      // 
      this.maxTimeOffsetTextBox.Enabled = false;
      this.maxTimeOffsetTextBox.Location = new System.Drawing.Point(131, 129);
      this.maxTimeOffsetTextBox.Name = "maxTimeOffsetTextBox";
      this.maxTimeOffsetTextBox.Size = new System.Drawing.Size(96, 20);
      this.maxTimeOffsetTextBox.TabIndex = 34;
      this.maxTimeOffsetTextBox.Validated += new System.EventHandler(this.timeOffsetTextBox_Validated);
      this.maxTimeOffsetTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.timeOffsetTextBox_Validating);
      // 
      // datasetView1
      // 
      this.datasetView1.Location = new System.Drawing.Point(3, 32);
      this.datasetView1.Name = "datasetView1";
      this.datasetView1.Size = new System.Drawing.Size(770, 414);
      this.datasetView1.TabIndex = 36;
      // 
      // ProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.datasetView1);
      this.Controls.Add(this.learningTaskGroupBox);
      this.Controls.Add(this.partitioningGroupBox);
      this.Controls.Add(this.importButton);
      this.Name = "ProblemView";
      this.Size = new System.Drawing.Size(776, 615);
      this.partitioningGroupBox.ResumeLayout(false);
      this.partitioningGroupBox.PerformLayout();
      this.learningTaskGroupBox.ResumeLayout(false);
      this.learningTaskGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.TextBox trainingSamplesStartTextBox;
    private System.Windows.Forms.Label trainingLabel;
    private System.Windows.Forms.TextBox trainingSamplesEndTextBox;
    private System.Windows.Forms.TextBox validationSamplesEndTextBox;
    private System.Windows.Forms.TextBox validationSamplesStartTextBox;
    private System.Windows.Forms.Label validationLabel;
    private System.Windows.Forms.TextBox testSamplesEndTextBox;
    private System.Windows.Forms.TextBox testSamplesStartTextBox;
    private System.Windows.Forms.Label testLabel;
    private System.Windows.Forms.CheckBox autoregressiveCheckBox;
    private System.Windows.Forms.GroupBox partitioningGroupBox;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.RadioButton classificationRadioButton;
    private System.Windows.Forms.RadioButton regressionRadioButton;
    private System.Windows.Forms.RadioButton timeSeriesRadioButton;
    private System.Windows.Forms.Label minTimeOffsetLabel;
    private System.Windows.Forms.TextBox minTimeOffsetTextBox;
    private System.Windows.Forms.GroupBox learningTaskGroupBox;
    private System.Windows.Forms.Label maxTimeOffsetLabel;
    private System.Windows.Forms.TextBox maxTimeOffsetTextBox;
    private System.Windows.Forms.Label autoregressiveLabel;
    private HeuristicLab.DataAnalysis.DatasetView datasetView1;
  }
}
