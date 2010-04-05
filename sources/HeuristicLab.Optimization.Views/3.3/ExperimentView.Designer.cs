#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization.Views {
  partial class ExperimentView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.tabControl = new System.Windows.Forms.TabControl();
      this.batchRunsTabPage = new System.Windows.Forms.TabPage();
      this.resultsTabPage = new System.Windows.Forms.TabPage();
      this.runsView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.startButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.pauseButton = new System.Windows.Forms.Button();
      this.batchRunListView = new HeuristicLab.Optimization.Views.BatchRunListView();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.batchRunsTabPage.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(607, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Size = new System.Drawing.Size(607, 20);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.batchRunsTabPage);
      this.tabControl.Controls.Add(this.resultsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 52);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(679, 400);
      this.tabControl.TabIndex = 4;
      // 
      // batchRunsTabPage
      // 
      this.batchRunsTabPage.Controls.Add(this.batchRunListView);
      this.batchRunsTabPage.Location = new System.Drawing.Point(4, 22);
      this.batchRunsTabPage.Name = "batchRunsTabPage";
      this.batchRunsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.batchRunsTabPage.Size = new System.Drawing.Size(671, 374);
      this.batchRunsTabPage.TabIndex = 1;
      this.batchRunsTabPage.Text = "Batch Runs";
      this.batchRunsTabPage.UseVisualStyleBackColor = true;
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Controls.Add(this.runsView);
      this.resultsTabPage.Location = new System.Drawing.Point(4, 22);
      this.resultsTabPage.Name = "resultsTabPage";
      this.resultsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsTabPage.Size = new System.Drawing.Size(671, 374);
      this.resultsTabPage.TabIndex = 2;
      this.resultsTabPage.Text = "Results";
      this.resultsTabPage.UseVisualStyleBackColor = true;
      // 
      // runsView
      // 
      this.runsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.runsView.Caption = "RunCollection";
      this.runsView.Content = null;
      this.runsView.Location = new System.Drawing.Point(6, 6);
      this.runsView.Name = "runsView";
      this.runsView.Size = new System.Drawing.Size(659, 362);
      this.runsView.TabIndex = 0;
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Play;
      this.startButton.Location = new System.Drawing.Point(0, 458);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(24, 24);
      this.startButton.TabIndex = 5;
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Experiment");
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Stop;
      this.stopButton.Location = new System.Drawing.Point(60, 458);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(24, 24);
      this.stopButton.TabIndex = 7;
      this.toolTip.SetToolTip(this.stopButton, "Stop Experiment");
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // resetButton
      // 
      this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Restart;
      this.resetButton.Location = new System.Drawing.Point(90, 458);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(24, 24);
      this.resetButton.TabIndex = 8;
      this.toolTip.SetToolTip(this.resetButton, "Reset Experiment");
      this.resetButton.UseVisualStyleBackColor = true;
      this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeLabel.AutoSize = true;
      this.executionTimeLabel.Location = new System.Drawing.Point(453, 465);
      this.executionTimeLabel.Name = "executionTimeLabel";
      this.executionTimeLabel.Size = new System.Drawing.Size(83, 13);
      this.executionTimeLabel.TabIndex = 9;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(542, 462);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(137, 20);
      this.executionTimeTextBox.TabIndex = 10;
      // 
      // pauseButton
      // 
      this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.pauseButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Pause;
      this.pauseButton.Location = new System.Drawing.Point(30, 458);
      this.pauseButton.Name = "pauseButton";
      this.pauseButton.Size = new System.Drawing.Size(24, 24);
      this.pauseButton.TabIndex = 6;
      this.toolTip.SetToolTip(this.pauseButton, "Pause Experiment");
      this.pauseButton.UseVisualStyleBackColor = true;
      this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
      // 
      // batchRunListView
      // 
      this.batchRunListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.batchRunListView.Caption = "BatchRunList";
      this.batchRunListView.Content = null;
      this.batchRunListView.Location = new System.Drawing.Point(6, 6);
      this.batchRunListView.Name = "batchRunListView";
      this.batchRunListView.Size = new System.Drawing.Size(659, 362);
      this.batchRunListView.TabIndex = 0;
      // 
      // ExperimentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.executionTimeLabel);
      this.Controls.Add(this.pauseButton);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.resetButton);
      this.Name = "ExperimentView";
      this.Size = new System.Drawing.Size(679, 482);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.batchRunsTabPage.ResumeLayout(false);
      this.resultsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage batchRunsTabPage;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.Label executionTimeLabel;
    private System.Windows.Forms.TextBox executionTimeTextBox;
    private System.Windows.Forms.TabPage resultsTabPage;
    private RunCollectionView runsView;
    private System.Windows.Forms.Button pauseButton;
    private BatchRunListView batchRunListView;

  }
}
