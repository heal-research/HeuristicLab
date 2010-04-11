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
  partial class AlgorithmView {
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
        if (problemTypeSelectorDialog != null) problemTypeSelectorDialog.Dispose();
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
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.parameterCollectionView = new HeuristicLab.Core.Views.ParameterCollectionView();
      this.problemTabPage = new System.Windows.Forms.TabPage();
      this.problemPanel = new System.Windows.Forms.Panel();
      this.problemViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.saveProblemButton = new System.Windows.Forms.Button();
      this.openProblemButton = new System.Windows.Forms.Button();
      this.newProblemButton = new System.Windows.Forms.Button();
      this.resultsTabPage = new System.Windows.Forms.TabPage();
      this.resultsView = new HeuristicLab.Optimization.Views.ResultCollectionView();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.runsView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.startButton = new System.Windows.Forms.Button();
      this.pauseButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.stopButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.problemPanel.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
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
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Controls.Add(this.problemTabPage);
      this.tabControl.Controls.Add(this.resultsTabPage);
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 52);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(679, 400);
      this.tabControl.TabIndex = 4;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.parameterCollectionView);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(671, 374);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parameterCollectionView.Caption = "ParameterCollection";
      this.parameterCollectionView.Content = null;
      this.parameterCollectionView.Location = new System.Drawing.Point(6, 6);
      this.parameterCollectionView.Name = "parameterCollectionView";
      this.parameterCollectionView.Size = new System.Drawing.Size(659, 362);
      this.parameterCollectionView.TabIndex = 0;
      // 
      // problemTabPage
      // 
      this.problemTabPage.Controls.Add(this.problemPanel);
      this.problemTabPage.Controls.Add(this.saveProblemButton);
      this.problemTabPage.Controls.Add(this.openProblemButton);
      this.problemTabPage.Controls.Add(this.newProblemButton);
      this.problemTabPage.Location = new System.Drawing.Point(4, 22);
      this.problemTabPage.Name = "problemTabPage";
      this.problemTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.problemTabPage.Size = new System.Drawing.Size(671, 374);
      this.problemTabPage.TabIndex = 1;
      this.problemTabPage.Text = "Problem";
      this.problemTabPage.UseVisualStyleBackColor = true;
      // 
      // problemPanel
      // 
      this.problemPanel.AllowDrop = true;
      this.problemPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.problemPanel.Controls.Add(this.problemViewHost);
      this.problemPanel.Location = new System.Drawing.Point(6, 36);
      this.problemPanel.Name = "problemPanel";
      this.problemPanel.Size = new System.Drawing.Size(659, 332);
      this.problemPanel.TabIndex = 3;
      this.problemPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.problemPanel_DragEnterOver);
      this.problemPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.problemPanel_DragDrop);
      this.problemPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.problemPanel_DragEnterOver);
      // 
      // problemViewHost
      // 
      this.problemViewHost.Content = null;
      this.problemViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.problemViewHost.Location = new System.Drawing.Point(0, 0);
      this.problemViewHost.Name = "problemViewHost";
      this.problemViewHost.Size = new System.Drawing.Size(659, 332);
      this.problemViewHost.TabIndex = 0;
      this.problemViewHost.ViewType = null;
      // 
      // saveProblemButton
      // 
      this.saveProblemButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Save;
      this.saveProblemButton.Location = new System.Drawing.Point(66, 6);
      this.saveProblemButton.Name = "saveProblemButton";
      this.saveProblemButton.Size = new System.Drawing.Size(24, 24);
      this.saveProblemButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.saveProblemButton, "Save Problem");
      this.saveProblemButton.UseVisualStyleBackColor = true;
      this.saveProblemButton.Click += new System.EventHandler(this.saveProblemButton_Click);
      // 
      // openProblemButton
      // 
      this.openProblemButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Open;
      this.openProblemButton.Location = new System.Drawing.Point(36, 6);
      this.openProblemButton.Name = "openProblemButton";
      this.openProblemButton.Size = new System.Drawing.Size(24, 24);
      this.openProblemButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.openProblemButton, "Open Problem");
      this.openProblemButton.UseVisualStyleBackColor = true;
      this.openProblemButton.Click += new System.EventHandler(this.openProblemButton_Click);
      // 
      // newProblemButton
      // 
      this.newProblemButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.NewDocument;
      this.newProblemButton.Location = new System.Drawing.Point(6, 6);
      this.newProblemButton.Name = "newProblemButton";
      this.newProblemButton.Size = new System.Drawing.Size(24, 24);
      this.newProblemButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newProblemButton, "New Problem");
      this.newProblemButton.UseVisualStyleBackColor = true;
      this.newProblemButton.Click += new System.EventHandler(this.newProblemButton_Click);
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Controls.Add(this.resultsView);
      this.resultsTabPage.Location = new System.Drawing.Point(4, 22);
      this.resultsTabPage.Name = "resultsTabPage";
      this.resultsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsTabPage.Size = new System.Drawing.Size(671, 374);
      this.resultsTabPage.TabIndex = 2;
      this.resultsTabPage.Text = "Results";
      this.resultsTabPage.UseVisualStyleBackColor = true;
      // 
      // resultsView
      // 
      this.resultsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.resultsView.Caption = "ResultsCollection";
      this.resultsView.Content = null;
      this.resultsView.Location = new System.Drawing.Point(6, 6);
      this.resultsView.Name = "resultsView";
      this.resultsView.Size = new System.Drawing.Size(659, 362);
      this.resultsView.TabIndex = 0;
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.runsView);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(671, 374);
      this.runsTabPage.TabIndex = 3;
      this.runsTabPage.Text = "Runs";
      this.runsTabPage.UseVisualStyleBackColor = true;
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
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Algorithm");
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // pauseButton
      // 
      this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.pauseButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Pause;
      this.pauseButton.Location = new System.Drawing.Point(30, 458);
      this.pauseButton.Name = "pauseButton";
      this.pauseButton.Size = new System.Drawing.Size(24, 24);
      this.pauseButton.TabIndex = 6;
      this.toolTip.SetToolTip(this.pauseButton, "Pause Algorithm");
      this.pauseButton.UseVisualStyleBackColor = true;
      this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
      // 
      // resetButton
      // 
      this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Restart;
      this.resetButton.Location = new System.Drawing.Point(90, 458);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(24, 24);
      this.resetButton.TabIndex = 8;
      this.toolTip.SetToolTip(this.resetButton, "Reset Algorithm");
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
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "Problem";
      this.openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      this.openFileDialog.Title = "Open Problem";
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.DefaultExt = "hl";
      this.saveFileDialog.FileName = "Problem";
      this.saveFileDialog.Filter = "Uncompressed HeuristicLab Files|*.hl|HeuristicLab Files|*.hl|All Files|*.*";
      this.saveFileDialog.FilterIndex = 2;
      this.saveFileDialog.Title = "Save Problem";
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Stop;
      this.stopButton.Location = new System.Drawing.Point(60, 458);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(24, 24);
      this.stopButton.TabIndex = 7;
      this.toolTip.SetToolTip(this.stopButton, "Stop Algorithm");
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // AlgorithmView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.pauseButton);
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.executionTimeLabel);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.resetButton);
      this.Name = "AlgorithmView";
      this.Size = new System.Drawing.Size(679, 482);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      this.problemPanel.ResumeLayout(false);
      this.resultsTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.TabControl tabControl;
    protected System.Windows.Forms.TabPage parametersTabPage;
    protected System.Windows.Forms.TabPage problemTabPage;
    protected HeuristicLab.Core.Views.ParameterCollectionView parameterCollectionView;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost problemViewHost;
    protected System.Windows.Forms.Button newProblemButton;
    protected System.Windows.Forms.Button saveProblemButton;
    protected System.Windows.Forms.Button openProblemButton;
    protected System.Windows.Forms.Button startButton;
    protected System.Windows.Forms.Button pauseButton;
    protected System.Windows.Forms.Button resetButton;
    protected System.Windows.Forms.Label executionTimeLabel;
    protected System.Windows.Forms.TextBox executionTimeTextBox;
    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    protected System.Windows.Forms.SaveFileDialog saveFileDialog;
    protected System.Windows.Forms.TabPage resultsTabPage;
    protected HeuristicLab.Optimization.Views.ResultCollectionView resultsView;
    protected System.Windows.Forms.Button stopButton;
    protected System.Windows.Forms.TabPage runsTabPage;
    protected RunCollectionView runsView;
    protected System.Windows.Forms.Panel problemPanel;

  }
}
