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
namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  partial class MultiObjectiveTestFunctionParetoFrontScatterPlotView {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.chooseDimensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.chooseYDimensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // chart
      // 
      chartArea2.Name = "ChartArea";
      this.chart.ChartAreas.Add(chartArea2);
      this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
      legend2.Alignment = System.Drawing.StringAlignment.Center;
      legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend2.Name = "Default";
      this.chart.Legends.Add(legend2);
      this.chart.Location = new System.Drawing.Point(0, 42);
      this.chart.Margin = new System.Windows.Forms.Padding(6);
      this.chart.Name = "chart";
      this.chart.Size = new System.Drawing.Size(1054, 712);
      this.chart.TabIndex = 1;
      this.chart.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart_CustomizeLegend);
      this.chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart_MouseDown);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      // 
      // menuStrip1
      // 
      this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseDimensionToolStripMenuItem,
            this.chooseYDimensionToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(1054, 42);
      this.menuStrip1.TabIndex = 2;
      this.menuStrip1.Text = "menuStrip1";
      this.menuStrip1.ShowItemToolTips = true;
      // 
      // chooseDimensionToolStripMenuItem
      // 
      this.chooseDimensionToolStripMenuItem.Name = "chooseDimensionToolStripMenuItem";
      this.chooseDimensionToolStripMenuItem.Size = new System.Drawing.Size(253, 38);
      this.chooseDimensionToolStripMenuItem.Text = "Objective 0";
      this.chooseDimensionToolStripMenuItem.ToolTipText = "Choose X-Dimension";
      // 
      // chooseYDimensionToolStripMenuItem
      // 
      this.chooseYDimensionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
      this.chooseYDimensionToolStripMenuItem.Name = "chooseYDimensionToolStripMenuItem";
      this.chooseYDimensionToolStripMenuItem.Size = new System.Drawing.Size(252, 38);
      this.chooseYDimensionToolStripMenuItem.Text = "Objective 1";
      this.chooseYDimensionToolStripMenuItem.ToolTipText = "Choose Y-Dimension";
      // 
      // testToolStripMenuItem
      // 
      this.testToolStripMenuItem.Name = "testToolStripMenuItem";
      this.testToolStripMenuItem.Size = new System.Drawing.Size(269, 38);
      this.testToolStripMenuItem.Text = "Test";
      // 
      // MOQualitiesScatterPlotView
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
      this.Controls.Add(this.chart);
      this.Controls.Add(this.menuStrip1);
      this.Margin = new System.Windows.Forms.Padding(6);
      this.Name = "MOQualitiesScatterPlotView";
      this.Size = new System.Drawing.Size(1054, 754);
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem chooseDimensionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem chooseYDimensionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
  }
}
