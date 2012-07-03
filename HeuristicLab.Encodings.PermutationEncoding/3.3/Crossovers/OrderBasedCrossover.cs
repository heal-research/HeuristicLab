#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// Performs a cross over permutation of two permutation arrays by taking randomly a selection of values 
  /// (not an interval!) from the first permutation keeping the correct order and filling 
  /// the missing entries with the elements from the second permutation, also in the right order.
  /// </summary>
  /// <remarks>
  /// This is in some papers also called Order Crossover #2.<br />
  /// It is implemented as described in Syswerda, G. (1991). Schedule Optimization Using Genetic Algorithms. In Davis, L. (Ed.) Handbook of Genetic Algorithms, Van Nostrand Reinhold, New York, pp. 332-349.
  /// </remarks>
  [Item("OrderBasedCrossover", "An operator which performs an order based crossover of two permutations. It is implemented as described in Syswerda, G. (1991). Schedule Optimization Using Genetic Algorithms. In Davis, L. (Ed.) Handbook of Genetic Algorithms, Van Nostrand Reinhold, New York, pp. 332-349.")]
  [StorableClass]
  public class OrderBasedCrossover : PermutationCrossover {
    [StorableConstructor]
    protected OrderBasedCrossover(bool deserializing) : base(deserializing) { }
    protected OrderBasedCrossover(OrderBasedCrossover original, Cloner cloner) : base(original, cloner) { }
    public OrderBasedCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrderBasedCrossover(this, cloner);
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/> by
    /// randomly selecting some values from the first permutation that will be inserted one after each 
    /// other; the missing ones are picked in the correct order from the second permutation.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent permutation to cross.</param>
    /// <param name="parent2">The second parent permutation to cross.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("OrderBasedCrossover: The parent permutations are of unequal length.");
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
      return new Permutation(parent1.PermutationType, result);
    }

    /// <summary>
    /// Checks number of parents and calls <see cref="Apply(IRandom, Permutation, Permutation)"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("OrderCrossover: Number of parents is not equal to 2.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
