#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  partial class SlaveView {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.slaveStats = new HeuristicLab.Clients.Hive.SlaveCore.Views.SlaveStats();
      this.jobsView = new HeuristicLab.Clients.Hive.SlaveCore.Views.TaskView();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(3, 3);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.slaveStats);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.jobsView);
      this.splitContainer.Size = new System.Drawing.Size(514, 314);
      this.splitContainer.SplitterDistance = 200;
      this.splitContainer.TabIndex = 2;
      // 
      // slaveStats
      // 
      this.slaveStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.slaveStats.Caption = "HeuristicLab Slave View";
      this.slaveStats.Content = null;
      this.slaveStats.Location = new System.Drawing.Point(3, 3);
      this.slaveStats.Name = "slaveStats";
      this.slaveStats.ReadOnly = false;
      this.slaveStats.Size = new System.Drawing.Size(508, 194);
      this.slaveStats.TabIndex = 0;
      // 
      // jobsView
      // 
      this.jobsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.jobsView.Caption = "Jobs View";
      this.jobsView.Content = null;
      this.jobsView.Location = new System.Drawing.Point(3, 3);
      this.jobsView.Name = "jobsView";
      this.jobsView.ReadOnly = false;
      this.jobsView.Size = new System.Drawing.Size(508, 104);
      this.jobsView.TabIndex = 0;
      // 
      // SlaveView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "SlaveView";
      this.Size = new System.Drawing.Size(520, 320);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private SlaveStats slaveStats;
    private System.Windows.Forms.SplitContainer splitContainer;
    private TaskView jobsView;

  }
}
