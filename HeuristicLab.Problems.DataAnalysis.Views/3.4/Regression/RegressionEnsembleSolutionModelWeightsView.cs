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

using System;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Ensemble Solutions Weights")]
  [Content(typeof(RegressionEnsembleSolution), false)]
  internal sealed partial class RegressionEnsembleSolutionModelWeightsView : DataAnalysisSolutionEvaluationView {
    public override System.Drawing.Image ViewImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Properties; }
    }

    public RegressionEnsembleSolutionModelWeightsView() {
      InitializeComponent();
    }

    public new RegressionEnsembleSolution Content {
      get { return (RegressionEnsembleSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      averageEstimatesCheckBox.Enabled = Content != null && !Locked && !ReadOnly;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Model.Changed += Content_ModelChanged;
    }
    private void RegisterArrayEvents(DoubleArray array) {
      array.ItemChanged += DoubleArray_Changed;
    }

    protected override void DeregisterContentEvents() {
      Content.Model.Changed -= Content_ModelChanged;
      base.DeregisterContentEvents();
    }
    private void DeregisterArrayEvents(DoubleArray array) {
      array.ItemChanged -= DoubleArray_Changed;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        if (arrayView.Content != null) DeregisterArrayEvents((DoubleArray)arrayView.Content);
        var array = new DoubleArray(Content.Model.ModelWeights.ToArray());
        array.Resizable = false;
        array.ElementNames = Content.RegressionSolutions.Select(s => s.Name);

        RegisterArrayEvents(array);

        arrayView.Content = array;
        arrayView.Locked = Content.ProblemData == RegressionEnsembleProblemData.EmptyProblemData;
        averageEstimatesCheckBox.Checked = Content.Model.AverageModelEstimates;
      } else {
        arrayView.Content = null;
        averageEstimatesCheckBox.Checked = false;
      }
    }

    private void Content_ModelChanged(object sender, EventArgs eventArgs) {
      var array = (DoubleArray)arrayView.Content;
      var modelWeights = Content.Model.ModelWeights.ToList();

      if (array.Length != modelWeights.Count) {
        DeregisterArrayEvents(array);
        array = new DoubleArray(Content.Model.ModelWeights.ToArray());
        array.Resizable = false;
        RegisterArrayEvents(array);
      }

      for (int i = 0; i < modelWeights.Count; i++)
        array[i] = modelWeights[i];

      array.ElementNames = Content.RegressionSolutions.Select(s => s.Name);
      averageEstimatesCheckBox.Checked = Content.Model.AverageModelEstimates;
    }

    private void DoubleArray_Changed(object sender, EventArgs<int> eventArgs) {
      var array = (DoubleArray)arrayView.Content;
      var index = eventArgs.Value;
      var newWeight = array[index];
      var model = Content.Model.Models.ElementAt(index);

      Content.Model.SetModelWeight(model, newWeight);
    }

    private void averageEstimatesCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content == null) return;
      Content.Model.AverageModelEstimates = averageEstimatesCheckBox.Checked;
    }
  }
}
