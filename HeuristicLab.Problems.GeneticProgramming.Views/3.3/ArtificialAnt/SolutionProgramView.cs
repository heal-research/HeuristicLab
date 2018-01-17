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

using System;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.GeneticProgramming.ArtificialAnt;

namespace HeuristicLab.Problems.GeneticProgramming.Views.ArtificialAnt {
  [View("Artificial Ant Program View")]
  [Content(typeof(Solution), false)]
  public sealed partial class SolutionProgramView : ItemView {
    private readonly GraphicalSymbolicExpressionTreeView treeView;

    public new Solution Content {
      get { return (Solution)base.Content; }
      set { base.Content = value; }
    }

    public SolutionProgramView() {
      InitializeComponent();
      treeView = new GraphicalSymbolicExpressionTreeView();
      treeView.Dock = DockStyle.Fill;
      symExpressionGroupBox.Controls.Add(treeView);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        treeView.Content = null;
      } else {
        treeView.Content = Content.SymbolicExpressionTree;
      }
    }
  }
}
