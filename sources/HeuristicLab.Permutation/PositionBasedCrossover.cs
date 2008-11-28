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
  /// Performs a cross over permutation between two permutation arrays based on randomly chosen positions.
  /// </summary>
  public class PositionBasedCrossover : PermutationCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// based on randomly chosen positions to define which position to take from where.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The permutation array of parent 1.</param>
    /// <param name="parent2">The permutation array of parent 2.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
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
      return result;
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="parent2"/>
    /// based on randomly chosen positions to define which position to take from where.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The permutation array of parent 1.</param>
    /// <param name="parent2">The permutation array of parent 2.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    protected override int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
