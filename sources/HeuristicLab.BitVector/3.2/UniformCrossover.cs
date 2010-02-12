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

namespace HeuristicLab.BitVector {
  /// <summary>
  /// Uniform crossover for bit vectors.
  /// </summary>
  public class UniformCrossover : BitVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Uniform crossover for bit vectors."; }
    }

    /// <summary>
    /// Performs a uniform crossover randomly choosing the value of the first or second parent for each slot
    /// given parent bit vectors.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created bit vector, resulting from the uniform crossover.</returns>
    public static bool[] Apply(IRandom random, bool[] parent1, bool[] parent2) {
      int length = parent1.Length;
      bool[] result = new bool[length];

      // for each slot
      for (int i = 0; i < length; i++)
        // take value from first or second parent with equal probability
        result[i] = random.Next(2) == 0 ? parent1[i] : parent2[i];

      return result;
    }

    /// <summary>
    /// Performs a uniform crossover randomly choosing the value of the first or second parent for each slot
    /// given parent bit vectors.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two bit vectors that should be crossed.</param>
    /// <returns>The newly created bit vector, resulting from the uniform crossover.</returns>
    protected override bool[] Cross(IScope scope, IRandom random, bool[][] parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in UniformCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
