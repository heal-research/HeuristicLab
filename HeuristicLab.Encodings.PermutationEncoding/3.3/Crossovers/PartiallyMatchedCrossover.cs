#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// An operator which performs the partially matched crossover on two permutations.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Fogel, D.B. 1988. An Evolutionary Approach to the Traveling Salesman Problem. Biological Cybernetics, 60, pp. 139-144, Springer-Verlag.
  /// which references Goldberg, D.E., and Lingle, R. 1985. Alleles, loci, and the traveling salesman problem. Proceedings of an International Conference on Genetic Algorithms and their Applications. Carnegie-Mellon University, pp. 154-159.
  /// as the original source of the operator.
  /// </remarks>
  [Item("PartiallyMatchedCrossover", "An operator which performs the partially matched crossover on two permutations. It is implemented as described in Fogel, D.B. 1988. An Evolutionary Approach to the Traveling Salesman Problem. Biological Cybernetics, 60, pp. 139-144, Springer-Verlag.")]
  [StorableType("BE92E88C-0A05-4E56-884A-BAFFE6A86F4F")]
  public class PartiallyMatchedCrossover : PermutationCrossover {
    [StorableConstructor]
    protected PartiallyMatchedCrossover(StorableConstructorFlag _) : base(_) { }
    protected PartiallyMatchedCrossover(PartiallyMatchedCrossover original, Cloner cloner) : base(original, cloner) { }
    public PartiallyMatchedCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PartiallyMatchedCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the partially matched crossover on <paramref name="parent1"/> and <paramref name="parent2"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length or when the permutations are shorter than 4 elements.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the numbers in the permutation elements are not in the range [0,N) with N = length of the permutation.</exception>
    /// <remarks>
    /// Initially the new offspring is a clone of <paramref name="parent2"/>.
    /// Then a segment is extracted from <paramref name="parent1"/> and copied to the offspring position by position.
    /// Whenever a position is copied, the number at that position currently in the offspring is transfered to the position where the copied number has been.
    /// E.g.: Position 15 is selected to be copied from <paramref name="parent1"/> to <paramref name="parent2"/>. At position 15 there is a '3' in <paramref name="parent1"/> and a '5' in the new offspring.
    /// The '5' in the offspring is then moved to replace the '3' in the offspring and '3' is written at position 15.
    /// </remarks>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent permutation to cross.</param>
    /// <param name="parent2">The second parent permutation to cross.</param>
    /// <returns>The new permutation resulting from the crossover.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("PartiallyMatchedCrossover: The parent permutations are of unequal length.");
      if (parent1.Length < 4) throw new ArgumentException("PartiallyMatchedCrossover: The parent permutation must be at least of size 4.");
      int length = parent1.Length;
      int[] result = new int[length];
      int[] invResult = new int[length];

      int breakPoint1, breakPoint2;
      do {
        breakPoint1 = random.Next(length - 1);
        breakPoint2 = random.Next(breakPoint1 + 1, length);
      } while (breakPoint2 - breakPoint1 >= length - 2); // prevent the case [0,length-1) -> clone of parent1

      // clone parent2 and calculate inverse permutation (number -> index)
      try {
        for (int j = 0; j < length; j++) {
          result[j] = parent2[j];
          invResult[result[j]] = j;
        }
      }
      catch (IndexOutOfRangeException) {
        throw new InvalidOperationException("PartiallyMatchedCrossover: The permutation must consist of consecutive numbers from 0 to N-1 with N = length of the permutation.");
      }

      for (int j = breakPoint1; j <= breakPoint2; j++) {
        int orig = result[j]; // save the former value
        result[j] = parent1[j]; // overwrite the former value with the new value
        int index = invResult[result[j]]; // look where the new value is in the offspring
        result[index] = orig; // write the former value to this position
        invResult[orig] = index; // update the inverse mapping
        // it's not necessary to do 'invResult[result[j]] = j' as this will not be needed again
      }

      return new Permutation(parent1.PermutationType, result);
    }

    /// <summary>
    /// Checks number of parents and calls <see cref="Apply(IRandom, Permutation, Permutation)"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two permutations in <paramref name="parents"/>.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("PartiallyMatchedCrossover: The number of parents is not equal to 2.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
