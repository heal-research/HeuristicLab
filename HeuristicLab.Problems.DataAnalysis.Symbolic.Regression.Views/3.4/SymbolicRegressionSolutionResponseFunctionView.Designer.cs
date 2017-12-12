#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class SymbolicRegressionSolutionResponseFunctionView {
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
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.responseChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.comboBox = new System.Windows.Forms.ComboBox();
      this.freeVariableLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.responseChart)).BeginInit();
      this.SuspendLayout();
      // 
      // flowLayoutPanel
      // 
      this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.flowLayoutPanel.AutoScroll = true;
      this.flowLayoutPanel.Location = new System.Drawing.Point(3, 202);
      this.flowLayoutPanel.Name = "flowLayoutPanel";
      this.flowLayoutPanel.Size = new System.Drawing.Size(261, 268);
      this.flowLayoutPanel.TabIndex = 2;
      this.flowLayoutPanel.WrapContents = false;
      // 
      // responseChart
      // 
      this.responseChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      chartArea1.Name = "ChartArea";
      this.responseChart.ChartAreas.Add(chartArea1);
      legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
      legend1.Name = "Legend";
      this.responseChart.Legends.Add(legend1);
      this.responseChart.Location = new System.Drawing.Point(3, 30);
      this.responseChart.Name = "responseChart";
      series1.ChartArea = "ChartArea";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
      series1.Legend = "Legend";
      series1.MarkerColor = System.Drawing.Color.Gold;
      series1.MarkerSize = 3;
      series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
      series1.Name = "Training Data (edge)";
      series2.ChartArea = "ChartArea";
      series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
      series2.Legend = "Legend";
      series2.MarkerColor = System.Drawing.Color.OrangeRed;
      series2.MarkerSize = 3;
      series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
      series2.Name = "Test Data (edge)";
      series3.ChartArea = "ChartArea";
      series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
      series3.Legend = "Legend";
      series3.MarkerColor = System.Drawing.Color.Gold;
      series3.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
      series3.Name = "Training Data";
      series4.ChartArea = "ChartArea";
      series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
      series4.Legend = "Legend";
      series4.MarkerColor = System.Drawing.Color.OrangeRed;
      series4.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
      series4.Name = "Test Data";
      series5.BorderWidth = 3;
      series5.ChartArea = "ChartArea";
      series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
      series5.Color = System.Drawing.Color.DodgerBlue;
      series5.Legend = "Legend";
      series5.MarkerSize = 1;
      series5.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Cross;
      series5.Name = "Model Response";
      series5.ShadowColor = System.Drawing.Color.DodgerBlue;
      this.responseChart.Series.Add(series1);
      this.responseChart.Series.Add(series2);
      this.responseChart.Series.Add(series3);
      this.responseChart.Series.Add(series4);
      this.responseChart.Series.Add(series5);
      this.responseChart.Size = new System.Drawing.Size(258, 166);
      this.responseChart.TabIndex = 3;
      // 
      // comboBox
      // 
      this.comboBox.FormattingEnabled = true;
      this.comboBox.Location = new System.Drawing.Point(80, 3);
      this.comboBox.Name = "comboBox";
      this.comboBox.Size = new System.Drawing.Size(166, 21);
      this.comboBox.TabIndex = 4;
      this.comboBox.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSelectedIndexChanged);
      // 
      // freeVariableLabel
      // 
      this.freeVariableLabel.AutoSize = true;
      this.freeVariableLabel.Location = new System.Drawing.Point(3, 6);
      this.freeVariableLabel.Name = "freeVariableLabel";
      this.freeVariableLabel.Size = new System.Drawing.Size(71, 13);
      this.freeVariableLabel.TabIndex = 5;
      this.freeVariableLabel.Text = "Free variable:";
      // 
      // SymbolicDataAnalysisSolutionResponseFunctionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.freeVariableLabel);
      this.Controls.Add(this.comboBox);
      this.Controls.Add(this.responseChart);
      this.Controls.Add(this.flowLayoutPanel);
      this.Name = "SymbolicDataAnalysisSolutionResponseFunctionView";
      this.Size = new System.Drawing.Size(264, 473);
      ((System.ComponentModel.ISupportInitialize)(this.responseChart)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
    private System.Windows.Forms.DataVisualization.Charting.Chart responseChart;
    private System.Windows.Forms.ComboBox comboBox;
    private System.Windows.Forms.Label freeVariableLabel;
  }
}
