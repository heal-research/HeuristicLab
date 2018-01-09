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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Residual Analysis")]
  [Content(typeof(IRegressionSolution))]
  public sealed partial class RegressionSolutionResidualAnalysisView : DataAnalysisSolutionEvaluationView {

    // names should be relatively save to prevent collisions with variable names in the dataset
    private const string TargetLabel = "> Target";
    private const string PredictionLabel = "> Prediction";
    private const string ResidualLabel = "> Residual";
    private const string AbsResidualLabel = "> Residual (abs.)";
    private const string RelativeErrorLabel = "> Relative Error";
    private const string AbsRelativeErrorLabel = "> Relative Error (abs.)";
    private const string PartitionLabel = "> Partition";

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public RegressionSolutionResidualAnalysisView() : base() {
      InitializeComponent();
    }

    #region events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    private void Content_ModelChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        bubbleChartView.Content = null;
      } else {
        UpdateBubbleChart();
      }
    }

    private void UpdateBubbleChart() {
      if (Content == null) return;
      var selectedXAxis = bubbleChartView.SelectedXAxis;
      var selectedYAxis = bubbleChartView.SelectedYAxis;

      var problemData = Content.ProblemData;
      var ds = problemData.Dataset;
      var runs = new RunCollection();
      // determine relevant variables (at least two different values)
      var doubleVars = ds.DoubleVariables.Where(vn => ds.GetDoubleValues(vn).Max() > ds.GetDoubleValues(vn).Min()).ToArray();
      var stringVars = ds.StringVariables.Where(vn => ds.GetStringValues(vn).Distinct().Skip(1).Any()).ToArray();
      var dateTimeVars = ds.DateTimeVariables.Where(vn => ds.GetDateTimeValues(vn).Distinct().Skip(1).Any()).ToArray();

      // produce training and test values separately as they might overlap (e.g. for ensembles)
      var predictedValuesTrain = Content.EstimatedTrainingValues.ToArray();
      int j = 0; // idx for predictedValues array
      foreach (var i in problemData.TrainingIndices) {
        var run = CreateRunForIdx(i, problemData, doubleVars, stringVars, dateTimeVars);
        var targetValue = ds.GetDoubleValue(problemData.TargetVariable, i);
        AddErrors(run, predictedValuesTrain[j++], targetValue);
        run.Results.Add(PartitionLabel, new StringValue("Training"));
        run.Color = Color.Gold;
        runs.Add(run);
      }
      var predictedValuesTest = Content.EstimatedTestValues.ToArray();
      j = 0;
      foreach (var i in problemData.TestIndices) {
        var run = CreateRunForIdx(i, problemData, doubleVars, stringVars, dateTimeVars);
        var targetValue = ds.GetDoubleValue(problemData.TargetVariable, i);
        AddErrors(run, predictedValuesTest[j++], targetValue);
        run.Results.Add(PartitionLabel, new StringValue("Test"));
        run.Color = Color.Red;
        runs.Add(run);
      }
      if (string.IsNullOrEmpty(selectedXAxis))
        selectedXAxis = "Index";
      if (string.IsNullOrEmpty(selectedYAxis))
        selectedYAxis = "Residual";

      bubbleChartView.Content = runs;
      bubbleChartView.SelectedXAxis = selectedXAxis;
      bubbleChartView.SelectedYAxis = selectedYAxis;
    }

    private void AddErrors(IRun run, double pred, double target) {
      var residual = target - pred;
      var relError = residual / target;
      run.Results.Add(TargetLabel, new DoubleValue(target));
      run.Results.Add(PredictionLabel, new DoubleValue(pred));
      run.Results.Add(ResidualLabel, new DoubleValue(residual));
      run.Results.Add(AbsResidualLabel, new DoubleValue(Math.Abs(residual)));
      run.Results.Add(RelativeErrorLabel, new DoubleValue(relError));
      run.Results.Add(AbsRelativeErrorLabel, new DoubleValue(Math.Abs(relError)));
    }

    private IRun CreateRunForIdx(int i, IRegressionProblemData problemData, IEnumerable<string> doubleVars, IEnumerable<string> stringVars, IEnumerable<string> dateTimeVars) {
      var ds = problemData.Dataset;
      var run = new Run();
      foreach (var variableName in doubleVars) {
        run.Results.Add(variableName, new DoubleValue(ds.GetDoubleValue(variableName, i)));
      }
      foreach (var variableName in stringVars) {
        run.Results.Add(variableName, new StringValue(ds.GetStringValue(variableName, i)));
      }
      foreach (var variableName in dateTimeVars) {
        run.Results.Add(variableName, new DateTimeValue(ds.GetDateTimeValue(variableName, i)));
      }
      return run;
    }
    #endregion

  }
}
