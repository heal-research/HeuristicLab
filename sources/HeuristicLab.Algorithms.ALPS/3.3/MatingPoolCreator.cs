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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ALPS {
  [Item("MatingPoolCreator", "An operator which creates mating pools based on a set of sub-populations. For each sub-population, the individuals from the previous sub-population are copied into the current sub-population.")]
  [StorableClass]
  public sealed class MatingPoolCreator : SingleSuccessorOperator {
    public IValueLookupParameter<IntValue> MatingPoolRangeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MatingPoolRange"]; }
    }

    [StorableConstructor]
    private MatingPoolCreator(bool deserializing)
      : base(deserializing) { }
    private MatingPoolCreator(MatingPoolCreator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MatingPoolCreator(this, cloner);
    }
    public MatingPoolCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("MatingPoolRange", "The range of sub-populations used for creating a mating pool. (1 = current + previous sub-population)", new IntValue(1)));
    }

    /// <summary>
    /// Copies the subscopes of the n previous scopes into the current scope. (default n = 1)
    /// <pre>
    ///          __ scope __              __ scope __
    ///         /     |     \            /     |     \
    ///       Pop1   Pop2   Pop3  =>   Pop1   Pop2   Pop3
    ///       /|\    /|\    /|\         /|\   /|\    /|\
    ///       ABC    DEF    GHI         ABC   DEF    GHI
    ///                                       ABC    DEF
    /// </pre>
    /// </summary>
    /// <returns>The next operation.</returns>
    public override IOperation Apply() {
      var subScopes = ExecutionContext.Scope.SubScopes;
      int range = MatingPoolRangeParameter.ActualValue.Value;

      for (int targetIndex = subScopes.Count - 1; targetIndex > 0; targetIndex--) {
        var targetScope = subScopes[targetIndex];
        for (int n = 1; (n <= range) && (targetIndex - n >= 0); n++) {
          var prevScope = subScopes[targetIndex - n];
          var individuals = prevScope.SubScopes;
          foreach (var individual in individuals) {
            targetScope.SubScopes.Add((IScope)individual.Clone());
          }
        }
      }

      return base.Apply();
    }
  }
}