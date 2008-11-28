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

namespace HeuristicLab.Permutation {
  /// <summary>
  /// Performs a cross over permutation between two permuation arrays
  /// by taking a randomly chosen interval from the first, keeping the position, 
  /// then all positions from the second permutation which are still free in the child 
  /// (the position is free and the value is "free")
  /// and then missing ones from the second array in the order they occur in the second array.
  /// </summary>
  public class PartiallyMatchedCrossover : PermutationCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Performs a cross over permuation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by taking a randomly chosen interval from <paramref name="parent1"/>, preserving the position, 
    /// then all positions from <paramref name="parent2"/> which are still free in the child 
    /// (the position is free and the value is "free")
    /// and then missing ones from <paramref name="parent2"/> in the order they occur 
    /// in <paramref name="parent2"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] numbersCopied = new bool[length];

      int breakPoint1 = random.Next(length - 1);
      int breakPoint2 = random.Next(breakPoint1 + 1, length);

      for (int j = breakPoint1; j <= breakPoint2; j++) {  // copy part of first permutation
        result[j] = parent1[j];
        numbersCopied[result[j]] = true;
      }

      int index = breakPoint1;
      int i = (breakPoint1 == 0 ? (breakPoint2 + 1) : 0);
      while (i < length) {  // copy rest from second permutation
        if (!numbersCopied[parent2[i]]) {  // copy directly
          result[i] = parent2[i];
        } else {  // copy from area between breakpoints
          while (numbersCopied[parent2[index]]) {  // find next valid index
            index++;
          }
          result[i] = parent2[index];
          index++;
        }

        i++;
        if (i == breakPoint1) {  // skip area between breakpoints
          i = breakPoint2 + 1;
        }
      }
      return result;
    }

    /// <summary>
    /// Performs a cross over permuation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by taking a randomly chosen interval from <paramref name="parent1"/>, preserving the position, 
    /// then all positions from <paramref name="parent2"/> which are still free in the child 
    /// (the position is free and the value is "free")
    /// and then missing ones from <paramref name="parent2"/> in the order they occur 
    /// in <paramref name="parent2"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    protected override int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
