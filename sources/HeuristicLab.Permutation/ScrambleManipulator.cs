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
  /// Manipulates a permutation array by randomly scrambling the elements in a randomly chosen interval.
  /// </summary>
  public class ScrambleManipulator : PermutationManipulatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Mixes the elements of the given <paramref name="permutation"/> randomly 
    /// in a randomly chosen interval.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="permutation">The permutation array to manipulate.</param>
    /// <returns>The new permuation array with the manipulated data.</returns>
    public static int[] Apply(IRandom random, int[] permutation) {
      int[] result = (int[])permutation.Clone();
      int breakPoint1, breakPoint2;
      int[] scrambledIndices, remainingIndices, temp;
      int selectedIndex, index;

      breakPoint1 = random.Next(permutation.Length - 1);
      breakPoint2 = random.Next(breakPoint1 + 1, permutation.Length);

      scrambledIndices = new int[breakPoint2 - breakPoint1 + 1];
      remainingIndices = new int[breakPoint2 - breakPoint1 + 1];
      for (int i = 0; i < remainingIndices.Length; i++) {  // initialise indices
        remainingIndices[i] = i;
      }
      for (int i = 0; i < scrambledIndices.Length; i++) {  // generate permutation of indices
        selectedIndex = random.Next(remainingIndices.Length);
        scrambledIndices[i] = remainingIndices[selectedIndex];

        temp = remainingIndices;
        remainingIndices = new int[temp.Length - 1];
        index = 0;
        for (int j = 0; j < remainingIndices.Length; j++) {
          if (index == selectedIndex) {
            index++;
          }
          remainingIndices[j] = temp[index];
          index++;
        }
      }

      for (int i = 0; i <= (breakPoint2 - breakPoint1); i++) {  // scramble permutation between breakpoints
        result[breakPoint1 + i] = permutation[breakPoint1 + scrambledIndices[i]];
      }
      return result;
    }

    /// <summary>
    /// Mixes the elements of the given <paramref name="permutation"/> randomly 
    /// in a randomly chosen interval.
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
