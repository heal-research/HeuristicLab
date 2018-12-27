#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Operator that migrates the selected sub scopes in each subscope in an unidirectional ring.
  /// </summary>
  [Item("UnidirectionalRingMigrator", "Migrates the selected sub scopes in each subscope in an unidirectional ring.")]
  [StorableClass]
  public class UnidirectionalRingMigrator : SingleSuccessorOperator, IMigrator {
    public IValueLookupParameter<BoolValue> ClockwiseMigrationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ClockwiseMigration"]; }
    }

    [StorableConstructor]
    protected UnidirectionalRingMigrator(bool deserializing) : base(deserializing) { }
    protected UnidirectionalRingMigrator(UnidirectionalRingMigrator original, Cloner cloner) : base(original, cloner) { }

    public UnidirectionalRingMigrator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("ClockwiseMigration", "True to migrate individuals clockwise, false to migrate individuals counterclockwise.", new BoolValue(true)));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (!Parameters.ContainsKey("ClockwiseMigration")) {
        Parameters.Add(new ValueLookupParameter<BoolValue>("ClockwiseMigration", "True to migrate individuals clockwise, false to migrate individuals counterclockwise.", new BoolValue(false)));
      }
      #endregion
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new UnidirectionalRingMigrator(this, cloner);
    }

    /// <summary>
    /// Migrates every first sub scope of each child to its right or left neighbour (like a ring).
    /// If clockwise migration (default) is used the selected scopes A D G becomes G A D, contrary to counterclockwise where A D G becomes D G A.
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
      bool clockwise = ClockwiseMigrationParameter.ActualValue.Value;
      IScope scope = ExecutionContext.Scope;
      List<IScope> emigrantsList = new List<IScope>();

      for (int i = 0; i < scope.SubScopes.Count; i++) {
        IScope emigrants = scope.SubScopes[i].SubScopes[1];
        scope.SubScopes[i].SubScopes.Remove(emigrants);
        emigrantsList.Add(emigrants);
      }

      if (clockwise) {
        // shift last emigrants to start of list
        emigrantsList.Insert(0, emigrantsList[emigrantsList.Count - 1]);
        emigrantsList.RemoveAt(emigrantsList.Count - 1);
      } else {
        // shift first emigrants to end of list
        emigrantsList.Add(emigrantsList[0]);
        emigrantsList.RemoveAt(0);
      }

      for (int i = 0; i < scope.SubScopes.Count; i++)
        scope.SubScopes[i].SubScopes.Add(emigrantsList[i]);

      return base.Apply();
    }
  }
}
