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

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  partial class RefreshableHiveJobView {
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
      this.components = new System.ComponentModel.Container();
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.tasksTabPage = new System.Windows.Forms.TabPage();
      this.jobsTreeView = new HeuristicLab.Clients.Hive.Views.HiveTaskItemTreeView();
      this.permissionTabPage = new System.Windows.Forms.TabPage();
      this.refreshPermissionsButton = new System.Windows.Forms.Button();
      this.hiveExperimentPermissionListView = new HeuristicLab.Clients.Hive.JobManager.Views.HiveJobPermissionListView();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.runCollectionViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.stateTabPage = new System.Windows.Forms.TabPage();
      this.stateLogViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.logTabPage = new System.Windows.Forms.TabPage();
      this.logView = new HeuristicLab.Core.Views.LogView();
      this.startButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.pauseButton = new System.Windows.Forms.Button();
      this.resourceIdsLabel = new System.Windows.Forms.Label();
      this.resourceNamesTextBox = new System.Windows.Forms.TextBox();
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.refreshButton = new System.Windows.Forms.Button();
      this.isPrivilegedCheckBox = new System.Windows.Forms.CheckBox();
      this.refreshAutomaticallyCheckBox = new System.Windows.Forms.CheckBox();
      this.infoGroupBox = new System.Windows.Forms.GroupBox();
      this.finishedTextBox = new System.Windows.Forms.TextBox();
      this.calculatingTextBox = new System.Windows.Forms.TextBox();
      this.jobsTextBox = new System.Windows.Forms.TextBox();
      this.finishedLabel = new System.Windows.Forms.Label();
      this.calculatingLabel = new System.Windows.Forms.Label();
      this.jobsLabel = new System.Windows.Forms.Label();
      this.searchButton = new System.Windows.Forms.Button();
      this.tabControl.SuspendLayout();
      this.tasksTabPage.SuspendLayout();
      this.permissionTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      this.stateTabPage.SuspendLayout();
      this.logTabPage.SuspendLayout();
      this.infoGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.tasksTabPage);
      this.tabControl.Controls.Add(this.permissionTabPage);
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Controls.Add(this.stateTabPage);
      this.tabControl.Controls.Add(this.logTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 78);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(717, 452);
      this.tabControl.TabIndex = 4;
      this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
      // 
      // tasksTabPage
      // 
      this.tasksTabPage.Controls.Add(this.jobsTreeView);
      this.tasksTabPage.Location = new System.Drawing.Point(4, 22);
      this.tasksTabPage.Name = "tasksTabPage";
      this.tasksTabPage.Size = new System.Drawing.Size(709, 426);
      this.tasksTabPage.TabIndex = 5;
      this.tasksTabPage.Text = "Tasks";
      this.tasksTabPage.UseVisualStyleBackColor = true;
      // 
      // jobsTreeView
      // 
      this.jobsTreeView.AllowDrop = true;
      this.jobsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.jobsTreeView.Caption = "ItemTree View";
      this.jobsTreeView.Content = null;
      this.jobsTreeView.Location = new System.Drawing.Point(2, 3);
      this.jobsTreeView.Name = "jobsTreeView";
      this.jobsTreeView.ReadOnly = false;
      this.jobsTreeView.Size = new System.Drawing.Size(704, 420);
      this.jobsTreeView.TabIndex = 4;
      this.jobsTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.jobsTreeView_DragDrop);
      this.jobsTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.jobsTreeView_DragEnter);
      this.jobsTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.jobsTreeView_DragOver);
      // 
      // permissionTabPage
      // 
      this.permissionTabPage.Controls.Add(this.refreshPermissionsButton);
      this.permissionTabPage.Controls.Add(this.hiveExperimentPermissionListView);
      this.permissionTabPage.Location = new System.Drawing.Point(4, 22);
      this.permissionTabPage.Name = "permissionTabPage";
      this.permissionTabPage.Size = new System.Drawing.Size(709, 426);
      this.permissionTabPage.TabIndex = 7;
      this.permissionTabPage.Text = "Permissions";
      this.permissionTabPage.UseVisualStyleBackColor = true;
      // 
      // refreshPermissionsButton
      // 
      this.refreshPermissionsButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshPermissionsButton.Location = new System.Drawing.Point(3, 3);
      this.refreshPermissionsButton.Name = "refreshPermissionsButton";
      this.refreshPermissionsButton.Size = new System.Drawing.Size(24, 24);
      this.refreshPermissionsButton.TabIndex = 23;
      this.toolTip.SetToolTip(this.refreshPermissionsButton, "Refresh Data");
      this.refreshPermissionsButton.UseVisualStyleBackColor = true;
      this.refreshPermissionsButton.Click += new System.EventHandler(this.refreshPermissionsButton_Click);
      // 
      // hiveExperimentPermissionListView
      // 
      this.hiveExperimentPermissionListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.hiveExperimentPermissionListView.Caption = "HiveExperimentPermissionList View";
      this.hiveExperimentPermissionListView.Content = null;
      this.hiveExperimentPermissionListView.HiveExperimentId = new System.Guid("00000000-0000-0000-0000-000000000000");
      this.hiveExperimentPermissionListView.Location = new System.Drawing.Point(3, 33);
      this.hiveExperimentPermissionListView.Name = "hiveExperimentPermissionListView";
      this.hiveExperimentPermissionListView.ReadOnly = false;
      this.hiveExperimentPermissionListView.ShowDetails = true;
      this.hiveExperimentPermissionListView.Size = new System.Drawing.Size(703, 390);
      this.hiveExperimentPermissionListView.TabIndex = 0;
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.runCollectionViewHost);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(709, 426);
      this.runsTabPage.TabIndex = 8;
      this.runsTabPage.Text = "Runs";
      this.runsTabPage.UseVisualStyleBackColor = true;
      // 
      // runCollectionViewHost
      // 
      this.runCollectionViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.runCollectionViewHost.Caption = "View";
      this.runCollectionViewHost.Content = null;
      this.runCollectionViewHost.Enabled = false;
      this.runCollectionViewHost.Location = new System.Drawing.Point(3, 3);
      this.runCollectionViewHost.Name = "runCollectionViewHost";
      this.runCollectionViewHost.ReadOnly = false;
      this.runCollectionViewHost.Size = new System.Drawing.Size(703, 420);
      this.runCollectionViewHost.TabIndex = 2;
      this.runCollectionViewHost.ViewsLabelVisible = true;
      this.runCollectionViewHost.ViewType = null;
      // 
      // stateTabPage
      // 
      this.stateTabPage.Controls.Add(this.stateLogViewHost);
      this.stateTabPage.Location = new System.Drawing.Point(4, 22);
      this.stateTabPage.Name = "stateTabPage";
      this.stateTabPage.Size = new System.Drawing.Size(709, 426);
      this.stateTabPage.TabIndex = 6;
      this.stateTabPage.Text = "Execution History";
      this.stateTabPage.UseVisualStyleBackColor = true;
      // 
      // stateLogViewHost
      // 
      this.stateLogViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stateLogViewHost.Caption = "StateLog View";
      this.stateLogViewHost.Content = null;
      this.stateLogViewHost.Enabled = false;
      this.stateLogViewHost.Location = new System.Drawing.Point(3, 3);
      this.stateLogViewHost.Name = "stateLogViewHost";
      this.stateLogViewHost.ReadOnly = false;
      this.stateLogViewHost.Size = new System.Drawing.Size(703, 420);
      this.stateLogViewHost.TabIndex = 0;
      this.stateLogViewHost.ViewsLabelVisible = true;
      this.stateLogViewHost.ViewType = null;
      // 
      // logTabPage
      // 
      this.logTabPage.Controls.Add(this.logView);
      this.logTabPage.Location = new System.Drawing.Point(4, 22);
      this.logTabPage.Name = "logTabPage";
      this.logTabPage.Size = new System.Drawing.Size(709, 426);
      this.logTabPage.TabIndex = 3;
      this.logTabPage.Text = "Log";
      this.logTabPage.UseVisualStyleBackColor = true;
      // 
      // logView
      // 
      this.logView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.logView.Caption = "Log View";
      this.logView.Content = null;
      this.logView.Location = new System.Drawing.Point(3, 3);
      this.logView.Name = "logView";
      this.logView.ReadOnly = false;
      this.logView.Size = new System.Drawing.Size(703, 420);
      this.logView.TabIndex = 0;
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.startButton.Location = new System.Drawing.Point(0, 536);
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
      this.stopButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Stop;
      this.stopButton.Location = new System.Drawing.Point(60, 536);
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
      this.resetButton.Enabled = false;
      this.resetButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Restart;
      this.resetButton.Location = new System.Drawing.Point(90, 536);
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
      this.executionTimeLabel.Location = new System.Drawing.Point(491, 543);
      this.executionTimeLabel.Name = "executionTimeLabel";
      this.executionTimeLabel.Size = new System.Drawing.Size(83, 13);
      this.executionTimeLabel.TabIndex = 9;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(580, 540);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(137, 20);
      this.executionTimeTextBox.TabIndex = 10;
      // 
      // pauseButton
      // 
      this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.pauseButton.Enabled = false;
      this.pauseButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Pause;
      this.pauseButton.Location = new System.Drawing.Point(30, 536);
      this.pauseButton.Name = "pauseButton";
      this.pauseButton.Size = new System.Drawing.Size(24, 24);
      this.pauseButton.TabIndex = 6;
      this.toolTip.SetToolTip(this.pauseButton, "Pause Experiment");
      this.pauseButton.UseVisualStyleBackColor = true;
      this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
      // 
      // resourceIdsLabel
      // 
      this.resourceIdsLabel.AutoSize = true;
      this.resourceIdsLabel.Location = new System.Drawing.Point(3, 56);
      this.resourceIdsLabel.Name = "resourceIdsLabel";
      this.resourceIdsLabel.Size = new System.Drawing.Size(61, 13);
      this.resourceIdsLabel.TabIndex = 12;
      this.resourceIdsLabel.Text = "Resources:";
      // 
      // resourceNamesTextBox
      // 
      this.resourceNamesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.resourceNamesTextBox.Location = new System.Drawing.Point(70, 52);
      this.resourceNamesTextBox.Name = "resourceNamesTextBox";
      this.resourceNamesTextBox.Size = new System.Drawing.Size(385, 20);
      this.resourceNamesTextBox.TabIndex = 2;
      this.resourceNamesTextBox.Validated += new System.EventHandler(this.resourceNamesTextBox_Validated);
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(3, 30);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 20;
      this.nameLabel.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(70, 27);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(501, 20);
      this.nameTextBox.TabIndex = 1;
      this.nameTextBox.Validated += new System.EventHandler(this.nameTextBox_Validated);
      // 
      // refreshButton
      // 
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(3, 0);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 22;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh Data");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // isPrivilegedCheckBox
      // 
      this.isPrivilegedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.isPrivilegedCheckBox.AutoSize = true;
      this.isPrivilegedCheckBox.Location = new System.Drawing.Point(491, 54);
      this.isPrivilegedCheckBox.Name = "isPrivilegedCheckBox";
      this.isPrivilegedCheckBox.Size = new System.Drawing.Size(72, 17);
      this.isPrivilegedCheckBox.TabIndex = 3;
      this.isPrivilegedCheckBox.Text = "Privileged";
      this.toolTip.SetToolTip(this.isPrivilegedCheckBox, "If checked, the task will be executed in a privileged sandbox on the slave.");
      this.isPrivilegedCheckBox.UseVisualStyleBackColor = true;
      this.isPrivilegedCheckBox.CheckedChanged += new System.EventHandler(this.isPrivilegedCheckBox_CheckChanged);
      // 
      // refreshAutomaticallyCheckBox
      // 
      this.refreshAutomaticallyCheckBox.AutoSize = true;
      this.refreshAutomaticallyCheckBox.Location = new System.Drawing.Point(30, 5);
      this.refreshAutomaticallyCheckBox.Name = "refreshAutomaticallyCheckBox";
      this.refreshAutomaticallyCheckBox.Size = new System.Drawing.Size(127, 17);
      this.refreshAutomaticallyCheckBox.TabIndex = 24;
      this.refreshAutomaticallyCheckBox.Text = "&Refresh automatically";
      this.refreshAutomaticallyCheckBox.UseVisualStyleBackColor = true;
      this.refreshAutomaticallyCheckBox.CheckedChanged += new System.EventHandler(this.refreshAutomaticallyCheckBox_CheckedChanged);
      // 
      // infoGroupBox
      // 
      this.infoGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.infoGroupBox.Controls.Add(this.finishedTextBox);
      this.infoGroupBox.Controls.Add(this.calculatingTextBox);
      this.infoGroupBox.Controls.Add(this.jobsTextBox);
      this.infoGroupBox.Controls.Add(this.finishedLabel);
      this.infoGroupBox.Controls.Add(this.calculatingLabel);
      this.infoGroupBox.Controls.Add(this.jobsLabel);
      this.infoGroupBox.Location = new System.Drawing.Point(580, 5);
      this.infoGroupBox.Name = "infoGroupBox";
      this.infoGroupBox.Size = new System.Drawing.Size(133, 89);
      this.infoGroupBox.TabIndex = 25;
      this.infoGroupBox.TabStop = false;
      this.infoGroupBox.Text = "Tasks";
      // 
      // finishedTextBox
      // 
      this.finishedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.finishedTextBox.Location = new System.Drawing.Point(74, 64);
      this.finishedTextBox.Name = "finishedTextBox";
      this.finishedTextBox.Size = new System.Drawing.Size(53, 20);
      this.finishedTextBox.TabIndex = 5;
      // 
      // calculatingTextBox
      // 
      this.calculatingTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.calculatingTextBox.Location = new System.Drawing.Point(74, 40);
      this.calculatingTextBox.Name = "calculatingTextBox";
      this.calculatingTextBox.Size = new System.Drawing.Size(53, 20);
      this.calculatingTextBox.TabIndex = 4;
      // 
      // jobsTextBox
      // 
      this.jobsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.jobsTextBox.Location = new System.Drawing.Point(74, 16);
      this.jobsTextBox.Name = "jobsTextBox";
      this.jobsTextBox.Size = new System.Drawing.Size(53, 20);
      this.jobsTextBox.TabIndex = 3;
      // 
      // finishedLabel
      // 
      this.finishedLabel.AutoSize = true;
      this.finishedLabel.Location = new System.Drawing.Point(6, 67);
      this.finishedLabel.Name = "finishedLabel";
      this.finishedLabel.Size = new System.Drawing.Size(49, 13);
      this.finishedLabel.TabIndex = 2;
      this.finishedLabel.Text = "Finished:";
      // 
      // calculatingLabel
      // 
      this.calculatingLabel.AutoSize = true;
      this.calculatingLabel.Location = new System.Drawing.Point(6, 43);
      this.calculatingLabel.Name = "calculatingLabel";
      this.calculatingLabel.Size = new System.Drawing.Size(62, 13);
      this.calculatingLabel.TabIndex = 1;
      this.calculatingLabel.Text = "Calculating:";
      // 
      // jobsLabel
      // 
      this.jobsLabel.AutoSize = true;
      this.jobsLabel.Location = new System.Drawing.Point(6, 19);
      this.jobsLabel.Name = "jobsLabel";
      this.jobsLabel.Size = new System.Drawing.Size(46, 13);
      this.jobsLabel.TabIndex = 0;
      this.jobsLabel.Text = "Waiting:";
      // 
      // searchButton
      // 
      this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.searchButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Zoom;
      this.searchButton.Location = new System.Drawing.Point(461, 50);
      this.searchButton.Name = "searchButton";
      this.searchButton.Size = new System.Drawing.Size(24, 24);
      this.searchButton.TabIndex = 26;
      this.searchButton.UseVisualStyleBackColor = true;
      this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
      // 
      // RefreshableHiveJobView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.searchButton);
      this.Controls.Add(this.isPrivilegedCheckBox);
      this.Controls.Add(this.infoGroupBox);
      this.Controls.Add(this.refreshAutomaticallyCheckBox);
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.nameLabel);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.executionTimeLabel);
      this.Controls.Add(this.resourceNamesTextBox);
      this.Controls.Add(this.pauseButton);
      this.Controls.Add(this.resourceIdsLabel);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.resetButton);
      this.Name = "RefreshableHiveJobView";
      this.Size = new System.Drawing.Size(717, 560);
      this.tabControl.ResumeLayout(false);
      this.tasksTabPage.ResumeLayout(false);
      this.permissionTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.stateTabPage.ResumeLayout(false);
      this.logTabPage.ResumeLayout(false);
      this.infoGroupBox.ResumeLayout(false);
      this.infoGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.Label executionTimeLabel;
    private System.Windows.Forms.TextBox executionTimeTextBox;
    private System.Windows.Forms.Button pauseButton;
    private System.Windows.Forms.Label resourceIdsLabel;
    private System.Windows.Forms.TextBox resourceNamesTextBox;
    private System.Windows.Forms.TabPage logTabPage;
    private Core.Views.LogView logView;
    private System.Windows.Forms.TabPage tasksTabPage;
    private HeuristicLab.Clients.Hive.Views.HiveTaskItemTreeView jobsTreeView;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.CheckBox refreshAutomaticallyCheckBox;
    private System.Windows.Forms.GroupBox infoGroupBox;
    private System.Windows.Forms.TextBox finishedTextBox;
    private System.Windows.Forms.TextBox calculatingTextBox;
    private System.Windows.Forms.TextBox jobsTextBox;
    private System.Windows.Forms.Label finishedLabel;
    private System.Windows.Forms.Label calculatingLabel;
    private System.Windows.Forms.Label jobsLabel;
    private System.Windows.Forms.CheckBox isPrivilegedCheckBox;
    private System.Windows.Forms.TabPage stateTabPage;
    private HeuristicLab.MainForm.WindowsForms.ViewHost stateLogViewHost;
    private System.Windows.Forms.TabPage permissionTabPage;
    private HiveJobPermissionListView hiveExperimentPermissionListView;
    private System.Windows.Forms.Button refreshPermissionsButton;
    private System.Windows.Forms.TabPage runsTabPage;
    private MainForm.WindowsForms.ViewHost runCollectionViewHost;
    private System.Windows.Forms.Button searchButton;

  }
}
