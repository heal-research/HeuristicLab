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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateExperimentDialog));
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.createBatchRunCheckBox = new System.Windows.Forms.CheckBox();
      this.createBatchRunLabel = new System.Windows.Forms.Label();
      this.repetitionsLabel = new System.Windows.Forms.Label();
      this.repetitionsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.experimentCreationBackgroundWorker = new System.ComponentModel.BackgroundWorker();
      this.instanceDiscoveryProgressBar = new System.Windows.Forms.ProgressBar();
      this.instanceDiscoveryProgressLabel = new System.Windows.Forms.Label();
      this.selectAllCheckBox = new System.Windows.Forms.CheckBox();
      this.selectNoneCheckBox = new System.Windows.Forms.CheckBox();
      this.instanceDiscoveryBackgroundWorker = new System.ComponentModel.BackgroundWorker();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.parametersSplitContainer = new System.Windows.Forms.SplitContainer();
      this.parametersListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.generateButton = new System.Windows.Forms.Button();
      this.stringConvertibleArrayView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.detailsTypeLabel = new System.Windows.Forms.Label();
      this.choicesListView = new System.Windows.Forms.ListView();
      this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.instancesTabPage = new System.Windows.Forms.TabPage();
      this.label1 = new System.Windows.Forms.Label();
      this.instancesTreeView = new System.Windows.Forms.TreeView();
      this.experimentsToCreateDescriptionLabel = new System.Windows.Forms.Label();
      this.variationsLabel = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.warningProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.experimentCreationProgressBar = new System.Windows.Forms.ProgressBar();
      ((System.ComponentModel.ISupportInitialize)(this.repetitionsNumericUpDown)).BeginInit();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.parametersSplitContainer)).BeginInit();
      this.parametersSplitContainer.Panel1.SuspendLayout();
      this.parametersSplitContainer.Panel2.SuspendLayout();
      this.parametersSplitContainer.SuspendLayout();
      this.instancesTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(246, 377);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 7;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(327, 377);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 8;
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
      this.repetitionsNumericUpDown.Size = new System.Drawing.Size(289, 20);
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
      // experimentCreationBackgroundWorker
      // 
      this.experimentCreationBackgroundWorker.WorkerReportsProgress = true;
      this.experimentCreationBackgroundWorker.WorkerSupportsCancellation = true;
      this.experimentCreationBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.experimentCreationBackgroundWorker_DoWork);
      this.experimentCreationBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.experimentCreationBackgroundWorker_ProgressChanged);
      this.experimentCreationBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.experimentCreationBackgroundWorker_RunWorkerCompleted);
      // 
      // instanceDiscoveryProgressBar
      // 
      this.instanceDiscoveryProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.instanceDiscoveryProgressBar.Location = new System.Drawing.Point(6, 141);
      this.instanceDiscoveryProgressBar.Name = "instanceDiscoveryProgressBar";
      this.instanceDiscoveryProgressBar.Size = new System.Drawing.Size(367, 23);
      this.instanceDiscoveryProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.instanceDiscoveryProgressBar.TabIndex = 4;
      this.instanceDiscoveryProgressBar.Visible = false;
      // 
      // instanceDiscoveryProgressLabel
      // 
      this.instanceDiscoveryProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.instanceDiscoveryProgressLabel.BackColor = System.Drawing.SystemColors.Control;
      this.instanceDiscoveryProgressLabel.Location = new System.Drawing.Point(6, 167);
      this.instanceDiscoveryProgressLabel.Name = "instanceDiscoveryProgressLabel";
      this.instanceDiscoveryProgressLabel.Size = new System.Drawing.Size(367, 23);
      this.instanceDiscoveryProgressLabel.TabIndex = 5;
      this.instanceDiscoveryProgressLabel.Text = "label1";
      this.instanceDiscoveryProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.instanceDiscoveryProgressLabel.Visible = false;
      // 
      // selectAllCheckBox
      // 
      this.selectAllCheckBox.AutoSize = true;
      this.selectAllCheckBox.Location = new System.Drawing.Point(52, 12);
      this.selectAllCheckBox.Name = "selectAllCheckBox";
      this.selectAllCheckBox.Size = new System.Drawing.Size(36, 17);
      this.selectAllCheckBox.TabIndex = 1;
      this.selectAllCheckBox.Text = "all";
      this.selectAllCheckBox.UseVisualStyleBackColor = true;
      this.selectAllCheckBox.CheckedChanged += new System.EventHandler(this.selectAllCheckBox_CheckedChanged);
      // 
      // selectNoneCheckBox
      // 
      this.selectNoneCheckBox.AutoSize = true;
      this.selectNoneCheckBox.Location = new System.Drawing.Point(94, 12);
      this.selectNoneCheckBox.Name = "selectNoneCheckBox";
      this.selectNoneCheckBox.Size = new System.Drawing.Size(50, 17);
      this.selectNoneCheckBox.TabIndex = 2;
      this.selectNoneCheckBox.Text = "none";
      this.selectNoneCheckBox.UseVisualStyleBackColor = true;
      this.selectNoneCheckBox.CheckedChanged += new System.EventHandler(this.selectNoneCheckBox_CheckedChanged);
      // 
      // instanceDiscoveryBackgroundWorker
      // 
      this.instanceDiscoveryBackgroundWorker.WorkerReportsProgress = true;
      this.instanceDiscoveryBackgroundWorker.WorkerSupportsCancellation = true;
      this.instanceDiscoveryBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.instanceDiscoveryBackgroundWorker_DoWork);
      this.instanceDiscoveryBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.instanceDiscoveryBackgroundWorker_ProgressChanged);
      this.instanceDiscoveryBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.instanceDiscoveryBackgroundWorker_RunWorkerCompleted);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Controls.Add(this.instancesTabPage);
      this.tabControl.Location = new System.Drawing.Point(15, 54);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(387, 317);
      this.tabControl.TabIndex = 4;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.parametersSplitContainer);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(379, 291);
      this.parametersTabPage.TabIndex = 1;
      this.parametersTabPage.Text = "Parameter Variations";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // parametersSplitContainer
      // 
      this.parametersSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.parametersSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.parametersSplitContainer.Location = new System.Drawing.Point(3, 3);
      this.parametersSplitContainer.Name = "parametersSplitContainer";
      // 
      // parametersSplitContainer.Panel1
      // 
      this.parametersSplitContainer.Panel1.Controls.Add(this.parametersListView);
      // 
      // parametersSplitContainer.Panel2
      // 
      this.parametersSplitContainer.Panel2.Controls.Add(this.generateButton);
      this.parametersSplitContainer.Panel2.Controls.Add(this.stringConvertibleArrayView);
      this.parametersSplitContainer.Panel2.Controls.Add(this.detailsTypeLabel);
      this.parametersSplitContainer.Panel2.Controls.Add(this.choicesListView);
      this.parametersSplitContainer.Size = new System.Drawing.Size(373, 285);
      this.parametersSplitContainer.SplitterDistance = 155;
      this.parametersSplitContainer.TabIndex = 1;
      // 
      // parametersListView
      // 
      this.parametersListView.CheckBoxes = true;
      this.parametersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.parametersListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.parametersListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.parametersListView.Location = new System.Drawing.Point(0, 0);
      this.parametersListView.MultiSelect = false;
      this.parametersListView.Name = "parametersListView";
      this.parametersListView.ShowItemToolTips = true;
      this.parametersListView.Size = new System.Drawing.Size(155, 285);
      this.parametersListView.TabIndex = 0;
      this.parametersListView.UseCompatibleStateImageBehavior = false;
      this.parametersListView.View = System.Windows.Forms.View.Details;
      this.parametersListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.parametersListView_ItemChecked);
      this.parametersListView.SelectedIndexChanged += new System.EventHandler(this.parametersListView_SelectedIndexChanged);
      // 
      // columnHeader1
      // 
      this.columnHeader1.Width = 150;
      // 
      // generateButton
      // 
      this.generateButton.Location = new System.Drawing.Point(16, 10);
      this.generateButton.Name = "generateButton";
      this.generateButton.Size = new System.Drawing.Size(75, 23);
      this.generateButton.TabIndex = 8;
      this.generateButton.Text = "Generate...";
      this.generateButton.UseVisualStyleBackColor = true;
      this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
      // 
      // stringConvertibleArrayView
      // 
      this.stringConvertibleArrayView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.stringConvertibleArrayView.Caption = "StringConvertibleArray View";
      this.stringConvertibleArrayView.Content = null;
      this.stringConvertibleArrayView.Location = new System.Drawing.Point(16, 39);
      this.stringConvertibleArrayView.Name = "stringConvertibleArrayView";
      this.stringConvertibleArrayView.ReadOnly = false;
      this.stringConvertibleArrayView.Size = new System.Drawing.Size(183, 233);
      this.stringConvertibleArrayView.TabIndex = 7;
      // 
      // detailsTypeLabel
      // 
      this.detailsTypeLabel.AutoSize = true;
      this.detailsTypeLabel.Location = new System.Drawing.Point(13, 15);
      this.detailsTypeLabel.Name = "detailsTypeLabel";
      this.detailsTypeLabel.Size = new System.Drawing.Size(48, 13);
      this.detailsTypeLabel.TabIndex = 0;
      this.detailsTypeLabel.Text = "Choices:";
      // 
      // choicesListView
      // 
      this.choicesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.choicesListView.CheckBoxes = true;
      this.choicesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
      this.choicesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.choicesListView.Location = new System.Drawing.Point(16, 34);
      this.choicesListView.Name = "choicesListView";
      this.choicesListView.Size = new System.Drawing.Size(183, 238);
      this.choicesListView.TabIndex = 6;
      this.choicesListView.UseCompatibleStateImageBehavior = false;
      this.choicesListView.View = System.Windows.Forms.View.Details;
      this.choicesListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.choiceListView_ItemChecked);
      // 
      // columnHeader2
      // 
      this.columnHeader2.Width = 150;
      // 
      // instancesTabPage
      // 
      this.instancesTabPage.Controls.Add(this.label1);
      this.instancesTabPage.Controls.Add(this.instanceDiscoveryProgressBar);
      this.instancesTabPage.Controls.Add(this.selectNoneCheckBox);
      this.instancesTabPage.Controls.Add(this.instanceDiscoveryProgressLabel);
      this.instancesTabPage.Controls.Add(this.selectAllCheckBox);
      this.instancesTabPage.Controls.Add(this.instancesTreeView);
      this.instancesTabPage.Location = new System.Drawing.Point(4, 22);
      this.instancesTabPage.Name = "instancesTabPage";
      this.instancesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.instancesTabPage.Size = new System.Drawing.Size(379, 291);
      this.instancesTabPage.TabIndex = 0;
      this.instancesTabPage.Text = "Problem Instances";
      this.instancesTabPage.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(40, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Select:";
      // 
      // instancesTreeView
      // 
      this.instancesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.instancesTreeView.CheckBoxes = true;
      this.instancesTreeView.Location = new System.Drawing.Point(6, 35);
      this.instancesTreeView.Name = "instancesTreeView";
      this.instancesTreeView.Size = new System.Drawing.Size(367, 250);
      this.instancesTreeView.TabIndex = 6;
      this.instancesTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.instancesTreeView_AfterCheck);
      // 
      // experimentsToCreateDescriptionLabel
      // 
      this.experimentsToCreateDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.experimentsToCreateDescriptionLabel.AutoSize = true;
      this.experimentsToCreateDescriptionLabel.Location = new System.Drawing.Point(12, 382);
      this.experimentsToCreateDescriptionLabel.Name = "experimentsToCreateDescriptionLabel";
      this.experimentsToCreateDescriptionLabel.Size = new System.Drawing.Size(56, 13);
      this.experimentsToCreateDescriptionLabel.TabIndex = 5;
      this.experimentsToCreateDescriptionLabel.Text = "Variations:";
      // 
      // variationsLabel
      // 
      this.variationsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.variationsLabel.AutoSize = true;
      this.variationsLabel.Location = new System.Drawing.Point(74, 382);
      this.variationsLabel.Name = "variationsLabel";
      this.variationsLabel.Size = new System.Drawing.Size(13, 13);
      this.variationsLabel.TabIndex = 6;
      this.variationsLabel.Text = "1";
      this.variationsLabel.TextChanged += new System.EventHandler(this.experimentsLabel_TextChanged);
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // warningProvider
      // 
      this.warningProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.warningProvider.ContainerControl = this;
      this.warningProvider.Icon = ((System.Drawing.Icon)(resources.GetObject("warningProvider.Icon")));
      // 
      // experimentCreationProgressBar
      // 
      this.experimentCreationProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.experimentCreationProgressBar.Location = new System.Drawing.Point(187, 377);
      this.experimentCreationProgressBar.Name = "experimentCreationProgressBar";
      this.experimentCreationProgressBar.Size = new System.Drawing.Size(134, 23);
      this.experimentCreationProgressBar.TabIndex = 9;
      this.experimentCreationProgressBar.Visible = false;
      // 
      // CreateExperimentDialog
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(414, 412);
      this.Controls.Add(this.experimentCreationProgressBar);
      this.Controls.Add(this.variationsLabel);
      this.Controls.Add(this.experimentsToCreateDescriptionLabel);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.repetitionsNumericUpDown);
      this.Controls.Add(this.repetitionsLabel);
      this.Controls.Add(this.createBatchRunLabel);
      this.Controls.Add(this.createBatchRunCheckBox);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CreateExperimentDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Create Experiment";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateExperimentDialog_FormClosing);
      this.Load += new System.EventHandler(this.CreateExperimentDialog_Load);
      ((System.ComponentModel.ISupportInitialize)(this.repetitionsNumericUpDown)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.parametersSplitContainer.Panel1.ResumeLayout(false);
      this.parametersSplitContainer.Panel2.ResumeLayout(false);
      this.parametersSplitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.parametersSplitContainer)).EndInit();
      this.parametersSplitContainer.ResumeLayout(false);
      this.instancesTabPage.ResumeLayout(false);
      this.instancesTabPage.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).EndInit();
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
    private System.ComponentModel.BackgroundWorker experimentCreationBackgroundWorker;
    private System.Windows.Forms.ProgressBar instanceDiscoveryProgressBar;
    private System.Windows.Forms.Label instanceDiscoveryProgressLabel;
    private System.Windows.Forms.CheckBox selectAllCheckBox;
    private System.Windows.Forms.CheckBox selectNoneCheckBox;
    private System.ComponentModel.BackgroundWorker instanceDiscoveryBackgroundWorker;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage parametersTabPage;
    private System.Windows.Forms.TabPage instancesTabPage;
    private System.Windows.Forms.SplitContainer parametersSplitContainer;
    private System.Windows.Forms.ListView parametersListView;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.Label experimentsToCreateDescriptionLabel;
    private System.Windows.Forms.Label variationsLabel;
    private System.Windows.Forms.Label detailsTypeLabel;
    private System.Windows.Forms.ListView choicesListView;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.ErrorProvider warningProvider;
    private System.Windows.Forms.TreeView instancesTreeView;
    private Data.Views.StringConvertibleArrayView stringConvertibleArrayView;
    private System.Windows.Forms.ProgressBar experimentCreationProgressBar;
    private System.Windows.Forms.Button generateButton;

  }
}