#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// A base class for operators that perform a crossover of int-valued vectors.
  /// </summary>
  [Item("IntegerVectorCrossover", "A base class for operators that perform a crossover of int-valued vectors.")]
  [StorableClass]
  public abstract class IntegerVectorCrossover : IntegerVectorOperator, IIntegerVectorCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<IntegerVector>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<IntegerVector>)Parameters["Parents"]; }
    }
    public ILookupParameter<IntegerVector> ChildParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["Child"]; }
    }

    [StorableConstructor]
    protected IntegerVectorCrossover(bool deserializing) : base(deserializing) { }
    protected IntegerVectorCrossover(IntegerVectorCrossover original, Cloner cloner) : base(original, cloner) { }
    protected IntegerVectorCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic crossover operators."));
      Parameters.Add(new ScopeTreeLookupParameter<IntegerVector>("Parents", "The parent vectors which should be crossed."));
      ParentsParameter.ActualName = "IntegerVector";
      Parameters.Add(new LookupParameter<IntegerVector>("Child", "The child vector resulting from the crossover."));
      ChildParameter.ActualName = "IntegerVector";
    }

    public sealed override IOperation InstrumentedApply() {
      ChildParameter.ActualValue = Cross(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      return base.InstrumentedApply();
    }

    protected abstract IntegerVector Cross(IRandom random, ItemArray<IntegerVector> parents);
  }
}
