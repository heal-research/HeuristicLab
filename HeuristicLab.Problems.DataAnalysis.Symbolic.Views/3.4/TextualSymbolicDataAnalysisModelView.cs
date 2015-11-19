#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [View("Textual Representation")]
  [Content(typeof(ISymbolicDataAnalysisModel), false)]
  public partial class TextualSymbolicDataAnalysisModelView : AsynchronousContentView {
    public TextualSymbolicDataAnalysisModelView()
      : base() {
      InitializeComponent();
    }

    public new ISymbolicDataAnalysisModel Content {
      get { return (ISymbolicDataAnalysisModel)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null)
        symbolicExpressionTreeView.Content = Content.SymbolicExpressionTree;
      else
        symbolicExpressionTreeView.Content = null;
    }
  }
}
