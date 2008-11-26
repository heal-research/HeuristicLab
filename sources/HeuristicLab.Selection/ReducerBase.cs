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
  /// Base class for all reducers.
  /// </summary>
  public abstract class ReducerBase : OperatorBase {
    /// <summary>
    /// Reduces the given <paramref name="scope"/> so that it contains in the end only the reduced
    /// elements.
    /// </summary>
    /// <remarks>Calls <see cref="Reduce"/>.</remarks>
    /// <param name="scope">The scope to reduce.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      ICollection<IScope> subScopes = Reduce(scope);

      while (scope.SubScopes.Count > 0)
        scope.RemoveSubScope(scope.SubScopes[0]);

      foreach (IScope s in subScopes)
        scope.AddSubScope(s);

      return null;
    }

    /// <summary>
    /// Reduces the current <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope to reduce.</param>
    /// <returns>The reduced list of scopes, that should be kept.</returns>
    protected abstract ICollection<IScope> Reduce(IScope scope);
  }
}
