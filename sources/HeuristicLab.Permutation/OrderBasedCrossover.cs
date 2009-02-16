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
  /// Performs a cross over permutation of two permutation arrays by taking randomly a selection of values 
  /// (not an interval!) from the first permutation keeping the correct order and filling 
  /// the missing entries with the elements from the second permutation, also in the right order.
  /// </summary>
  public class OrderBasedCrossover : PermutationCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/> by
    /// randomly selecting some values from the first permutation that will be inserted one after each 
    /// other; the missing ones are picked in the correct order from the second permutation.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
      int length = parent1.Length;
      int[] result = new int[length];
      int[] selectedNumbers = new int[random.Next(length + 1)];
      bool[] numberSelected = new bool[length];
      int index, selectedIndex, currentIndex;

      for (int i = 0; i < selectedNumbers.Length; i++) {  // select numbers for array
        index = 0;
        while (numberSelected[parent1[index]]) {  // find first valid index
          index++;
        }

        selectedIndex = random.Next(length - i);
        currentIndex = 0;
        while ((index < parent1.Length) && (currentIndex != selectedIndex)) {  // find selected number
          index++;
          if (!numberSelected[parent1[index]]) {
            currentIndex++;
          }
        }
        numberSelected[parent1[index]] = true;
      }

      index = 0;
      for (int i = 0; i < parent1.Length; i++) {  // copy selected numbers in array
        if (numberSelected[parent1[i]]) {
          selectedNumbers[index] = parent1[i];
          index++;
        }
      }

      index = 0;
      for (int i = 0; i < result.Length; i++) {  // copy rest of second permutation and selected numbers in order of first permutation
        if (numberSelected[parent2[i]]) {
          result[i] = selectedNumbers[index];
          index++;
        } else {
          result[i] = parent2[i];
        }
      }
      return result;
    }

    /// <summary>
    /// Performs an order-based crossover operation for two given parent permutations.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override int[] Cross(IScope scope, IRandom random, int[][] parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in OrderBasedCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
