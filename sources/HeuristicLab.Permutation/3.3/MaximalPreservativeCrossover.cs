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
  /// <summary>An operator which performs the maximal preservative crossover on two permutations.</summary>
  /// <remarks>
  /// Performs a crossover between two permuation arrays by preserving a large number of edges in both parents.
  /// The operator also maintains the position in the arrays to some extent.
  /// It is implemented as described in Mühlenbein, H. Evolution in time and space - the parallel genetic algorithm. FOUNDATIONS OF GENETIC ALGORITHMS, Morgan Kaufmann, 1991, 316-337.
  /// The length of the segment copied from the first parent to the offspring is uniformly distributed in the interval [3, N/3) with N = length of the permutation.
  /// </remarks>
  [Item("MaximalPreservativeCrossover", "An operator which performs the maximal preservative crossover on two permutations.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class MaximalPreservativeCrossover : PermutationCrossover {
    /// <summary>
    /// Performs the maximal preservative crossover on <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by preserving a large number of edges in both parents.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the numbers in the permutation array are not in the range [0,N) with N = length of the permutation.</exception>
    /// <remarks>
    /// First one segment is copied from the first parent to the offspring in the same position.
    /// Then the tour is completed by adding the next number from the second parent if such an edge exists,
    /// or from the first parent, or from the next number of the second parent.
    /// The last case results in an unwanted mutation.
    /// </remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent permutation to cross.</param>
    /// <param name="parent2">The second parent permutation to cross.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("MaximalPreservativeCrossover: The parent permutations are of unequal length");
      if (parent1.Length < 4) throw new ArgumentException("MaximalPreservativeCrossover: The parent permutation must be at least of size 4");
      int length = parent1.Length;
      Permutation result = new Permutation(length);
      bool[] numberCopied = new bool[length];
      int breakPoint1, breakPoint2, subsegmentLength, index;

      subsegmentLength = random.Next(3, Math.Max(length / 3, 4)); // as mentioned in "Pohlheim, H. Evolutionäre Algorithmen: Verfahren, Operatoren und Hinweise für die Praxis, 1999, p.44, Springer.
      breakPoint1 = random.Next(length);
      breakPoint2 = breakPoint1 + subsegmentLength;
      if (breakPoint2 >= length) breakPoint2 -= length;

      // copy string between position [breakPoint1, breakPoint2) from parent1 to the offspring
      index = breakPoint1;
      do {
        result[index] = parent1[index];
        numberCopied[result[index]] = true;
        index++;
        if (index >= length) index -= length;
      } while (index != breakPoint2);

      // calculate inverse permutation (number -> index) to help finding the follower of a given number
      int[] invParent1 = new int[length];
      int[] invParent2 = new int[length];
      try {
        for (int i = 0; i < length; i++) {
          invParent1[parent1[i]] = i;
          invParent2[parent2[i]] = i;
        }
      } catch (IndexOutOfRangeException) {
        throw new InvalidOperationException("MaximalPreservativeCrossover: The permutation must consist of consecutive numbers from 0 to N-1 with N = length of the permutation");
      }

      int prevIndex = ((index > 0) ? (index - 1) : (length - 1));
      do {
        // look for the follower of the last number in parent2
        int p2Follower = GetFollower(parent2, invParent2[result[prevIndex]]);
        if (!numberCopied[p2Follower]) {
          result[index] = p2Follower;
        } else {
          // if that follower has already been added, look for the follower of the last number in parent1
          int p1Follower = GetFollower(parent1, invParent1[result[prevIndex]]);
          if (!numberCopied[p1Follower]) {
            result[index] = p1Follower;
          } else {
            // if that has also been added, look for the next not already added number in parent2
            int tempIndex = index;
            for (int i = 0; i < parent2.Length; i++) {
              if (!numberCopied[parent2[tempIndex]]) {
                result[index] = parent2[tempIndex];
                break;
              }
              tempIndex++;
              if (tempIndex >= parent2.Length) tempIndex = 0;
            }
          }
        }
        numberCopied[result[index]] = true;
        prevIndex = index;
        index++;
        if (index >= length) index -= length;
      } while (index != breakPoint1);

      return result;
    }

    private static int GetFollower(Permutation parent, int index) {
      if (index + 1 == parent.Length)
        return parent[0];
      return parent[index + 1];
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
      if (parents.Length != 2) throw new InvalidOperationException("MaximalPreservativeCrossover: Number of parents is not equal to 2.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
