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

namespace HeuristicLab.Selection {
  /// <summary>
  /// Base class for all selectors that use a random number generator.
  /// </summary>
  public abstract class StochasticSelectorBase : SelectorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="StochasticSelectorBase"/> with two variable infos
    /// (<c>Random</c> and <c>Selected</c>).
    /// </summary>
    public StochasticSelectorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Selected", "Number of selected sub-scopes", typeof(IntData), VariableKind.In));
    }

    /// <summary>
    /// Copies or moves randomly chosen sub scopes from the given <paramref name="source"/> to the specified 
    /// <paramref name="target"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Select(HeuristicLab.Core.IRandom, HeuristicLab.Core.IScope, int,
    /// HeuristicLab.Core.IScope, bool)"/></remarks>
    /// <param name="source">The source scope from where to copy/move the sub scopes.</param>
    /// <param name="target">The target scope where to add the sub scopes.</param>
    /// <param name="copySelected">Boolean flag whether the sub scopes shall be moved or copied.</param>
    protected sealed override void Select(IScope source, IScope target, bool copySelected) {
      IRandom random = GetVariableValue<IRandom>("Random", source, true);
      IntData selected = GetVariableValue<IntData>("Selected", source, true);

      Select(random, source, selected.Data, target, copySelected);
    }

    /// <summary>
    /// Copies or moves randomly chosen sub scopes from the given <paramref name="source"/> to the specified 
    /// <paramref name="target"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="source">The source scope from where to copy/move the sub scopes.</param>
    /// <param name="selected">The number of sub scopes to copy/move.</param>
    /// <param name="target">The target scope where to add the sub scopes.</param>
    /// <param name="copySelected">Boolean flag whether the sub scopes shall be moved or copied.</param>
    protected abstract void Select(IRandom random, IScope source, int selected, IScope target, bool copySelected);
  }
}
