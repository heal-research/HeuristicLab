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

namespace HeuristicLab.IntVector {
  /// <summary>
  /// Single point crossover for integer vectors.
  /// </summary>
  public class SinglePointCrossover : IntVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Single point crossover for integer vectors."; }
    }

    /// <summary>
    /// Performs a single point crossover at a randomly chosen position of the two 
    /// given parent integer vectors.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created integer vector, resulting from the single point crossover.</returns>
    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
      int length = parent1.Length;
      int[] result = new int[length];
      int breakPoint = random.Next(1, length);

      for (int i = 0; i < breakPoint; i++)
        result[i] = parent1[i];
      for (int i = breakPoint; i < length; i++)
        result[i] = parent2[i];

      return result;
    }

    /// <summary>
    /// Performs a single point crossover at a randomly chosen position of two 
    /// given parent integer vectors.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two integer vectors that should be crossed.</param>
    /// <returns>The newly created integer vector, resulting from the single point crossover.</returns>
    protected override int[] Cross(IScope scope, IRandom random, int[][] parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in SinglePointCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
