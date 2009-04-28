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

namespace HeuristicLab.Permutation {
  /// <summary>
  /// Base class for manipulation permutation.
  /// </summary>
  public abstract class PermutationManipulatorBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="PermutationManipulatorBase"/> with two variable infos
    /// (<c>Random</c> and <c>Permutation</c>).
    /// </summary>
    public PermutationManipulatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Permutation", "Permutation to manipulate", typeof(Permutation), VariableKind.In | VariableKind.Out));
    }

    /// <summary>
    /// Manipulates the permutation data of the given <paramref name="scope"/> according to a
    /// specified random number generator.
    /// </summary>
    /// <remarks>Calls <see cref="Manipulate"/>.</remarks>
    /// <param name="scope">The scope of the permutation data and the random number generator.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      Permutation perm = GetVariableValue<Permutation>("Permutation", scope, false);
      perm.Data = Manipulate(scope, random, perm.Data);
      return null;
    }

    /// <summary>
    /// Manipulates the given <paramref name="permutation"/> with the specified <paramref name="random"/> 
    /// number generator.
    /// </summary>
    /// <param name="scope">The scope of the variables.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation to manipulate.</param>
    /// <returns>The manipulated permutation as int array.</returns>
    protected abstract int[] Manipulate(IScope scope, IRandom random, int[] permutation);
  }
}
