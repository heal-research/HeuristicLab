#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis.OnlineCalculators;

namespace HeuristicLab.Problems.DataAnalysis.Views.Classification {
  [View("Solution Comparison")]
  [Content(typeof(IClassificationSolution))]
  public partial class ClassificationSolutionComparisonView : DataAnalysisSolutionEvaluationView {
    private List<IClassificationSolution> solutions;

    public ClassificationSolutionComparisonView() {
      InitializeComponent();
    }

    public new IClassificationSolution Content {
      get { return (IClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

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

    protected virtual void Content_ModelChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ModelChanged, sender, e);
      else UpdateDataGridView();
    }
    protected virtual void Content_ProblemDataChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ProblemDataChanged, sender, e);
      else {
        UpdateDataGridView();
      }
    }
    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateDataGridView();
    }

    private void UpdateDataGridView() {
      if (InvokeRequired) {
        Invoke((Action)UpdateDataGridView);
      } else {
        if (Content == null) {
          dataGridView.Rows.Clear();
          dataGridView.Columns.Clear();
          solutions.Clear();
        } else {

          IClassificationProblemData problemData = Content.ProblemData;
          var dataset = problemData.Dataset;
          solutions = new List<IClassificationSolution>() { Content };
          solutions.AddRange(GenerateClassificationSolutions(problemData));

          dataGridView.ColumnCount = 4;
          dataGridView.RowCount = solutions.Count();
          dataGridView.Columns[0].HeaderText = "Accuracy (training)";
          dataGridView.Columns[1].HeaderText = "Accuracy (test)";
          dataGridView.Columns[2].HeaderText = "Matthews Correlation Coefficient (training)";
          dataGridView.Columns[3].HeaderText = "Matthews Correlation Coefficient (test)";
          if (problemData.Classes == 2) {
            dataGridView.ColumnCount = 6;
            dataGridView.Columns[4].HeaderText = "F1 Score (training)";
            dataGridView.Columns[5].HeaderText = "F1 Score (test)";
          }

          for (int row = 0; row < solutions.Count; row++) {
            var solution = solutions[row];

            dataGridView.Rows[row].HeaderCell.Value = solution.Name;
            dataGridView[0, row].Value = solution.TrainingAccuracy;
            dataGridView[1, row].Value = solution.TestAccuracy;

            var trainingIndizes = problemData.TrainingIndices;
            var originalTrainingValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, trainingIndizes);
            var estimatedTrainingValues = solution.Model.GetEstimatedClassValues(dataset, trainingIndizes);

            var testIndices = problemData.TestIndices;
            var originalTestValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, testIndices);
            var estimatedTestValues = solution.Model.GetEstimatedClassValues(dataset, testIndices);

            OnlineCalculatorError errorState;
            dataGridView[2, row].Value = MatthewsCorrelationCoefficientCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
            dataGridView[3, row].Value = MatthewsCorrelationCoefficientCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
            if (problemData.Classes == 2) {
              dataGridView[4, row].Value = FOneScoreCalculator.Calculate(originalTrainingValues, estimatedTrainingValues, out errorState);
              dataGridView[5, row].Value = FOneScoreCalculator.Calculate(originalTestValues, estimatedTestValues, out errorState);
            }
          }

          dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
          dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }
      }
    }

    private IEnumerable<IClassificationSolution> GenerateClassificationSolutions(IClassificationProblemData problemData) {
      var newSolutions = new List<IClassificationSolution>();
      var zeroR = ZeroR.CreateZeroRSolution(problemData);
      zeroR.Name = "ZeroR Classification Solution";
      newSolutions.Add(zeroR);
      var oneR = OneR.CreateOneRSolution(problemData);
      oneR.Name = "OneR Classification Solution";
      newSolutions.Add(oneR);
      try {
        var lda = LinearDiscriminantAnalysis.CreateLinearDiscriminantAnalysisSolution(problemData);
        lda.Name = "Linear Discriminant Analysis Solution";
        newSolutions.Add(lda);
      }
      catch (NotSupportedException) { }
      catch (ArgumentException) { }
      return newSolutions;
    }

    private void dataGridView_MouseDoubleClick(object sender, MouseEventArgs e) {
      var hittestinfo = dataGridView.HitTest(e.X, e.Y);
      if (hittestinfo.Type != DataGridViewHitTestType.RowHeader) { return; }
      if (hittestinfo.RowIndex > solutions.Count) { return; }

      MainFormManager.MainForm.ShowContent(solutions[hittestinfo.RowIndex]);
    }
  }
}
