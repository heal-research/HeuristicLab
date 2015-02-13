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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.Views {
  partial class VariableTrackbar {
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
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.valueLabel = new System.Windows.Forms.Label();
      this.valueTextBox = new System.Windows.Forms.TextBox();
      this.trackBar = new System.Windows.Forms.TrackBar();
      this.boxPlotChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.groupBox = new System.Windows.Forms.GroupBox();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.boxPlotChart)).BeginInit();
      this.groupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // valueLabel
      // 
      this.valueLabel.AutoSize = true;
      this.valueLabel.Location = new System.Drawing.Point(6, 22);
      this.valueLabel.Name = "valueLabel";
      this.valueLabel.Size = new System.Drawing.Size(37, 13);
      this.valueLabel.TabIndex = 0;
      this.valueLabel.Text = "Value:";
      // 
      // valueTextBox
      // 
      this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.valueTextBox.Location = new System.Drawing.Point(49, 19);
      this.valueTextBox.Name = "valueTextBox";
      this.valueTextBox.ReadOnly = true;
      this.valueTextBox.Size = new System.Drawing.Size(56, 20);
      this.valueTextBox.TabIndex = 1;
      // 
      // trackBar
      // 
      this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.trackBar.Location = new System.Drawing.Point(6, 45);
      this.trackBar.Name = "trackBar";
      this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
      this.trackBar.Size = new System.Drawing.Size(45, 277);
      this.trackBar.TabIndex = 2;
      this.trackBar.TickStyle = System.Windows.Forms.TickStyle.None;
      this.trackBar.ValueChanged += new System.EventHandler(this.TrackBarValueChanged);
      // 
      // boxPlotChart
      // 
      this.boxPlotChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.boxPlotChart.BackColor = System.Drawing.SystemColors.Control;
      chartArea1.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
      chartArea1.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
      chartArea1.BackColor = System.Drawing.SystemColors.Control;
      chartArea1.BackSecondaryColor = System.Drawing.Color.White;
      chartArea1.Name = "ChartArea";
      this.boxPlotChart.ChartAreas.Add(chartArea1);
      this.boxPlotChart.Location = new System.Drawing.Point(39, 45);
      this.boxPlotChart.Name = "boxPlotChart";
      series1.ChartArea = "ChartArea";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.BoxPlot;
      series1.Name = "BoxPlot";
      series1.YValuesPerPoint = 6;
      series2.ChartArea = "ChartArea";
      series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series2.Enabled = false;
      series2.Name = "DataSeries";
      this.boxPlotChart.Series.Add(series1);
      this.boxPlotChart.Series.Add(series2);
      this.boxPlotChart.Size = new System.Drawing.Size(66, 277);
      this.boxPlotChart.TabIndex = 3;
      this.boxPlotChart.Text = "boxPlotChart";
      // 
      // groupBox
      // 
      this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox.Controls.Add(this.valueLabel);
      this.groupBox.Controls.Add(this.boxPlotChart);
      this.groupBox.Controls.Add(this.valueTextBox);
      this.groupBox.Controls.Add(this.trackBar);
      this.groupBox.Location = new System.Drawing.Point(3, 3);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(111, 328);
      this.groupBox.TabIndex = 4;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "groupBox";
      // 
      // VariableTrackbar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.groupBox);
      this.Name = "VariableTrackbar";
      this.Size = new System.Drawing.Size(117, 334);
      ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.boxPlotChart)).EndInit();
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label valueLabel;
    private System.Windows.Forms.TextBox valueTextBox;
    private System.Windows.Forms.TrackBar trackBar;
    private System.Windows.Forms.DataVisualization.Charting.Chart boxPlotChart;
    private System.Windows.Forms.GroupBox groupBox;
  }
}
