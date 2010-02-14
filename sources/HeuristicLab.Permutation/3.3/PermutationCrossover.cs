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

using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Permutation {
  /// <summary>
  /// A base class for permutation crossover operators.
  /// </summary>
  [Item("PermutationCrossover", "A base class for permutation crossover operators.")]
  [EmptyStorableClass]
  public abstract class PermutationCrossover : SingleSuccessorOperator {
    public LookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public SubScopesLookupParameter<Permutation> ParentsParameter {
      get { return (SubScopesLookupParameter<Permutation>)Parameters["Parents"]; }
    }
    public LookupParameter<Permutation> ChildParameter {
      get { return (LookupParameter<Permutation>)Parameters["Child"]; }
    }

    protected PermutationCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic crossover operators."));
      Parameters.Add(new SubScopesLookupParameter<Permutation>("Parents", "The parent permutations which should be crossed."));
      Parameters.Add(new LookupParameter<Permutation>("Child", "The child permutation resulting from the crossover."));
    }

    public sealed override IExecutionSequence Apply() {
      ChildParameter.ActualValue = Cross(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      return base.Apply();
    }

    protected abstract Permutation Cross(IRandom random, ItemArray<Permutation> parents);
  }
}
