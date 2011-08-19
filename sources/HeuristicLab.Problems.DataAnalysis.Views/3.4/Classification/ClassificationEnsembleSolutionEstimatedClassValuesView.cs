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
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Estimated Class Values")]
  [Content(typeof(ClassificationEnsembleSolution))]
  public partial class ClassificationEnsembleSolutionEstimatedClassValuesView :
    ClassificationSolutionEstimatedClassValuesView {
    private const string RowColumnName = "Row";
    private const string TargetClassValuesColumnName = "TargetVariable";
    private const string EstimatedClassValuesColumnName = "EstimatedClassValues";
    private const string ConfidenceColumnName = "Confidence";

    private const string SamplesComboBoxAllSamples = "All Samples";
    private const string SamplesComboBoxTrainingSamples = "Training Samples";
    private const string SamplesComboBoxTestSamples = "Test Samples";

    public new ClassificationEnsembleSolution Content {
      get { return (ClassificationEnsembleSolution)base.Content; }
      set { base.Content = value; }
    }

    public ClassificationEnsembleSolutionEstimatedClassValuesView()
      : base() {
      InitializeComponent();
      SamplesComboBox.Items.AddRange(new string[] { SamplesComboBoxAllSamples, SamplesComboBoxTrainingSamples, SamplesComboBoxTestSamples });
      SamplesComboBox.SelectedIndex = 0;
    }

    private void SamplesComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateEstimatedValues();
    }

    protected override void UpdateEstimatedValues() {
      if (InvokeRequired) {
        Invoke((Action)UpdateEstimatedValues);
        return;
      }
      if (Content == null) {
        matrixView.Content = null;
        return;
      }

      int[] indizes;
      double[] estimatedClassValues;

      switch (SamplesComboBox.SelectedItem.ToString()) {
        case SamplesComboBoxAllSamples: {
            indizes = Enumerable.Range(0, Content.ProblemData.Dataset.Rows).ToArray();
            estimatedClassValues = Content.EstimatedClassValues.ToArray();
            break;
          }
        case SamplesComboBoxTrainingSamples: {
            indizes = Content.ProblemData.TrainingIndizes.ToArray();
            estimatedClassValues = Content.EstimatedTrainingClassValues.ToArray();
            break;
          }
        case SamplesComboBoxTestSamples: {
            indizes = Content.ProblemData.TestIndizes.ToArray();
            estimatedClassValues = Content.EstimatedTestClassValues.ToArray();
            break;
          }
        default:
          throw new ArgumentException();
      }

      int classValuesCount = Content.ProblemData.ClassValues.Count;
      int modelCount = Content.Model.Models.Count();
      string[,] values = new string[indizes.Length, 4 + classValuesCount + modelCount];
      double[] target = Content.ProblemData.Dataset.GetVariableValues(Content.ProblemData.TargetVariable);
      List<List<double?>> estimatedValuesVector = GetEstimatedValues(SamplesComboBox.SelectedItem.ToString(), indizes,
                                                            Content.ClassificationSolutions);

      for (int i = 0; i < indizes.Length; i++) {
        int row = indizes[i];
        values[i, 0] = row.ToString();
        values[i, 1] = target[i].ToString();
        values[i, 2] = estimatedClassValues[i].ToString();
        var groups = estimatedValuesVector[i].GroupBy(x => x).Select(g => new { Key = g.Key, Count = g.Count() }).ToList();
        var estimationCount = groups.Where(g => g.Key != null).Select(g => g.Count).Sum();
        values[i, 3] = (((double)groups.Where(g => g.Key == estimatedClassValues[i]).Single().Count) / estimationCount).ToString();
        for (int classIndex = 0; classIndex < Content.ProblemData.ClassValues.Count; classIndex++) {
          var group = groups.Where(g => g.Key == Content.ProblemData.ClassValues[classIndex]).SingleOrDefault();
          if (group == null) values[i, 4 + classIndex] = 0.ToString();
          else values[i, 4 + classIndex] = group.Count.ToString();
        }
        for (int modelIndex = 0; modelIndex < estimatedValuesVector[i].Count; modelIndex++) {
          values[i, 4 + classValuesCount + modelIndex] = estimatedValuesVector[i][modelIndex] == null
                                                           ? string.Empty
                                                           : estimatedValuesVector[i][modelIndex].ToString();
        }

      }

      StringMatrix matrix = new StringMatrix(values);
      List<string> columnNames = new List<string>() { "Id", TargetClassValuesColumnName, EstimatedClassValuesColumnName, ConfidenceColumnName };
      columnNames.AddRange(Content.ProblemData.ClassNames);
      columnNames.AddRange(Content.Model.Models.Select(m => m.Name));
      matrix.ColumnNames = columnNames;
      matrix.SortableView = true;
      matrixView.Content = matrix;
    }

    private List<List<double?>> GetEstimatedValues(string samplesSelection, int[] rows, IEnumerable<IClassificationSolution> solutions) {
      List<List<double?>> values = new List<List<double?>>();
      int solutionIndex = 0;
      foreach (var solution in solutions) {
        double[] estimation = solution.GetEstimatedClassValues(rows).ToArray();
        for (int i = 0; i < rows.Length; i++) {
          var row = rows[i];
          if (solutionIndex == 0) values.Add(new List<double?>());

          if (samplesSelection == SamplesComboBoxAllSamples)
            values[i].Add(estimation[i]);
          else if (samplesSelection == SamplesComboBoxTrainingSamples && solution.ProblemData.IsTrainingSample(row))
            values[i].Add(estimation[i]);
          else if (samplesSelection == SamplesComboBoxTestSamples && solution.ProblemData.IsTestSample(row))
            values[i].Add(estimation[i]);
          else
            values[i].Add(null);
        }
        solutionIndex++;
      }
      return values;
    }



  }
}
