#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  partial class GraphicalSymbolicExpressionTreeView {
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
      this.symbolicExpressionTreeChart = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart();
      this.SuspendLayout();
      // 
      // functionTreeChart
      // 
      this.symbolicExpressionTreeChart.BackgroundColor = System.Drawing.Color.White;
      this.symbolicExpressionTreeChart.Dock = System.Windows.Forms.DockStyle.Fill;
      this.symbolicExpressionTreeChart.Tree = null;
      this.symbolicExpressionTreeChart.LineColor = System.Drawing.Color.Black;
      this.symbolicExpressionTreeChart.Location = new System.Drawing.Point(0, 0);
      this.symbolicExpressionTreeChart.Name = "functionTreeChart";
      this.symbolicExpressionTreeChart.Size = new System.Drawing.Size(407, 367);
      this.symbolicExpressionTreeChart.Spacing = 5;
      this.symbolicExpressionTreeChart.TabIndex = 0;
      this.symbolicExpressionTreeChart.TextFont = new System.Drawing.Font(FontFamily.GenericSerif, 8F);
      // 
      // FunctionTreeView
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.symbolicExpressionTreeChart);
      this.Name = "FunctionTreeView";
      this.Size = new System.Drawing.Size(407, 367);
      this.ResumeLayout(false);

    }

    #endregion

    private SymbolicExpressionTreeChart symbolicExpressionTreeChart;
  }
}
