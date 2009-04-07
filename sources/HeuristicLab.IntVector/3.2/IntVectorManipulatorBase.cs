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

namespace HeuristicLab.IntVector {
  /// <summary>
  /// Base class for all integer vector manipulators.
  /// </summary>
  public abstract class IntVectorManipulatorBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="IntVectorManipulatorBase"/> with two variable infos
    /// (<c>Random</c> and <c>IntVector</c>).
    /// </summary>
    public IntVectorManipulatorBase() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("IntVector", "Integer vector to manipulate", typeof(IntArrayData), VariableKind.In | VariableKind.Out));
    }

    /// <summary>
    /// Manipulates the integer vector.
    /// </summary>
    /// <param name="scope">The current scope whose integer vector to manipulate.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      IntArrayData vector = GetVariableValue<IntArrayData>("IntVector", scope, false);
      vector.Data = Manipulate(scope, random, vector.Data);
      return null;
    }

    /// <summary>
    /// Manipulates the given integer <paramref name="vector"/> with the given random number generator.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>
    /// <returns>The manipulated integer vector.</returns>
    protected abstract int[] Manipulate(IScope scope, IRandom random, int[] vector);
  }
}
