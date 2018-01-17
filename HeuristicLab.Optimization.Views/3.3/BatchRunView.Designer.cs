#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchRunView));
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.optimizerTabPage = new System.Windows.Forms.TabPage();
      this.optimizerViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.openOptimizerButton = new System.Windows.Forms.Button();
      this.newOptimizerButton = new System.Windows.Forms.Button();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.runsView = new HeuristicLab.Optimization.Views.RunCollectionView();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.repetitionsLabel = new System.Windows.Forms.Label();
      this.repetitionsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.optimizerTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.repetitionsNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // startButton
      // 
      this.startButton.Location = new System.Drawing.Point(0, 459);
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Batchrun");
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
      this.pauseButton.Location = new System.Drawing.Point(30, 459);
      this.toolTip.SetToolTip(this.pauseButton, "Pause Batchrun");
      // 
      // stopButton
      // 
      this.stopButton.Location = new System.Drawing.Point(60, 459);
      this.toolTip.SetToolTip(this.stopButton, "Stop Batchrun");
      // 
      // resetButton
      // 
      this.resetButton.Location = new System.Drawing.Point(90, 458);
      this.toolTip.SetToolTip(this.resetButton, "Reset Batchrun");
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(72, 0);
      this.nameTextBox.Size = new System.Drawing.Size(582, 20);
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
      this.tabControl.Controls.Add(this.optimizerTabPage);
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 52);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(679, 401);
      this.tabControl.TabIndex = 5;
      // 
      // optimizerTabPage
      // 
      this.optimizerTabPage.AllowDrop = true;
      this.optimizerTabPage.Controls.Add(this.optimizerViewHost);
      this.optimizerTabPage.Controls.Add(this.openOptimizerButton);
      this.optimizerTabPage.Controls.Add(this.newOptimizerButton);
      this.optimizerTabPage.Location = new System.Drawing.Point(4, 22);
      this.optimizerTabPage.Name = "optimizerTabPage";
      this.optimizerTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.optimizerTabPage.Size = new System.Drawing.Size(671, 375);
      this.optimizerTabPage.TabIndex = 1;
      this.optimizerTabPage.Text = "Optimizer";
      this.optimizerTabPage.UseVisualStyleBackColor = true;
      this.optimizerTabPage.DragDrop += new System.Windows.Forms.DragEventHandler(this.optimizerTabPage_DragDrop);
      this.optimizerTabPage.DragEnter += new System.Windows.Forms.DragEventHandler(this.optimizerTabPage_DragEnterOver);
      this.optimizerTabPage.DragOver += new System.Windows.Forms.DragEventHandler(this.optimizerTabPage_DragEnterOver);
      // 
      // optimizerViewHost
      // 
      this.optimizerViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.optimizerViewHost.Caption = "View";
      this.optimizerViewHost.Content = null;
      this.optimizerViewHost.Enabled = false;
      this.optimizerViewHost.Location = new System.Drawing.Point(6, 36);
      this.optimizerViewHost.Name = "optimizerViewHost";
      this.optimizerViewHost.ReadOnly = false;
      this.optimizerViewHost.Size = new System.Drawing.Size(659, 333);
      this.optimizerViewHost.TabIndex = 3;
      this.optimizerViewHost.ViewsLabelVisible = true;
      this.optimizerViewHost.ViewType = null;
      // 
      // openOptimizerButton
      // 
      this.openOptimizerButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.openOptimizerButton.Location = new System.Drawing.Point(36, 6);
      this.openOptimizerButton.Name = "openOptimizerButton";
      this.openOptimizerButton.Size = new System.Drawing.Size(24, 24);
      this.openOptimizerButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.openOptimizerButton, "Open Optimizer");
      this.openOptimizerButton.UseVisualStyleBackColor = true;
      this.openOptimizerButton.Click += new System.EventHandler(this.openOptimizerButton_Click);
      // 
      // newOptimizerButton
      // 
      this.newOptimizerButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.NewDocument;
      this.newOptimizerButton.Location = new System.Drawing.Point(6, 6);
      this.newOptimizerButton.Name = "newOptimizerButton";
      this.newOptimizerButton.Size = new System.Drawing.Size(24, 24);
      this.newOptimizerButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newOptimizerButton, "New Optimizer");
      this.newOptimizerButton.UseVisualStyleBackColor = true;
      this.newOptimizerButton.Click += new System.EventHandler(this.newOptimizerButton_Click);
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.runsView);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(671, 374);
      this.runsTabPage.TabIndex = 2;
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
      this.runsView.Size = new System.Drawing.Size(659, 362);
      this.runsView.TabIndex = 0;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "Optimizer";
      this.openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      this.openFileDialog.Title = "Open Optimizer";
      // 
      // repetitionsLabel
      // 
      this.repetitionsLabel.AutoSize = true;
      this.repetitionsLabel.Location = new System.Drawing.Point(3, 28);
      this.repetitionsLabel.Name = "repetitionsLabel";
      this.repetitionsLabel.Size = new System.Drawing.Size(63, 13);
      this.repetitionsLabel.TabIndex = 3;
      this.repetitionsLabel.Text = "&Repetitions:";
      // 
      // repetitionsNumericUpDown
      // 
      this.repetitionsNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.repetitionsNumericUpDown.Location = new System.Drawing.Point(72, 26);
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
      this.repetitionsNumericUpDown.TabIndex = 4;
      this.repetitionsNumericUpDown.ThousandsSeparator = true;
      this.repetitionsNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.repetitionsNumericUpDown.ValueChanged += new System.EventHandler(this.repetitionsNumericUpDown_ValueChanged);
      this.repetitionsNumericUpDown.Validated += new System.EventHandler(this.repetitionsNumericUpDown_Validated);
      // 
      // BatchRunView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.repetitionsNumericUpDown);
      this.Controls.Add(this.repetitionsLabel);
      this.Controls.Add(this.tabControl);
      this.Name = "BatchRunView";
      this.Size = new System.Drawing.Size(679, 482);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.repetitionsLabel, 0);
      this.Controls.SetChildIndex(this.repetitionsNumericUpDown, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.optimizerTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.repetitionsNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage optimizerTabPage;
    private HeuristicLab.MainForm.WindowsForms.ViewHost optimizerViewHost;
    private System.Windows.Forms.Button newOptimizerButton;
    private System.Windows.Forms.Button openOptimizerButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabPage runsTabPage;
    private System.Windows.Forms.Label repetitionsLabel;
    private System.Windows.Forms.NumericUpDown repetitionsNumericUpDown;
    private RunCollectionView runsView;
  }
}
