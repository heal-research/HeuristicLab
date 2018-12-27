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


namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  partial class CrossValidationView {
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
      this.startButton = new System.Windows.Forms.Button();
      this.pauseButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.foldsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.foldsLabel = new System.Windows.Forms.Label();
      this.workersNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.workersLabel = new System.Windows.Forms.Label();
      this.samplesStartLabel = new System.Windows.Forms.Label();
      this.samplesEndLabel = new System.Windows.Forms.Label();
      this.samplesEndStringConvertibleValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.samplesStartStringConvertibleValueView = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.algorithmTabPage = new System.Windows.Forms.TabPage();
      this.algorithmNamedItemView = new HeuristicLab.Core.Views.NamedItemView();
      this.algorithmTabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.algorithmProblemTabPage = new System.Windows.Forms.TabPage();
      this.openProblemButton = new System.Windows.Forms.Button();
      this.algorithmProblemViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.newProblemButton = new System.Windows.Forms.Button();
      this.algorithmParametersTabPage = new System.Windows.Forms.TabPage();
      this.algorithmParameterCollectionView = new HeuristicLab.Core.Views.ParameterCollectionView();
      this.openAlgorithmButton = new System.Windows.Forms.Button();
      this.newAlgorithmButton = new System.Windows.Forms.Button();
      this.resultsTabPage = new System.Windows.Forms.TabPage();
      this.resultCollectionView = new HeuristicLab.Optimization.Views.ResultCollectionView();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.storeAlgorithmInEachRunCheckBox = new System.Windows.Forms.CheckBox();
      this.runCollectionView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.shuffleSamplesCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.foldsNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.workersNumericUpDown)).BeginInit();
      this.tabControl.SuspendLayout();
      this.algorithmTabPage.SuspendLayout();
      this.algorithmTabControl.SuspendLayout();
      this.algorithmProblemTabPage.SuspendLayout();
      this.algorithmParametersTabPage.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(80, 0);
      this.nameTextBox.Size = new System.Drawing.Size(511, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(597, 3);
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.startButton.Location = new System.Drawing.Point(0, 514);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(24, 24);
      this.startButton.TabIndex = 5;
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // pauseButton
      // 
      this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.pauseButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Pause;
      this.pauseButton.Location = new System.Drawing.Point(30, 514);
      this.pauseButton.Name = "pauseButton";
      this.pauseButton.Size = new System.Drawing.Size(24, 24);
      this.pauseButton.TabIndex = 6;
      this.pauseButton.UseVisualStyleBackColor = true;
      this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Stop;
      this.stopButton.Location = new System.Drawing.Point(60, 514);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(24, 24);
      this.stopButton.TabIndex = 7;
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // resetButton
      // 
      this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Restart;
      this.resetButton.Location = new System.Drawing.Point(90, 514);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(24, 24);
      this.resetButton.TabIndex = 8;
      this.resetButton.UseVisualStyleBackColor = true;
      this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(478, 517);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(137, 20);
      this.executionTimeTextBox.TabIndex = 10;
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeLabel.AutoSize = true;
      this.executionTimeLabel.Location = new System.Drawing.Point(389, 520);
      this.executionTimeLabel.Name = "executionTimeLabel";
      this.executionTimeLabel.Size = new System.Drawing.Size(83, 13);
      this.executionTimeLabel.TabIndex = 9;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // foldsNumericUpDown
      // 
      this.foldsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.foldsNumericUpDown.Location = new System.Drawing.Point(55, 27);
      this.foldsNumericUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
      this.foldsNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
      this.foldsNumericUpDown.Name = "foldsNumericUpDown";
      this.foldsNumericUpDown.Size = new System.Drawing.Size(248, 20);
      this.foldsNumericUpDown.TabIndex = 3;
      this.foldsNumericUpDown.ThousandsSeparator = true;
      this.foldsNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
      this.foldsNumericUpDown.ValueChanged += new System.EventHandler(this.foldsNumericUpDown_ValueChanged);
      this.foldsNumericUpDown.Validated += new System.EventHandler(this.foldsNumericUpDown_Validated);
      // 
      // foldsLabel
      // 
      this.foldsLabel.AutoSize = true;
      this.foldsLabel.Location = new System.Drawing.Point(3, 29);
      this.foldsLabel.Name = "foldsLabel";
      this.foldsLabel.Size = new System.Drawing.Size(35, 13);
      this.foldsLabel.TabIndex = 2;
      this.foldsLabel.Text = "&Folds:";
      // 
      // workersNumericUpDown
      // 
      this.workersNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.workersNumericUpDown.Location = new System.Drawing.Point(55, 1);
      this.workersNumericUpDown.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
      this.workersNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.workersNumericUpDown.Name = "workersNumericUpDown";
      this.workersNumericUpDown.Size = new System.Drawing.Size(248, 20);
      this.workersNumericUpDown.TabIndex = 1;
      this.workersNumericUpDown.ThousandsSeparator = true;
      this.workersNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.workersNumericUpDown.ValueChanged += new System.EventHandler(this.workersNumericUpDown_ValueChanged);
      this.workersNumericUpDown.Validated += new System.EventHandler(this.workersNumericUpDown_Validated);
      // 
      // workersLabel
      // 
      this.workersLabel.AutoSize = true;
      this.workersLabel.Location = new System.Drawing.Point(3, 3);
      this.workersLabel.Name = "workersLabel";
      this.workersLabel.Size = new System.Drawing.Size(50, 13);
      this.workersLabel.TabIndex = 0;
      this.workersLabel.Text = "&Workers:";
      // 
      // samplesStartLabel
      // 
      this.samplesStartLabel.AutoSize = true;
      this.samplesStartLabel.Location = new System.Drawing.Point(3, 3);
      this.samplesStartLabel.Name = "samplesStartLabel";
      this.samplesStartLabel.Size = new System.Drawing.Size(75, 13);
      this.samplesStartLabel.TabIndex = 0;
      this.samplesStartLabel.Text = "&Samples Start:";
      // 
      // samplesEndLabel
      // 
      this.samplesEndLabel.AutoSize = true;
      this.samplesEndLabel.Location = new System.Drawing.Point(3, 29);
      this.samplesEndLabel.Name = "samplesEndLabel";
      this.samplesEndLabel.Size = new System.Drawing.Size(72, 13);
      this.samplesEndLabel.TabIndex = 2;
      this.samplesEndLabel.Text = "&Samples End:";
      // 
      // samplesEndStringConvertibleValueView
      // 
      this.samplesEndStringConvertibleValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.samplesEndStringConvertibleValueView.Caption = "StringConvertibleValue View";
      this.samplesEndStringConvertibleValueView.Content = null;
      this.samplesEndStringConvertibleValueView.LabelVisible = false;
      this.samplesEndStringConvertibleValueView.Location = new System.Drawing.Point(80, 26);
      this.samplesEndStringConvertibleValueView.Name = "samplesEndStringConvertibleValueView";
      this.samplesEndStringConvertibleValueView.ReadOnly = false;
      this.samplesEndStringConvertibleValueView.Size = new System.Drawing.Size(223, 20);
      this.samplesEndStringConvertibleValueView.TabIndex = 3;
      // 
      // samplesStartStringConvertibleValueView
      // 
      this.samplesStartStringConvertibleValueView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.samplesStartStringConvertibleValueView.Caption = "StringConvertibleValue View";
      this.samplesStartStringConvertibleValueView.Content = null;
      this.samplesStartStringConvertibleValueView.LabelVisible = false;
      this.samplesStartStringConvertibleValueView.Location = new System.Drawing.Point(80, 0);
      this.samplesStartStringConvertibleValueView.Name = "samplesStartStringConvertibleValueView";
      this.samplesStartStringConvertibleValueView.ReadOnly = false;
      this.samplesStartStringConvertibleValueView.Size = new System.Drawing.Size(223, 20);
      this.samplesStartStringConvertibleValueView.TabIndex = 1;
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.algorithmTabPage);
      this.tabControl.Controls.Add(this.resultsTabPage);
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 115);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(616, 393);
      this.tabControl.TabIndex = 4;
      // 
      // algorithmTabPage
      // 
      this.algorithmTabPage.AllowDrop = true;
      this.algorithmTabPage.Controls.Add(this.algorithmNamedItemView);
      this.algorithmTabPage.Controls.Add(this.algorithmTabControl);
      this.algorithmTabPage.Controls.Add(this.openAlgorithmButton);
      this.algorithmTabPage.Controls.Add(this.newAlgorithmButton);
      this.algorithmTabPage.Location = new System.Drawing.Point(4, 22);
      this.algorithmTabPage.Name = "algorithmTabPage";
      this.algorithmTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.algorithmTabPage.Size = new System.Drawing.Size(608, 367);
      this.algorithmTabPage.TabIndex = 0;
      this.algorithmTabPage.Text = "Algorithm";
      this.algorithmTabPage.UseVisualStyleBackColor = true;
      this.algorithmTabPage.DragDrop += new System.Windows.Forms.DragEventHandler(this.algorithmTabPage_DragDrop);
      this.algorithmTabPage.DragEnter += new System.Windows.Forms.DragEventHandler(this.algorithmTabPage_DragEnterOver);
      this.algorithmTabPage.DragOver += new System.Windows.Forms.DragEventHandler(this.algorithmTabPage_DragEnterOver);
      // 
      // algorithmNamedItemView
      // 
      this.algorithmNamedItemView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmNamedItemView.Caption = "NamedItem View";
      this.algorithmNamedItemView.Content = null;
      this.algorithmNamedItemView.Location = new System.Drawing.Point(6, 36);
      this.algorithmNamedItemView.Name = "algorithmNamedItemView";
      this.algorithmNamedItemView.ReadOnly = false;
      this.algorithmNamedItemView.Size = new System.Drawing.Size(596, 23);
      this.algorithmNamedItemView.TabIndex = 2;
      // 
      // algorithmTabControl
      // 
      this.algorithmTabControl.AllowDrop = true;
      this.algorithmTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmTabControl.Controls.Add(this.algorithmProblemTabPage);
      this.algorithmTabControl.Controls.Add(this.algorithmParametersTabPage);
      this.algorithmTabControl.Location = new System.Drawing.Point(8, 65);
      this.algorithmTabControl.Name = "algorithmTabControl";
      this.algorithmTabControl.SelectedIndex = 0;
      this.algorithmTabControl.Size = new System.Drawing.Size(594, 296);
      this.algorithmTabControl.TabIndex = 3;
      // 
      // algorithmProblemTabPage
      // 
      this.algorithmProblemTabPage.AllowDrop = true;
      this.algorithmProblemTabPage.Controls.Add(this.openProblemButton);
      this.algorithmProblemTabPage.Controls.Add(this.algorithmProblemViewHost);
      this.algorithmProblemTabPage.Controls.Add(this.newProblemButton);
      this.algorithmProblemTabPage.Location = new System.Drawing.Point(4, 22);
      this.algorithmProblemTabPage.Name = "algorithmProblemTabPage";
      this.algorithmProblemTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.algorithmProblemTabPage.Size = new System.Drawing.Size(586, 270);
      this.algorithmProblemTabPage.TabIndex = 0;
      this.algorithmProblemTabPage.Text = "Problem";
      this.algorithmProblemTabPage.UseVisualStyleBackColor = true;
      this.algorithmProblemTabPage.DragDrop += new System.Windows.Forms.DragEventHandler(this.algorithmProblemTabPage_DragDrop);
      this.algorithmProblemTabPage.DragEnter += new System.Windows.Forms.DragEventHandler(this.algorithmProblemTabPage_DragEnterOver);
      this.algorithmProblemTabPage.DragOver += new System.Windows.Forms.DragEventHandler(this.algorithmProblemTabPage_DragEnterOver);
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
      // algorithmProblemViewHost
      // 
      this.algorithmProblemViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmProblemViewHost.Caption = "View";
      this.algorithmProblemViewHost.Content = null;
      this.algorithmProblemViewHost.Enabled = false;
      this.algorithmProblemViewHost.Location = new System.Drawing.Point(6, 36);
      this.algorithmProblemViewHost.Name = "algorithmProblemViewHost";
      this.algorithmProblemViewHost.ReadOnly = false;
      this.algorithmProblemViewHost.Size = new System.Drawing.Size(574, 228);
      this.algorithmProblemViewHost.TabIndex = 2;
      this.algorithmProblemViewHost.ViewsLabelVisible = true;
      this.algorithmProblemViewHost.ViewType = null;
      // 
      // newProblemButton
      // 
      this.newProblemButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.NewDocument;
      this.newProblemButton.Location = new System.Drawing.Point(6, 6);
      this.newProblemButton.Name = "newProblemButton";
      this.newProblemButton.Size = new System.Drawing.Size(24, 24);
      this.newProblemButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newProblemButton, "NewProblem");
      this.newProblemButton.UseVisualStyleBackColor = true;
      this.newProblemButton.Click += new System.EventHandler(this.newProblemButton_Click);
      // 
      // algorithmParametersTabPage
      // 
      this.algorithmParametersTabPage.Controls.Add(this.algorithmParameterCollectionView);
      this.algorithmParametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.algorithmParametersTabPage.Name = "algorithmParametersTabPage";
      this.algorithmParametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.algorithmParametersTabPage.Size = new System.Drawing.Size(586, 270);
      this.algorithmParametersTabPage.TabIndex = 1;
      this.algorithmParametersTabPage.Text = "Parameters";
      this.algorithmParametersTabPage.UseVisualStyleBackColor = true;
      // 
      // algorithmParameterCollectionView
      // 
      this.algorithmParameterCollectionView.AllowEditingOfHiddenParameters = true;
      this.algorithmParameterCollectionView.Caption = "ParameterCollection View";
      this.algorithmParameterCollectionView.Content = null;
      this.algorithmParameterCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.algorithmParameterCollectionView.Location = new System.Drawing.Point(3, 3);
      this.algorithmParameterCollectionView.Name = "algorithmParameterCollectionView";
      this.algorithmParameterCollectionView.ReadOnly = false;
      this.algorithmParameterCollectionView.ShowDetails = true;
      this.algorithmParameterCollectionView.Size = new System.Drawing.Size(580, 264);
      this.algorithmParameterCollectionView.TabIndex = 0;
      // 
      // openAlgorithmButton
      // 
      this.openAlgorithmButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.openAlgorithmButton.Location = new System.Drawing.Point(38, 6);
      this.openAlgorithmButton.Name = "openAlgorithmButton";
      this.openAlgorithmButton.Size = new System.Drawing.Size(24, 24);
      this.openAlgorithmButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.openAlgorithmButton, "Open Algorithm");
      this.openAlgorithmButton.UseVisualStyleBackColor = true;
      this.openAlgorithmButton.Click += new System.EventHandler(this.openAlgorithmButton_Click);
      // 
      // newAlgorithmButton
      // 
      this.newAlgorithmButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.NewDocument;
      this.newAlgorithmButton.Location = new System.Drawing.Point(8, 6);
      this.newAlgorithmButton.Name = "newAlgorithmButton";
      this.newAlgorithmButton.Size = new System.Drawing.Size(24, 24);
      this.newAlgorithmButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newAlgorithmButton, "New Algorithm");
      this.newAlgorithmButton.UseVisualStyleBackColor = true;
      this.newAlgorithmButton.Click += new System.EventHandler(this.newAlgorithmButton_Click);
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Controls.Add(this.resultCollectionView);
      this.resultsTabPage.Location = new System.Drawing.Point(4, 22);
      this.resultsTabPage.Name = "resultsTabPage";
      this.resultsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsTabPage.Size = new System.Drawing.Size(608, 367);
      this.resultsTabPage.TabIndex = 1;
      this.resultsTabPage.Text = "Results";
      this.resultsTabPage.UseVisualStyleBackColor = true;
      // 
      // resultCollectionView
      // 
      this.resultCollectionView.Caption = "ResultCollection View";
      this.resultCollectionView.Content = null;
      this.resultCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resultCollectionView.Location = new System.Drawing.Point(3, 3);
      this.resultCollectionView.Name = "resultCollectionView";
      this.resultCollectionView.ReadOnly = true;
      this.resultCollectionView.ShowDetails = true;
      this.resultCollectionView.Size = new System.Drawing.Size(602, 361);
      this.resultCollectionView.TabIndex = 0;
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.storeAlgorithmInEachRunCheckBox);
      this.runsTabPage.Controls.Add(this.runCollectionView);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(608, 367);
      this.runsTabPage.TabIndex = 2;
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
      this.storeAlgorithmInEachRunCheckBox.Location = new System.Drawing.Point(440, 4);
      this.storeAlgorithmInEachRunCheckBox.Name = "storeAlgorithmInEachRunCheckBox";
      this.storeAlgorithmInEachRunCheckBox.Size = new System.Drawing.Size(161, 17);
      this.storeAlgorithmInEachRunCheckBox.TabIndex = 1;
      this.storeAlgorithmInEachRunCheckBox.Text = "&Store Algorithm in each Run:";
      this.toolTip.SetToolTip(this.storeAlgorithmInEachRunCheckBox, "Check to store a copy of the algorithm in each run.");
      this.storeAlgorithmInEachRunCheckBox.UseVisualStyleBackColor = true;
      this.storeAlgorithmInEachRunCheckBox.CheckedChanged += new System.EventHandler(this.storeAlgorithmInEachRunCheckBox_CheckedChanged);
      // 
      // runCollectionView
      // 
      this.runCollectionView.Caption = "RunCollection View";
      this.runCollectionView.Content = null;
      this.runCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.runCollectionView.Location = new System.Drawing.Point(3, 3);
      this.runCollectionView.Name = "runCollectionView";
      this.runCollectionView.ReadOnly = false;
      this.runCollectionView.Size = new System.Drawing.Size(602, 361);
      this.runCollectionView.TabIndex = 0;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "Algorithm";
      this.openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      this.openFileDialog.Title = "Open Algorithm";
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(0, 26);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.shuffleSamplesCheckBox);
      this.splitContainer.Panel1.Controls.Add(this.samplesEndStringConvertibleValueView);
      this.splitContainer.Panel1.Controls.Add(this.samplesStartStringConvertibleValueView);
      this.splitContainer.Panel1.Controls.Add(this.samplesStartLabel);
      this.splitContainer.Panel1.Controls.Add(this.samplesEndLabel);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.foldsNumericUpDown);
      this.splitContainer.Panel2.Controls.Add(this.foldsLabel);
      this.splitContainer.Panel2.Controls.Add(this.workersLabel);
      this.splitContainer.Panel2.Controls.Add(this.workersNumericUpDown);
      this.splitContainer.Size = new System.Drawing.Size(616, 73);
      this.splitContainer.SplitterDistance = 306;
      this.splitContainer.TabIndex = 3;
      // 
      // shuffleSamplesCheckBox
      // 
      this.shuffleSamplesCheckBox.AutoSize = true;
      this.shuffleSamplesCheckBox.Location = new System.Drawing.Point(6, 52);
      this.shuffleSamplesCheckBox.Name = "shuffleSamplesCheckBox";
      this.shuffleSamplesCheckBox.Size = new System.Drawing.Size(102, 17);
      this.shuffleSamplesCheckBox.TabIndex = 5;
      this.shuffleSamplesCheckBox.Text = "Shuffle Samples";
      this.shuffleSamplesCheckBox.UseVisualStyleBackColor = true;
      this.shuffleSamplesCheckBox.CheckedChanged += new System.EventHandler(this.shuffleSamplesCheckBox_CheckedChanged);
      // 
      // CrossValidationView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.executionTimeLabel);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.pauseButton);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.resetButton);
      this.Controls.Add(this.splitContainer);
      this.Name = "CrossValidationView";
      this.Size = new System.Drawing.Size(616, 538);
      this.Controls.SetChildIndex(this.splitContainer, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.foldsNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.workersNumericUpDown)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.algorithmTabPage.ResumeLayout(false);
      this.algorithmTabControl.ResumeLayout(false);
      this.algorithmProblemTabPage.ResumeLayout(false);
      this.algorithmParametersTabPage.ResumeLayout(false);
      this.resultsTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.runsTabPage.PerformLayout();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Button pauseButton;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.TextBox executionTimeTextBox;
    private System.Windows.Forms.Label executionTimeLabel;
    private System.Windows.Forms.NumericUpDown foldsNumericUpDown;
    private System.Windows.Forms.Label foldsLabel;
    private System.Windows.Forms.NumericUpDown workersNumericUpDown;
    private System.Windows.Forms.Label workersLabel;
    private System.Windows.Forms.Label samplesStartLabel;
    private Data.Views.StringConvertibleValueView samplesStartStringConvertibleValueView;
    private Data.Views.StringConvertibleValueView samplesEndStringConvertibleValueView;
    private System.Windows.Forms.Label samplesEndLabel;
    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage algorithmTabPage;
    private System.Windows.Forms.TabPage resultsTabPage;
    private System.Windows.Forms.Button openAlgorithmButton;
    private System.Windows.Forms.Button newAlgorithmButton;
    private Core.Views.NamedItemView algorithmNamedItemView;
    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl algorithmTabControl;
    private System.Windows.Forms.TabPage algorithmProblemTabPage;
    private System.Windows.Forms.TabPage algorithmParametersTabPage;
    private MainForm.WindowsForms.ViewHost algorithmProblemViewHost;
    private System.Windows.Forms.Button openProblemButton;
    private System.Windows.Forms.Button newProblemButton;
    private Core.Views.ParameterCollectionView algorithmParameterCollectionView;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabPage runsTabPage;
    private Optimization.Views.RunCollectionView runCollectionView;
    private System.Windows.Forms.CheckBox storeAlgorithmInEachRunCheckBox;
    private System.Windows.Forms.SplitContainer splitContainer;
    private Optimization.Views.ResultCollectionView resultCollectionView;
    private System.Windows.Forms.CheckBox shuffleSamplesCheckBox;
  }
}
