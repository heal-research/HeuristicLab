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
  /// Intermediate multiple crossover: Creates a new offspring by computing the centroid of a list of
  /// parents.
  /// </summary>
  public class IntermediateMultiCrossover : RealVectorMultiCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"This creates a new offspring by computing the centroid of a number of parents";
      }
    }

    /// <summary>
    /// Performs an intermediate multiple crossover of the given list of <paramref name="parents"/>.
    /// </summary>
    /// <param name="parents">The list of parents of which to perform the crossover.</param>
    /// <returns>The newly created real vector, resulting from the intermediate multiple crossover.</returns>
    public static double[] Apply(IList<double[]> parents) {
      int length = parents[0].Length;
      double[] result = new double[length];
      for (int i = 0; i < length; i++) {
        double sum = 0.0;
        for (int j = 0; j < parents.Count; j++)
          sum += parents[j][i];
        result[i] = sum / parents.Count;
      }
      return result;
    }

    /// <summary>
    /// Performs an intermediate multiple crossover of the given list of <paramref name="parents"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">The list of parents of which to perform the crossover.</param>
    /// <returns>The newly created real vector, resulting from the intermediate multiple crossover.</returns>
    protected override double[] Cross(IScope scope, IRandom random, IList<double[]> parents) {
      return Apply(parents);
    }
  }
}
