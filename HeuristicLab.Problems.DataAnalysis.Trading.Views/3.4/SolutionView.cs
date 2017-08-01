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

using System;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Problems.DataAnalysis.Trading.Views {
  [View("Trading Solution View")]
  [Content(typeof(Solution), true)]
  public partial class SolutionView : DataAnalysisSolutionView {
    public SolutionView() {
      InitializeComponent();

      var solutionEvaluationViewTypes = ApplicationManager.Manager.GetTypes(typeof(ISolutionEvaluationView));
      foreach (Type viewType in solutionEvaluationViewTypes)
        AddViewListViewItem(viewType, HeuristicLab.Common.Resources.VSImageLibrary.Graph);
    }

    public new Solution Content {
      get { return (Solution)base.Content; }
      set { base.Content = value; }
    }
  }
}
