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
  /// Local crossover for real vectors: Takes for each element the allele of the first parent times a 
  /// always newly created randomly chosen factor and adds the allele of the second parent times (1 - the randomly chosen factor).
  /// </summary>
  public class LocalCrossover : RealVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Local crossover for real vectors."; }
    }

    /// <summary>
    /// Performs a local crossover on the two given parent vectors.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <returns>The newly created real vector, resulting from the local crossover.</returns>
    public static double[] Apply(IRandom random, double[] parent1, double[] parent2) {
      double factor;
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        factor = random.NextDouble();
        result[i] = (factor * parent1[i]) + ((1 - factor) * parent2[i]);
      }
      return result;
    }

    /// <summary>
    /// Performs a local crossover operation for two given parent real vectors.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two real vectors that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[][] parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in LocalCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
