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

namespace HeuristicLab.Selection {
  /// <summary>
  /// Copies or moves a defined number of sub scopes from a source scope to a target scope, being selected
  /// randomly.
  /// </summary>
  public class RandomSelector : StochasticSelectorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Copies or moves a number of sub scopes (<paramref name="selected"/>) from <paramref name="source"/>
    /// to the <paramref name="target"/>, chosen randomly.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="source">The source scope from which to copy/move the sub scopes.</param>
    /// <param name="selected">The number of sub scopes to move.</param>
    /// <param name="target">The target where to copy/move the sub scopes.</param>
    /// <param name="copySelected">Boolean flag whether the sub scopes shall be copied or moved.</param>
    protected override void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected) {
      for (int i = 0; i < selected; i++) {
        if (copySelected) {
          target.AddSubScope((IScope)source.SubScopes[random.Next(source.SubScopes.Count)].Clone());
        } else {
          IScope s = source.SubScopes[random.Next(source.SubScopes.Count)];
          source.RemoveSubScope(s);
          target.AddSubScope(s);
        }
      }
    }
  }
}
