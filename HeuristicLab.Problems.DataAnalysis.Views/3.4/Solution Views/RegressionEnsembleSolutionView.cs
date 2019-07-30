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

using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("RegressionEnsembleSolution View")]
  [Content(typeof(RegressionEnsembleSolution), false)]
  public partial class RegressionEnsembleSolutionView : RegressionSolutionView {
    public RegressionEnsembleSolutionView() {
      InitializeComponent();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      //loading of problemdata is currently not support for ensemble solutions
      loadProblemDataButton.Enabled = false;
      loadProblemDataButton.Visible = false;
    }

    public new RegressionEnsembleSolution Content {
      get { return (RegressionEnsembleSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      itemsListView.Items.Remove(itemsListView.FindItemWithText("Model: RegressionEnsembleModel"));
    }

    #region drag & drop
    protected override void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        var droppedData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        if (droppedData is IValueParameter) droppedData = ((IValueParameter)droppedData).Value;
        if (droppedData is IRegressionProblem) droppedData = ((IRegressionProblem)droppedData).ProblemData;

        RegressionEnsembleProblemData ensembleProblemData = droppedData as RegressionEnsembleProblemData;
        IRegressionProblemData problemData = droppedData as IRegressionProblemData;
        if (ensembleProblemData != null) {
          Content.ProblemData = (RegressionEnsembleProblemData)ensembleProblemData.Clone();
        } else if (problemData != null) {
          Content.ProblemData = new RegressionEnsembleProblemData((IRegressionProblemData)problemData.Clone());
        }
      }
    }
    #endregion
  }
}
