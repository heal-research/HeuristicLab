#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Operators;

namespace HeuristicLab.Selection {
  /// <summary>
  /// Takes only sub scopes from the left child of the tree.
  /// </summary>
  public class LeftReducer : ReducerBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Takes only the sub scopes from the left sub scope of the tree.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <returns>All sub scopes from the left part of the tree.</returns>
    protected override ICollection<IScope> Reduce(IScope scope) {
      List<IScope> subScopes = new List<IScope>();

      if (scope.SubScopes.Count > 0)
        subScopes.AddRange(scope.SubScopes[0].SubScopes);
      return subScopes;
    }
  }
}
