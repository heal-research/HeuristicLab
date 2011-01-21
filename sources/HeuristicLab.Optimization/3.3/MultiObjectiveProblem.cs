#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Multi-Objective Problem", "A base class for multi-objective problems.")]
  [StorableClass]
  public abstract class MultiObjectiveProblem<T, U> : Problem<T, U>, IMultiObjectiveProblem
    where T : class, IMultiObjectiveEvaluator
    where U : class, ISolutionCreator {
    private const string MaximizationParameterName = "Maximization";

    [StorableConstructor]
    protected MultiObjectiveProblem(bool deserializing) : base(deserializing) { }
    protected MultiObjectiveProblem(MultiObjectiveProblem<T, U> original, Cloner cloner) : base(original, cloner) { }
    protected MultiObjectiveProblem()
      : base() {
      Parameters.Add(new ValueParameter<BoolArray>(MaximizationParameterName, "Determines for each objective whether it should be maximized or minimized."));
    }

    public ValueParameter<BoolArray> MaximizationParameter {
      get { return (ValueParameter<BoolArray>)Parameters[MaximizationParameterName]; }
    }
    IParameter IMultiObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public BoolArray Maximization {
      get { return MaximizationParameter.Value; }
      protected set { MaximizationParameter.Value = value; }
    }

    IMultiObjectiveEvaluator IMultiObjectiveProblem.Evaluator {
      get { return Evaluator; }
    }
  }
}
