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

namespace HeuristicLab.BitVector {
  /// <summary>
  /// Base class for all bit vector manipulators.
  /// </summary>
  public abstract class BitVectorManipulatorBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="BitVectorManipulatorBase"/> with two variable infos
    /// (<c>Random</c> and <c>BitVector</c>).
    /// </summary>
    public BitVectorManipulatorBase() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("BitVector", "Bit vector to manipulate", typeof(BoolArrayData), VariableKind.In | VariableKind.Out));
    }

    /// <summary>
    /// Manipulates the bit vector.
    /// </summary>
    /// <param name="scope">The current scope whose bit vector to manipulate.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      BoolArrayData vector = GetVariableValue<BoolArrayData>("BitVector", scope, false);
      vector.Data = Manipulate(scope, random, vector.Data);
      return null;
    }

    /// <summary>
    /// Manipulates the given bit <paramref name="vector"/> with the given random number generator.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The bit vector to manipulate.</param>
    /// <returns>The manipulated bit vector.</returns>
    protected abstract bool[] Manipulate(IScope scope, IRandom random, bool[] vector);
  }
}
