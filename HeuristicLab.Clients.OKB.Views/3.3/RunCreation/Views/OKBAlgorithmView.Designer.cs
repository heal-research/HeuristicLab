#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Optimization.Views;
namespace HeuristicLab.Clients.OKB.RunCreation {
  partial class OKBAlgorithmView {
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
      this.startButton = new System.Windows.Forms.Button();
      this.pauseButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.stopButton = new System.Windows.Forms.Button();
      this.algorithmComboBox = new System.Windows.Forms.ComboBox();
      this.algorithmLabel = new System.Windows.Forms.Label();
      this.refreshButton = new System.Windows.Forms.Button();
      this.cloneAlgorithmButton = new System.Windows.Forms.Button();
      this.storeRunsAutomaticallyCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(72, 29);
      this.nameTextBox.Size = new System.Drawing.Size(577, 20);
      this.nameTextBox.TabIndex = 5;
      // 
      // nameLabel
      // 
      this.nameLabel.Location = new System.Drawing.Point(3, 32);
      this.nameLabel.TabIndex = 4;
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(659, 32);
      this.infoLabel.TabIndex = 6;
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
      this.tabControl.Location = new System.Drawing.Point(0, 55);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(679, 397);
      this.tabControl.TabIndex = 7;
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
      this.problemTabPage.Size = new System.Drawing.Size(671, 371);
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
      this.problemViewHost.Size = new System.Drawing.Size(659, 329);
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
      this.parametersTabPage.Size = new System.Drawing.Size(671, 371);
      this.parametersTabPage.TabIndex = 1;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.AllowEditingOfHiddenParameters = false;
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parameterCollectionView.Caption = "ParameterCollection View";
      this.parameterCollectionView.Content = null;
      this.parameterCollectionView.Location = new System.Drawing.Point(6, 6);
      this.parameterCollectionView.Name = "parameterCollectionView";
      this.parameterCollectionView.ReadOnly = false;
      this.parameterCollectionView.Size = new System.Drawing.Size(659, 359);
      this.parameterCollectionView.TabIndex = 0;
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Controls.Add(this.resultsView);
      this.resultsTabPage.Location = new System.Drawing.Point(4, 22);
      this.resultsTabPage.Name = "resultsTabPage";
      this.resultsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsTabPage.Size = new System.Drawing.Size(671, 371);
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
      this.resultsView.Size = new System.Drawing.Size(659, 359);
      this.resultsView.TabIndex = 0;
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.storeRunsAutomaticallyCheckBox);
      this.runsTabPage.Controls.Add(this.storeAlgorithmInEachRunCheckBox);
      this.runsTabPage.Controls.Add(this.runsView);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(671, 371);
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
      this.storeAlgorithmInEachRunCheckBox.TabIndex = 2;
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
      this.runsView.Size = new System.Drawing.Size(659, 359);
      this.runsView.TabIndex = 0;
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.startButton.Location = new System.Drawing.Point(0, 458);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(24, 24);
      this.startButton.TabIndex = 8;
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Algorithm");
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // pauseButton
      // 
      this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.pauseButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Pause;
      this.pauseButton.Location = new System.Drawing.Point(30, 458);
      this.pauseButton.Name = "pauseButton";
      this.pauseButton.Size = new System.Drawing.Size(24, 24);
      this.pauseButton.TabIndex = 9;
      this.toolTip.SetToolTip(this.pauseButton, "Pause Algorithm");
      this.pauseButton.UseVisualStyleBackColor = true;
      this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
      // 
      // resetButton
      // 
      this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Restart;
      this.resetButton.Location = new System.Drawing.Point(90, 458);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(24, 24);
      this.resetButton.TabIndex = 11;
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
      this.executionTimeLabel.TabIndex = 12;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(542, 462);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(137, 20);
      this.executionTimeTextBox.TabIndex = 13;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "Problem";
      this.openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      this.openFileDialog.Title = "Open Problem";
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Stop;
      this.stopButton.Location = new System.Drawing.Point(60, 458);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(24, 24);
      this.stopButton.TabIndex = 10;
      this.toolTip.SetToolTip(this.stopButton, "Stop Algorithm");
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // algorithmComboBox
      // 
      this.algorithmComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.algorithmComboBox.FormattingEnabled = true;
      this.algorithmComboBox.Location = new System.Drawing.Point(72, 0);
      this.algorithmComboBox.Name = "algorithmComboBox";
      this.algorithmComboBox.Size = new System.Drawing.Size(547, 21);
      this.algorithmComboBox.TabIndex = 1;
      this.algorithmComboBox.SelectedValueChanged += new System.EventHandler(this.algorithmComboBox_SelectedValueChanged);
      // 
      // algorithmLabel
      // 
      this.algorithmLabel.AutoSize = true;
      this.algorithmLabel.Location = new System.Drawing.Point(3, 5);
      this.algorithmLabel.Name = "algorithmLabel";
      this.algorithmLabel.Size = new System.Drawing.Size(53, 13);
      this.algorithmLabel.TabIndex = 0;
      this.algorithmLabel.Text = "&Algorithm:";
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(655, -1);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh Algorithms");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // cloneAlgorithmButton
      // 
      this.cloneAlgorithmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cloneAlgorithmButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Clone;
      this.cloneAlgorithmButton.Location = new System.Drawing.Point(625, -1);
      this.cloneAlgorithmButton.Name = "cloneAlgorithmButton";
      this.cloneAlgorithmButton.Size = new System.Drawing.Size(24, 24);
      this.cloneAlgorithmButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.cloneAlgorithmButton, "Clone Algorithm");
      this.cloneAlgorithmButton.UseVisualStyleBackColor = true;
      this.cloneAlgorithmButton.Click += new System.EventHandler(this.cloneAlgorithmButton_Click);
      // 
      // storeRunsAutomaticallyCheckBox
      // 
      this.storeRunsAutomaticallyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.storeRunsAutomaticallyCheckBox.AutoSize = true;
      this.storeRunsAutomaticallyCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.storeRunsAutomaticallyCheckBox.Checked = true;
      this.storeRunsAutomaticallyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.storeRunsAutomaticallyCheckBox.Location = new System.Drawing.Point(318, 6);
      this.storeRunsAutomaticallyCheckBox.Name = "storeRunsAutomaticallyCheckBox";
      this.storeRunsAutomaticallyCheckBox.Size = new System.Drawing.Size(147, 17);
      this.storeRunsAutomaticallyCheckBox.TabIndex = 1;
      this.storeRunsAutomaticallyCheckBox.Text = "Store Runs &Automatically:";
      this.toolTip.SetToolTip(this.storeRunsAutomaticallyCheckBox, "Store finished runs automatically in the OKB.");
      this.storeRunsAutomaticallyCheckBox.UseVisualStyleBackColor = true;
      this.storeRunsAutomaticallyCheckBox.CheckedChanged += new System.EventHandler(this.storeRunsAutomaticallyCheckBox_CheckedChanged);
      // 
      // OKBAlgorithmView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.algorithmComboBox);
      this.Controls.Add(this.algorithmLabel);
      this.Controls.Add(this.cloneAlgorithmButton);
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.pauseButton);
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.executionTimeLabel);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.resetButton);
      this.Name = "OKBAlgorithmView";
      this.Size = new System.Drawing.Size(679, 482);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.refreshButton, 0);
      this.Controls.SetChildIndex(this.cloneAlgorithmButton, 0);
      this.Controls.SetChildIndex(this.algorithmLabel, 0);
      this.Controls.SetChildIndex(this.algorithmComboBox, 0);
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

    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage parametersTabPage;
    private System.Windows.Forms.TabPage problemTabPage;
    private HeuristicLab.Core.Views.ParameterCollectionView parameterCollectionView;
    private HeuristicLab.MainForm.WindowsForms.ViewHost problemViewHost;
    private System.Windows.Forms.Button newProblemButton;
    private System.Windows.Forms.Button openProblemButton;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Button pauseButton;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.Label executionTimeLabel;
    private System.Windows.Forms.TextBox executionTimeTextBox;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabPage resultsTabPage;
    private HeuristicLab.Optimization.Views.ResultCollectionView resultsView;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.TabPage runsTabPage;
    private RunCollectionView runsView;
    private System.Windows.Forms.CheckBox storeAlgorithmInEachRunCheckBox;
    private System.Windows.Forms.ComboBox algorithmComboBox;
    private System.Windows.Forms.Label algorithmLabel;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button cloneAlgorithmButton;
    private System.Windows.Forms.CheckBox storeRunsAutomaticallyCheckBox;
  }
}
