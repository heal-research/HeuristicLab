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
  /// Base class for crossover operators.
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
    /// Retrieves the random number generator and calls <see cref="Cross(HeuristicLab.Core.IScope, HeuristicLab.Core.IRandom)"/>.
    /// </summary>
    /// <param name="scope">The current scope which represents a new child.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      Cross(scope, random);
      return null;
    }

    /// <summary>
    /// Performs a crossover of all parents (sub-scopes) of <paramref name="scope"/> to create a new child.
    /// Note that all children have to be prepared using the <see cref="HeuristicLab.Evolutionary.ChildrenInitializer"/>.
    /// </summary>
    /// <param name="scope">The current scope which represents a new child.</param>
    /// <param name="random">A random number generator.</param>
    protected abstract void Cross(IScope scope, IRandom random);
  }
}
