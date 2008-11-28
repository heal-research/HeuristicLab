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
  /// Manipulates a permutation array by moving randomly one element to another position in the array.
  /// </summary>
  public class InsertionManipulator : PermutationManipulatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Moves an randomly chosen element in the specified <paramref name="permutation"/> array 
    /// to another randomly generated position.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The array to manipulate.</param>
    /// <returns>The new permuation array with the manipulated data.</returns>
    public static int[] Apply(IRandom random, int[] permutation) {
      int[] result = (int[])permutation.Clone();
      int cutIndex, insertIndex, number;

      cutIndex = random.Next(permutation.Length);
      insertIndex = random.Next(permutation.Length);
      number = permutation[cutIndex];

      int i = 0;  // index in new permutation
      int j = 0;  // index in old permutation
      while (i < permutation.Length) {
        if (j == cutIndex) {
          j++;
        }
        if (i == insertIndex) {
          result[i] = number;
          i++;
        }
        if ((i < permutation.Length) && (j < permutation.Length)) {
          result[i] = permutation[j];
          i++;
          j++;
        }
      }
      return result;
    }

    /// <summary>
    /// Moves an randomly chosen element in the specified <paramref name="permutation"/> array 
    /// to another randomly generated position.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The array to manipulate.</param>
    /// <returns>The new permuation array with the manipulated data.</returns>
    protected override int[] Manipulate(IScope scope, IRandom random, int[] permutation) {
      return Apply(random, permutation);
    }
  }
}
