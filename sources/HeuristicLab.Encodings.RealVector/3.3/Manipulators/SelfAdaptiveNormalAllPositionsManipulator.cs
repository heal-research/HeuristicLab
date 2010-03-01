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
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Manipulates each dimension in the real vector with the mutation strength given 
  /// in the strategy parameter vector.
  /// </summary>
  public class SelfAdaptiveNormalAllPositionsManipulator : RealVectorManipulatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"Manipulates each dimension in the real vector with the mutation strength given in the strategy parameter vector"; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SelfAdaptiveNormalAllPositionsManipulator"/> with one
    /// variable info (<c>StrategyVector</c>).
    /// </summary>
    public SelfAdaptiveNormalAllPositionsManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("StrategyVector", "The strategy vector determining the strength of the mutation", typeof(DoubleArrayData), VariableKind.In));
    }

    /// <summary>
    /// Performs a self adaptive normally distributed all position manipulation on the given 
    /// <paramref name="vector"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the strategy vector is not
    /// as long as the vector to get manipulated.</exception>
    /// <param name="strategyParameters">The strategy vector determining the strength of the mutation.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <returns>The manipulated real vector.</returns>
    public static double[] Apply(double[] strategyParameters, IRandom random, double[] vector) {
      NormalDistributedRandom N = new NormalDistributedRandom(random, 0.0, 1.0);
      for (int i = 0; i < vector.Length; i++) {
        vector[i] = vector[i] + (N.NextDouble() * strategyParameters[i % strategyParameters.Length]);
      }
      return vector;
    }

    /// <summary>
    /// Performs a self adaptive normally distributed all position manipulation on the given 
    /// <paramref name="vector"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <returns>The manipulated real vector.</returns>
    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double[] strategyVector = scope.GetVariableValue<DoubleArrayData>("StrategyVector", true).Data;
      return Apply(strategyVector, random, vector);
    }
  }
}
