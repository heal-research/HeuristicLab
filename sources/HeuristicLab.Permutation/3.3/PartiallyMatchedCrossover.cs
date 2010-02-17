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
  /// An operator which performs the partially matched crossover on two permutations.
  /// </summar>
  /// <remarks>
  /// Implemented as described in Fogel, D.B. 1988. An Evolutionary Approach to the Traveling Salesman Problem. Biological Cybernetics, 60, pp. 139-144, Springer-Verlag.
  /// which references Goldberg, D.E., and Lingle, R. 1985. Alleles, loci, and the traveling salesman problem. Proceedings of an International Conference on Genetic Algorithms and their Applications. Carnegie-Mellon University, pp. 154-159.
  /// as the original source of the operator.
  /// </remarks>
  [Item("PartiallyMatchedCrossover", "An operator which performs the partially matched crossover on two permutations.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class PartiallyMatchedCrossover : PermutationCrossover {
    /// <summary>
    /// Performs the partially matched crossover on <paramref name="parent1"/> and <paramref name="parent2"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length or when the permutations are shorter than 4 elements.</exception>
    /// <remarks>
    /// First a segment from the first parent is copied to the offspring.
    /// Then the rest of the offspring is filled with the numbers from parent2.
    /// When a number is encountered in parent2 that is included in the segment which came from the first parent,
    /// the number in parent2 is used that was replaced by the corresponding number from parent1.
    /// </remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent permutation to cross.</param>
    /// <param name="parent2">The second parent permutation to cross.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("PartiallyMatchedCrossover: The parent permutations are of unequal length");
      if (parent1.Length < 4) throw new ArgumentException("PartiallyMatchedCrossover: The parent permutation must be at least of size 4");
      int length = parent1.Length;
      Permutation result = new Permutation(length); ;
      int[] invResult = new int[length];

      int breakPoint1, breakPoint2;
      do {
        breakPoint1 = random.Next(length - 1);
        breakPoint2 = random.Next(breakPoint1 + 1, length);
      } while (breakPoint2 - breakPoint1 >= length - 2); // prevent the case [0,length-1) -> clone of parent1

      // clone parent2 and calculate inverse permutation (number -> index)
      for (int j = 0; j < length; j++) {
        result[j] = parent2[j];
        invResult[result[j]] = j;
      }

      for (int j = breakPoint1; j <= breakPoint2; j++) {
        int orig = result[j]; // save the former value
        result[j] = parent1[j]; // overwrite the former value with the new value
        int index = invResult[result[j]]; // look where the new value is in the child
        result[index] = orig; // write the former value to this position
        invResult[orig] = index; // update the inverse mapping
        // it's not necessary to do 'invResult[result[j]] = j' as this will not be needed again
      }

      return result;
    }

    /// <summary>
    /// Checks number of parents and calls <see cref="Apply"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two permutations in <paramref name="parents"/>.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("PartiallyMatchedCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
