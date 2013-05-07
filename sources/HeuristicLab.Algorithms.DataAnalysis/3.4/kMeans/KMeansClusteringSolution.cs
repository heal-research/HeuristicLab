#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Optimization;
using HeuristicLab.Data;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a k-Means clustering solution for a clustering problem which can be visualized in the GUI.
  /// </summary>
  [Item("k-Means clustering solution", "Represents a k-Means solution for a clustering problem which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class KMeansClusteringSolution : ClusteringSolution {
    private const string TrainingIntraClusterSumOfSquaresResultName = "Intra-cluster sum of squares (training)";
    private const string TestIntraClusterSumOfSquaresResultName = "Intra-cluster sum of squares (test)";
    public new KMeansClusteringModel Model {
      get { return (KMeansClusteringModel)base.Model; }
      set { base.Model = value; }
    }

    [StorableConstructor]
    private KMeansClusteringSolution(bool deserializing) : base(deserializing) { }
    private KMeansClusteringSolution(KMeansClusteringSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public KMeansClusteringSolution(KMeansClusteringModel model, IClusteringProblemData problemData)
      : base(model, problemData) {
      double trainingIntraClusterSumOfSquares = KMeansClusteringUtil.CalculateIntraClusterSumOfSquares(model, problemData.Dataset, problemData.TrainingIndices);
      double testIntraClusterSumOfSquares = KMeansClusteringUtil.CalculateIntraClusterSumOfSquares(model, problemData.Dataset, problemData.TestIndices);
      this.Add(new Result(TrainingIntraClusterSumOfSquaresResultName, "The sum of squared distances of points of the training partition to the cluster center (is minimized by k-Means).", new DoubleValue(trainingIntraClusterSumOfSquares)));
      this.Add(new Result(TestIntraClusterSumOfSquaresResultName, "The sum of squared distances of points of the test partition to the cluster center (is minimized by k-Means).", new DoubleValue(testIntraClusterSumOfSquares)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KMeansClusteringSolution(this, cloner);
    }
  }
}
