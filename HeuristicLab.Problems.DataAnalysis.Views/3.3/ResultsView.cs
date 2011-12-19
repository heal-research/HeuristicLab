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
using System.Collections.Generic;
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis.Evaluators;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [Content(typeof(DataAnalysisSolution), false)]
  [View("Results View")]
  public partial class ResultsView : AsynchronousContentView {
    private List<string> rowNames = new List<string>() { "Mean squared error", "Pearson's R²", "Average relative error" };
    private List<string> columnNames = new List<string>() { "Training", "Test" };

    public ResultsView() {
      InitializeComponent();
    }

    public new DataAnalysisSolution Content {
      get { return (DataAnalysisSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
      Content.EstimatedValuesChanged += new EventHandler(Content_EstimatedValuesChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
      Content.EstimatedValuesChanged -= new EventHandler(Content_EstimatedValuesChanged);
    }

    private void Content_ModelChanged(object sender, EventArgs e) {
      UpdateView();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateView();
    }
    private void Content_EstimatedValuesChanged(object sender, EventArgs e) {
      UpdateView();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateView();
    }
    private void UpdateView() {
      if (Content != null) {
        DoubleMatrix matrix = new DoubleMatrix(rowNames.Count, columnNames.Count);
        matrix.RowNames = rowNames;
        matrix.ColumnNames = columnNames;
        matrix.SortableView = false;

        IEnumerable<double> originalTrainingValues = Content.ProblemData.Dataset.GetEnumeratedVariableValues(Content.ProblemData.TargetVariable.Value, Content.ProblemData.TrainingIndizes);
        IEnumerable<double> originalTestValues = Content.ProblemData.Dataset.GetEnumeratedVariableValues(Content.ProblemData.TargetVariable.Value, Content.ProblemData.TestIndizes);
        matrix[0, 0] = SimpleMSEEvaluator.Calculate(originalTrainingValues, Content.EstimatedTrainingValues);
        matrix[0, 1] = SimpleMSEEvaluator.Calculate(originalTestValues, Content.EstimatedTestValues);
        matrix[1, 0] = SimpleRSquaredEvaluator.Calculate(originalTrainingValues, Content.EstimatedTrainingValues);
        matrix[1, 1] = SimpleRSquaredEvaluator.Calculate(originalTestValues, Content.EstimatedTestValues);
        matrix[2, 0] = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(originalTrainingValues, Content.EstimatedTrainingValues);
        matrix[2, 1] = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate(originalTestValues, Content.EstimatedTestValues);

        matrixView.Content = matrix;
      } else
        matrixView.Content = null;
    }
  }
}
