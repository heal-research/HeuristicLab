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
  /// by taking a randomly chosen interval from the frist, preserving the positions,
  /// then the missing values from the second array in the order they occur in the second array.
  /// </summary>
  public class OrderCrossover : PermutationCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and 
    /// <paramref name="parent2"/> by taking a randomly chosen interval from <paramref name="parent1"/>,
    /// preserving the positions and then the missing values from <paramref name="parent2"/> in the 
    /// order they occur in <paramref name="parent2"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
      int[] result = new int[parent1.Length];
      bool[] copied = new bool[result.Length];

      int breakPoint1 = random.Next(result.Length - 1);
      int breakPoint2 = random.Next(breakPoint1 + 1, result.Length);

      for (int i = breakPoint1; i <= breakPoint2; i++) {  // copy part of first permutation
        result[i] = parent1[i];
        copied[parent1[i]] = true;
      }

      int index = 0;
      for (int i = 0; i < parent2.Length; i++) {  // copy remaining part of second permutation
        if (index == breakPoint1) {  // skip already copied part
          index = breakPoint2 + 1;
        }
        if (!copied[parent2[i]]) {
          result[index] = parent2[i];
          index++;
        }
      }
      return result;
    }

    /// <summary>
    /// Performs an order crossover operation for two given parent permutations.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override int[] Cross(IScope scope, IRandom random, int[][] parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in OrderCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
