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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Single-Objective Heuristic OptimizationProblem", "A base class for single-objective heuristic optimization problems.")]
  [StorableClass]
  public abstract class SingleObjectiveHeuristicOptimizationProblem<T, U> : HeuristicOptimizationProblem<T, U>, ISingleObjectiveHeuristicOptimizationProblem
    where T : class, ISingleObjectiveEvaluator
    where U : class, ISolutionCreator {
    private const string MaximizationParameterName = "Maximization";
    private const string BestKnownQualityParameterName = "BestKnownQuality";

    [StorableConstructor]
    protected SingleObjectiveHeuristicOptimizationProblem(bool deserializing) : base(deserializing) { }
    protected SingleObjectiveHeuristicOptimizationProblem(SingleObjectiveHeuristicOptimizationProblem<T, U> original, Cloner cloner) : base(original, cloner) { }
    protected SingleObjectiveHeuristicOptimizationProblem()
      : base() {
      Parameters.Add(new ValueParameter<BoolValue>(MaximizationParameterName, "Set to false if the problem should be minimized.", new BoolValue()));
      Parameters.Add(new OptionalValueParameter<DoubleValue>(BestKnownQualityParameterName, "The quality of the best known solution of this problem."));
    }

    protected SingleObjectiveHeuristicOptimizationProblem(T evaluator, U solutionCreator)
      : base(evaluator, solutionCreator) {
      Parameters.Add(new ValueParameter<BoolValue>(MaximizationParameterName, "Set to false if the problem should be minimized.", new BoolValue()));
      Parameters.Add(new OptionalValueParameter<DoubleValue>(BestKnownQualityParameterName, "The quality of the best known solution of this problem."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (BestKnownQualityParameter is ValueParameter<DoubleValue>) {
        Parameters.Remove(BestKnownQualityParameterName);
        Parameters.Add(new OptionalValueParameter<DoubleValue>(BestKnownQualityParameterName, "The quality of the best known solution of this problem."));
      }
      #endregion
    }
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public BoolValue Maximization {
      get { return MaximizationParameter.Value; }
      set { MaximizationParameter.Value = value; }
    }

    public IValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[BestKnownQualityParameterName]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }

    ISingleObjectiveEvaluator ISingleObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
  }
}
