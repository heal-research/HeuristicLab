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
  /// Performs a cross over permutation between two permuation arrays by preserving a preferably big
  /// region from one permutation array.
  /// </summary>
  public class MaximalPreservativeCrossover : PermutationCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by preserving a preferably big randomly chosen region of one permutation and taking 
    /// the missing ones from the other permuation array.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The permutation array of parent 1.</param>
    /// <param name="parent2">The permutation array of parent 2.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] numberCopied = new bool[length];
      int breakPoint1, breakPoint2, subsegmentLength, index;

      if (length >= 20) {  // length of subsegment must be >= 10 and <= length / 2
        breakPoint1 = random.Next(length - 9);
        subsegmentLength = random.Next(10, Math.Min((int)(length / 2), length - breakPoint1) + 1);
        breakPoint2 = breakPoint1 + subsegmentLength - 1;
      } else {
        breakPoint1 = random.Next(length - 1);
        breakPoint2 = random.Next(breakPoint1 + 1, length);
      }

      index = 0;
      for (int i = breakPoint1; i <= breakPoint2; i++) {  // copy part of first permutation
        result[index] = parent1[i];
        numberCopied[result[index]] = true;
        index++;
      }

      for (int i = 0; i < parent2.Length; i++) {  // copy remaining part of second permutation
        if (!numberCopied[parent2[i]]) {
          result[index] = parent2[i];
          index++;
        }
      }
      return result;
    }

    /// <summary>
    /// Performs a maximal preservative crossover operation for two given parent permutations.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override int[] Cross(IScope scope, IRandom random, int[][] parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in MaximalPreservativeCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
