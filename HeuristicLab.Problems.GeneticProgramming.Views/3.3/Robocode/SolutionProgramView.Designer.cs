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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;

namespace HeuristicLab.Problems.GeneticProgramming.Views.Robocode {
  partial class SolutionProgramView {
    private void InitializeComponent() {
      this.graphTreeView = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.GraphicalSymbolicExpressionTreeView();
      this.SuspendLayout();
      // 
      // graphTreeView
      // 
      this.graphTreeView.AllowDrop = true;
      this.graphTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.graphTreeView.Caption = "Graphical SymbolicExpressionTree View";
      this.graphTreeView.Content = null;
      this.graphTreeView.Location = new System.Drawing.Point(6, 3);
      this.graphTreeView.Name = "graphTreeView";
      this.graphTreeView.ReadOnly = false;
      this.graphTreeView.Size = new System.Drawing.Size(342, 213);
      this.graphTreeView.TabIndex = 0;
      // 
      // SolutionProgramView
      // 
      this.Controls.Add(this.graphTreeView);
      this.Name = "SolutionProgramView";
      this.Size = new System.Drawing.Size(351, 219);
      this.ResumeLayout(false);

    }

    private GraphicalSymbolicExpressionTreeView graphTreeView;
  }
}
