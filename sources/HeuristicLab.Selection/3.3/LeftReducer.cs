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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// An operator which reduces to the sub-scopes of the leftmost sub-scope of the current scope.
  /// </summary>
  [Item("LeftReducer", "An operator which reduces to the sub-scopes of the leftmost sub-scope of the current scope.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public sealed class LeftReducer : Reducer {
    public LeftReducer() : base() { }

    protected override ScopeList Reduce(ScopeList scopes) {
      ScopeList reduced = new ScopeList();
      if (scopes.Count > 0) reduced.AddRange(scopes[0].SubScopes);
      return reduced;
    }
  }
}
