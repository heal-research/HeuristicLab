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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Nearest neighbour classification data analysis algorithm.
  /// </summary>
  [Item("Nearest Neighbour Classification", "Nearest neighbour classification data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class NearestNeighbourClassification : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string KParameterName = "K";
    private const string NearestNeighbourClassificationModelResultName = "Nearest neighbour classification solution";

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
    private NearestNeighbourClassification(bool deserializing) : base(deserializing) { }
    private NearestNeighbourClassification(NearestNeighbourClassification original, Cloner cloner)
      : base(original, cloner) {
    }
    public NearestNeighbourClassification()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(KParameterName, "The number of nearest neighbours to consider for regression.", new IntValue(3)));
      Problem = new ClassificationProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NearestNeighbourClassification(this, cloner);
    }

    #region nearest neighbour
    protected override void Run() {
      var solution = CreateNearestNeighbourClassificationSolution(Problem.ProblemData, K);
      Results.Add(new Result(NearestNeighbourClassificationModelResultName, "The nearest neighbour classification solution.", solution));
    }

    public static IClassificationSolution CreateNearestNeighbourClassificationSolution(IClassificationProblemData problemData, int k) {
      var problemDataClone = (IClassificationProblemData)problemData.Clone();
      return new NearestNeighbourClassificationSolution(problemDataClone, Train(problemDataClone, k));
    }

    public static INearestNeighbourModel Train(IClassificationProblemData problemData, int k) {
      return new NearestNeighbourModel(problemData.Dataset,
        problemData.TrainingIndices,
        k,
        problemData.TargetVariable,
        problemData.AllowedInputVariables,
        problemData.ClassValues.ToArray());
    }
    #endregion
  }
}
