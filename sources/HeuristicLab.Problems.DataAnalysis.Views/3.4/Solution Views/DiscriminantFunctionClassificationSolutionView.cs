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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("DiscriminantFunctionClassificationSolution View")]
  [Content(typeof(DiscriminantFunctionClassificationSolutionBase), true)]
  public partial class DiscriminantFunctionClassificationSolutionView : DataAnalysisSolutionView {
    public DiscriminantFunctionClassificationSolutionView() {
      InitializeComponent();

      var classificationSolutionEvaluationViewTypes = ApplicationManager.Manager.GetTypes(typeof(IClassificationSolutionEvaluationView), true);
      foreach (Type viewType in classificationSolutionEvaluationViewTypes)
        AddViewListViewItem(viewType, HeuristicLab.Common.Resources.VSImageLibrary.Graph);
      RemoveViewListViewItem(typeof(ClassificationSolutionEstimatedClassValuesView));

      var regressionSolutionEvaluationViewTypes = ApplicationManager.Manager.GetTypes(typeof(IDiscriminantFunctionClassificationSolutionEvaluationView), true);
      foreach (Type viewType in regressionSolutionEvaluationViewTypes)
        AddViewListViewItem(viewType, HeuristicLab.Common.Resources.VSImageLibrary.Graph);
    }

    public new DiscriminantFunctionClassificationSolutionBase Content {
      get { return (DiscriminantFunctionClassificationSolutionBase)base.Content; }
      set { base.Content = value; }
    }
  }
}
