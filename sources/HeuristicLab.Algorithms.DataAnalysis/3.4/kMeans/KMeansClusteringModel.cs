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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a k-Means clustering model.
  /// </summary>
  [StorableClass]
  [Item("KMeansClusteringModel", "Represents a k-Means clustering model.")]
  public sealed class KMeansClusteringModel : NamedItem, IClusteringModel {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
    }

    public IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private string[] allowedInputVariables;
    public IEnumerable<string> AllowedInputVariables {
      get { return allowedInputVariables; }
    }
    [Storable]
    private List<double[]> centers;
    public IEnumerable<double[]> Centers {
      get {
        return centers.Select(x => (double[])x.Clone());
      }
    }
    [StorableConstructor]
    private KMeansClusteringModel(bool deserializing) : base(deserializing) { }
    private KMeansClusteringModel(KMeansClusteringModel original, Cloner cloner)
      : base(original, cloner) {
      this.allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      this.centers = new List<double[]>(original.Centers);
    }
    public KMeansClusteringModel(double[,] centers, IEnumerable<string> allowedInputVariables)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      // disect center matrix into list of double[]
      // centers are given as double matrix where number of rows = dimensions and number of columns = clusters
      // each column is a cluster center
      this.centers = new List<double[]>();
      for (int i = 0; i < centers.GetLength(1); i++) {
        double[] c = new double[centers.GetLength(0)];
        for (int j = 0; j < c.Length; j++) {
          c[j] = centers[j, i];
        }
        this.centers.Add(c);
      }
      this.allowedInputVariables = allowedInputVariables.ToArray();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KMeansClusteringModel(this, cloner);
    }


    public IEnumerable<int> GetClusterValues(IDataset dataset, IEnumerable<int> rows) {
      return KMeansClusteringUtil.FindClosestCenters(centers, dataset, allowedInputVariables, rows);
    }
  }
}
