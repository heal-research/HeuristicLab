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

using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [Content(typeof(GradientBoostedTreesSolution), true)]
  [View("GradientBoostedTreesSolution View")]
  public partial class GradientBoostedTreesSolutionView : RegressionSolutionView {
    public GradientBoostedTreesSolutionView() {
      InitializeComponent();
    }

    protected new GradientBoostedTreesSolution Content {
      get { return (GradientBoostedTreesSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      itemsListView.Items.Remove(itemsListView.FindItemWithText("Model: Gradient boosted tree model"));
    }
  }
}
