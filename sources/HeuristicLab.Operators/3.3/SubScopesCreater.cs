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
  /// Operator to create sub scopes in a given scope.
  /// </summary>
  public class SubScopesCreater : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SubScopesCreater"/> with one variable info (<c>SubScopes</c>).
    /// </summary>
    public SubScopesCreater()
      : base() {
      AddVariableInfo(new VariableInfo("SubScopes", "Number of sub-scopes", typeof(IntData), VariableKind.In));
    }

    /// <summary>
    /// Creates a specified number of sub scopes in the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to create the sub scopes.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IntData count = GetVariableValue<IntData>("SubScopes", scope, true);

      for (int i = 0; i < count.Data; i++)
        scope.AddSubScope(new Scope(i.ToString()));

      return null;
    }
  }
}
