#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views.Solution_Views {
  [View("DataAnalysisSolutionView")]
  [Content(typeof(DataAnalysisSolution), true)]
  public partial class NamedDataAnalysisSolutionView : NamedItemView {
    public NamedDataAnalysisSolutionView() {
      InitializeComponent();
    }

    public new DataAnalysisSolution Content {
      get { return (DataAnalysisSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      panel.Controls.Clear();

      if (Content != null) {
        var viewType = MainFormManager.GetViewTypes(Content.GetType(), true).Where(t => typeof(DataAnalysisSolutionView).IsAssignableFrom(t)).FirstOrDefault();
        if (viewType != null) {
          var view = (DataAnalysisSolutionView)Activator.CreateInstance(viewType);
          view.Dock = DockStyle.Fill;
          view.Content = Content;
          panel.Controls.Add(view);
        }
      }
    }
  }
}
