#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.Views {
  partial class HiveTaskView {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.stateLogTabPage = new System.Windows.Forms.TabPage();
      this.stateLogViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.detailsTabPage = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.groupBoxGenerel = new System.Windows.Forms.GroupBox();
      this.priorityComboBox = new System.Windows.Forms.ComboBox();
      this.jobIdLabel = new System.Windows.Forms.Label();
      this.jobIdTextBox = new System.Windows.Forms.TextBox();
      this.lastUpdatedLabel = new System.Windows.Forms.Label();
      this.lastUpdatedTextBox = new System.Windows.Forms.TextBox();
      this.priorityLabel = new System.Windows.Forms.Label();
      this.configurationGroupBox = new System.Windows.Forms.GroupBox();
      this.memoryNeededComboBox = new System.Windows.Forms.ComboBox();
      this.coresNeededComboBox = new System.Windows.Forms.ComboBox();
      this.memoryNeededLabel = new System.Windows.Forms.Label();
      this.coresNeededLabel = new System.Windows.Forms.Label();
      this.computeInParallelLabel = new System.Windows.Forms.Label();
      this.computeInParallelCheckBox = new System.Windows.Forms.CheckBox();
      this.jobStatusGroupBox = new System.Windows.Forms.GroupBox();
      this.commandTextBox = new System.Windows.Forms.TextBox();
      this.commandLabel = new System.Windows.Forms.Label();
      this.stateTextBox = new System.Windows.Forms.TextBox();
      this.dateCalculatedLabel = new System.Windows.Forms.Label();
      this.stateLabel = new System.Windows.Forms.Label();
      this.dateCalculatedText = new System.Windows.Forms.TextBox();
      this.dateFinishedTextBox = new System.Windows.Forms.TextBox();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.dateCreatedTextBox = new System.Windows.Forms.TextBox();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.exceptionLabel = new System.Windows.Forms.Label();
      this.dateCreatedLabel = new System.Windows.Forms.Label();
      this.dateFinishedLabel = new System.Windows.Forms.Label();
      this.exceptionTextBox = new System.Windows.Forms.TextBox();
      this.modifyItemButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.stateLogTabPage.SuspendLayout();
      this.detailsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.groupBoxGenerel.SuspendLayout();
      this.configurationGroupBox.SuspendLayout();
      this.jobStatusGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // stateLogTabPage
      // 
      this.stateLogTabPage.Controls.Add(this.stateLogViewHost);
      this.stateLogTabPage.Location = new System.Drawing.Point(4, 22);
      this.stateLogTabPage.Name = "stateLogTabPage";
      this.stateLogTabPage.Size = new System.Drawing.Size(563, 375);
      this.stateLogTabPage.TabIndex = 5;
      this.stateLogTabPage.Text = "Execution History";
      this.stateLogTabPage.UseVisualStyleBackColor = true;
      // 
      // stateLogViewHost
      // 
      this.stateLogViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stateLogViewHost.Caption = "View";
      this.stateLogViewHost.Content = null;
      this.stateLogViewHost.Enabled = false;
      this.stateLogViewHost.Location = new System.Drawing.Point(3, 3);
      this.stateLogViewHost.Name = "stateLogViewHost";
      this.stateLogViewHost.ReadOnly = false;
      this.stateLogViewHost.Size = new System.Drawing.Size(557, 407);
      this.stateLogViewHost.TabIndex = 0;
      this.stateLogViewHost.ViewsLabelVisible = true;
      this.stateLogViewHost.ViewType = null;
      // 
      // detailsTabPage
      // 
      this.detailsTabPage.Controls.Add(this.splitContainer1);
      this.detailsTabPage.Controls.Add(this.modifyItemButton);
      this.detailsTabPage.Location = new System.Drawing.Point(4, 22);
      this.detailsTabPage.Name = "detailsTabPage";
      this.detailsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.detailsTabPage.Size = new System.Drawing.Size(563, 375);
      this.detailsTabPage.TabIndex = 0;
      this.detailsTabPage.Text = "Details";
      this.detailsTabPage.UseVisualStyleBackColor = true;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(6, 6);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.jobStatusGroupBox);
      this.splitContainer1.Size = new System.Drawing.Size(551, 200);
      this.splitContainer1.SplitterDistance = 275;
      this.splitContainer1.TabIndex = 44;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.groupBoxGenerel);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.configurationGroupBox);
      this.splitContainer2.Size = new System.Drawing.Size(275, 200);
      this.splitContainer2.SplitterDistance = 100;
      this.splitContainer2.TabIndex = 0;
      // 
      // groupBoxGenerel
      // 
      this.groupBoxGenerel.Controls.Add(this.priorityComboBox);
      this.groupBoxGenerel.Controls.Add(this.jobIdLabel);
      this.groupBoxGenerel.Controls.Add(this.jobIdTextBox);
      this.groupBoxGenerel.Controls.Add(this.lastUpdatedLabel);
      this.groupBoxGenerel.Controls.Add(this.lastUpdatedTextBox);
      this.groupBoxGenerel.Controls.Add(this.priorityLabel);
      this.groupBoxGenerel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBoxGenerel.Location = new System.Drawing.Point(0, 0);
      this.groupBoxGenerel.Name = "groupBoxGenerel";
      this.groupBoxGenerel.Size = new System.Drawing.Size(275, 100);
      this.groupBoxGenerel.TabIndex = 43;
      this.groupBoxGenerel.TabStop = false;
      this.groupBoxGenerel.Text = "General";
      // 
      // priorityComboBox
      // 
      this.priorityComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.priorityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.priorityComboBox.FormattingEnabled = true;
      this.priorityComboBox.Items.AddRange(new object[] {
            "Low",
            "Normal",
            "Urgent",
            "Critical"});
      this.priorityComboBox.Location = new System.Drawing.Point(87, 39);
      this.priorityComboBox.Name = "priorityComboBox";
      this.priorityComboBox.Size = new System.Drawing.Size(182, 21);
      this.priorityComboBox.TabIndex = 43;
      this.priorityComboBox.SelectedIndexChanged += new System.EventHandler(this.priorityComboBox_SelectedIndexChanged);
      // 
      // jobIdLabel
      // 
      this.jobIdLabel.AutoSize = true;
      this.jobIdLabel.Location = new System.Drawing.Point(6, 16);
      this.jobIdLabel.Name = "jobIdLabel";
      this.jobIdLabel.Size = new System.Drawing.Size(19, 13);
      this.jobIdLabel.TabIndex = 25;
      this.jobIdLabel.Text = "Id:";
      // 
      // jobIdTextBox
      // 
      this.jobIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.jobIdTextBox.Location = new System.Drawing.Point(87, 13);
      this.jobIdTextBox.Name = "jobIdTextBox";
      this.jobIdTextBox.Size = new System.Drawing.Size(182, 20);
      this.jobIdTextBox.TabIndex = 26;
      // 
      // lastUpdatedLabel
      // 
      this.lastUpdatedLabel.AutoSize = true;
      this.lastUpdatedLabel.Location = new System.Drawing.Point(6, 68);
      this.lastUpdatedLabel.Name = "lastUpdatedLabel";
      this.lastUpdatedLabel.Size = new System.Drawing.Size(75, 13);
      this.lastUpdatedLabel.TabIndex = 1;
      this.lastUpdatedLabel.Text = "Last changed:";
      // 
      // lastUpdatedTextBox
      // 
      this.lastUpdatedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lastUpdatedTextBox.Location = new System.Drawing.Point(87, 66);
      this.lastUpdatedTextBox.Name = "lastUpdatedTextBox";
      this.lastUpdatedTextBox.Size = new System.Drawing.Size(182, 20);
      this.lastUpdatedTextBox.TabIndex = 2;
      // 
      // priorityLabel
      // 
      this.priorityLabel.AutoSize = true;
      this.priorityLabel.Location = new System.Drawing.Point(6, 42);
      this.priorityLabel.Name = "priorityLabel";
      this.priorityLabel.Size = new System.Drawing.Size(41, 13);
      this.priorityLabel.TabIndex = 42;
      this.priorityLabel.Text = "Priority:";
      // 
      // configurationGroupBox
      // 
      this.configurationGroupBox.Controls.Add(this.memoryNeededComboBox);
      this.configurationGroupBox.Controls.Add(this.coresNeededComboBox);
      this.configurationGroupBox.Controls.Add(this.memoryNeededLabel);
      this.configurationGroupBox.Controls.Add(this.coresNeededLabel);
      this.configurationGroupBox.Controls.Add(this.computeInParallelLabel);
      this.configurationGroupBox.Controls.Add(this.computeInParallelCheckBox);
      this.configurationGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.configurationGroupBox.Location = new System.Drawing.Point(0, 0);
      this.configurationGroupBox.Name = "configurationGroupBox";
      this.configurationGroupBox.Size = new System.Drawing.Size(275, 96);
      this.configurationGroupBox.TabIndex = 27;
      this.configurationGroupBox.TabStop = false;
      this.configurationGroupBox.Text = "Resource demands";
      // 
      // memoryNeededComboBox
      // 
      this.memoryNeededComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.memoryNeededComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.memoryNeededComboBox.FormattingEnabled = true;
      this.memoryNeededComboBox.Items.AddRange(new object[] {
            "128",
            "256",
            "512",
            "1024",
            "2048"});
      this.memoryNeededComboBox.Location = new System.Drawing.Point(134, 40);
      this.memoryNeededComboBox.Name = "memoryNeededComboBox";
      this.memoryNeededComboBox.Size = new System.Drawing.Size(134, 21);
      this.memoryNeededComboBox.TabIndex = 42;
      // 
      // coresNeededComboBox
      // 
      this.coresNeededComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.coresNeededComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.coresNeededComboBox.FormattingEnabled = true;
      this.coresNeededComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "6",
            "8",
            "10",
            "12"});
      this.coresNeededComboBox.Location = new System.Drawing.Point(134, 13);
      this.coresNeededComboBox.Name = "coresNeededComboBox";
      this.coresNeededComboBox.Size = new System.Drawing.Size(134, 21);
      this.coresNeededComboBox.TabIndex = 41;
      this.coresNeededComboBox.SelectedIndexChanged += new System.EventHandler(this.coresNeededComboBox_SelectedIndexChanged);
      // 
      // memoryNeededLabel
      // 
      this.memoryNeededLabel.AutoSize = true;
      this.memoryNeededLabel.Location = new System.Drawing.Point(6, 43);
      this.memoryNeededLabel.Name = "memoryNeededLabel";
      this.memoryNeededLabel.Size = new System.Drawing.Size(122, 13);
      this.memoryNeededLabel.TabIndex = 40;
      this.memoryNeededLabel.Text = "Memory needed (in MB):";
      // 
      // coresNeededLabel
      // 
      this.coresNeededLabel.AutoSize = true;
      this.coresNeededLabel.Location = new System.Drawing.Point(6, 16);
      this.coresNeededLabel.Name = "coresNeededLabel";
      this.coresNeededLabel.Size = new System.Drawing.Size(104, 13);
      this.coresNeededLabel.TabIndex = 39;
      this.coresNeededLabel.Text = "Nr. of needed cores:";
      // 
      // computeInParallelLabel
      // 
      this.computeInParallelLabel.AutoSize = true;
      this.computeInParallelLabel.Location = new System.Drawing.Point(6, 67);
      this.computeInParallelLabel.Name = "computeInParallelLabel";
      this.computeInParallelLabel.Size = new System.Drawing.Size(107, 13);
      this.computeInParallelLabel.TabIndex = 36;
      this.computeInParallelLabel.Text = "Distribute child tasks:";
      // 
      // computeInParallelCheckBox
      // 
      this.computeInParallelCheckBox.AutoSize = true;
      this.computeInParallelCheckBox.Location = new System.Drawing.Point(135, 67);
      this.computeInParallelCheckBox.Name = "computeInParallelCheckBox";
      this.computeInParallelCheckBox.Size = new System.Drawing.Size(15, 14);
      this.computeInParallelCheckBox.TabIndex = 35;
      this.computeInParallelCheckBox.UseVisualStyleBackColor = true;
      this.computeInParallelCheckBox.CheckedChanged += new System.EventHandler(this.computeInParallelCheckBox_CheckedChanged);
      // 
      // jobStatusGroupBox
      // 
      this.jobStatusGroupBox.Controls.Add(this.commandTextBox);
      this.jobStatusGroupBox.Controls.Add(this.commandLabel);
      this.jobStatusGroupBox.Controls.Add(this.stateTextBox);
      this.jobStatusGroupBox.Controls.Add(this.dateCalculatedLabel);
      this.jobStatusGroupBox.Controls.Add(this.stateLabel);
      this.jobStatusGroupBox.Controls.Add(this.dateCalculatedText);
      this.jobStatusGroupBox.Controls.Add(this.dateFinishedTextBox);
      this.jobStatusGroupBox.Controls.Add(this.executionTimeTextBox);
      this.jobStatusGroupBox.Controls.Add(this.dateCreatedTextBox);
      this.jobStatusGroupBox.Controls.Add(this.executionTimeLabel);
      this.jobStatusGroupBox.Controls.Add(this.exceptionLabel);
      this.jobStatusGroupBox.Controls.Add(this.dateCreatedLabel);
      this.jobStatusGroupBox.Controls.Add(this.dateFinishedLabel);
      this.jobStatusGroupBox.Controls.Add(this.exceptionTextBox);
      this.jobStatusGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.jobStatusGroupBox.Location = new System.Drawing.Point(0, 0);
      this.jobStatusGroupBox.Name = "jobStatusGroupBox";
      this.jobStatusGroupBox.Size = new System.Drawing.Size(272, 200);
      this.jobStatusGroupBox.TabIndex = 24;
      this.jobStatusGroupBox.TabStop = false;
      this.jobStatusGroupBox.Text = "Task status";
      // 
      // commandTextBox
      // 
      this.commandTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.commandTextBox.Location = new System.Drawing.Point(95, 39);
      this.commandTextBox.Name = "commandTextBox";
      this.commandTextBox.Size = new System.Drawing.Size(171, 20);
      this.commandTextBox.TabIndex = 24;
      // 
      // commandLabel
      // 
      this.commandLabel.AutoSize = true;
      this.commandLabel.Location = new System.Drawing.Point(7, 42);
      this.commandLabel.Name = "commandLabel";
      this.commandLabel.Size = new System.Drawing.Size(57, 13);
      this.commandLabel.TabIndex = 25;
      this.commandLabel.Text = "Command:";
      // 
      // stateTextBox
      // 
      this.stateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stateTextBox.Location = new System.Drawing.Point(95, 13);
      this.stateTextBox.Name = "stateTextBox";
      this.stateTextBox.Size = new System.Drawing.Size(171, 20);
      this.stateTextBox.TabIndex = 2;
      // 
      // dateCalculatedLabel
      // 
      this.dateCalculatedLabel.AutoSize = true;
      this.dateCalculatedLabel.Location = new System.Drawing.Point(7, 120);
      this.dateCalculatedLabel.Name = "dateCalculatedLabel";
      this.dateCalculatedLabel.Size = new System.Drawing.Size(85, 13);
      this.dateCalculatedLabel.TabIndex = 23;
      this.dateCalculatedLabel.Text = "Date calculated:";
      // 
      // stateLabel
      // 
      this.stateLabel.AutoSize = true;
      this.stateLabel.Location = new System.Drawing.Point(7, 16);
      this.stateLabel.Name = "stateLabel";
      this.stateLabel.Size = new System.Drawing.Size(35, 13);
      this.stateLabel.TabIndex = 0;
      this.stateLabel.Text = "State:";
      // 
      // dateCalculatedText
      // 
      this.dateCalculatedText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dateCalculatedText.Location = new System.Drawing.Point(95, 117);
      this.dateCalculatedText.Name = "dateCalculatedText";
      this.dateCalculatedText.Size = new System.Drawing.Size(171, 20);
      this.dateCalculatedText.TabIndex = 22;
      // 
      // dateFinishedTextBox
      // 
      this.dateFinishedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dateFinishedTextBox.Location = new System.Drawing.Point(95, 143);
      this.dateFinishedTextBox.Name = "dateFinishedTextBox";
      this.dateFinishedTextBox.Size = new System.Drawing.Size(171, 20);
      this.dateFinishedTextBox.TabIndex = 6;
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(95, 65);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.Size = new System.Drawing.Size(171, 20);
      this.executionTimeTextBox.TabIndex = 4;
      // 
      // dateCreatedTextBox
      // 
      this.dateCreatedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dateCreatedTextBox.Location = new System.Drawing.Point(95, 91);
      this.dateCreatedTextBox.Name = "dateCreatedTextBox";
      this.dateCreatedTextBox.Size = new System.Drawing.Size(171, 20);
      this.dateCreatedTextBox.TabIndex = 5;
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.AutoSize = true;
      this.executionTimeLabel.Location = new System.Drawing.Point(7, 68);
      this.executionTimeLabel.Name = "executionTimeLabel";
      this.executionTimeLabel.Size = new System.Drawing.Size(79, 13);
      this.executionTimeLabel.TabIndex = 13;
      this.executionTimeLabel.Text = "Execution time:";
      // 
      // exceptionLabel
      // 
      this.exceptionLabel.AutoSize = true;
      this.exceptionLabel.Location = new System.Drawing.Point(7, 172);
      this.exceptionLabel.Name = "exceptionLabel";
      this.exceptionLabel.Size = new System.Drawing.Size(57, 13);
      this.exceptionLabel.TabIndex = 19;
      this.exceptionLabel.Text = "Exception:";
      // 
      // dateCreatedLabel
      // 
      this.dateCreatedLabel.AutoSize = true;
      this.dateCreatedLabel.Location = new System.Drawing.Point(7, 94);
      this.dateCreatedLabel.Name = "dateCreatedLabel";
      this.dateCreatedLabel.Size = new System.Drawing.Size(72, 13);
      this.dateCreatedLabel.TabIndex = 14;
      this.dateCreatedLabel.Text = "Date created:";
      // 
      // dateFinishedLabel
      // 
      this.dateFinishedLabel.AutoSize = true;
      this.dateFinishedLabel.Location = new System.Drawing.Point(7, 146);
      this.dateFinishedLabel.Name = "dateFinishedLabel";
      this.dateFinishedLabel.Size = new System.Drawing.Size(72, 13);
      this.dateFinishedLabel.TabIndex = 15;
      this.dateFinishedLabel.Text = "Date finished:";
      // 
      // exceptionTextBox
      // 
      this.exceptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.exceptionTextBox.Location = new System.Drawing.Point(95, 169);
      this.exceptionTextBox.Name = "exceptionTextBox";
      this.exceptionTextBox.Size = new System.Drawing.Size(171, 20);
      this.exceptionTextBox.TabIndex = 10;
      this.exceptionTextBox.DoubleClick += new System.EventHandler(this.exceptionTextBox_DoubleClick);
      // 
      // modifyItemButton
      // 
      this.modifyItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.modifyItemButton.Location = new System.Drawing.Point(3, 224);
      this.modifyItemButton.Name = "modifyItemButton";
      this.modifyItemButton.Size = new System.Drawing.Size(551, 23);
      this.modifyItemButton.TabIndex = 3;
      this.modifyItemButton.Text = "Modify Item";
      this.modifyItemButton.UseVisualStyleBackColor = true;
      this.modifyItemButton.Click += new System.EventHandler(this.modifyItemButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.detailsTabPage);
      this.tabControl.Controls.Add(this.stateLogTabPage);
      this.tabControl.Location = new System.Drawing.Point(3, 3);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(571, 401);
      this.tabControl.TabIndex = 25;
      // 
      // HiveTaskView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabControl);
      this.Name = "HiveTaskView";
      this.Size = new System.Drawing.Size(577, 407);
      this.stateLogTabPage.ResumeLayout(false);
      this.detailsTabPage.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.groupBoxGenerel.ResumeLayout(false);
      this.groupBoxGenerel.PerformLayout();
      this.configurationGroupBox.ResumeLayout(false);
      this.configurationGroupBox.PerformLayout();
      this.jobStatusGroupBox.ResumeLayout(false);
      this.jobStatusGroupBox.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.TabPage stateLogTabPage;
    protected MainForm.WindowsForms.ViewHost stateLogViewHost;
    protected System.Windows.Forms.TabPage detailsTabPage;
    protected System.Windows.Forms.Label priorityLabel;
    protected System.Windows.Forms.Label jobIdLabel;
    protected System.Windows.Forms.TextBox jobIdTextBox;
    protected System.Windows.Forms.GroupBox jobStatusGroupBox;
    protected System.Windows.Forms.TextBox stateTextBox;
    protected System.Windows.Forms.Label stateLabel;
    protected System.Windows.Forms.TextBox dateFinishedTextBox;
    protected System.Windows.Forms.TextBox executionTimeTextBox;
    protected System.Windows.Forms.Label executionTimeLabel;
    protected System.Windows.Forms.Label exceptionLabel;
    protected System.Windows.Forms.Label dateFinishedLabel;
    protected System.Windows.Forms.TextBox exceptionTextBox;
    protected System.Windows.Forms.TabControl tabControl;
    protected System.Windows.Forms.Label dateCalculatedLabel;
    protected System.Windows.Forms.TextBox dateCalculatedText;
    protected System.Windows.Forms.TextBox dateCreatedTextBox;
    protected System.Windows.Forms.Label dateCreatedLabel;
    protected System.Windows.Forms.TextBox commandTextBox;
    protected System.Windows.Forms.Label commandLabel;
    protected System.Windows.Forms.Button modifyItemButton;
    protected System.Windows.Forms.TextBox lastUpdatedTextBox;
    protected System.Windows.Forms.Label lastUpdatedLabel;
    private System.Windows.Forms.GroupBox groupBoxGenerel;
    protected System.Windows.Forms.GroupBox configurationGroupBox;
    protected System.Windows.Forms.Label memoryNeededLabel;
    protected System.Windows.Forms.Label coresNeededLabel;
    protected System.Windows.Forms.Label computeInParallelLabel;
    protected System.Windows.Forms.CheckBox computeInParallelCheckBox;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.SplitContainer splitContainer2;
    protected System.Windows.Forms.ComboBox priorityComboBox;
    private System.Windows.Forms.ComboBox coresNeededComboBox;
    protected System.Windows.Forms.ComboBox memoryNeededComboBox;

  }
}
