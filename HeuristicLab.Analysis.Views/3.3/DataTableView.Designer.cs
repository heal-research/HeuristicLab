#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class DataTableView {
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
      System.Windows.Forms.DataVisualization.Charting.ChartArea defaultChartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend defaultLegend = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series defaultSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Title defaultTitle = new System.Windows.Forms.DataVisualization.Charting.Title();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(287, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Size = new System.Drawing.Size(287, 20);
      // 
      // chart
      // 
      this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.chart.BorderlineColor = System.Drawing.Color.Black;
      this.chart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
      defaultChartArea.AxisX.Minimum = 0;
      defaultChartArea.CursorX.IsUserEnabled = true;
      defaultChartArea.CursorX.IsUserSelectionEnabled = true;
      defaultChartArea.CursorY.IsUserEnabled = true;
      defaultChartArea.CursorY.IsUserSelectionEnabled = true;
      defaultChartArea.Name = "Default";
      this.chart.ChartAreas.Add(defaultChartArea);
      defaultLegend.Alignment = System.Drawing.StringAlignment.Center;
      defaultLegend.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      defaultLegend.Name = "Default";
      this.chart.Legends.Add(defaultLegend);
      this.chart.Location = new System.Drawing.Point(0, 52);
      this.chart.Name = "chart";
      defaultSeries.ChartArea = "Default";
      defaultSeries.Legend = "Default";
      defaultSeries.Name = "Default";
      this.chart.Series.Add(defaultSeries);
      this.chart.Size = new System.Drawing.Size(359, 222);
      this.chart.TabIndex = 4;
      this.chart.Text = "chart";
      defaultTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      defaultTitle.Name = "Default";
      defaultTitle.Text = "Title";
      this.chart.Titles.Add(defaultTitle);
      this.chart.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart_CustomizeLegend);
      this.chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart_MouseDown);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      // 
      // DataTableView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chart);
      this.Name = "DataTableView";
      this.Size = new System.Drawing.Size(359, 274);
      this.Controls.SetChildIndex(this.chart, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;

  }
}
