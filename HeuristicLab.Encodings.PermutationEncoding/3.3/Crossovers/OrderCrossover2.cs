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
  /// An operator which performs a slight variant of the order crossover on two permutations.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press. p. 135.<br />
  /// Crosses two permutations by copying a randomly chosen interval from the first permutation, preserving
  /// the positions. But then, instead of starting from the end of the copied interval, the missing values are copied from the start of the permutation in the order they occur.
  /// </remarks>
  [Item("OrderCrossover2", "An operator which performs an order crossover of two permutations. It is implemented as described in Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press. p. 135.")]
  [StorableType("9E9A828E-E853-43FD-A39B-F47D17BCE539")]
  public class OrderCrossover2 : PermutationCrossover {
    [StorableConstructor]
    protected OrderCrossover2(StorableConstructorFlag _) : base(_) { }
    protected OrderCrossover2(OrderCrossover2 original, Cloner cloner) : base(original, cloner) { }
    public OrderCrossover2() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrderCrossover2(this, cloner);
    }

    /// <summary>
    /// Performs a slight variation of the order crossover of two permutations.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <remarks>
    /// Crosses two permutations by copying a randomly chosen interval from the first permutation, preserving
    /// the positions. Then, from the beginning of the permutation, copies the missing values from the second permutation
    /// in the order they occur.
    /// </remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent permutation to cross.</param>
    /// <param name="parent2">The second parent permutation to cross.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("OrderCrossover2: The parent permutations are of unequal length.");
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
      if (parents.Length != 2) throw new InvalidOperationException("OrderCrossover2: Number of parents is not equal to 2.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
