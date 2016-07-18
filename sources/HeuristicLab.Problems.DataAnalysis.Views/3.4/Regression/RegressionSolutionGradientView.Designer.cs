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

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  partial class RegressionSolutionGradientView {
    /// <summary> 
    /// Required designer variableNames.
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.gradientChart = new HeuristicLab.Problems.DataAnalysis.Views.GradientChart();
      this.configurationGroupBox = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.configurationGroupBox.SuspendLayout();
      this.SuspendLayout();
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
      this.splitContainer.Panel1.Controls.Add(this.gradientChart);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.configurationGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(715, 591);
      this.splitContainer.SplitterDistance = 376;
      this.splitContainer.TabIndex = 1;
      // 
      // gradientChart
      // 
      this.gradientChart.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gradientChart.DrawingSteps = 1000;
      this.gradientChart.Location = new System.Drawing.Point(0, 0);
      this.gradientChart.Name = "gradientChart";
      this.gradientChart.ShowCursor = false;
      this.gradientChart.ShowLegend = false;
      this.gradientChart.Size = new System.Drawing.Size(715, 376);
      this.gradientChart.TabIndex = 0;
      this.gradientChart.XAxisTicks = 10;
      this.gradientChart.YAxisTicks = 5;
      // 
      // configurationGroupBox
      // 
      this.configurationGroupBox.Controls.Add(this.tableLayoutPanel);
      this.configurationGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.configurationGroupBox.Location = new System.Drawing.Point(0, 0);
      this.configurationGroupBox.Name = "configurationGroupBox";
      this.configurationGroupBox.Size = new System.Drawing.Size(715, 211);
      this.configurationGroupBox.TabIndex = 1;
      this.configurationGroupBox.TabStop = false;
      this.configurationGroupBox.Text = "Configuration";
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.AutoScroll = true;
      this.tableLayoutPanel.ColumnCount = 1;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 1;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel.Size = new System.Drawing.Size(709, 192);
      this.tableLayoutPanel.TabIndex = 0;
      // 
      // RegressionSolutionGradientView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "RegressionSolutionGradientView";
      this.Size = new System.Drawing.Size(715, 591);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.configurationGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox configurationGroupBox;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private Problems.DataAnalysis.Views.GradientChart gradientChart;
  }
}
