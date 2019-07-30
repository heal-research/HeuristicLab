#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("StdDevStrategyVectorCrossover", "Crosses the strategy vector by using intermediate recombination (average crossover).")]
  [StorableType("3CCF8FC5-6D6E-4E3E-AD8B-E23B869B97FC")]
  public class StdDevStrategyVectorCrossover : SingleSuccessorOperator, IStochasticOperator, IRealVectorStdDevStrategyParameterCrossover {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<RealVector>> ParentsParameter {
      get { return (ILookupParameter<ItemArray<RealVector>>)Parameters["ParentStrategyParameter"]; }
    }
    public ILookupParameter<RealVector> StrategyParameterParameter {
      get { return (ILookupParameter<RealVector>)Parameters["StrategyParameter"]; }
    }

    [StorableConstructor]
    protected StdDevStrategyVectorCrossover(StorableConstructorFlag _) : base(_) { }
    protected StdDevStrategyVectorCrossover(StdDevStrategyVectorCrossover original, Cloner cloner) : base(original, cloner) { }
    public StdDevStrategyVectorCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("ParentStrategyParameter", "The strategy parameters to cross."));
      Parameters.Add(new LookupParameter<RealVector>("StrategyParameter", "The crossed strategy parameter."));
      ParentsParameter.ActualName = "StrategyParameter";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StdDevStrategyVectorCrossover(this, cloner);
    }

    public override IOperation Apply() {
      StrategyParameterParameter.ActualValue = AverageCrossover.Apply(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      return base.Apply();
    }
  }
}
