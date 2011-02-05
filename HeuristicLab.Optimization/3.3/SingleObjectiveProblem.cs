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
  [Item("Single-Objective Problem", "A base class for single-objective problems.")]
  [StorableClass]
  public abstract class SingleObjectiveProblem<T, U> : Problem<T, U>, ISingleObjectiveProblem
    where T : class, ISingleObjectiveEvaluator
    where U : class, ISolutionCreator {
    private const string MaximizationParameterName = "Maximization";
    private const string BestKnownQualityParameterName = "BestKnownQuality";

    [StorableConstructor]
    protected SingleObjectiveProblem(bool deserializing) : base(deserializing) { }
    protected SingleObjectiveProblem(SingleObjectiveProblem<T, U> original, Cloner cloner) : base(original, cloner) { }
    protected SingleObjectiveProblem()
      : base() {
      Parameters.Add(new ValueParameter<BoolValue>(MaximizationParameterName, "Set to false if the problem should be minimized."));
      Parameters.Add(new ValueParameter<DoubleValue>(BestKnownQualityParameterName, "The quality of the best known solution of this problem."));
    }

    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public BoolValue Maximization {
      get { return MaximizationParameter.Value; }
      protected set { MaximizationParameter.Value = value; }
    }

    public ValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueParameter<DoubleValue>)Parameters[BestKnownQualityParameterName]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      protected set { BestKnownQualityParameter.Value = value; }
    }

    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return Evaluator; }
    }
  }
}
