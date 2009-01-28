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

namespace HeuristicLab.RealVector {
  /// <summary>
  /// Random convex crossover for real vectors: Takes for each element the allele of the first parent times a 
  /// once created randomly chosen factor and adds the allele of the second parent times 
  /// (1 - the randomly chosen factor).
  /// </summary>
  public class RandomConvexCrossover : RealVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Random convex crossover for real vectors."; }
    }

    /// <summary>
    /// Performs a random convex crossover on the two given parents.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent vector for the crossover.</param>
    /// <param name="parent2">The second parent vector for the crossover.</param>
    /// <returns>The newly created real vector, resulting from the random convex crossover.</returns>
    public static double[] Apply(IRandom random, double[] parent1, double[] parent2) {
      int length = parent1.Length;
      double[] result = new double[length];
      double factor = random.NextDouble();

      for (int i = 0; i < length; i++)
        result[i] = (factor * parent1[i]) + ((1 - factor) * parent2[i]);
      return result;
    }

    /// <summary>
    /// Performs a random convex crossover on the two given parents.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent vector for the crossover.</param>
    /// <param name="parent2">The second parent vector for the crossover.</param>
    /// <returns>The newly created real vector, resulting from the random convex crossover.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
