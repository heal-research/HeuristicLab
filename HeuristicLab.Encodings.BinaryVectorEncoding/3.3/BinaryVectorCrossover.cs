#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// A base class for operators that perform a crossover of bool-valued vectors.
  /// </summary>
  [Item("BinaryVectorCrossover", "A base class for operators that perform a crossover of bool-valued vectors.")]
  [StorableClass]
  public abstract class BinaryVectorCrossover : InstrumentedOperator, IBinaryVectorCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<BinaryVector>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<BinaryVector>)Parameters["Parents"]; }
    }
    public ILookupParameter<BinaryVector> ChildParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["Child"]; }
    }

    [StorableConstructor]
    protected BinaryVectorCrossover(bool deserializing) : base(deserializing) { }
    protected BinaryVectorCrossover(BinaryVectorCrossover original, Cloner cloner) : base(original, cloner) { }
    protected BinaryVectorCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic crossover operators."));
      Parameters.Add(new ScopeTreeLookupParameter<BinaryVector>("Parents", "The parent vectors which should be crossed."));
      ParentsParameter.ActualName = "BinaryVector";
      Parameters.Add(new LookupParameter<BinaryVector>("Child", "The child vector resulting from the crossover."));
      ChildParameter.ActualName = "BinaryVector";
    }

    public sealed override IOperation InstrumentedApply() {
      ChildParameter.ActualValue = Cross(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      return base.InstrumentedApply();
    }

    protected abstract BinaryVector Cross(IRandom random, ItemArray<BinaryVector> parents);
  }
}
