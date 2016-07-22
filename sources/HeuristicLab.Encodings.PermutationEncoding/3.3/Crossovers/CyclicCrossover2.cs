#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// An operator which performs a variant of the cyclic crossover on two permutations.
  /// </summary>
  /// <remarks>It is implemented as described in Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press. p. 136.<br />
  /// The operator first selects a random position in the permutation and from that position copies the first cycle to the offspring.
  /// Then all empty positions are filled from the second parent.
  /// </remarks>
  [Item("CyclicCrossover2", "An operator which performs the cyclic crossover on two permutations. It is implemented as described in Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press. p. 136.")]
  [StorableClass]
  public class CyclicCrossover2 : PermutationCrossover {
    [StorableConstructor]
    protected CyclicCrossover2(bool deserializing) : base(deserializing) { }
    protected CyclicCrossover2(CyclicCrossover2 original, Cloner cloner) : base(original, cloner) { }
    public CyclicCrossover2() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CyclicCrossover2(this, cloner);
    }

    /// <summary>
    /// Performs a variant of the cyclic crossover on <paramref name="parent1"/> and <paramref name="parent2"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the numbers in the permutation elements are not in the range [0;N) with N = length of the permutation.</exception>
    /// <remarks>
    /// Start at a randomly chosen position x in parent1 and transfer it to the child at the same position.
    /// Now this position x is no longer available for the node on position x in parent2, so
    /// the value of the node at position x in parent2 is searched in parent1 and is then transferred
    /// to the child preserving the position. Now this new position y is no longer available for the node in parent2 and so on.<br/>
    /// This procedure is repeated until it is again at position x, then the cycle is over.<br/>
    /// All further positions are copied from the second permutation.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown if the two parents are not of equal length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("CyclicCrossover2: The parent permutations are of unequal length.");
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] indexCopied = new bool[length];
      int j, number;

      j = random.Next(length);  // get number to start cycle
      while (!indexCopied[j]) {  // copy whole cycle to new permutation
        result[j] = parent1[j];
        number = parent2[j];
        indexCopied[j] = true;

        j = 0;
        while ((j < length) && (parent1[j] != number)) {  // search copied number in second permutation
          j++;
        }
      }

      for (int i = 0; i < length; i++) {  // copy rest of secound permutation to new permutation
        if (!indexCopied[i]) {
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
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("CyclicCrossover2: The number of parents is not equal to 2.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
