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
  /// Performs a cross over permutation between two permutation arrays based on randomly chosen positions.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Syswerda, G. (1991). Schedule Optimization Using Genetic Algorithms. In Davis, L. (Ed.) Handbook of Genetic Algorithms, Van Nostrand Reinhold, New York, pp 332-349.
  /// </remarks>
  [Item("PositionBasedCrossover", "An operator which performs the position based crossover on two permutations. It is implemented as described in Syswerda, G. (1991). Schedule Optimization Using Genetic Algorithms. In Davis, L. (Ed.) Handbook of Genetic Algorithms, Van Nostrand Reinhold, New York, pp 332-349.")]
  [StorableType("9C707A94-E432-4A1C-94F4-FA69ED639289")]
  public class PositionBasedCrossover : PermutationCrossover {
    [StorableConstructor]
    protected PositionBasedCrossover(StorableConstructorFlag _) : base(_) { }
    protected PositionBasedCrossover(PositionBasedCrossover original, Cloner cloner) : base(original, cloner) { }
    public PositionBasedCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PositionBasedCrossover(this, cloner);
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// based on randomly chosen positions to define which position to take from where.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">First parent</param>
    /// <param name="parent2">Second Parent</param>
    /// <returns>Child</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("PositionBasedCrossover: The parent permutations are of unequal length.");
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] randomPosition = new bool[length];
      bool[] numberCopied = new bool[length];
      int randomPosNumber = random.Next(length);

      for (int i = 0; i < randomPosNumber; i++) {  // generate random bit mask
        randomPosition[random.Next(length)] = true;
      }

      for (int i = 0; i < length; i++) {  // copy numbers masked as true from second permutation
        if (randomPosition[i]) {
          result[i] = parent2[i];
          numberCopied[parent2[i]] = true;
        }
      }

      int index = 0;
      for (int i = 0; i < length; i++) {  // copy numbers masked as false from first permutation
        if (!numberCopied[parent1[i]]) {
          if (randomPosition[index]) {
            while (randomPosition[index]) {
              index++;
            }
          }
          result[index] = parent1[i];
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
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in PositionBasedCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
