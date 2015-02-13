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

using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.EvolutionStrategy {
  [Item("SelfAdaptiveCrossover", "Applies one crossover on the endogeneous strategy parameters and another on the actual solution encoding.")]
  [StorableClass]
  public class SelfAdaptiveCrossover : SingleSuccessorOperator, ICrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueParameter<IRealVectorCrossover> StrategyVectorCrossoverParameter {
      get { return (ValueParameter<IRealVectorCrossover>)Parameters["StrategyVectorCrossover"]; }
    }
    public ConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (ConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }

    public IRealVectorCrossover StrategyVectorCrossover {
      get { return StrategyVectorCrossoverParameter.Value; }
      set { StrategyVectorCrossoverParameter.Value = value; }
    }
    public ICrossover Crossover {
      get { return CrossoverParameter.Value; }
      set { CrossoverParameter.Value = value; }
    }

    public SelfAdaptiveCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ValueParameter<IRealVectorCrossover>("StrategyVectorCrossover", "The crossover operator for the strategy vectors.", new AverageCrossover()));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The solution specific crossover operator."));
    }

    public override IOperation Apply() {
      OperationCollection next = new OperationCollection();
      next.Add(ExecutionContext.CreateOperation(StrategyVectorCrossover));
      if (Crossover != null)
        next.Add(ExecutionContext.CreateOperation(Crossover));
      next.Add(base.Apply());
      return next;
    }
  }
}
