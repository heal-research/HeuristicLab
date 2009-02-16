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
  /// of all parents is calculated.
  /// </summary>
  public class CompleteContinuousCrossover : RealVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Complete continuous crossover for real vectors."; }
    }

    /// <summary>
    /// Performs a complete continuous crossover of multiple given real vectors.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">An array containing the parents that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the complete continuous crossover.</returns>
    public static double[] Apply(IRandom random, double[][] parents) {
      int length = parents[0].Length;
      double[] result = new double[length];
      for (int i = 0; i < length; i++) {
        double sum = 0.0;
        for (int j = 0; j < parents.Length; j++)
          sum += parents[j][i];
        result[i] = sum / parents.Length;
      }
      return result;
    }

    /// <summary>
    /// Performs a complete continuous crossover operation for multiple given parent real vectors.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are less than two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the real vectors that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[][] parents) {
      if (parents.Length < 2) throw new InvalidOperationException("ERROR in CompleteContinuousCrossover: The number of parents is less than 2");
      return Apply(random, parents);
    }
  }
}
