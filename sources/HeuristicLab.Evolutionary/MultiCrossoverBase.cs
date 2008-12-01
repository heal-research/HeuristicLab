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
using HeuristicLab.Operators;

namespace HeuristicLab.Evolutionary {
  /// <summary>
  /// Base class for cross over operators that use more than two parents.
  /// </summary>
  public abstract class MultiCrossoverBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="MultiCrossoverBase"/> with two variable infos
    /// (<c>Parents</c> and <c>Random</c>).
    /// </summary>
    public MultiCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("Parents", "Number of parents that should be crossed", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
    }

    /// <summary>
    /// Replaces the parents (sub scopes of the given <paramref name="scope"/>) with created children
    /// by crossing over a specified number of parents.
    /// </summary>
    /// <remarks>Adds the children to the given <paramref name="scope"/> and removes the parents.</remarks>
    /// <exception cref="InvalidOperationException">Thrown when the size of the mating pool and the 
    /// number of parents don't match.</exception>
    /// <param name="scope">The scope whose sub scopes shall be crossed over.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      int parents = GetVariableValue<IntData>("Parents", scope, true).Data;

      if ((scope.SubScopes.Count % parents) != 0)
        throw new InvalidOperationException("Size of mating pool and number of parents don't match");

      int children = scope.SubScopes.Count / parents;
      for (int i = 0; i < children; i++) {
        IScope[] parentScopes = new IScope[parents];
        for (int j = 0; j < parentScopes.Length; j++)
          parentScopes[j] = scope.SubScopes[j];

        IScope child = new Scope(i.ToString());
        scope.AddSubScope(child);
        Cross(scope, random, parentScopes, child);

        for (int j = 0; j < parentScopes.Length; j++)
          scope.RemoveSubScope(parentScopes[j]);
      }
      return null;
    }
    /// <summary>
    /// Performs a cross over of a number of <paramref name="parents"/>
    /// to create a new <paramref name="child"/>.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">The scopes to cross over.</param>
    /// <param name="child">The result of the cross over.</param>
    protected abstract void Cross(IScope scope, IRandom random, IScope[] parents, IScope child);
  }
}
