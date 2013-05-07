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

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  partial class TaskView {
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
      this.lstJobs = new System.Windows.Forms.ListView();
      this.columnTaskId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnExecutionTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.SuspendLayout();
      // 
      // lstJobs
      // 
      this.lstJobs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lstJobs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnTaskId,
            this.columnExecutionTime});
      this.lstJobs.Location = new System.Drawing.Point(3, 3);
      this.lstJobs.Name = "lstJobs";
      this.lstJobs.Size = new System.Drawing.Size(380, 164);
      this.lstJobs.TabIndex = 0;
      this.lstJobs.UseCompatibleStateImageBehavior = false;
      this.lstJobs.View = System.Windows.Forms.View.Details;
      // 
      // columnTaskId
      // 
      this.columnTaskId.Text = "Task Id";
      this.columnTaskId.Width = 120;
      // 
      // columnExecutionTime
      // 
      this.columnExecutionTime.Text = "Execution Time";
      this.columnExecutionTime.Width = 120;
      // 
      // JobsView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lstJobs);
      this.Name = "JobsView";
      this.Size = new System.Drawing.Size(386, 170);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView lstJobs;
    private System.Windows.Forms.ColumnHeader columnTaskId;
    private System.Windows.Forms.ColumnHeader columnExecutionTime;
  }
}
