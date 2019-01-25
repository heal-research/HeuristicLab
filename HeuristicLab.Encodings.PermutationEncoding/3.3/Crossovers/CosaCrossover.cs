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
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// Performs the crossover described in the COSA optimization method.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Wendt, O. 1994. COSA: COoperative Simulated Annealing - Integration von Genetischen Algorithmen und Simulated Annealing am Beispiel der Tourenplanung. Dissertation Thesis. IWI Frankfurt.<br />
  /// The operator actually performs a 2-opt mutation on the first parent, but it uses the second parent to determine which new edge should be inserted.
  /// Thus the mutation is not random as the second breakpoint depends on the information that is encoded in other members of the population.
  /// The idea is that the child should not sit right inbetween the two parents, but rather go a little bit from one parent in direction to the other.
  /// </remarks>
  [Item("CosaCrossover", "An operator which performs the crossover described in the COSA optimization method. It is implemented as described in Wendt, O. 1994. COSA: COoperative Simulated Annealing - Integration von Genetischen Algorithmen und Simulated Annealing am Beispiel der Tourenplanung. Dissertation Thesis. IWI Frankfurt.")]
  [StorableType("2C6B5F27-A921-4CDD-A484-35B8824CDDBE")]
  public class CosaCrossover : PermutationCrossover {
    [StorableConstructor]
    protected CosaCrossover(StorableConstructorFlag _) : base(_) { }
    protected CosaCrossover(CosaCrossover original, Cloner cloner) : base(original, cloner) { }
    public CosaCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CosaCrossover(this, cloner);
    }

    /// <summary>
    /// The operator actually performs a 2-opt mutation on the first parent, but it uses the second parent to determine which new edge should be inserted.
    /// Thus the mutation is not random as the second breakpoint depends on the information that is encoded in other members of the population.
    /// The idea is that the child should not sit right inbetween the two parents, but rather go a little bit from one parent in direction to the other.
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
      while ((i < parent2.Length) && (parent2[i] != parent1[crossPoint])) {  // find index of cross point in second permutation
        i++;
      }
      int newEdge = parent2[(i + 1) % length]; // the number that follows the cross point number in parent2 is the new edge that we want to insert
      endIndex = 0;
      while ((endIndex < parent1.Length) && (parent1[endIndex] != newEdge)) {  // find index of the new edge in the first permutation
        endIndex++;
      }

      if (startIndex <= endIndex) {
        // copy parent1 to child and reverse the order in between startIndex and endIndex
        for (i = 0; i < parent1.Length; i++) {
          if (i >= startIndex && i <= endIndex) {
            result[i] = parent1[endIndex - i + startIndex];
          } else {
            result[i] = parent1[i];
          }
        }
      } else { // startIndex > endIndex
        for (i = 0; i < parent1.Length; i++) {
          if (i >= startIndex || i <= endIndex) {
            result[i] = parent1[(endIndex - i + startIndex + length) % length]; // add length to wrap around when dropping below index 0
          } else {
            result[i] = parent1[i];
          }
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
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("CosaCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
