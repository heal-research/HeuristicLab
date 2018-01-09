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

namespace HeuristicLab.Analysis.Views {
  partial class GanttDataView {
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
      this.ganttChart = new HeuristicLab.Visualization.ChartControlsExtensions.GanttChart();
      this.SuspendLayout();
      // 
      // ganttChart
      // 
      this.ganttChart.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ganttChart.Location = new System.Drawing.Point(0, 0);
      this.ganttChart.Name = "ganttChart";
      this.ganttChart.Size = new System.Drawing.Size(367, 249);
      this.ganttChart.TabIndex = 0;
      // 
      // GanttDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.ganttChart);
      this.Name = "GanttDataView";
      this.Size = new System.Drawing.Size(367, 249);
      this.ResumeLayout(false);

    }

    #endregion

    private Visualization.ChartControlsExtensions.GanttChart ganttChart;
  }
}
