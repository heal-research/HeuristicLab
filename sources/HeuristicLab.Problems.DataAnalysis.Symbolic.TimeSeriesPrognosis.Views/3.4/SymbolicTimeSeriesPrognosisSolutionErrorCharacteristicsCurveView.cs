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
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis.Views {
  [View("Error Characteristics Curve")]
  [Content(typeof(ISymbolicTimeSeriesPrognosisSolution))]
  public partial class SymbolicTimeSeriesPrognosisSolutionErrorCharacteristicsCurveView : TimeSeriesPrognosisSolutionErrorCharacteristicsCurveView {
    private ITimeSeriesPrognosisSolution linearTimeSeriesPrognosisSolution;
    private ITimeSeriesPrognosisSolution naiveSolution;

    public SymbolicTimeSeriesPrognosisSolutionErrorCharacteristicsCurveView() {
      InitializeComponent();
    }

    public new ISymbolicTimeSeriesPrognosisSolution Content {
      get { return (ISymbolicTimeSeriesPrognosisSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      if (Content != null) {
        linearTimeSeriesPrognosisSolution = CreateLinearTimeSeriesPrognosisSolution();
        naiveSolution = CreateNaiveSolution();
      } else {
        linearTimeSeriesPrognosisSolution = null;
        naiveSolution = null;
      }

      base.OnContentChanged();
    }

    protected override void UpdateChart() {
      base.UpdateChart();
      if (Content == null || linearTimeSeriesPrognosisSolution == null) return;
      AddTimeSeriesPrognosisSolution(linearTimeSeriesPrognosisSolution);
      AddTimeSeriesPrognosisSolution(naiveSolution);
    }

    private ITimeSeriesPrognosisSolution CreateLinearTimeSeriesPrognosisSolution() {
      if (Content == null) throw new InvalidOperationException();
      double rmse, cvRmsError;
      var problemData = (ITimeSeriesPrognosisProblemData)ProblemData.Clone();

      //clear checked inputVariables
      foreach (var inputVariable in problemData.InputVariables.CheckedItems) {
        problemData.InputVariables.SetItemCheckedState(inputVariable.Value, false);
      }

      //check inputVariables used in the symbolic time series prognosis model
      var usedVariables =
        Content.Model.SymbolicExpressionTree.IterateNodesPostfix().OfType<VariableTreeNode>().Select(
          node => node.VariableName).Distinct();
      foreach (var variable in usedVariables) {
        problemData.InputVariables.SetItemCheckedState(
          problemData.InputVariables.Where(x => x.Value == variable).First(), true);
      }

      int maxLag = Content.Model.SymbolicExpressionTree.IterateNodesPostfix()
        .OfType<LaggedVariableTreeNode>()
        .Select(n => -n.Lag)
        .Max();

      var solution = LinearTimeSeriesPrognosis.CreateLinearTimeSeriesPrognosisSolution(problemData, maxLag, out rmse, out cvRmsError);
      solution.Name = "Linear Model";
      return solution;
    }
    private ITimeSeriesPrognosisSolution CreateNaiveSolution() {
      if (Content == null) throw new InvalidOperationException();
      double rmse, cvRmsError;
      var problemData = (ITimeSeriesPrognosisProblemData)ProblemData.Clone();

      //clear checked inputVariables
      foreach (var inputVariable in problemData.InputVariables.CheckedItems) {
        problemData.InputVariables.SetItemCheckedState(inputVariable.Value, false);
      }

      foreach (var variable in problemData.InputVariables) {
        if (variable.Value == problemData.TargetVariable) {
          problemData.InputVariables.SetItemCheckedState(variable, true);
        }
      }

      int maxLag = 1;

      var solution = LinearTimeSeriesPrognosis.CreateLinearTimeSeriesPrognosisSolution(problemData, maxLag, out rmse, out cvRmsError);
      solution.Name = "AR(1) Model";
      return solution;
    }

    protected override void Content_ModelChanged(object sender, EventArgs e) {
      linearTimeSeriesPrognosisSolution = CreateLinearTimeSeriesPrognosisSolution();
      naiveSolution = CreateNaiveSolution();
      base.Content_ModelChanged(sender, e);
    }

    protected override void Content_ProblemDataChanged(object sender, EventArgs e) {
      linearTimeSeriesPrognosisSolution = CreateLinearTimeSeriesPrognosisSolution();
      naiveSolution = CreateNaiveSolution();
      base.Content_ProblemDataChanged(sender, e);
    }

    private void chart_MouseDown(object sender, MouseEventArgs e) {
      if (e.Clicks < 2) return;
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType != ChartElementType.LegendItem) return;
      if (result.Series.Name == linearTimeSeriesPrognosisSolution.Name && result.Series.Name != naiveSolution.Name) return;

      MainFormManager.MainForm.ShowContent((ITimeSeriesPrognosisSolution)result.Series.Tag);
    }
  }
}
