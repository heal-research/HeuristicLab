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
  /// Performs the absolute position topological crossover on two permutation arrays by taking
  /// the entries with the same index (starting at position 0) from both parents 
  /// (minding already inserted values).
  /// </summary>
  /// <remarks>It is implemented as described in Moraglio, A. and Poli, R. 2005. Topological crossover for the permutation representation. In Proceedings of the 2005 Workshops on Genetic and Evolutionary Computation. pp. 332-338.<br />
  /// </remarks>
  /// <example>First take the value at position 0 from parent1 then take the value at position 0
  /// from parent2 if it has not already been inserted, afterwards take the value at position 1 from
  /// parent1 if it has not already been inserted, then from parent2 and so on.</example>
  [Item("AbsolutePositionTopologicalCrossover", "An operator which performs a cross over permutation between two permutation arrays by taking the entries with the same index (starting at position 0) from both parents (minding already inserted values). It is implemented as described in Moraglio, A. and Poli, R. 2005. Topological crossover for the permutation representation. In Proceedings of the 2005 Workshops on Genetic and Evolutionary Computation. pp. 332-338.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public class AbsolutePositionTopologicalCrossover : PermutationCrossover {
    /// <summary>
    /// Performs the absolute position topological crossover on <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by taking the values from both parents one by one with the same index starting at position 0.
    /// </summary>
    /// <example>First take the value at position 0 from parent1 then take the value at position 0
    /// from parent2 if it has not already been inserted, afterwards take the value at position 1 from
    /// parent1 if it has not already been inserted, then from parent2 and so on.</example>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("AbsolutePositionTopologicalCrossover: The parent permutations are of unequal length.");
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] numberCopied = new bool[length];
      int index;

      index = 0;
      for (int i = 0; i < length; i++) {  // copy numbers from both parent permutations
        if (!numberCopied[parent1[i]]) {
          result[index] = parent1[i];
          numberCopied[parent1[i]] = true;
          index++;
        }
        if (!numberCopied[parent2[i]]) {
          result[index] = parent2[i];
          numberCopied[parent2[i]] = true;
          index++;
        }
      }
      return new Permutation(result);
    }

    /// <summary>
    /// Checks number of parents and calls <see cref="Apply(IRandom, Permutation, Permutation)"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in AbsolutePositionTopologicalCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
