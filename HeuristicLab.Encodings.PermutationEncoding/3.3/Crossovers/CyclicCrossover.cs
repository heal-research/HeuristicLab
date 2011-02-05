#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// An operator which performs the cyclic crossover on two permutations.
  /// </summary>
  /// <remarks>It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.<br />
  /// The operator first determines all cycles in the permutation and then composes the offspring by alternating between the cycles of the two parents.
  /// </remarks>
  [Item("CyclicCrossover", "An operator which performs the cyclic crossover on two permutations. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class CyclicCrossover : PermutationCrossover {
    [StorableConstructor]
    protected CyclicCrossover(bool deserializing) : base(deserializing) { }
    protected CyclicCrossover(CyclicCrossover original, Cloner cloner) : base(original, cloner) { }
    public CyclicCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CyclicCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the cyclic crossover on <paramref name="parent1"/> and <paramref name="parent2"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the numbers in the permutation elements are not in the range [0;N) with N = length of the permutation.</exception>
    /// <remarks>
    /// First this method randomly determines from which parent to start with equal probability.
    /// Then it copies the first cycle from the chosen parent starting from index 0 in the permutation.
    /// After the cycle is complete it copies the next cycle starting from the index closest to 0 which was not yet copied from the other parent.
    /// It continues to switch between parents for each completed cycle until no new cycle is found anymore.<br /><br />
    /// The stochasticity of this operator is rather low. There are only two possible outcomes for a given pair of parents.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown if the two parents are not of equal length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("CyclicCrossover: The parent permutations are of unequal length.");
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] indexCopied = new bool[length];
      int[] invParent1 = new int[length];
      int[] invParent2 = new int[length];

      // calculate inverse mappings (number -> index) for parent1 and parent2
      try {
        for (int i = 0; i < length; i++) {
          invParent1[parent1[i]] = i;
          invParent2[parent2[i]] = i;
        }
      }
      catch (IndexOutOfRangeException) {
        throw new InvalidOperationException("CyclicCrossover: The permutation must consist of numbers in the interval [0;N) with N = length of the permutation.");
      }

      // randomly choose whether to start copying from parent1 or parent2
      bool copyFromParent1 = ((random.NextDouble() < 0.5) ? (false) : (true));

      int j = 0;
      do {
        do {
          if (copyFromParent1) {
            result[j] = parent1[j]; // copy number at position j from parent1
            indexCopied[j] = true;
            j = invParent2[result[j]]; // set position j to the position of the copied number in parent2
          } else {
            result[j] = parent2[j]; // copy number at position j from parent2
            indexCopied[j] = true;
            j = invParent1[result[j]]; // set position j to the position of the copied number in parent1
          }
        } while (!indexCopied[j]);
        copyFromParent1 = !copyFromParent1;
        j = 0;
        while (j < length && indexCopied[j]) j++;
      } while (j < length);

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
      if (parents.Length != 2) throw new InvalidOperationException("CyclicCrossover: The number of parents is not equal to 2.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
