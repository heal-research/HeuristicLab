#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// k-Means clustering algorithm data analysis algorithm.
  /// </summary>
  [Item("k-Means", "The k-Means clustering algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysis, Priority = 10)]
  [StorableClass]
  public sealed class KMeansClustering : FixedDataAnalysisAlgorithm<IClusteringProblem> {
    private const string KParameterName = "k";
    private const string RestartsParameterName = "Restarts";
    private const string KMeansSolutionResultName = "k-Means clustering solution";
    #region parameter properties
    public IValueParameter<IntValue> KParameter {
      get { return (IValueParameter<IntValue>)Parameters[KParameterName]; }
    }
    public IValueParameter<IntValue> RestartsParameter {
      get { return (IValueParameter<IntValue>)Parameters[RestartsParameterName]; }
    }
    #endregion
    #region properties
    public IntValue K {
      get { return KParameter.Value; }
    }
    public IntValue Restarts {
      get { return RestartsParameter.Value; }
    }
    #endregion
    [StorableConstructor]
    private KMeansClustering(bool deserializing) : base(deserializing) { }
    private KMeansClustering(KMeansClustering original, Cloner cloner)
      : base(original, cloner) {
    }
    public KMeansClustering()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>(KParameterName, "The number of clusters.", new IntValue(3)));
      Parameters.Add(new ValueParameter<IntValue>(RestartsParameterName, "The number of restarts.", new IntValue(0)));
      Problem = new ClusteringProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KMeansClustering(this, cloner);
    }

    #region k-Means clustering
    protected override void Run(CancellationToken cancellationToken) {
      var solution = CreateKMeansSolution(Problem.ProblemData, K.Value, Restarts.Value);
      Results.Add(new Result(KMeansSolutionResultName, "The k-Means clustering solution.", solution));
    }

    public static KMeansClusteringSolution CreateKMeansSolution(IClusteringProblemData problemData, int k, int restarts) {
      var dataset = problemData.Dataset;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      int info;
      double[,] centers;
      int[] xyc;
      double[,] inputMatrix = dataset.ToArray(allowedInputVariables, rows);
      if (inputMatrix.ContainsNanOrInfinity())
        throw new NotSupportedException("k-Means clustering does not support NaN or infinity values in the input dataset.");

      alglib.kmeansgenerate(inputMatrix, inputMatrix.GetLength(0), inputMatrix.GetLength(1), k, restarts + 1, out info, out centers, out xyc);
      if (info != 1) throw new ArgumentException("Error in calculation of k-Means clustering solution");

      KMeansClusteringSolution solution = new KMeansClusteringSolution(new KMeansClusteringModel(centers, allowedInputVariables), (IClusteringProblemData)problemData.Clone());
      return solution;
    }
    #endregion
  }
}
