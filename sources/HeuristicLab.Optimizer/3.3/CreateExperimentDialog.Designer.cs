#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimizer {
  partial class CreateExperimentDialog {
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
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.createBatchRunCheckBox = new System.Windows.Forms.CheckBox();
      this.createBatchRunLabel = new System.Windows.Forms.Label();
      this.repetitionsLabel = new System.Windows.Forms.Label();
      this.repetitionsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.instancesLabel = new System.Windows.Forms.Label();
      this.instancesListView = new System.Windows.Forms.ListView();
      this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.experimentCreationBackgroundWorker = new System.ComponentModel.BackgroundWorker();
      this.experimentCreationProgressBar = new System.Windows.Forms.ProgressBar();
      this.progressLabel = new System.Windows.Forms.Label();
      this.selectAllCheckBox = new System.Windows.Forms.CheckBox();
      this.selectNoneCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.repetitionsNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(113, 259);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 4;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(194, 259);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 5;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // createBatchRunCheckBox
      // 
      this.createBatchRunCheckBox.AutoSize = true;
      this.createBatchRunCheckBox.Checked = true;
      this.createBatchRunCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.createBatchRunCheckBox.Location = new System.Drawing.Point(113, 8);
      this.createBatchRunCheckBox.Name = "createBatchRunCheckBox";
      this.createBatchRunCheckBox.Size = new System.Drawing.Size(15, 14);
      this.createBatchRunCheckBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.createBatchRunCheckBox, "Check to create a batch run for executing an optimizer multiple times.");
      this.createBatchRunCheckBox.UseVisualStyleBackColor = true;
      this.createBatchRunCheckBox.CheckedChanged += new System.EventHandler(this.createBatchRunCheckBox_CheckedChanged);
      // 
      // createBatchRunLabel
      // 
      this.createBatchRunLabel.AutoSize = true;
      this.createBatchRunLabel.Location = new System.Drawing.Point(12, 9);
      this.createBatchRunLabel.Name = "createBatchRunLabel";
      this.createBatchRunLabel.Size = new System.Drawing.Size(95, 13);
      this.createBatchRunLabel.TabIndex = 0;
      this.createBatchRunLabel.Text = "&Create Batch Run:";
      // 
      // repetitionsLabel
      // 
      this.repetitionsLabel.AutoSize = true;
      this.repetitionsLabel.Location = new System.Drawing.Point(12, 31);
      this.repetitionsLabel.Name = "repetitionsLabel";
      this.repetitionsLabel.Size = new System.Drawing.Size(63, 13);
      this.repetitionsLabel.TabIndex = 2;
      this.repetitionsLabel.Text = "&Repetitions:";
      // 
      // repetitionsNumericUpDown
      // 
      this.repetitionsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.repetitionsNumericUpDown.Location = new System.Drawing.Point(113, 28);
      this.repetitionsNumericUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
      this.repetitionsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.repetitionsNumericUpDown.Name = "repetitionsNumericUpDown";
      this.repetitionsNumericUpDown.Size = new System.Drawing.Size(156, 20);
      this.repetitionsNumericUpDown.TabIndex = 3;
      this.repetitionsNumericUpDown.ThousandsSeparator = true;
      this.toolTip.SetToolTip(this.repetitionsNumericUpDown, "Number of repetitions executed by the batch run.");
      this.repetitionsNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.repetitionsNumericUpDown.Validated += new System.EventHandler(this.repetitionsNumericUpDown_Validated);
      // 
      // instancesLabel
      // 
      this.instancesLabel.AutoSize = true;
      this.instancesLabel.Location = new System.Drawing.Point(12, 54);
      this.instancesLabel.Name = "instancesLabel";
      this.instancesLabel.Size = new System.Drawing.Size(56, 13);
      this.instancesLabel.TabIndex = 2;
      this.instancesLabel.Text = "&Instances:";
      // 
      // instancesListView
      // 
      this.instancesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.instancesListView.CheckBoxes = true;
      this.instancesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
      this.instancesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.instancesListView.Location = new System.Drawing.Point(113, 77);
      this.instancesListView.Name = "instancesListView";
      this.instancesListView.Size = new System.Drawing.Size(156, 176);
      this.instancesListView.TabIndex = 6;
      this.instancesListView.UseCompatibleStateImageBehavior = false;
      this.instancesListView.View = System.Windows.Forms.View.Details;
      this.instancesListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.instancesListView_ItemChecked);
      // 
      // columnHeader
      // 
      this.columnHeader.Width = 150;
      // 
      // experimentCreationBackgroundWorker
      // 
      this.experimentCreationBackgroundWorker.WorkerReportsProgress = true;
      this.experimentCreationBackgroundWorker.WorkerSupportsCancellation = true;
      this.experimentCreationBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.experimentCreationBackgroundWorker_DoWork);
      this.experimentCreationBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.experimentCreationBackgroundWorker_ProgressChanged);
      this.experimentCreationBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.experimentCreationBackgroundWorker_RunWorkerCompleted);
      // 
      // experimentCreationProgressBar
      // 
      this.experimentCreationProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.experimentCreationProgressBar.Location = new System.Drawing.Point(15, 259);
      this.experimentCreationProgressBar.Name = "experimentCreationProgressBar";
      this.experimentCreationProgressBar.Size = new System.Drawing.Size(92, 23);
      this.experimentCreationProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.experimentCreationProgressBar.TabIndex = 7;
      this.experimentCreationProgressBar.Visible = false;
      // 
      // progressLabel
      // 
      this.progressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.progressLabel.BackColor = System.Drawing.SystemColors.Control;
      this.progressLabel.Location = new System.Drawing.Point(113, 259);
      this.progressLabel.Name = "progressLabel";
      this.progressLabel.Size = new System.Drawing.Size(75, 23);
      this.progressLabel.TabIndex = 8;
      this.progressLabel.Text = "label1";
      this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.progressLabel.Visible = false;
      // 
      // selectAllCheckBox
      // 
      this.selectAllCheckBox.AutoSize = true;
      this.selectAllCheckBox.Location = new System.Drawing.Point(113, 54);
      this.selectAllCheckBox.Name = "selectAllCheckBox";
      this.selectAllCheckBox.Size = new System.Drawing.Size(67, 17);
      this.selectAllCheckBox.TabIndex = 9;
      this.selectAllCheckBox.Text = "select all";
      this.selectAllCheckBox.UseVisualStyleBackColor = true;
      this.selectAllCheckBox.CheckedChanged += new System.EventHandler(this.selectAllCheckBox_CheckedChanged);
      // 
      // selectNoneCheckBox
      // 
      this.selectNoneCheckBox.AutoSize = true;
      this.selectNoneCheckBox.Location = new System.Drawing.Point(186, 54);
      this.selectNoneCheckBox.Name = "selectNoneCheckBox";
      this.selectNoneCheckBox.Size = new System.Drawing.Size(81, 17);
      this.selectNoneCheckBox.TabIndex = 9;
      this.selectNoneCheckBox.Text = "select none";
      this.selectNoneCheckBox.UseVisualStyleBackColor = true;
      this.selectNoneCheckBox.CheckedChanged += new System.EventHandler(this.selectNoneCheckBox_CheckedChanged);
      // 
      // CreateExperimentDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(281, 294);
      this.Controls.Add(this.selectNoneCheckBox);
      this.Controls.Add(this.selectAllCheckBox);
      this.Controls.Add(this.instancesListView);
      this.Controls.Add(this.repetitionsNumericUpDown);
      this.Controls.Add(this.instancesLabel);
      this.Controls.Add(this.repetitionsLabel);
      this.Controls.Add(this.createBatchRunLabel);
      this.Controls.Add(this.createBatchRunCheckBox);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.progressLabel);
      this.Controls.Add(this.experimentCreationProgressBar);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CreateExperimentDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Create Experiment";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateExperimentDialog_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.repetitionsNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.CheckBox createBatchRunCheckBox;
    private System.Windows.Forms.Label createBatchRunLabel;
    private System.Windows.Forms.Label repetitionsLabel;
    private System.Windows.Forms.NumericUpDown repetitionsNumericUpDown;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Label instancesLabel;
    private System.Windows.Forms.ListView instancesListView;
    private System.Windows.Forms.ColumnHeader columnHeader;
    private System.ComponentModel.BackgroundWorker experimentCreationBackgroundWorker;
    private System.Windows.Forms.ProgressBar experimentCreationProgressBar;
    private System.Windows.Forms.Label progressLabel;
    private System.Windows.Forms.CheckBox selectAllCheckBox;
    private System.Windows.Forms.CheckBox selectNoneCheckBox;

  }
}