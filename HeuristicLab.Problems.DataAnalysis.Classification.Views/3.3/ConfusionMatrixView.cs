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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Classification.Views {
  [View("Confusion Matrix View")]
  [Content(typeof(SymbolicClassificationSolution))]
  public partial class ConfusionMatrixView : AsynchronousContentView {
    private const string TrainingSamples = "Training";
    private const string TestSamples = "Test";
    public ConfusionMatrixView() {
      InitializeComponent();
      cmbSamples.Items.Add(TrainingSamples);
      cmbSamples.Items.Add(TestSamples);
      cmbSamples.SelectedIndex = 0;
    }

    public new SymbolicClassificationSolution Content {
      get { return (SymbolicClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.EstimatedValuesChanged += new EventHandler(Content_EstimatedValuesChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
      Content.ThresholdsChanged += new EventHandler(Content_ThresholdsChanged);
    }


    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.EstimatedValuesChanged -= new EventHandler(Content_EstimatedValuesChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
      Content.ThresholdsChanged -= new EventHandler(Content_ThresholdsChanged);
    }

    private void Content_EstimatedValuesChanged(object sender, EventArgs e) {
      FillDataGridView();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateDataGridView();
    }
    private void Content_ThresholdsChanged(object sender, EventArgs e) {
      FillDataGridView();
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
        } else {
          dataGridView.ColumnCount = Content.ProblemData.NumberOfClasses;
          dataGridView.RowCount = Content.ProblemData.NumberOfClasses;

          int i = 0;
          foreach (string headerText in Content.ProblemData.ClassNames) {
            dataGridView.Columns[i].HeaderText = "Actual " + headerText;
            dataGridView.Rows[i].HeaderCell.Value = "Predicted " + headerText;
            i++;
          }
          dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
          dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

          FillDataGridView();
        }
      }
    }

    private void FillDataGridView() {
      if (InvokeRequired) Invoke((Action)FillDataGridView);
      else {
        if (Content == null) return;

        double[,] confusionMatrix = new double[Content.ProblemData.NumberOfClasses, Content.ProblemData.NumberOfClasses];
        IEnumerable<int> rows;

        if (cmbSamples.SelectedItem.ToString() == TrainingSamples) {
          rows = Content.ProblemData.TrainingIndizes;
        } else if (cmbSamples.SelectedItem.ToString() == TestSamples) {
          rows = Content.ProblemData.TestIndizes;
        } else throw new InvalidOperationException();

        Dictionary<double, int> classValueIndexMapping = new Dictionary<double, int>();
        int index = 0;
        foreach (double classValue in Content.ProblemData.SortedClassValues) {
          classValueIndexMapping.Add(classValue, index);
          index++;
        }

        double[] targetValues = Content.ProblemData.Dataset.GetEnumeratedVariableValues(Content.ProblemData.TargetVariable.Value, rows).ToArray();
        double[] predictedValues = Content.GetEstimatedClassValues(rows).ToArray();

        for (int i = 0; i < targetValues.Length; i++) {
          double targetValue = targetValues[i];
          double predictedValue = predictedValues[i];
          int targetIndex = classValueIndexMapping[targetValue];
          int predictedIndex = classValueIndexMapping[predictedValue];

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
