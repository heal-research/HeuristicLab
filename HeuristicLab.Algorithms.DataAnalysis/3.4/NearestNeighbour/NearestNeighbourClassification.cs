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
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Nearest neighbour classification data analysis algorithm.
  /// </summary>
  [Item("Nearest Neighbour Classification (kNN)", "Nearest neighbour classification data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisClassification, Priority = 150)]
  [StorableType("98161D6F-D977-45EA-B899-E47EE017865E")]
  public sealed class NearestNeighbourClassification : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string KParameterName = "K";
    private const string NearestNeighbourClassificationModelResultName = "Nearest neighbour classification solution";
    private const string WeightsParameterName = "Weights";


    #region parameter properties
    public IFixedValueParameter<IntValue> KParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[KParameterName]; }
    }
    public IValueParameter<DoubleArray> WeightsParameter {
      get { return (IValueParameter<DoubleArray>)Parameters[WeightsParameterName]; }
    }
    #endregion
    #region properties
    public int K {
      get { return KParameter.Value.Value; }
      set {
        if (value <= 0) throw new ArgumentException("K must be larger than zero.", "K");
        else KParameter.Value.Value = value;
      }
    }
    public DoubleArray Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private NearestNeighbourClassification(StorableConstructorFlag _) : base(_) { }
    private NearestNeighbourClassification(NearestNeighbourClassification original, Cloner cloner)
      : base(original, cloner) {
    }
    public NearestNeighbourClassification()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(KParameterName, "The number of nearest neighbours to consider for regression.", new IntValue(3)));
      Parameters.Add(new OptionalValueParameter<DoubleArray>(WeightsParameterName, "Optional: use weights to specify individual scaling values for all features. If not set the weights are calculated automatically (each feature is scaled to unit variance)"));
      Problem = new ClassificationProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(WeightsParameterName)) {
        Parameters.Add(new OptionalValueParameter<DoubleArray>(WeightsParameterName, "Optional: use weights to specify individual scaling values for all features. If not set the weights are calculated automatically (each feature is scaled to unit variance)"));
      }
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NearestNeighbourClassification(this, cloner);
    }

    #region nearest neighbour
    protected override void Run(CancellationToken cancellationToken) {
      double[] weights = null;
      if (Weights != null) weights = Weights.CloneAsArray();
      var solution = CreateNearestNeighbourClassificationSolution(Problem.ProblemData, K, weights);
      Results.Add(new Result(NearestNeighbourClassificationModelResultName, "The nearest neighbour classification solution.", solution));
    }

    public static IClassificationSolution CreateNearestNeighbourClassificationSolution(IClassificationProblemData problemData, int k, double[] weights = null) {
      var problemDataClone = (IClassificationProblemData)problemData.Clone();
      return new NearestNeighbourClassificationSolution(Train(problemDataClone, k, weights), problemDataClone);
    }

    public static INearestNeighbourModel Train(IClassificationProblemData problemData, int k, double[] weights = null) {
      return new NearestNeighbourModel(problemData.Dataset,
        problemData.TrainingIndices,
        k,
        problemData.TargetVariable,
        problemData.AllowedInputVariables,
        weights,
        problemData.ClassValues.ToArray());
    }
    #endregion
  }
}
