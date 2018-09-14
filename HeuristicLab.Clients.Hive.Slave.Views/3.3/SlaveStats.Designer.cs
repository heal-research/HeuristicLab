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

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  partial class SlaveStats {
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
      System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel1 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
      System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel2 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
      System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel3 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
      System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel4 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.taskChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.coresChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.label1 = new System.Windows.Forms.Label();
      this.txtSlaveState = new System.Windows.Forms.TextBox();
      this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
      this.coresGroupBox = new System.Windows.Forms.GroupBox();
      this.groupBoxTaskChart = new System.Windows.Forms.GroupBox();
      ((System.ComponentModel.ISupportInitialize)(this.taskChart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.coresChart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
      this.mainSplitContainer.Panel1.SuspendLayout();
      this.mainSplitContainer.Panel2.SuspendLayout();
      this.mainSplitContainer.SuspendLayout();
      this.coresGroupBox.SuspendLayout();
      this.groupBoxTaskChart.SuspendLayout();
      this.SuspendLayout();
      // 
      // taskChart
      // 
      this.taskChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      customLabel1.Text = "Jobs";
      customLabel2.Text = "Aborted Jobs";
      customLabel3.Text = "Finished Jobs";
      customLabel4.Text = "Fetched Jobs";
      chartArea1.AxisX.CustomLabels.Add(customLabel1);
      chartArea1.AxisX.CustomLabels.Add(customLabel2);
      chartArea1.AxisX.CustomLabels.Add(customLabel3);
      chartArea1.AxisX.CustomLabels.Add(customLabel4);
      chartArea1.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
      chartArea1.AxisX.MajorGrid.Enabled = false;
      chartArea1.AxisX.MajorTickMark.Enabled = false;
      chartArea1.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
      chartArea1.AxisY.Title = "Number of...";
      chartArea1.InnerPlotPosition.Auto = false;
      chartArea1.InnerPlotPosition.Height = 97F;
      chartArea1.InnerPlotPosition.Width = 97F;
      chartArea1.InnerPlotPosition.X = 3F;
      chartArea1.InnerPlotPosition.Y = 3F;
      chartArea1.Name = "ChartArea1";
      this.taskChart.ChartAreas.Add(chartArea1);
      legend1.Name = "Legend1";
      this.taskChart.Legends.Add(legend1);
      this.taskChart.Location = new System.Drawing.Point(6, 19);
      this.taskChart.Name = "taskChart";
      series1.ChartArea = "ChartArea1";
      series1.Legend = "Legend1";
      series1.Name = "Series1";
      series2.ChartArea = "ChartArea1";
      series2.Legend = "Legend1";
      series2.Name = "Series2";
      series3.ChartArea = "ChartArea1";
      series3.Legend = "Legend1";
      series3.Name = "Series3";
      series4.ChartArea = "ChartArea1";
      series4.Legend = "Legend1";
      series4.Name = "Series4";
      series5.ChartArea = "ChartArea1";
      series5.Legend = "Legend1";
      series5.Name = "Series5";
      this.taskChart.Series.Add(series1);
      this.taskChart.Series.Add(series2);
      this.taskChart.Series.Add(series3);
      this.taskChart.Series.Add(series4);
      this.taskChart.Series.Add(series5);
      this.taskChart.Size = new System.Drawing.Size(406, 178);
      this.taskChart.TabIndex = 7;
      // 
      // coresChart
      // 
      this.coresChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      chartArea2.Name = "ChartArea1";
      this.coresChart.ChartAreas.Add(chartArea2);
      legend2.Name = "Legend1";
      this.coresChart.Legends.Add(legend2);
      this.coresChart.Location = new System.Drawing.Point(6, 19);
      this.coresChart.Name = "coresChart";
      series6.ChartArea = "ChartArea1";
      series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
      series6.Legend = "Legend1";
      series6.Name = "Series1";
      this.coresChart.Series.Add(series6);
      this.coresChart.Size = new System.Drawing.Size(187, 72);
      this.coresChart.TabIndex = 9;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(5, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(65, 13);
      this.label1.TabIndex = 14;
      this.label1.Text = "Slave State:";
      // 
      // txtSlaveState
      // 
      this.txtSlaveState.Enabled = false;
      this.txtSlaveState.Location = new System.Drawing.Point(76, 3);
      this.txtSlaveState.Name = "txtSlaveState";
      this.txtSlaveState.Size = new System.Drawing.Size(100, 20);
      this.txtSlaveState.TabIndex = 15;
      // 
      // mainSplitContainer
      // 
      this.mainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.mainSplitContainer.Location = new System.Drawing.Point(3, 3);
      this.mainSplitContainer.Name = "mainSplitContainer";
      this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // mainSplitContainer.Panel1
      // 
      this.mainSplitContainer.Panel1.Controls.Add(this.coresGroupBox);
      this.mainSplitContainer.Panel1.Controls.Add(this.txtSlaveState);
      this.mainSplitContainer.Panel1.Controls.Add(this.label1);
      // 
      // mainSplitContainer.Panel2
      // 
      this.mainSplitContainer.Panel2.Controls.Add(this.groupBoxTaskChart);
      this.mainSplitContainer.Size = new System.Drawing.Size(424, 319);
      this.mainSplitContainer.SplitterDistance = 106;
      this.mainSplitContainer.TabIndex = 16;
      // 
      // coresGroupBox
      // 
      this.coresGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.coresGroupBox.Controls.Add(this.coresChart);
      this.coresGroupBox.Location = new System.Drawing.Point(216, 6);
      this.coresGroupBox.Name = "coresGroupBox";
      this.coresGroupBox.Size = new System.Drawing.Size(199, 97);
      this.coresGroupBox.TabIndex = 0;
      this.coresGroupBox.TabStop = false;
      this.coresGroupBox.Text = "Cores";
      // 
      // groupBoxTaskChart
      // 
      this.groupBoxTaskChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBoxTaskChart.Controls.Add(this.taskChart);
      this.groupBoxTaskChart.Location = new System.Drawing.Point(3, 3);
      this.groupBoxTaskChart.Name = "groupBoxTaskChart";
      this.groupBoxTaskChart.Size = new System.Drawing.Size(418, 203);
      this.groupBoxTaskChart.TabIndex = 0;
      this.groupBoxTaskChart.TabStop = false;
      this.groupBoxTaskChart.Text = "Tasks";
      // 
      // SlaveStats
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mainSplitContainer);
      this.Name = "SlaveStats";
      this.Size = new System.Drawing.Size(430, 325);
      ((System.ComponentModel.ISupportInitialize)(this.taskChart)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.coresChart)).EndInit();
      this.mainSplitContainer.Panel1.ResumeLayout(false);
      this.mainSplitContainer.Panel1.PerformLayout();
      this.mainSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
      this.mainSplitContainer.ResumeLayout(false);
      this.coresGroupBox.ResumeLayout(false);
      this.groupBoxTaskChart.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataVisualization.Charting.Chart taskChart;
    private System.Windows.Forms.DataVisualization.Charting.Chart coresChart;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtSlaveState;
    private System.Windows.Forms.SplitContainer mainSplitContainer;
    private System.Windows.Forms.GroupBox groupBoxTaskChart;
    private System.Windows.Forms.GroupBox coresGroupBox;
  }
}
