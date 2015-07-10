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

namespace HeuristicLab.Visualization.ChartControlsExtensions {
  partial class GanttChart {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      this.SuspendLayout();
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      chartArea1.AxisX.ScaleView.SmallScrollMinSizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Milliseconds;
      chartArea1.AxisY.ScaleView.SmallScrollMinSizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Milliseconds;
      chartArea1.CursorX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Milliseconds;
      chartArea1.CursorY.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Milliseconds;
      chartArea1.Name = "ChartArea1";
      this.chart.ChartAreas.Add(chartArea1);
      legend1.Name = "Legend";
      this.chart.Legends.Add(legend1);
      this.chart.Location = new System.Drawing.Point(0, 0);
      this.chart.Name = "chart";
      series1.ChartArea = "ChartArea1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.RangeBar;
      series1.CustomProperties = "DrawingStyle=Cylinder";
      series1.IsVisibleInLegend = false;
      series1.Legend = "Legend";
      series1.Name = "Series1";
      series1.YValuesPerPoint = 2;
      series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
      this.chart.Series.Add(series1);
      this.chart.Size = new System.Drawing.Size(687, 347);
      this.chart.TabIndex = 0;
      this.chart.Text = "chart";
      // 
      // GanttChart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.chart);
      this.Name = "GanttChart";
      this.Size = new System.Drawing.Size(687, 347);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    public Visualization.ChartControlsExtensions.EnhancedChart chart;


  }
}
