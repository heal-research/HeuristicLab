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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Nearest neighbour regression data analysis algorithm.
  /// </summary>
  [Item("Nearest Neighbour Regression (kNN)", "Nearest neighbour regression data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 150)]
  [StorableClass]
  public sealed class NearestNeighbourRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string KParameterName = "K";
    private const string NearestNeighbourRegressionModelResultName = "Nearest neighbour regression solution";

    #region parameter properties
    public IFixedValueParameter<IntValue> KParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[KParameterName]; }
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
    #endregion

    [StorableConstructor]
    private NearestNeighbourRegression(bool deserializing) : base(deserializing) { }
    private NearestNeighbourRegression(NearestNeighbourRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public NearestNeighbourRegression()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(KParameterName, "The number of nearest neighbours to consider for regression.", new IntValue(3)));
      Problem = new RegressionProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NearestNeighbourRegression(this, cloner);
    }

    #region nearest neighbour
    protected override void Run() {
      var solution = CreateNearestNeighbourRegressionSolution(Problem.ProblemData, K);
      Results.Add(new Result(NearestNeighbourRegressionModelResultName, "The nearest neighbour regression solution.", solution));
    }

    public static IRegressionSolution CreateNearestNeighbourRegressionSolution(IRegressionProblemData problemData, int k) {
      var clonedProblemData = (IRegressionProblemData)problemData.Clone();
      return new NearestNeighbourRegressionSolution(clonedProblemData, Train(problemData, k));
    }

    public static INearestNeighbourModel Train(IRegressionProblemData problemData, int k) {
      return new NearestNeighbourModel(problemData.Dataset,
        problemData.TrainingIndices,
        k,
        problemData.TargetVariable,
        problemData.AllowedInputVariables);
    }
    #endregion
  }
}
