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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.chart = new HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.exportChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
      this.contextMenuStrip.SuspendLayout();
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
      chartArea1.CursorX.IsUserEnabled = true;
      chartArea1.CursorX.IsUserSelectionEnabled = true;
      chartArea1.CursorY.IsUserEnabled = true;
      chartArea1.CursorY.IsUserSelectionEnabled = true;
      chartArea1.Name = "ChartArea1";
      this.chart.ChartAreas.Add(chartArea1);
      this.chart.ContextMenuStrip = this.contextMenuStrip;
      legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend1.Name = "Legend1";
      this.chart.Legends.Add(legend1);
      this.chart.EnableDoubleClickResetsZoom = true;
      this.chart.EnableMiddleClickPanning = true;
      this.chart.Location = new System.Drawing.Point(0, 52);
      this.chart.Name = "chart";
      series1.ChartArea = "ChartArea1";
      series1.Legend = "Legend1";
      series1.Name = "Series1";
      this.chart.Series.Add(series1);
      this.chart.Size = new System.Drawing.Size(359, 222);
      this.chart.TabIndex = 4;
      this.chart.Text = "chart1";
      this.chart.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart_CustomizeLegend);
      this.chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart_MouseDown);
      this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportChartToolStripMenuItem});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(153, 48);
      // 
      // exportChartToolStripMenuItem
      // 
      this.exportChartToolStripMenuItem.Name = "exportChartToolStripMenuItem";
      this.exportChartToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.exportChartToolStripMenuItem.Text = "Export chart...";
      this.exportChartToolStripMenuItem.Click += new System.EventHandler(this.exportChartToolStripMenuItem_Click);
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.DefaultExt = "emf";
      this.saveFileDialog.Filter = "EMF Files|*.emf";
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
      this.contextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Visualization.ChartControlsExtensions.EnhancedChart chart;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem exportChartToolStripMenuItem;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;

  }
}
