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
  /// Performs a cross over permutation between two int arrays by taking first a whole cycle and then the
  /// missing ones from the second parent.
  /// </summary>
  /// <remarks>A whole cycle means: <br/>
  /// Start at a randomly chosen position x in parent1 and transfer it to the child at the same position.
  /// Now this position x is no longer available for the node on position x in parent2, so
  /// the value of the node at position x in parent2 is searched in parent1 and is then transferred
  /// to the child preserving the position. Now this new position y is no longer available for the node in parent2 ....<br/>
  /// This procedure is repeated till it is again at position x, then the cycle is over.
  /// </remarks>
  public class CyclicCrossover : PermutationCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by copying a whole cycle starting at a randomly chosen position in parent1 and taking the rest
    /// from parent2.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
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
      return result;
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// by copying a whole cycle starting at a randomly chosen position in parent1 and taking the rest
    /// from parent2.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    protected override int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
