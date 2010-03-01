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

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Discrete crossover for real vectors: For each position in the new vector an allele
  /// of one of the parents is randomly selected.
  /// </summary>
  public class DiscreteCrossover : RealVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"Discrete crossover for real vectors: creates a new offspring by combining the alleles in the parents such that each allele is randomly selected from one parent."; }
    }

    /// <summary>
    /// Performs a discrete crossover operation on multiple given parents.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the parents that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    public static double[] Apply(IRandom random, double[][] parents) {
      int length = parents[0].Length;
      double[] result = new double[length];
      for (int i = 0; i < length; i++) {
        result[i] = parents[random.Next(parents.Length)][i];
      }
      return result;
    }

    /// <summary>
    /// Performs a discrete crossover operation for multiple given parent real vectors.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are less than two parents.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the real vectors that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[][] parents) {
      if (parents.Length < 2) throw new InvalidOperationException("ERROR in DiscreteCrossover: The number of parents is less than 2");
      return Apply(random, parents);
    }
  }
}
