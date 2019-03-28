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

namespace HeuristicLab.Optimization.Views {
  partial class ExperimentListView {
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
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.optimizersTabPage = new System.Windows.Forms.TabPage();
      this.optimizerListView = new HeuristicLab.Optimization.Views.OptimizerListView();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.runsViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.optimizersTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // startButton
      // 
      this.startButton.Location = new System.Drawing.Point(0, 458);
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Experiment");
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(542, 461);
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Location = new System.Drawing.Point(453, 464);
      // 
      // pauseButton
      // 
      this.pauseButton.Location = new System.Drawing.Point(30, 458);
      this.toolTip.SetToolTip(this.pauseButton, "Pause Experiment");
      // 
      // stopButton
      // 
      this.stopButton.Location = new System.Drawing.Point(60, 458);
      this.toolTip.SetToolTip(this.stopButton, "Stop Experiment");
      // 
      // resetButton
      // 
      this.resetButton.Location = new System.Drawing.Point(90, 458);
      this.toolTip.SetToolTip(this.resetButton, "Reset Experiment");
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
      this.tabControl.Controls.Add(this.optimizersTabPage);
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 26);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(679, 427);
      this.tabControl.TabIndex = 3;
      // 
      // optimizersTabPage
      // 
      this.optimizersTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.optimizersTabPage.Controls.Add(this.optimizerListView);
      this.optimizersTabPage.Location = new System.Drawing.Point(4, 22);
      this.optimizersTabPage.Name = "optimizersTabPage";
      this.optimizersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.optimizersTabPage.Size = new System.Drawing.Size(671, 401);
      this.optimizersTabPage.TabIndex = 1;
      this.optimizersTabPage.Text = "Optimizers";
      // 
      // optimizerListView
      // 
      this.optimizerListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.optimizerListView.Caption = "OptimizerList View";
      this.optimizerListView.Content = null;
      this.optimizerListView.Location = new System.Drawing.Point(6, 6);
      this.optimizerListView.Name = "optimizerListView";
      this.optimizerListView.ReadOnly = false;
      this.optimizerListView.Size = new System.Drawing.Size(659, 389);
      this.optimizerListView.TabIndex = 0;
      // 
      // runsTabPage
      // 
      this.runsTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.runsTabPage.Controls.Add(this.runsViewHost);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.runsTabPage.Size = new System.Drawing.Size(671, 401);
      this.runsTabPage.TabIndex = 2;
      this.runsTabPage.Text = "Runs";
      // 
      // runsViewHost
      // 
      this.runsViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.runsViewHost.Caption = "View";
      this.runsViewHost.Content = null;
      this.runsViewHost.Enabled = false;
      this.runsViewHost.Location = new System.Drawing.Point(2, 6);
      this.runsViewHost.Name = "runsViewHost";
      this.runsViewHost.ReadOnly = false;
      this.runsViewHost.Size = new System.Drawing.Size(657, 389);
      this.runsViewHost.TabIndex = 0;
      this.runsViewHost.ViewsLabelVisible = true;
      this.runsViewHost.ViewType = null;
      // 
      // ExperimentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.tabControl);
      this.Name = "ExperimentView";
      this.Size = new System.Drawing.Size(679, 482);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.optimizersTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage optimizersTabPage;
    private OptimizerListView optimizerListView;
    private System.Windows.Forms.TabPage runsTabPage;
    private HeuristicLab.MainForm.WindowsForms.ViewHost runsViewHost;

  }
}
