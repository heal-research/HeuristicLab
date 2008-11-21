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
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Removes one specified or - if nothing specified - all operators from the given scope.
  /// </summary>
  public class SubScopesRemover : OperatorBase {
    /// <inheritdoc/>
    public override string Description {
      get {
        return @"This operator either removes the subscope specified in the variable SubScopeIndex or if this variable is absent, removes all subscopes.";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SubScopesRemover"/> with one variable info
    /// (<c>SubScopeIndex</c>).
    /// </summary>
    public SubScopesRemover()
      : base() {
      AddVariableInfo(new VariableInfo("SubScopeIndex", "(Optional) the index of the subscope to remove", typeof(IntData), VariableKind.In));
    }

    /// <summary>
    /// Removes one sub scope with a specified index from the given <paramref name="scope"/>, or if no
    /// index has been specified, all sub scopes are removed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no sub scope with the specified index 
    /// exists.</exception>
    /// <param name="scope">The scope whose sub scope(s) to remove.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IntData index = GetVariableValue<IntData>("SubScopeIndex", scope, true, false);
      if (index == null) { // remove all scopes
        while (scope.SubScopes.Count > 0) {
          scope.RemoveSubScope(scope.SubScopes[0]);
        }
      } else {
        if (index.Data < 0 && index.Data >= scope.SubScopes.Count) throw new InvalidOperationException("ERROR: no scope with index " + index.Data + " exists");
        scope.RemoveSubScope(scope.SubScopes[index.Data]);
      }
      return null;
    }
  }
}
