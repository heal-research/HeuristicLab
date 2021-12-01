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

using System.Drawing;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class GraphicalSymbolicDataAnalysisModelView {
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
      // expressionTreeView
      // 
      this.symbolicExpressionTreeChart.AllowDrop = true;
      this.symbolicExpressionTreeChart.Tree = null;
      this.symbolicExpressionTreeChart.TextFont = new System.Drawing.Font(FontFamily.GenericSerif, 8F);
      this.symbolicExpressionTreeChart.Dock = System.Windows.Forms.DockStyle.Fill;
      this.symbolicExpressionTreeChart.Location = new System.Drawing.Point(0, 0);
      this.symbolicExpressionTreeChart.Name = "expressionTreeChart";
      this.symbolicExpressionTreeChart.Size = new System.Drawing.Size(352, 413);
      this.symbolicExpressionTreeChart.TabIndex = 0;
      // 
      // SymbolicExpressionModelView
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.symbolicExpressionTreeChart);
      this.Name = "SymbolicExpressionModelView";
      this.Size = new System.Drawing.Size(352, 413);
      this.ResumeLayout(false);

    }

    #endregion
    private HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart symbolicExpressionTreeChart;

  }
}
