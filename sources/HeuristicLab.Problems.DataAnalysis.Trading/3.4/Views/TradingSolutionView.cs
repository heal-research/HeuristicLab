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
  [View("TradingSolution View")]
  [Content(typeof(TradingSolution), true)]
  public partial class TradingSolutionView : DataAnalysisSolutionView {
    public TradingSolutionView() {
      InitializeComponent();

      var regressionSolutionEvaluationViewTypes = ApplicationManager.Manager.GetTypes(typeof(ITradingSolutionEvaluationView), true);
      foreach (Type viewType in regressionSolutionEvaluationViewTypes)
        AddViewListViewItem(viewType);
    }

    public new TradingSolution Content {
      get { return (TradingSolution)base.Content; }
      set { base.Content = value; }
    }
  }
}
