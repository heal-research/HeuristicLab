#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlgorithmView));
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.problemTabPage = new System.Windows.Forms.TabPage();
      this.problemViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.openProblemButton = new System.Windows.Forms.Button();
      this.newProblemButton = new System.Windows.Forms.Button();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.parameterCollectionView = new HeuristicLab.Core.Views.ParameterCollectionView();
      this.resultsTabPage = new System.Windows.Forms.TabPage();
      this.resultsView = new HeuristicLab.Optimization.Views.ResultCollectionView();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.storeAlgorithmInEachRunCheckBox = new System.Windows.Forms.CheckBox();
      this.runsView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // startButton
      // 
      this.startButton.Location = new System.Drawing.Point(0, 458);
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Algorithm");
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(542, 462);
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Location = new System.Drawing.Point(453, 465);
      // 
      // pauseButton
      // 
      this.pauseButton.Location = new System.Drawing.Point(30, 458);
      this.toolTip.SetToolTip(this.pauseButton, "Pause Algorithm");
      // 
      // stopButton
      // 
      this.stopButton.Location = new System.Drawing.Point(60, 458);
      this.toolTip.SetToolTip(this.stopButton, "Stop Algorithm");
      // 
      // resetButton
      // 
      this.resetButton.Location = new System.Drawing.Point(90, 458);
      this.toolTip.SetToolTip(this.resetButton, "Reset Algorithm");
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(60, 0);
      this.nameTextBox.Size = new System.Drawing.Size(594, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(660, 3);
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.problemTabPage);
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Controls.Add(this.resultsTabPage);
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 26);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(679, 430);
      this.tabControl.TabIndex = 3;
      // 
      // problemTabPage
      // 
      this.problemTabPage.AllowDrop = true;
      this.problemTabPage.Controls.Add(this.problemViewHost);
      this.problemTabPage.Controls.Add(this.openProblemButton);
      this.problemTabPage.Controls.Add(this.newProblemButton);
      this.problemTabPage.Location = new System.Drawing.Point(4, 22);
      this.problemTabPage.Name = "problemTabPage";
      this.problemTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.problemTabPage.Size = new System.Drawing.Size(671, 404);
      this.problemTabPage.TabIndex = 0;
      this.problemTabPage.Text = "Problem";
      this.problemTabPage.UseVisualStyleBackColor = true;
      this.problemTabPage.DragDrop += new System.Windows.Forms.DragEventHandler(this.problemTabPage_DragDrop);
      this.problemTabPage.DragEnter += new System.Windows.Forms.DragEventHandler(this.problemTabPage_DragEnterOver);
      this.problemTabPage.DragOver += new System.Windows.Forms.DragEventHandler(this.problemTabPage_DragEnterOver);
      // 
      // problemViewHost
      // 
      this.problemViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemViewHost.Caption = "View";
      this.problemViewHost.Content = null;
      this.problemViewHost.Enabled = false;
      this.problemViewHost.Location = new System.Drawing.Point(6, 36);
      this.problemViewHost.Name = "problemViewHost";
      this.problemViewHost.ReadOnly = false;
      this.problemViewHost.Size = new System.Drawing.Size(659, 362);
      this.problemViewHost.TabIndex = 3;
      this.problemViewHost.ViewsLabelVisible = true;
      this.problemViewHost.ViewType = null;
      // 
      // openProblemButton
      // 
      this.openProblemButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
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
      this.newProblemButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.NewDocument;
      this.newProblemButton.Location = new System.Drawing.Point(6, 6);
      this.newProblemButton.Name = "newProblemButton";
      this.newProblemButton.Size = new System.Drawing.Size(24, 24);
      this.newProblemButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newProblemButton, "New Problem");
      this.newProblemButton.UseVisualStyleBackColor = true;
      this.newProblemButton.Click += new System.EventHandler(this.newProblemButton_Click);
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.parameterCollectionView);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(671, 400);
      this.parametersTabPage.TabIndex = 1;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.AllowEditingOfHiddenParameters = true;
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.parameterCollectionView.Caption = "ParameterCollection View";
      this.parameterCollectionView.Content = null;
      this.parameterCollectionView.Location = new System.Drawing.Point(6, 6);
      this.parameterCollectionView.Name = "parameterCollectionView";
      this.parameterCollectionView.ReadOnly = false;
      this.parameterCollectionView.Size = new System.Drawing.Size(659, 388);
      this.parameterCollectionView.TabIndex = 0;
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Controls.Add(this.resultsView);
      this.resultsTabPage.Location = new System.Drawing.Point(4, 22);
      this.resultsTabPage.Name = "resultsTabPage";
      this.resultsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsTabPage.Size = new System.Drawing.Size(671, 400);
      this.resultsTabPage.TabIndex = 2;
      this.resultsTabPage.Text = "Results";
      this.resultsTabPage.UseVisualStyleBackColor = true;
      // 
      // resultsView
      // 
      this.resultsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.resultsView.Caption = "ResultCollection View";
      this.resultsView.Content = null;
      this.resultsView.Location = new System.Drawing.Point(6, 6);
      this.resultsView.Name = "resultsView";
      this.resultsView.ReadOnly = true;
      this.resultsView.Size = new System.Drawing.Size(659, 388);
      this.resultsView.TabIndex = 0;
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.storeAlgorithmInEachRunCheckBox);
      this.runsTabPage.Controls.Add(this.runsView);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(671, 400);
      this.runsTabPage.TabIndex = 3;
      this.runsTabPage.Text = "Runs";
      this.runsTabPage.UseVisualStyleBackColor = true;
      // 
      // storeAlgorithmInEachRunCheckBox
      // 
      this.storeAlgorithmInEachRunCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.storeAlgorithmInEachRunCheckBox.AutoSize = true;
      this.storeAlgorithmInEachRunCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.storeAlgorithmInEachRunCheckBox.Checked = true;
      this.storeAlgorithmInEachRunCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.storeAlgorithmInEachRunCheckBox.Location = new System.Drawing.Point(503, 6);
      this.storeAlgorithmInEachRunCheckBox.Name = "storeAlgorithmInEachRunCheckBox";
      this.storeAlgorithmInEachRunCheckBox.Size = new System.Drawing.Size(161, 17);
      this.storeAlgorithmInEachRunCheckBox.TabIndex = 1;
      this.storeAlgorithmInEachRunCheckBox.Text = "&Store Algorithm in each Run:";
      this.toolTip.SetToolTip(this.storeAlgorithmInEachRunCheckBox, "Check to store a copy of the algorithm in each run.");
      this.storeAlgorithmInEachRunCheckBox.UseVisualStyleBackColor = true;
      this.storeAlgorithmInEachRunCheckBox.CheckedChanged += new System.EventHandler(this.storeAlgorithmInEachRunCheckBox_CheckedChanged);
      // 
      // runsView
      // 
      this.runsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.runsView.Caption = "RunCollection View";
      this.runsView.Content = null;
      this.runsView.Location = new System.Drawing.Point(6, 6);
      this.runsView.Name = "runsView";
      this.runsView.ReadOnly = false;
      this.runsView.Size = new System.Drawing.Size(659, 388);
      this.runsView.TabIndex = 0;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "Problem";
      this.openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      this.openFileDialog.Title = "Open Problem";
      // 
      // AlgorithmView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabControl);
      this.Name = "AlgorithmView";
      this.Size = new System.Drawing.Size(679, 482);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.resultsTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.runsTabPage.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    protected System.Windows.Forms.TabPage parametersTabPage;
    protected System.Windows.Forms.TabPage problemTabPage;
    protected HeuristicLab.Core.Views.ParameterCollectionView parameterCollectionView;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost problemViewHost;
    protected System.Windows.Forms.Button newProblemButton;
    protected System.Windows.Forms.Button openProblemButton;
    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    protected System.Windows.Forms.TabPage resultsTabPage;
    protected HeuristicLab.Optimization.Views.ResultCollectionView resultsView;
    protected System.Windows.Forms.TabPage runsTabPage;
    protected RunCollectionView runsView;
    protected System.Windows.Forms.CheckBox storeAlgorithmInEachRunCheckBox;
  }
}
