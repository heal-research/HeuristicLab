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

namespace HeuristicLab.Evolutionary {
  /// <summary>
  /// Base class for crossing over operators.
  /// </summary>
  public abstract class CrossoverBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="CrossoverBase"/> with one variable info (<c>Random</c>).
    /// </summary>
    public CrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
    }

    /// <summary>
    /// Replaces the parents (the sub scopes of the current <paramref name="scope"/>) with created children
    /// by crossing over of two adjacent sub scopes.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the size of the mating pool 
    /// is not even.</exception>
    /// <param name="scope">The current scope whose sub scopes shall be parents.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);

      if (scope.SubScopes.Count != 2)
        throw new InvalidOperationException("ERROR: Number of parents is != 2");

      IScope parent1 = scope.SubScopes[0];
      IScope parent2 = scope.SubScopes[1];
      IScope child = scope;
      Cross(scope, random, parent1, parent2, child);

      return null;
    }

    /// <summary>
    /// Performs a cross over of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// to create a new <paramref name="child"/>.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <param name="child">The resulting child of the cross over.</param>
    protected abstract void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child);
  }
}
