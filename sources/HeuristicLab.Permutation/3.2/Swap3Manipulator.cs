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
  /// Manipulates a permutation array by swaping three randomly chosen elements.
  /// </summary>
  public class Swap3Manipulator : PermutationManipulatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Swaps three randomly chosen elements of the given <paramref name="permutation"/> array.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation array to manipulate.</param>
    /// <returns>The new permuation array with the manipulated data.</returns>
    public static int[] Apply(IRandom random, int[] permutation) {
      int[] result = (int[])permutation.Clone();
      int index1, index2, index3, temp;

      index1 = random.Next(result.Length);
      index2 = random.Next(result.Length);
      index3 = random.Next(result.Length);

      if (random.NextDouble() < 0.5) {
        // swap edges 1 and 2
        temp = result[index1];
        result[index1] = result[index2];
        result[index2] = temp;
        // swap edges 2 and 3
        temp = result[index2];
        result[index2] = result[index3];
        result[index3] = temp;
      } else {
        // swap edges 1 and 3
        temp = result[index1];
        result[index1] = result[index3];
        result[index3] = temp;
        // swap edges 2 and 3
        temp = result[index2];
        result[index2] = result[index3];
        result[index3] = temp;
      }
      return result;
    }

    /// <summary>
    /// Swaps three randomly chosen elements of the given <paramref name="permutation"/> array.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation array to manipulate.</param>
    /// <returns>The new permuation array with the manipulated data.</returns>
    protected override int[] Manipulate(IScope scope, IRandom random, int[] permutation) {
      return Apply(random, permutation);
    }
  }
}
