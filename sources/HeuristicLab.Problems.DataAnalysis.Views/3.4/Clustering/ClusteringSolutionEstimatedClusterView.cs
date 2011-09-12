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
using HeuristicLab.Data;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Estimated Clusters")]
  [Content(typeof(IClusteringSolution))]
  public partial class ClusteringSolutionEstimatedClusterView : DataAnalysisSolutionEvaluationView {
    private const string CLUSTER_NAMES = "Cluster";

    public new IClusteringSolution Content {
      get { return (IClusteringSolution)base.Content; }
      set {
        base.Content = value;
      }
    }

    private StringConvertibleMatrixView matrixView;

    public ClusteringSolutionEstimatedClusterView()
      : base() {
      InitializeComponent();
      matrixView = new StringConvertibleMatrixView();
      matrixView.ShowRowsAndColumnsTextBox = false;
      matrixView.ShowStatisticalInformation = false;
      matrixView.Dock = DockStyle.Fill;
      this.Controls.Add(matrixView);
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
      UpdateClusterValues();
    }

    private void UpdateClusterValues() {
      if (InvokeRequired) Invoke((Action)UpdateClusterValues);
      else {
        DoubleMatrix matrix = null;
        if (Content != null) {
          int[] clusters = Content.Model.GetClusterValues(Content.ProblemData.Dataset, Enumerable.Range(0, Content.ProblemData.Dataset.Rows)).ToArray();
          var dataset = Content.ProblemData.Dataset;
          int columns = Content.ProblemData.AllowedInputVariables.Count() + 1;          

          double[,] values = new double[dataset.Rows, columns];
          for (int row = 0; row < dataset.Rows; row++) {
            values[row, 0] = clusters[row];

            int column = 1;
            foreach (var columnName in Content.ProblemData.AllowedInputVariables) {
              values[row, column] = dataset.GetDoubleValue(columnName, row);
              column++;
            }
          }

          matrix = new DoubleMatrix(values);
          var columnNames = Content.ProblemData.AllowedInputVariables.ToList();
          columnNames.Insert(0, CLUSTER_NAMES);
          matrix.ColumnNames = columnNames;
        }
        matrixView.Content = matrix;
      }
    }
    #endregion
  }
}
