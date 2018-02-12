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

namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionBoxPlotView {
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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunCollectionBoxPlotView));
      this.xAxisLabel = new System.Windows.Forms.Label();
      this.xAxisComboBox = new System.Windows.Forms.ComboBox();
      this.yAxisLabel = new System.Windows.Forms.Label();
      this.yAxisComboBox = new System.Windows.Forms.ComboBox();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.openBubbleChartViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.noRunsLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.showStatisticsCheckBox = new System.Windows.Forms.CheckBox();
      this.statisticsGroupBox = new System.Windows.Forms.GroupBox();
      this.statisticsMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.tooltip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.statisticsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // xAxisLabel
      // 
      this.xAxisLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisLabel.AutoSize = true;
      this.xAxisLabel.Location = new System.Drawing.Point(363, 345);
      this.xAxisLabel.Name = "xAxisLabel";
      this.xAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.xAxisLabel.TabIndex = 12;
      this.xAxisLabel.Text = "x:";
      // 
      // xAxisComboBox
      // 
      this.xAxisComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.xAxisComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.xAxisComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.xAxisComboBox.FormattingEnabled = true;
      this.xAxisComboBox.Location = new System.Drawing.Point(384, 342);
      this.xAxisComboBox.Name = "xAxisComboBox";
      this.xAxisComboBox.Size = new System.Drawing.Size(600, 21);
      this.xAxisComboBox.TabIndex = 11;
      this.xAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.AxisComboBox_SelectedIndexChanged);
      // 
      // yAxisLabel
      // 
      this.yAxisLabel.AutoSize = true;
      this.yAxisLabel.Location = new System.Drawing.Point(3, 6);
      this.yAxisLabel.Name = "yAxisLabel";
      this.yAxisLabel.Size = new System.Drawing.Size(15, 13);
      this.yAxisLabel.TabIndex = 10;
      this.yAxisLabel.Text = "y:";
      // 
      // yAxisComboBox
      // 
      this.yAxisComboBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.yAxisComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.yAxisComboBox.FormattingEnabled = true;
      this.yAxisComboBox.Location = new System.Drawing.Point(24, 3);
      this.yAxisComboBox.Name = "yAxisComboBox";
      this.yAxisComboBox.Size = new System.Drawing.Size(600, 21);
      this.yAxisComboBox.TabIndex = 9;
      this.yAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.AxisComboBox_SelectedIndexChanged);
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      chartArea1.Name = "ChartArea1";
      this.chart.ChartAreas.Add(chartArea1);
      this.chart.Location = new System.Drawing.Point(-1, 30);
      this.chart.Name = "chart";
      series1.ChartArea = "ChartArea1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.BoxPlot;
      series1.IsVisibleInLegend = false;
      series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
      series1.Name = "DataSeries";
      series1.YValuesPerPoint = 6;
      this.chart.Series.Add(series1);
      this.chart.Size = new System.Drawing.Size(989, 306);
      this.chart.TabIndex = 17;
      this.chart.Text = "chart";
      this.chart.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.chart_AxisViewChanged);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      // 
      // openBubbleChartViewToolStripMenuItem
      // 
      this.openBubbleChartViewToolStripMenuItem.Name = "openBubbleChartViewToolStripMenuItem";
      this.openBubbleChartViewToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
      this.openBubbleChartViewToolStripMenuItem.Text = "Open BubbleChart View";
      this.openBubbleChartViewToolStripMenuItem.Click += new System.EventHandler(this.openBubbleChartViewToolStripMenuItem_Click);
      // 
      // noRunsLabel
      // 
      this.noRunsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.noRunsLabel.AutoSize = true;
      this.noRunsLabel.Location = new System.Drawing.Point(434, 167);
      this.noRunsLabel.Name = "noRunsLabel";
      this.noRunsLabel.Size = new System.Drawing.Size(138, 13);
      this.noRunsLabel.TabIndex = 22;
      this.noRunsLabel.Text = "No runs could be displayed.";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.showStatisticsCheckBox);
      this.splitContainer.Panel1.Controls.Add(this.xAxisLabel);
      this.splitContainer.Panel1.Controls.Add(this.noRunsLabel);
      this.splitContainer.Panel1.Controls.Add(this.yAxisLabel);
      this.splitContainer.Panel1.Controls.Add(this.xAxisComboBox);
      this.splitContainer.Panel1.Controls.Add(this.yAxisComboBox);
      this.splitContainer.Panel1.Controls.Add(this.chart);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.statisticsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(987, 550);
      this.splitContainer.SplitterDistance = 366;
      this.splitContainer.TabIndex = 23;
      // 
      // showStatisticsCheckBox
      // 
      this.showStatisticsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.showStatisticsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showStatisticsCheckBox.Checked = true;
      this.showStatisticsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showStatisticsCheckBox.Image = ((System.Drawing.Image)(resources.GetObject("showStatisticsCheckBox.Image")));
      this.showStatisticsCheckBox.Location = new System.Drawing.Point(3, 339);
      this.showStatisticsCheckBox.Name = "showStatisticsCheckBox";
      this.showStatisticsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showStatisticsCheckBox.TabIndex = 23;
      this.tooltip.SetToolTip(this.showStatisticsCheckBox, "Show/Hide Statistics");
      this.showStatisticsCheckBox.UseVisualStyleBackColor = true;
      this.showStatisticsCheckBox.CheckedChanged += new System.EventHandler(this.showStatisticsCheckBox_CheckedChanged);
      // 
      // statisticsGroupBox
      // 
      this.statisticsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.statisticsGroupBox.Controls.Add(this.statisticsMatrixView);
      this.statisticsGroupBox.Location = new System.Drawing.Point(3, 3);
      this.statisticsGroupBox.Name = "statisticsGroupBox";
      this.statisticsGroupBox.Size = new System.Drawing.Size(981, 174);
      this.statisticsGroupBox.TabIndex = 1;
      this.statisticsGroupBox.TabStop = false;
      this.statisticsGroupBox.Text = "Statistics";
      // 
      // statisticsMatrixView
      // 
      this.statisticsMatrixView.Caption = "StringConvertibleMatrix View";
      this.statisticsMatrixView.Content = null;
      this.statisticsMatrixView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.statisticsMatrixView.Location = new System.Drawing.Point(3, 16);
      this.statisticsMatrixView.Name = "statisticsMatrixView";
      this.statisticsMatrixView.ReadOnly = true;
      this.statisticsMatrixView.ShowRowsAndColumnsTextBox = false;
      this.statisticsMatrixView.ShowStatisticalInformation = false;
      this.statisticsMatrixView.Size = new System.Drawing.Size(975, 155);
      this.statisticsMatrixView.TabIndex = 0;
      // 
      // RunCollectionBoxPlotView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.splitContainer);
      this.Name = "RunCollectionBoxPlotView";
      this.Size = new System.Drawing.Size(987, 550);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.statisticsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label xAxisLabel;
    internal System.Windows.Forms.ComboBox xAxisComboBox;
    private System.Windows.Forms.Label yAxisLabel;
    internal System.Windows.Forms.ComboBox yAxisComboBox;
    private HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.Label noRunsLabel;
    private System.Windows.Forms.SplitContainer splitContainer;
    private Data.Views.StringConvertibleMatrixView statisticsMatrixView;
    private System.Windows.Forms.GroupBox statisticsGroupBox;
    private System.Windows.Forms.ToolTip tooltip;
    protected System.Windows.Forms.CheckBox showStatisticsCheckBox;
    private System.Windows.Forms.ToolStripMenuItem openBubbleChartViewToolStripMenuItem;
  }
}
