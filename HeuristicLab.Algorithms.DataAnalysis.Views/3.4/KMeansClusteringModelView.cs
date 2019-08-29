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
using System.Linq;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Data;
using System.Collections.Generic;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("k-Means Clustering Model")]
  [Content(typeof(KMeansClusteringModel), true)]
  public partial class KMeansClusteringModelView : AsynchronousContentView {

    public new KMeansClusteringModel Content {
      get { return (KMeansClusteringModel)base.Content; }
      set { base.Content = value; }
    }

    public KMeansClusteringModelView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        stringConvertibleMatrixView.Content = null;
      else
        UpdateCenters();
    }

    private void UpdateCenters() {
      double[,] centers = new double[Content.Centers.Count(), Content.Centers.First().Length];
      int row = 0;
      List<string> rowNames = new List<string>();
      foreach (double[] c in Content.Centers) {
        for (int col = 0; col < c.Length; col++) {
          centers[row, col] = c[col];
        }
        row++;
        rowNames.Add("Center " + row);
      }
      stringConvertibleMatrixView.Content = new DoubleMatrix(centers, Content.AllowedInputVariables, rowNames);
    }
  }
}
