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
  partial class BatchRunView {
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
        if (algorithmTypeSelectorDialog != null) algorithmTypeSelectorDialog.Dispose();
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
      this.algorithmTabPage = new System.Windows.Forms.TabPage();
      this.algorithmPanel = new System.Windows.Forms.Panel();
      this.algorithmViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.openAlgorithmButton = new System.Windows.Forms.Button();
      this.newAlgorithmButton = new System.Windows.Forms.Button();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.runsView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.startButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.repetitionsLabel = new System.Windows.Forms.Label();
      this.repetitionsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.pauseButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.algorithmTabPage.SuspendLayout();
      this.algorithmPanel.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.repetitionsNumericUpDown)).BeginInit();
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
      this.tabControl.Controls.Add(this.algorithmTabPage);
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 78);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(679, 374);
      this.tabControl.TabIndex = 6;
      // 
      // algorithmTabPage
      // 
      this.algorithmTabPage.Controls.Add(this.algorithmPanel);
      this.algorithmTabPage.Controls.Add(this.openAlgorithmButton);
      this.algorithmTabPage.Controls.Add(this.newAlgorithmButton);
      this.algorithmTabPage.Location = new System.Drawing.Point(4, 22);
      this.algorithmTabPage.Name = "algorithmTabPage";
      this.algorithmTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.algorithmTabPage.Size = new System.Drawing.Size(671, 348);
      this.algorithmTabPage.TabIndex = 1;
      this.algorithmTabPage.Text = "Algorithm";
      this.algorithmTabPage.UseVisualStyleBackColor = true;
      // 
      // algorithmPanel
      // 
      this.algorithmPanel.AllowDrop = true;
      this.algorithmPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmPanel.Controls.Add(this.algorithmViewHost);
      this.algorithmPanel.Location = new System.Drawing.Point(6, 36);
      this.algorithmPanel.Name = "algorithmPanel";
      this.algorithmPanel.Size = new System.Drawing.Size(659, 306);
      this.algorithmPanel.TabIndex = 3;
      this.algorithmPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.algorithmPanel_DragEnterOver);
      this.algorithmPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.algorithmPanel_DragDrop);
      this.algorithmPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.algorithmPanel_DragEnterOver);
      // 
      // algorithmViewHost
      // 
      this.algorithmViewHost.Content = null;
      this.algorithmViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.algorithmViewHost.Location = new System.Drawing.Point(0, 0);
      this.algorithmViewHost.Name = "algorithmViewHost";
      this.algorithmViewHost.Size = new System.Drawing.Size(659, 306);
      this.algorithmViewHost.TabIndex = 0;
      this.algorithmViewHost.ViewType = null;
      // 
      // openAlgorithmButton
      // 
      this.openAlgorithmButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Open;
      this.openAlgorithmButton.Location = new System.Drawing.Point(36, 6);
      this.openAlgorithmButton.Name = "openAlgorithmButton";
      this.openAlgorithmButton.Size = new System.Drawing.Size(24, 24);
      this.openAlgorithmButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.openAlgorithmButton, "Open Algorithm");
      this.openAlgorithmButton.UseVisualStyleBackColor = true;
      this.openAlgorithmButton.Click += new System.EventHandler(this.openAlgorithmButton_Click);
      // 
      // newAlgorithmButton
      // 
      this.newAlgorithmButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.NewDocument;
      this.newAlgorithmButton.Location = new System.Drawing.Point(6, 6);
      this.newAlgorithmButton.Name = "newAlgorithmButton";
      this.newAlgorithmButton.Size = new System.Drawing.Size(24, 24);
      this.newAlgorithmButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newAlgorithmButton, "New Algorithm");
      this.newAlgorithmButton.UseVisualStyleBackColor = true;
      this.newAlgorithmButton.Click += new System.EventHandler(this.newAlgorithmButton_Click);
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.runsView);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(671, 348);
      this.runsTabPage.TabIndex = 2;
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
      this.runsView.Size = new System.Drawing.Size(659, 336);
      this.runsView.TabIndex = 0;
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Play;
      this.startButton.Location = new System.Drawing.Point(0, 458);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(24, 24);
      this.startButton.TabIndex = 7;
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Batch Run");
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
      this.stopButton.TabIndex = 9;
      this.toolTip.SetToolTip(this.stopButton, "Stop Batch Run");
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
      this.resetButton.TabIndex = 10;
      this.toolTip.SetToolTip(this.resetButton, "Reset Batch Run");
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
      this.executionTimeLabel.TabIndex = 11;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(542, 462);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(137, 20);
      this.executionTimeTextBox.TabIndex = 12;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "Algorithm";
      this.openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      this.openFileDialog.Title = "Open Algorithm";
      // 
      // repetitionsLabel
      // 
      this.repetitionsLabel.AutoSize = true;
      this.repetitionsLabel.Location = new System.Drawing.Point(3, 54);
      this.repetitionsLabel.Name = "repetitionsLabel";
      this.repetitionsLabel.Size = new System.Drawing.Size(63, 13);
      this.repetitionsLabel.TabIndex = 4;
      this.repetitionsLabel.Text = "&Repetitions:";
      // 
      // repetitionsNumericUpDown
      // 
      this.repetitionsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.repetitionsNumericUpDown.Location = new System.Drawing.Point(72, 52);
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
      this.repetitionsNumericUpDown.Size = new System.Drawing.Size(607, 20);
      this.repetitionsNumericUpDown.TabIndex = 5;
      this.repetitionsNumericUpDown.ThousandsSeparator = true;
      this.repetitionsNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.repetitionsNumericUpDown.ValueChanged += new System.EventHandler(this.repetitionsNumericUpDown_ValueChanged);
      this.repetitionsNumericUpDown.Validated += new System.EventHandler(this.repetitionsNumericUpDown_Validated);
      // 
      // pauseButton
      // 
      this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.pauseButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Pause;
      this.pauseButton.Location = new System.Drawing.Point(30, 458);
      this.pauseButton.Name = "pauseButton";
      this.pauseButton.Size = new System.Drawing.Size(24, 24);
      this.pauseButton.TabIndex = 8;
      this.toolTip.SetToolTip(this.pauseButton, "Pause Batch Run");
      this.pauseButton.UseVisualStyleBackColor = true;
      this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
      // 
      // BatchRunView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.executionTimeLabel);
      this.Controls.Add(this.repetitionsNumericUpDown);
      this.Controls.Add(this.repetitionsLabel);
      this.Controls.Add(this.pauseButton);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.resetButton);
      this.Name = "BatchRunView";
      this.Size = new System.Drawing.Size(679, 482);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.repetitionsLabel, 0);
      this.Controls.SetChildIndex(this.repetitionsNumericUpDown, 0);
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
      this.algorithmTabPage.ResumeLayout(false);
      this.algorithmPanel.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.repetitionsNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage algorithmTabPage;
    private HeuristicLab.MainForm.WindowsForms.ViewHost algorithmViewHost;
    private System.Windows.Forms.Button newAlgorithmButton;
    private System.Windows.Forms.Button openAlgorithmButton;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.Label executionTimeLabel;
    private System.Windows.Forms.TextBox executionTimeTextBox;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabPage runsTabPage;
    private System.Windows.Forms.Label repetitionsLabel;
    private System.Windows.Forms.NumericUpDown repetitionsNumericUpDown;
    private RunCollectionView runsView;
    private System.Windows.Forms.Button pauseButton;
    private System.Windows.Forms.Panel algorithmPanel;

  }
}
