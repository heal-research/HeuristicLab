#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Operator that migrates the selected sub scopes in each subscope in an unidirectional ring.
  /// </summary>
  [Item("UnidirectionalRingMigrator", "Migrates the selected sub scopes in each subscope in an unidirectional ring.")]
  [StorableClass]
  public class UnidirectionalRingMigrator : SingleSuccessorOperator, IMigrator {
    [StorableConstructor]
    protected UnidirectionalRingMigrator(bool deserializing) : base(deserializing) { }
    protected UnidirectionalRingMigrator(UnidirectionalRingMigrator original, Cloner cloner) : base(original, cloner) { }
    public UnidirectionalRingMigrator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UnidirectionalRingMigrator(this, cloner);
    }

    /// <summary>
    /// Migrates every first sub scope of each child to its left neighbour (like a ring).
    /// <pre>                                                               
    ///          __ scope __              __ scope __
    ///         /     |     \            /     |     \
    ///       Pop1   Pop2   Pop3  =>   Pop1   Pop2   Pop3
    ///       / \    / \    / \        / \    / \    / \
    ///      R   S  R   S  R   S      R   S  R   S  R   S
    ///     /|\  | /|\  | /|\  |     /|\  | /|\  | /|\  |
    ///     ABC  A DEF  D GHI  G     ABC  G DEF  A GHI  D
    /// </pre>
    /// </summary>
    /// <returns>The next operation.</returns>
    public override IOperation Apply() {
      IScope scope = ExecutionContext.Scope;
      List<IScope> emigrantsList = new List<IScope>();

      for (int i = 0; i < scope.SubScopes.Count; i++) {
        IScope emigrants = scope.SubScopes[i].SubScopes[1];
        scope.SubScopes[i].SubScopes.Remove(emigrants);
        emigrantsList.Add(emigrants);
      }

      // shift first emigrants to end of list
      emigrantsList.Add(emigrantsList[0]);
      emigrantsList.RemoveAt(0);

      for (int i = 0; i < scope.SubScopes.Count; i++)
        scope.SubScopes[i].SubScopes.Add(emigrantsList[i]);

      return base.Apply();
    }
  }
}
