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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Confusion Matrix")]
  [Content(typeof(IClassificationSolution))]
  public partial class ClassificationSolutionConfusionMatrixView : DataAnalysisSolutionEvaluationView {
    private const string TrainingSamples = "Training";
    private const string TestSamples = "Test";
    public ClassificationSolutionConfusionMatrixView() {
      InitializeComponent();
      cmbSamples.Items.Add(TrainingSamples);
      cmbSamples.Items.Add(TestSamples);
      cmbSamples.SelectedIndex = 0;
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

    private void Content_ModelChanged(object sender, EventArgs e) {
      FillDataGridView();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateDataGridView();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateDataGridView();
    }

    private void UpdateDataGridView() {
      if (InvokeRequired) Invoke((Action)UpdateDataGridView);
      else {
        if (Content == null) {
          dataGridView.RowCount = 1;
          dataGridView.ColumnCount = 1;
          dataGridView.TopLeftHeaderCell.Value = string.Empty;
        } else {
          dataGridView.ColumnCount = Content.ProblemData.Classes + 1;
          dataGridView.RowCount = Content.ProblemData.Classes + 1;

          int i = 0;
          foreach (string headerText in Content.ProblemData.ClassNames) {
            dataGridView.Columns[i].HeaderText = "Actual " + headerText;
            dataGridView.Rows[i].HeaderCell.Value = "Predicted " + headerText;
            i++;
          }
          dataGridView.Columns[i].HeaderText = "Actual not classified";
          dataGridView.Rows[i].HeaderCell.Value = "Predicted not classified";

          dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
          dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

          dataGridView.TopLeftHeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
          dataGridView.TopLeftHeaderCell.Value = Content.Model.TargetVariable;

          FillDataGridView();
        }
      }
    }

    private void FillDataGridView() {
      if (InvokeRequired) Invoke((Action)FillDataGridView);
      else {
        if (Content == null) return;

        double[,] confusionMatrix = new double[Content.ProblemData.Classes + 1, Content.ProblemData.Classes + 1];
        IEnumerable<int> rows;

        double[] predictedValues;
        if (cmbSamples.SelectedItem.ToString() == TrainingSamples) {
          rows = Content.ProblemData.TrainingIndices;
          predictedValues = Content.EstimatedTrainingClassValues.ToArray();
        } else if (cmbSamples.SelectedItem.ToString() == TestSamples) {
          rows = Content.ProblemData.TestIndices;
          predictedValues = Content.EstimatedTestClassValues.ToArray();
        } else throw new InvalidOperationException();

        double[] targetValues = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable, rows).ToArray();

        Dictionary<double, int> classValueIndexMapping = new Dictionary<double, int>();
        int index = 0;
        foreach (double classValue in Content.ProblemData.ClassValues.OrderBy(x => x)) {
          classValueIndexMapping.Add(classValue, index);
          index++;
        }

        for (int i = 0; i < targetValues.Length; i++) {
          double targetValue = targetValues[i];
          double predictedValue = predictedValues[i];
          int targetIndex;
          int predictedIndex;
          if (!classValueIndexMapping.TryGetValue(targetValue, out targetIndex)) {
            targetIndex = Content.ProblemData.Classes;
          }
          if (!classValueIndexMapping.TryGetValue(predictedValue, out predictedIndex)) {
            predictedIndex = Content.ProblemData.Classes;
          }

          confusionMatrix[predictedIndex, targetIndex] += 1;
        }

        for (int row = 0; row < confusionMatrix.GetLength(0); row++) {
          for (int col = 0; col < confusionMatrix.GetLength(1); col++) {
            //TODO add scaling to relative values;
            dataGridView[col, row].Value = confusionMatrix[row, col];
          }
        }
      }
    }

    private void cmbSamples_SelectedIndexChanged(object sender, System.EventArgs e) {
      FillDataGridView();
    }
  }
}
