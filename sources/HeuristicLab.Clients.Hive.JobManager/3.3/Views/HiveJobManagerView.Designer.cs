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
  partial class HiveJobManagerView {
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
      this.hiveExperimentListView = new RefreshableHiveJobListView();
      this.refreshButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // hiveExperimentListView
      // 
      this.hiveExperimentListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.hiveExperimentListView.Caption = "HiveExperimentList View";
      this.hiveExperimentListView.Content = null;
      this.hiveExperimentListView.Location = new System.Drawing.Point(3, 33);
      this.hiveExperimentListView.Name = "hiveExperimentListView";
      this.hiveExperimentListView.ReadOnly = false;
      this.hiveExperimentListView.Size = new System.Drawing.Size(729, 488);
      this.hiveExperimentListView.TabIndex = 0;
      // 
      // refreshButton
      // 
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(3, 3);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 1;
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // HiveExperimentManagerView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.hiveExperimentListView);
      this.Name = "JobManagerView";
      this.Size = new System.Drawing.Size(735, 524);
      this.ResumeLayout(false);

    }
    #endregion

    private RefreshableHiveJobListView hiveExperimentListView;
    private System.Windows.Forms.Button refreshButton;


  }
}
