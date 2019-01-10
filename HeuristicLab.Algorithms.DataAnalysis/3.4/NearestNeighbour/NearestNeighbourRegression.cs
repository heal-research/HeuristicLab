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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Nearest neighbour regression data analysis algorithm.
  /// </summary>
  [Item("Nearest Neighbour Regression (kNN)", "Nearest neighbour regression data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 150)]
  [StorableType("3F940BE0-4F44-4F7F-A3EE-E47423C7F22D")]
  public sealed class NearestNeighbourRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string KParameterName = "K";
    private const string NearestNeighbourRegressionModelResultName = "Nearest neighbour regression solution";
    private const string WeightsParameterName = "Weights";
    private const string SelfMatchParameterName = "SelfMatch";

    #region parameter properties
    public IFixedValueParameter<IntValue> KParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[KParameterName]; }
    }
    public IFixedValueParameter<BoolValue> SelfMatchParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[SelfMatchParameterName]; }
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
    public bool SelfMatch {
      get { return SelfMatchParameter.Value.Value; }
      set { SelfMatchParameter.Value.Value = value; }
    }
    public DoubleArray Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private NearestNeighbourRegression(StorableConstructorFlag _) : base(_) { }
    private NearestNeighbourRegression(NearestNeighbourRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public NearestNeighbourRegression()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(KParameterName, "The number of nearest neighbours to consider for regression.", new IntValue(3)));
      Parameters.Add(new OptionalValueParameter<DoubleArray>(WeightsParameterName, "Optional: use weights to specify individual scaling values for all features. If not set the weights are calculated automatically (each feature is scaled to unit variance)"));
      Parameters.Add(new FixedValueParameter<BoolValue>(SelfMatchParameterName, "Should we use equal points for classification?", new BoolValue(false)));
      Problem = new RegressionProblem();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(WeightsParameterName)) {
        Parameters.Add(new OptionalValueParameter<DoubleArray>(WeightsParameterName, "Optional: use weights to specify individual scaling values for all features. If not set the weights are calculated automatically (each feature is scaled to unit variance)"));
      }
      if (!Parameters.ContainsKey(SelfMatchParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(SelfMatchParameterName, "Should we use equal points for classification?", new BoolValue(false)));
      }
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NearestNeighbourRegression(this, cloner);
    }

    #region nearest neighbour
    protected override void Run(CancellationToken cancellationToken) {
      double[] weights = null;
      if (Weights != null) weights = Weights.CloneAsArray();
      var solution = CreateNearestNeighbourRegressionSolution(Problem.ProblemData, K, SelfMatch, weights);
      Results.Add(new Result(NearestNeighbourRegressionModelResultName, "The nearest neighbour regression solution.", solution));
    }

    public static IRegressionSolution CreateNearestNeighbourRegressionSolution(IRegressionProblemData problemData, int k, bool selfMatch = false, double[] weights = null) {
      var clonedProblemData = (IRegressionProblemData)problemData.Clone();
      return new NearestNeighbourRegressionSolution(Train(problemData, k, selfMatch, weights), clonedProblemData);
    }

    public static INearestNeighbourModel Train(IRegressionProblemData problemData, int k, bool selfMatch = false, double[] weights = null) {
      return new NearestNeighbourModel(problemData.Dataset,
        problemData.TrainingIndices,
        k,
        selfMatch,
        problemData.TargetVariable,
        problemData.AllowedInputVariables,
        weights);
    }
    #endregion
  }
}
