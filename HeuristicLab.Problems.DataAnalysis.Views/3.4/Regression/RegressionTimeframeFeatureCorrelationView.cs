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

using System.Linq;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Regression Timeframe Feature Correlation View")]
  [Content(typeof(RegressionProblemData), false)]
  public partial class RegressionTimeframeFeatureCorrelationView : TimeframeFeatureCorrelationView {

    public new RegressionProblemData Content {
      get { return (RegressionProblemData)base.Content; }
      set { base.Content = value; }
    }

    public RegressionTimeframeFeatureCorrelationView() {
      InitializeComponent();
    }

    protected override void SetVariableSelectionComboBox() {
      base.SetVariableSelectionComboBox();
      variableSelectionComboBox.SelectedItem = Content.TargetVariable;
    }

    protected override bool[] SetInitialVariableVisibility() {
      int i = Content.Dataset.DoubleVariables.ToList().FindIndex(x => x == Content.TargetVariable);
      var initialVisibility = base.SetInitialVariableVisibility();
      initialVisibility[i] = true;
      return initialVisibility;
    }
  }
}
