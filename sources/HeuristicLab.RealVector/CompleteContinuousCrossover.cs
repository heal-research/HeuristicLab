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
  /// Complete continuous crossover for real vectors: for each element of the new vector the average
  /// of both parents is calculated.
  /// </summary>
  public class CompleteContinuousCrossover : RealVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Complete continuous crossover for real vectors."; }
    }

    /// <summary>
    /// Performs a complete continuous crossover of the two given real vectors.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover.</param>
    /// <param name="parent2">The second parent for the crossover.</param>
    /// <returns>The newly created real vector, resulting from the complete continuous crossover.</returns>
    public static double[] Apply(IRandom random, double[] parent1, double[] parent2) {
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++)
        result[i] = (parent1[i] + parent2[i]) / 2;
      return result;
    }

    /// <summary>
    /// Performs a complete continuous crossover of the two given real vectors.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover.</param>
    /// <param name="parent2">The second parent for the crossover.</param>
    /// <returns>The newly created real vector, resulting from the complete continuous crossover.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
