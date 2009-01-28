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
  /// Creates a new offspring by combining the alleles in the parents such that each allele is 
  /// randomly selected from one parent. It will also use the same strategy to combine the endogenous 
  /// strategy parameter vector. 
  /// </summary>
  public class SelfAdaptiveDiscreteMultiCrossover : RealVectorSelfAdaptiveMultiCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"This creates a new offspring by combining the alleles in the parents such that each allele is randomly selected from one parent. It will also use the same strategy to combine the endogenous strategy parameter vector.";
      }
    }

    /// <summary>
    /// Performs a self adaptive discrete multiple crossover on the given list of <paramref name="parents"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the parent vectors have different lengths.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The list of parents to crossover.</param>
    /// <param name="strategyParametersList">The strategy parameter list.</param>
    /// <param name="childIndividual">Output parameter; the created child.</param>
    /// <param name="strategyParameters">Output parameter; endogenous strategy parameters.</param>
    public static void Apply(IRandom random, IList<double[]> parents, IList<double[]> strategyParametersList, out double[] childIndividual, out double[] strategyParameters) {
      childIndividual = new double[parents[0].Length];
      strategyParameters = new double[strategyParametersList[0].Length];
      try {
        for (int i = 0; i < childIndividual.Length; i++) {
          int nextParent = random.Next(parents.Count);
          childIndividual[i] = parents[nextParent][i];
          strategyParameters[i] = strategyParametersList[nextParent][i];
        }
      } catch (IndexOutOfRangeException) {
        throw new InvalidOperationException("Cannot apply self adaptive multicrossover to real vectors of different length.");
      }
    }

    /// <summary>
    /// Performs a self adaptive discrete multiple crossover on the given list of <paramref name="parents"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The list of parents to crossover.</param>
    /// <param name="strategyParametersList">The strategy parameter list.</param>
    /// <param name="childIndividual">Output parameter; the created child.</param>
    /// <param name="strategyParameters">Output parameter; endogenous strategy parameters.</param>
    protected override void Cross(IScope scope, IRandom random, IList<double[]> parents, IList<double[]> strategyParametersList, out double[] childIndividual, out double[] strategyParameters) {
      Apply(random, parents, strategyParametersList, out childIndividual, out strategyParameters);
    }
  }
}
