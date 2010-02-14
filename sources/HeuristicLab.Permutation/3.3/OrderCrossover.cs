#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Permutation {
  /// <summary>
  /// An operator which performs an order crossover of two permutations.
  /// </summary>
  /// <remarks>
  /// Crosses two permutations by taking a randomly chosen interval from the frist permutation, preserving
  /// the positions, and then the missing values from the second permutation in the order they occur in the
  /// second permutation.
  /// </remarks>
  [Item("OrderCrossover", "An operator which performs an order crossover of two permutations.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class OrderCrossover : PermutationCrossover {
    /// <summary>
    /// Performs an order crossover of two permutations.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent permutation to cross.</param>
    /// <param name="parent2">The second parent permutation to cross.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      Permutation result = new Permutation(parent1.Length);
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
    /// Performs an order crossover of two permutations.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("Number of parents is not equal to 2.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
