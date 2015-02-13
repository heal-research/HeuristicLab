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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// A base class for permutation crossover operators.
  /// </summary>
  [Item("PermutationCrossover", "A base class for permutation crossover operators.")]
  [StorableClass]
  public abstract class PermutationCrossover : InstrumentedOperator, IPermutationCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<Permutation>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<Permutation>)Parameters["Parents"]; }
    }
    public ILookupParameter<Permutation> ChildParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Child"]; }
    }

    [StorableConstructor]
    protected PermutationCrossover(bool deserializing) : base(deserializing) { }
    protected PermutationCrossover(PermutationCrossover original, Cloner cloner) : base(original, cloner) { }
    protected PermutationCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic crossover operators."));
      Parameters.Add(new ScopeTreeLookupParameter<Permutation>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "Permutation";
      Parameters.Add(new LookupParameter<Permutation>("Child", "The child permutation resulting from the crossover."));
      ChildParameter.ActualName = "Permutation";
    }

    public sealed override IOperation InstrumentedApply() {
      ChildParameter.ActualValue = Cross(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      return base.InstrumentedApply();
    }

    protected abstract Permutation Cross(IRandom random, ItemArray<Permutation> parents);
  }
}
