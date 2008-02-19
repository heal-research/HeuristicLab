#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Charting.Gantt {
  partial class GanttControl {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.gridChartControl1 = new HeuristicLab.Charting.Grid.GridChartControl();
      this.SuspendLayout();
      // 
      // gridChartControl1
      // 
      this.gridChartControl1.BackColor = System.Drawing.SystemColors.Control;
      this.gridChartControl1.Chart = null;
      this.gridChartControl1.Location = new System.Drawing.Point(12, 12);
      this.gridChartControl1.Name = "gridChartControl1";
      this.gridChartControl1.ScaleOnResize = true;
      this.gridChartControl1.Size = new System.Drawing.Size(654, 351);
      this.gridChartControl1.TabIndex = 0;
      // 
      // TestControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(669, 367);
      this.Controls.Add(this.gridChartControl1);
      this.Name = "Gantt Chart";
      this.Text = "Gantt Chart";
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.Charting.Grid.GridChartControl gridChartControl1;
  }
}
