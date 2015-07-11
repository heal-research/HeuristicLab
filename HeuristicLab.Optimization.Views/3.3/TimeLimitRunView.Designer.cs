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

namespace HeuristicLab.Optimization.Views {
  partial class TimeLimitRunView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeLimitRunView));
      this.timeLimitLabel = new System.Windows.Forms.Label();
      this.timeLimitTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.snapshotsTextBox = new System.Windows.Forms.TextBox();
      this.storeAlgorithmInEachSnapshotCheckBox = new System.Windows.Forms.CheckBox();
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.algorithmTabPage = new System.Windows.Forms.TabPage();
      this.algorithmViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.openAlgorithmButton = new System.Windows.Forms.Button();
      this.newAlgorithmButton = new System.Windows.Forms.Button();
      this.snapshotsTabPage = new System.Windows.Forms.TabPage();
      this.snapshotsView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.runsView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.snapshotButton = new System.Windows.Forms.Button();
      this.sequenceButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.algorithmTabPage.SuspendLayout();
      this.snapshotsTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // startButton
      // 
      this.startButton.Location = new System.Drawing.Point(0, 440);
      this.startButton.TabIndex = 9;
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Optimizer");
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(412, 444);
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Location = new System.Drawing.Point(323, 447);
      // 
      // pauseButton
      // 
      this.pauseButton.Location = new System.Drawing.Point(30, 440);
      this.pauseButton.TabIndex = 10;
      this.toolTip.SetToolTip(this.pauseButton, "Pause Optimizer");
      // 
      // stopButton
      // 
      this.stopButton.Location = new System.Drawing.Point(60, 440);
      this.stopButton.TabIndex = 11;
      this.toolTip.SetToolTip(this.stopButton, "Stop Optimizer");
      // 
      // resetButton
      // 
      this.resetButton.Location = new System.Drawing.Point(90, 440);
      this.resetButton.TabIndex = 12;
      this.toolTip.SetToolTip(this.resetButton, "Reset Optimizer");
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(69, 0);
      this.nameTextBox.Size = new System.Drawing.Size(455, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(530, 3);
      // 
      // timeLimitLabel
      // 
      this.timeLimitLabel.AutoSize = true;
      this.timeLimitLabel.Location = new System.Drawing.Point(3, 29);
      this.timeLimitLabel.Name = "timeLimitLabel";
      this.timeLimitLabel.Size = new System.Drawing.Size(53, 13);
      this.timeLimitLabel.TabIndex = 3;
      this.timeLimitLabel.Text = "Time limit:";
      // 
      // timeLimitTextBox
      // 
      this.timeLimitTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.timeLimitTextBox.Location = new System.Drawing.Point(69, 26);
      this.timeLimitTextBox.Name = "timeLimitTextBox";
      this.timeLimitTextBox.Size = new System.Drawing.Size(455, 20);
      this.timeLimitTextBox.TabIndex = 4;
      this.timeLimitTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.timeLimitTextBox_Validating);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 55);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(60, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Snapshots:";
      // 
      // snapshotsTextBox
      // 
      this.snapshotsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.snapshotsTextBox.Location = new System.Drawing.Point(69, 52);
      this.snapshotsTextBox.Name = "snapshotsTextBox";
      this.snapshotsTextBox.Size = new System.Drawing.Size(175, 20);
      this.snapshotsTextBox.TabIndex = 6;
      this.snapshotsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.snapshotsTextBox_Validating);
      // 
      // storeAlgorithmInEachSnapshotCheckBox
      // 
      this.storeAlgorithmInEachSnapshotCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.storeAlgorithmInEachSnapshotCheckBox.AutoSize = true;
      this.storeAlgorithmInEachSnapshotCheckBox.Location = new System.Drawing.Point(340, 54);
      this.storeAlgorithmInEachSnapshotCheckBox.Name = "storeAlgorithmInEachSnapshotCheckBox";
      this.storeAlgorithmInEachSnapshotCheckBox.Size = new System.Drawing.Size(184, 17);
      this.storeAlgorithmInEachSnapshotCheckBox.TabIndex = 7;
      this.storeAlgorithmInEachSnapshotCheckBox.Text = "Store Algorithm in Each Snapshot";
      this.storeAlgorithmInEachSnapshotCheckBox.UseVisualStyleBackColor = true;
      this.storeAlgorithmInEachSnapshotCheckBox.CheckedChanged += new System.EventHandler(this.storeAlgorithmInEachSnapshotCheckBox_CheckedChanged);
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.algorithmTabPage);
      this.tabControl.Controls.Add(this.snapshotsTabPage);
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 78);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(546, 356);
      this.tabControl.TabIndex = 8;
      // 
      // algorithmTabPage
      // 
      this.algorithmTabPage.AllowDrop = true;
      this.algorithmTabPage.Controls.Add(this.algorithmViewHost);
      this.algorithmTabPage.Controls.Add(this.openAlgorithmButton);
      this.algorithmTabPage.Controls.Add(this.newAlgorithmButton);
      this.algorithmTabPage.Location = new System.Drawing.Point(4, 22);
      this.algorithmTabPage.Name = "algorithmTabPage";
      this.algorithmTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.algorithmTabPage.Size = new System.Drawing.Size(538, 330);
      this.algorithmTabPage.TabIndex = 1;
      this.algorithmTabPage.Text = "Algorithm";
      this.algorithmTabPage.UseVisualStyleBackColor = true;
      this.algorithmTabPage.DragDrop += new System.Windows.Forms.DragEventHandler(this.algorithmTabPage_DragDrop);
      this.algorithmTabPage.DragEnter += new System.Windows.Forms.DragEventHandler(this.algorithmTabPage_DragEnterOver);
      this.algorithmTabPage.DragOver += new System.Windows.Forms.DragEventHandler(this.algorithmTabPage_DragEnterOver);
      // 
      // algorithmViewHost
      // 
      this.algorithmViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmViewHost.Caption = "View";
      this.algorithmViewHost.Content = null;
      this.algorithmViewHost.Enabled = false;
      this.algorithmViewHost.Location = new System.Drawing.Point(6, 36);
      this.algorithmViewHost.Name = "algorithmViewHost";
      this.algorithmViewHost.ReadOnly = false;
      this.algorithmViewHost.Size = new System.Drawing.Size(526, 288);
      this.algorithmViewHost.TabIndex = 2;
      this.algorithmViewHost.ViewsLabelVisible = true;
      this.algorithmViewHost.ViewType = null;
      // 
      // openAlgorithmButton
      // 
      this.openAlgorithmButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.openAlgorithmButton.Location = new System.Drawing.Point(36, 6);
      this.openAlgorithmButton.Name = "openAlgorithmButton";
      this.openAlgorithmButton.Size = new System.Drawing.Size(24, 24);
      this.openAlgorithmButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.openAlgorithmButton, "Open Optimizer");
      this.openAlgorithmButton.UseVisualStyleBackColor = true;
      this.openAlgorithmButton.Click += new System.EventHandler(this.openAlgorithmButton_Click);
      // 
      // newAlgorithmButton
      // 
      this.newAlgorithmButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.NewDocument;
      this.newAlgorithmButton.Location = new System.Drawing.Point(6, 6);
      this.newAlgorithmButton.Name = "newAlgorithmButton";
      this.newAlgorithmButton.Size = new System.Drawing.Size(24, 24);
      this.newAlgorithmButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newAlgorithmButton, "New Optimizer");
      this.newAlgorithmButton.UseVisualStyleBackColor = true;
      this.newAlgorithmButton.Click += new System.EventHandler(this.newAlgorithmButton_Click);
      // 
      // snapshotsTabPage
      // 
      this.snapshotsTabPage.Controls.Add(this.snapshotsView);
      this.snapshotsTabPage.Location = new System.Drawing.Point(4, 22);
      this.snapshotsTabPage.Name = "snapshotsTabPage";
      this.snapshotsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.snapshotsTabPage.Size = new System.Drawing.Size(538, 330);
      this.snapshotsTabPage.TabIndex = 2;
      this.snapshotsTabPage.Text = "Snapshots";
      this.snapshotsTabPage.UseVisualStyleBackColor = true;
      // 
      // snapshotsView
      // 
      this.snapshotsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.snapshotsView.Caption = "RunCollection View";
      this.snapshotsView.Content = null;
      this.snapshotsView.Location = new System.Drawing.Point(6, 6);
      this.snapshotsView.Name = "snapshotsView";
      this.snapshotsView.ReadOnly = false;
      this.snapshotsView.Size = new System.Drawing.Size(526, 317);
      this.snapshotsView.TabIndex = 0;
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.runsView);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Size = new System.Drawing.Size(538, 330);
      this.runsTabPage.TabIndex = 3;
      this.runsTabPage.Text = "Runs";
      this.runsTabPage.UseVisualStyleBackColor = true;
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
      this.runsView.Size = new System.Drawing.Size(526, 317);
      this.runsView.TabIndex = 1;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "Optimizer";
      this.openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      this.openFileDialog.Title = "Open Optimizer";
      // 
      // snapshotButton
      // 
      this.snapshotButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.snapshotButton.Location = new System.Drawing.Point(120, 440);
      this.snapshotButton.Name = "snapshotButton";
      this.snapshotButton.Size = new System.Drawing.Size(24, 24);
      this.snapshotButton.TabIndex = 13;
      this.snapshotButton.Text = "Snapshot";
      this.toolTip.SetToolTip(this.snapshotButton, "Create Snapshot");
      this.snapshotButton.UseVisualStyleBackColor = true;
      this.snapshotButton.Click += new System.EventHandler(this.snapshotButton_Click);
      // 
      // sequenceButton
      // 
      this.sequenceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.sequenceButton.Location = new System.Drawing.Point(250, 50);
      this.sequenceButton.Name = "sequenceButton";
      this.sequenceButton.Size = new System.Drawing.Size(75, 23);
      this.sequenceButton.TabIndex = 16;
      this.sequenceButton.Text = "Sequence...";
      this.sequenceButton.UseVisualStyleBackColor = true;
      this.sequenceButton.Click += new System.EventHandler(this.sequenceButton_Click);
      // 
      // TimeLimitRunView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.sequenceButton);
      this.Controls.Add(this.snapshotButton);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.timeLimitTextBox);
      this.Controls.Add(this.timeLimitLabel);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.snapshotsTextBox);
      this.Controls.Add(this.storeAlgorithmInEachSnapshotCheckBox);
      this.Name = "TimeLimitRunView";
      this.Size = new System.Drawing.Size(549, 464);
      this.Controls.SetChildIndex(this.storeAlgorithmInEachSnapshotCheckBox, 0);
      this.Controls.SetChildIndex(this.snapshotsTextBox, 0);
      this.Controls.SetChildIndex(this.label1, 0);
      this.Controls.SetChildIndex(this.timeLimitLabel, 0);
      this.Controls.SetChildIndex(this.timeLimitTextBox, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.snapshotButton, 0);
      this.Controls.SetChildIndex(this.sequenceButton, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.algorithmTabPage.ResumeLayout(false);
      this.snapshotsTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label timeLimitLabel;
    private System.Windows.Forms.TextBox timeLimitTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox snapshotsTextBox;
    private System.Windows.Forms.CheckBox storeAlgorithmInEachSnapshotCheckBox;
    private MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage algorithmTabPage;
    private MainForm.WindowsForms.ViewHost algorithmViewHost;
    private System.Windows.Forms.Button openAlgorithmButton;
    private System.Windows.Forms.Button newAlgorithmButton;
    private System.Windows.Forms.TabPage snapshotsTabPage;
    private RunCollectionView snapshotsView;
    private System.Windows.Forms.TabPage runsTabPage;
    private RunCollectionView runsView;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.Button snapshotButton;
    private System.Windows.Forms.Button sequenceButton;
  }
}
