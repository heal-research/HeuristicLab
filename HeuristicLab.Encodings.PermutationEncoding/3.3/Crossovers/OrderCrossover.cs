#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// An operator which performs the order crossover on two permutations.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.<br />
  /// Crosses two permutations by copying a randomly chosen interval from the first permutation, preserving
  /// the positions. Then, starting from the end of the copied interval, copies the missing values from the second permutation
  /// in the order they occur.
  /// </remarks>
  [Item("OrderCrossover", "An operator which performs an order crossover of two permutations. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.")]
  [StorableType("DEF33012-C7C5-4C09-8FCD-1145353D06A2")]
  public class OrderCrossover : PermutationCrossover {
    [StorableConstructor]
    protected OrderCrossover(StorableConstructorFlag _) : base(_) { }
    protected OrderCrossover(OrderCrossover original, Cloner cloner) : base(original, cloner) { }
    public OrderCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrderCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the order crossover of two permutations.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the numbers in the permutation elements are not in the range [0,N) with N = length of the permutation.</exception>
    /// <remarks>
    /// Crosses two permutations by copying a randomly chosen interval from the first permutation, preserving
    /// the positions. Then, starting from the end of the copied interval, copies the missing values from the second permutation
    /// in the order they occur.
    /// </remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent permutation to cross.</param>
    /// <param name="parent2">The second parent permutation to cross.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("OrderCrossover: The parent permutations are of unequal length.");
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] copied = new bool[length];

      int breakPoint1 = random.Next(length - 1);
      int breakPoint2 = random.Next(breakPoint1 + 1, length);

      try {
        for (int j = breakPoint1; j <= breakPoint2; j++) {  // copy part of first permutation
          result[j] = parent1[j];
          copied[parent1[j]] = true;
        }

        int index = ((breakPoint2 + 1 >= length) ? (0) : (breakPoint2 + 1));
        int i = index; // for moving in parent2
        while (index != breakPoint1) {
          if (!copied[parent2[i]]) {
            result[index] = parent2[i];
            index++;
            if (index >= length) index = 0;
          }
          i++;
          if (i >= length) i = 0;
        }
      }
      catch (IndexOutOfRangeException) {
        throw new InvalidOperationException("OrderCrossover: The permutation must consist of numbers in the interval [0;N) with N = length of the permutation.");
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
