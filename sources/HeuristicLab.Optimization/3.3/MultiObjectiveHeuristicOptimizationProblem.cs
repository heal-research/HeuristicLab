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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Multi-Objective Heuristic Optimization Problem", "A base class for multi-objective heuristic optimization problems.")]
  [StorableClass]
  public abstract class MultiObjectiveHeuristicOptimizationProblem<T, U> : HeuristicOptimizationProblem<T, U>, IMultiObjectiveHeuristicOptimizationProblem
    where T : class, IMultiObjectiveEvaluator
    where U : class, ISolutionCreator {
    private const string MaximizationParameterName = "Maximization";

    [StorableConstructor]
    protected MultiObjectiveHeuristicOptimizationProblem(bool deserializing) : base(deserializing) { }
    protected MultiObjectiveHeuristicOptimizationProblem(MultiObjectiveHeuristicOptimizationProblem<T, U> original, Cloner cloner) : base(original, cloner) { }
    protected MultiObjectiveHeuristicOptimizationProblem()
      : base() {
      Parameters.Add(new ValueParameter<BoolArray>(MaximizationParameterName, "Determines for each objective whether it should be maximized or minimized."));
    }

    protected MultiObjectiveHeuristicOptimizationProblem(T evaluator, U solutionCreator)
      : base(evaluator, solutionCreator) {
      Parameters.Add(new ValueParameter<BoolArray>(MaximizationParameterName, "Determines for each objective whether it should be maximized or minimized."));
    }

    public ValueParameter<BoolArray> MaximizationParameter {
      get { return (ValueParameter<BoolArray>)Parameters[MaximizationParameterName]; }
    }
    IParameter IMultiObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public BoolArray Maximization {
      get { return MaximizationParameter.Value; }
      protected set { MaximizationParameter.Value = value; }
    }

    IMultiObjectiveEvaluator IMultiObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
  }
}
