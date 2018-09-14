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

using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.GeneticProgramming.LawnMower;

namespace HeuristicLab.Problems.GeneticProgramming.Views.LawnMower {
  [View("Lawn Mower Program View")]
  [Content(typeof(Solution), IsDefaultView = false)]
  public sealed partial class SolutionProgramView : NamedItemView {
    public new Solution Content {
      get { return (Solution)base.Content; }
      set { base.Content = value; }
    }

    public SolutionProgramView() {
      InitializeComponent();
      this.graphTreeView = new GraphicalSymbolicExpressionTreeView();
      graphTreeView.Dock = DockStyle.Fill;
      panel.Controls.Add(graphTreeView);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        graphTreeView.Content = null;
      } else {
        graphTreeView.Content = Content.Tree;
      }
    }
  }
}
