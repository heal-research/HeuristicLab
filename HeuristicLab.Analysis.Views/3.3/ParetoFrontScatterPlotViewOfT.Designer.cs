#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class ParetoFrontScatterPlotView<T> {
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
      this.scatterPlotView = new HeuristicLab.Analysis.Views.ScatterPlotView();
      this.xAxisComboBox = new System.Windows.Forms.ComboBox();
      this.yAxisComboBox = new System.Windows.Forms.ComboBox();
      this.xLabel = new System.Windows.Forms.Label();
      this.yLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // scatterPlotView
      // 
      this.scatterPlotView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scatterPlotView.Caption = "ScatterPlot View";
      this.scatterPlotView.Content = null;
      this.scatterPlotView.Location = new System.Drawing.Point(3, 37);
      this.scatterPlotView.Name = "scatterPlotView";
      this.scatterPlotView.ReadOnly = false;
      this.scatterPlotView.ShowChartOnly = true;
      this.scatterPlotView.Size = new System.Drawing.Size(615, 342);
      this.scatterPlotView.TabIndex = 3;
      // 
      // xAxisComboBox
      // 
      this.xAxisComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.xAxisComboBox.FormattingEnabled = true;
      this.xAxisComboBox.Location = new System.Drawing.Point(29, 3);
      this.xAxisComboBox.Name = "xAxisComboBox";
      this.xAxisComboBox.Size = new System.Drawing.Size(135, 28);
      this.xAxisComboBox.TabIndex = 4;
      this.xAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.axisComboBox_SelectedIndexChanged);
      // 
      // yAxisComboBox
      // 
      this.yAxisComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.yAxisComboBox.FormattingEnabled = true;
      this.yAxisComboBox.Location = new System.Drawing.Point(240, 3);
      this.yAxisComboBox.Name = "yAxisComboBox";
      this.yAxisComboBox.Size = new System.Drawing.Size(135, 28);
      this.yAxisComboBox.TabIndex = 5;
      this.yAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.axisComboBox_SelectedIndexChanged);
      // 
      // xLabel
      // 
      this.xLabel.AutoSize = true;
      this.xLabel.Location = new System.Drawing.Point(3, 6);
      this.xLabel.Name = "xLabel";
      this.xLabel.Size = new System.Drawing.Size(20, 20);
      this.xLabel.TabIndex = 6;
      this.xLabel.Text = "x:";
      // 
      // yLabel
      // 
      this.yLabel.AutoSize = true;
      this.yLabel.Location = new System.Drawing.Point(214, 6);
      this.yLabel.Name = "yLabel";
      this.yLabel.Size = new System.Drawing.Size(20, 20);
      this.yLabel.TabIndex = 7;
      this.yLabel.Text = "y:";
      // 
      // ParetoFrontScatterPlotView
      // 
      this.AllowDrop = true;
      this.Controls.Add(this.yLabel);
      this.Controls.Add(this.xLabel);
      this.Controls.Add(this.yAxisComboBox);
      this.Controls.Add(this.xAxisComboBox);
      this.Controls.Add(this.scatterPlotView);
      this.Margin = new System.Windows.Forms.Padding(6);
      this.Name = "ParetoFrontScatterPlotView";
      this.Size = new System.Drawing.Size(621, 382);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private HeuristicLab.Analysis.Views.ScatterPlotView scatterPlotView;
    private System.Windows.Forms.ComboBox xAxisComboBox;
    private System.Windows.Forms.ComboBox yAxisComboBox;
    private System.Windows.Forms.Label xLabel;
    private System.Windows.Forms.Label yLabel;
  }
}
