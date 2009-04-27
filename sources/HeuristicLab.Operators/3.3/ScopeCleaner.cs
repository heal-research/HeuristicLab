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
  /// Operator that removes all variables in the given scope and deletes also all subscopes.
  /// </summary>
  public class ScopeCleaner : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"Removes all variables in the current scope and deletes all subscopes"; }
    }

    /// <summary>
    /// Deletes all variable and sub scopes from the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="IScope.Clear"/> of interface <see cref="IScope"/>.</remarks>
    /// <param name="scope">The scope to clear.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      scope.Clear();
      return null;
    }
  }
}
