#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Permutation {
  /// <summary>
  /// Performs a cross over permutation between two permutation arrays by taking
  /// the entries with the same index (starting at position 0) from both parents 
  /// (minding already inserted values).
  /// </summary>
  /// <example>First take the value at position 0 from parent1 then take the value at position 0
  /// from parent2 if it has not already been inserted, afterwards take the value at position 1 from
  /// parent1 if it has not already been inserted, then from parent2 and so on.</example>
  public class AbsolutePositionTopologicalCrossover : PermutationCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by taking the values from both parents one by one with the same index starting at position 0.
    /// </summary>
    /// <example>First take the value at position 0 from parent1 then take the value at position 0
    /// from parent2 if it has not already been inserted, afterwards take the value at position 1 from
    /// parent1 if it has not already been inserted, then from parent2 and so on.</example>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
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
      return result;
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by taking the values from both parents one by one with the same index.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    protected override int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
