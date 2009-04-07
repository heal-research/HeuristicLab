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

namespace HeuristicLab.BitVector {
  /// <summary>
  /// Single bit flip manipulation for bit vectors.
  /// </summary>
  public class FlipManipulator : BitVectorManipulatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Single bit flip manipulation for bit vectors."; }
    }

    /// <summary>
    /// Changes randomly a single position in the given bit <paramref name="vector"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The bit vector to manipulate.</param>
    /// <returns>The new bit vector that has been manipulated.</returns>
    public static bool[] Apply(IRandom random, bool[] vector) {
      bool[] result = (bool[])vector.Clone();
      int index = random.Next(result.Length);
      result[index] = !result[index];
      return result;
    }

    /// <summary>
    /// Changes randomly a single position in the given bit <paramref name="vector"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The bit vector to manipulate.</param>
    /// <returns>The new bit vector that has been manipulated.</returns>
    protected override bool[] Manipulate(IScope scope, IRandom random, bool[] vector) {
      return Apply(random, vector);
    }
  }
}
