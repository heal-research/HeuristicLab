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

namespace HeuristicLab.RealVector {
  /// <summary>
  /// Discrete crossover for real vectors: Selects randomly either the value of the first or the 
  /// second parent.
  /// </summary>
  public class DiscreteCrossover : RealVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Discrete crossover for real vectors."; }
    }

    /// <summary>
    /// Performs a discrete crossover operation of the two given parents.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    public static double[] Apply(IRandom random, double[] parent1, double[] parent2) {
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < 0.5)
          result[i] = parent1[i];
        else
          result[i] = parent2[i];
      }
      return result;
    }

    /// <summary>
    /// Performs a discrete crossover operation of the two given parents.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
