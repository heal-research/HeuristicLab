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

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Discrete crossover for real vectors: creates a new offspring by combining the alleles in the parents such that each allele is 
  /// randomly selected from one parent. It will also use the same strategy to combine the endogenous 
  /// strategy parameter vector.
  /// </summary>
  public class SelfAdaptiveDiscreteCrossover : RealVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"Discrete crossover for real vectors: creates a new offspring by combining the alleles in the parents such that each allele is randomly selected from one parent. It will also use the same strategy to combine the endogenous strategy parameter vector.";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SelfAdaptiveDiscreteCrossover"/> with one additional
    /// variable infos (<c>StrategyVector</c>).
    /// </summary>
    public SelfAdaptiveDiscreteCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("StrategyVector", "Endogenous strategy parameter vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
    }

    /// <summary>
    /// Performs a discrete crossover operation for multiple given parent real vectors that combines the strategy vectors in the same way.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the parents that should be crossed.</param>
    /// <param name="strategies">An array containing the strategy vectors of the parents.</param>
    /// <param name="child">The newly created real vector, resulting from the crossover operation (output parameter).</param>
    /// <param name="strategy">The newly created strategy vector, resulting from the crossover operation (output parameter).</param>
    public static void Apply(IRandom random, double[][] parents, double[][] strategies, out double[] child, out double[] strategy) {
      child = new double[parents[0].Length];
      strategy = new double[strategies[0].Length];
      for (int i = 0; i < child.Length; i++) {
        int nextParent = random.Next(parents.Length);
        child[i] = parents[nextParent][i];
        strategy[i] = strategies[nextParent][i];
      }
    }

    /// <summary>
    /// Performs a discrete crossover operation for multiple given parent real vectors that combines the strategy vectors in the same way.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are less than two parents or if the length of the strategy vectors is not the same as the length of the real vectors.</exception>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the real vectors that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override double[] Cross(IScope scope, IRandom random, double[][] parents) {
      if (parents.Length < 2) throw new InvalidOperationException("ERROR in SelfAdaptiveDiscreteCrossover: The number of parents is less than 2");
      double[][] strategies = new double[scope.SubScopes.Count][];
      int length = parents[0].Length;
      for (int i = 0; i < scope.SubScopes.Count; i++) {
        strategies[i] = scope.SubScopes[i].GetVariableValue<DoubleArrayData>("StrategyVector", false).Data;
        if (strategies[i].Length != length) throw new InvalidOperationException("ERROR in SelfAdaptiveDiscreteCrossover: Strategy vectors do not have the same length as the real vectors");
      }

      double[] child, strategy;
      Apply(random, parents, strategies, out child, out strategy);
      scope.AddVariable(new Variable(scope.TranslateName("StrategyVector"), new DoubleArrayData(strategy)));
      return child;
    }
  }
}
