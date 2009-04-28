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

namespace HeuristicLab.Selection {
  /// <summary>
  /// Selects scopes based on their rank, which has been determined through their quality.
  /// </summary>
  public class LinearRankSelector : StochasticSelectorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LinearRankSelector"/> with the <c>CopySelected</c> flag
    /// set to <c>true</c>.
    /// </summary>
    public LinearRankSelector() {
      GetVariable("CopySelected").GetValue<BoolData>().Data = true;
    }

    /// <summary>
    /// Copies or moves sub scopes from the given <paramref name="source"/> to the specified
    /// <paramref name="target"/> according to their rank which is determined through their quality.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no source sub scopes are available.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="source">The source scope from where to copy/move the sub scopes.</param>
    /// <param name="selected">The number of sub scopes to copy/move.</param>
    /// <param name="target">The target scope where to add the sub scopes.</param>
    /// <param name="copySelected">Boolean flag whether the sub scopes shall be moved or copied.</param>
    protected override void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected) {
      int subScopes = source.SubScopes.Count;
      int lotSum = (subScopes * (subScopes + 1)) / 2;
      int selectedLot;
      int currentLot;
      int index;

      for (int i = 0; i < selected; i++) {
        if (subScopes < 1) throw new InvalidOperationException("No source scopes available to select.");

        selectedLot = random.Next(1, lotSum + 1);
        currentLot = subScopes;  // first individual is the best one
        index = 0;
        while (currentLot < selectedLot) {
          index++;
          currentLot += subScopes - index;
        }
        IScope selectedScope = source.SubScopes[index];
        if (copySelected)
          target.AddSubScope((IScope)selectedScope.Clone());
        else {
          source.RemoveSubScope(selectedScope);
          target.AddSubScope(selectedScope);
          subScopes--;
          lotSum = (subScopes * (subScopes + 1)) / 2;
        }
      }
    }
  }
}
