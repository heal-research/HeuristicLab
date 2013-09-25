#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class OptimizerHiveTaskView {
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
      this.restartButton = new System.Windows.Forms.Button();
      this.pauseButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.runsTabPage = new System.Windows.Forms.TabPage();
      this.runCollectionViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.stateLogTabPage.SuspendLayout();
      this.detailsTabPage.SuspendLayout();
      this.jobStatusGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.configurationGroupBox.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // detailsTabPage
      // 
      this.detailsTabPage.Controls.Add(this.restartButton);
      this.detailsTabPage.Controls.Add(this.pauseButton);
      this.detailsTabPage.Controls.Add(this.stopButton);
      this.detailsTabPage.Controls.SetChildIndex(this.modifyItemButton, 0);
      this.detailsTabPage.Controls.SetChildIndex(this.stopButton, 0);
      this.detailsTabPage.Controls.SetChildIndex(this.pauseButton, 0);
      this.detailsTabPage.Controls.SetChildIndex(this.restartButton, 0);
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.runsTabPage);
      this.tabControl.Controls.SetChildIndex(this.runsTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.stateLogTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.detailsTabPage, 0);
      // 
      // modifyItemButton
      // 
      this.modifyItemButton.Text = "Show/Modify Optimizer";
      // 
      // restartButton
      // 
      this.restartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.restartButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Restart;
      this.restartButton.Location = new System.Drawing.Point(3, 348);
      this.restartButton.Name = "restartButton";
      this.restartButton.Size = new System.Drawing.Size(24, 24);
      this.restartButton.TabIndex = 34;
      this.restartButton.UseVisualStyleBackColor = true;
      this.restartButton.Click += new System.EventHandler(this.restartButton_Click);
      // 
      // pauseButton
      // 
      this.pauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.pauseButton.Enabled = false;
      this.pauseButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Pause;
      this.pauseButton.Location = new System.Drawing.Point(33, 348);
      this.pauseButton.Name = "pauseButton";
      this.pauseButton.Size = new System.Drawing.Size(24, 24);
      this.pauseButton.TabIndex = 35;
      this.pauseButton.UseVisualStyleBackColor = true;
      this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Stop;
      this.stopButton.Location = new System.Drawing.Point(63, 348);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(24, 24);
      this.stopButton.TabIndex = 36;
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // runsTabPage
      // 
      this.runsTabPage.Controls.Add(this.runCollectionViewHost);
      this.runsTabPage.Location = new System.Drawing.Point(4, 22);
      this.runsTabPage.Name = "runsTabPage";
      this.runsTabPage.Size = new System.Drawing.Size(563, 375);
      this.runsTabPage.TabIndex = 6;
      this.runsTabPage.Text = "Runs";
      this.runsTabPage.UseVisualStyleBackColor = true;
      // 
      // runCollectionViewHost
      // 
      this.runCollectionViewHost.Caption = "View";
      this.runCollectionViewHost.Content = null;
      this.runCollectionViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.runCollectionViewHost.Enabled = false;
      this.runCollectionViewHost.Location = new System.Drawing.Point(0, 0);
      this.runCollectionViewHost.Name = "runCollectionViewHost";
      this.runCollectionViewHost.ReadOnly = false;
      this.runCollectionViewHost.Size = new System.Drawing.Size(563, 375);
      this.runCollectionViewHost.TabIndex = 1;
      this.runCollectionViewHost.ViewsLabelVisible = true;
      this.runCollectionViewHost.ViewType = null;
      // 
      // OptimizerHiveTaskView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Name = "OptimizerHiveTaskView";
      this.stateLogTabPage.ResumeLayout(false);
      this.detailsTabPage.ResumeLayout(false);
      this.jobStatusGroupBox.ResumeLayout(false);
      this.jobStatusGroupBox.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.configurationGroupBox.ResumeLayout(false);
      this.configurationGroupBox.PerformLayout();
      this.runsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button restartButton;
    private System.Windows.Forms.Button pauseButton;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.TabPage runsTabPage;
    private MainForm.WindowsForms.ViewHost runCollectionViewHost;


  }
}
