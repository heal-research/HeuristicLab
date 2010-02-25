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
  /// Performs a crossover operation between two permutation arrays by taking randomly chosen 
  /// reverse and forward intervals from the first permutation array inserting 
  /// it in the child on different positions depending on the second permutation array.
  /// </summary>
  /// <remarks>It is implemented as described in Wendt, O. 1994. COSA: COoperative Simulated Annealing - Integration von Genetischen Algorithmen und Simulated Annealing am Beispiel der Tourenplanung. Dissertation Thesis.<br />
  /// </remarks>
  [Item("CosaCrossover", "An operator which performs a crossover operation between two permutation arrays by taking randomly chosen reverse and forward intervals from the first permutation array inserting it in the child on different positions depending on the second permutation array.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class CosaCrossover : PermutationCrossover {
    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by taking first the reverse elements of a randomly chosen interval of parent1 
    /// and inserting it in the result at a position specified by the permutation of parent2. 
    /// The remaining elements to be inserted are taken again from parent1 in the forward direction.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("CosaCrossover: The parent permutations are of unequal length.");
      int length = parent1.Length;
      int[] result = new int[length];
      int crossPoint, startIndex, endIndex;

      crossPoint = random.Next(length);
      startIndex = (crossPoint + 1) % length;

      int i = 0;
      while ((i < parent2.Length) && (parent2[i] != parent1[startIndex])) {  // find index of start value in second permutation
        i++;
      }
      i = (i + 1) % length;
      int j = 0;
      while ((j < parent1.Length) && (parent1[j] != parent2[i])) {  // find index of parent2[i] in first permutation
        j++;
      }
      endIndex = (j - 1 + length) % length;

      i = endIndex;
      j = 0;
      do {  // permutation from end to crosspoint (backwards)
        result[j] = parent1[i];
        i = (i - 1 + length) % length;
        j++;
      } while (i != crossPoint);

      i = (endIndex + 1) % length;
      while (i != crossPoint) {  // permutation from end to crosspoint (forwards)
        result[j] = parent1[i];
        i = (i + 1) % length;
        j++;
      }
      result[j] = parent1[crossPoint];  // last station: crosspoint
      return new Permutation(result);
    }

    /// <summary>
    /// Performs a COSA crossover operation for two given parent permutations.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in CosaCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
