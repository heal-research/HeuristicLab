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
  /// Uniformly distributed change of a single position of an integer vector.
  /// </summary>
  public class UniformOnePositionManipulator : IntVectorManipulatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Uniformly distributed change of a single position of an integer vector."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="UniformOnePositionManipulator"/> with two variable infos
    /// (<c>Minimum</c> and <c>Maximum</c>).
    /// </summary>
    public UniformOnePositionManipulator() {
      AddVariableInfo(new VariableInfo("Minimum", "Minimum of the sampling range for the vector element (included)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum of the sampling range for the vector element (excluded)", typeof(IntData), VariableKind.In));
    }

    /// <summary>
    /// Changes randomly a single position in the given integer <paramref name="vector"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>
    /// <param name="min">The minimum value of the sampling range for 
    /// the vector element to change (inclusive).</param>
    /// <param name="max">The maximum value of the sampling range for
    /// the vector element to change (exclusive).</param>
    /// <returns>The new integer vector that has been manipulated.</returns>
    public static int[] Apply(IRandom random, int[] vector, int min, int max) {
      int[] result = (int[])vector.Clone();
      int index = random.Next(result.Length);
      result[index] = random.Next(min, max);
      return result;
    }

    /// <summary>
    /// Changes randomly a single position in the given integer <paramref name="vector"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The integer vector to manipulate.</param>
    /// <returns>The new integer vector that has been manipulated.</returns>
    protected override int[] Manipulate(IScope scope, IRandom random, int[] vector) {
      int min = GetVariableValue<IntData>("Minimum", scope, true).Data;
      int max = GetVariableValue<IntData>("Maximum", scope, true).Data;
      return Apply(random, vector, min, max);
    }
  }
}
