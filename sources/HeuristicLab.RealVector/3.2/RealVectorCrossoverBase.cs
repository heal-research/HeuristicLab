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
using HeuristicLab.Evolutionary;

namespace HeuristicLab.RealVector {
  /// <summary>
  /// Base class for all real vector crossover operators.
  /// </summary>
  public abstract class RealVectorCrossoverBase : CrossoverBase {
    /// <summary>
    /// Initializes a new instance of <see cref="RealVectorCrossoverBase"/> with one variable info
    /// (<c>RealVector</c>).
    /// </summary>
    public RealVectorCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
    }

    /// <summary>
    /// Performs a crossover by calling <see cref="Cross(HeuristicLab.Core.IScope, HeuristicLab.Core.IRandom, double[][]"/>
    /// and adds the created real vector to the current scope.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the parents have different lengths.</exception>
    /// <param name="scope">The current scope which represents a new child.</param>
    /// <param name="random">A random number generator.</param>
    protected sealed override void Cross(IScope scope, IRandom random) {
      double[][] parents = new double[scope.SubScopes.Count][];
      int length = -1;
      for (int i = 0; i < scope.SubScopes.Count; i++) {
        parents[i] = scope.SubScopes[i].GetVariableValue<DoubleArrayData>("RealVector", false).Data;
        if (i == 0) length = parents[i].Length;
        else if (parents[i].Length != length) throw new InvalidOperationException("ERROR in RealVectorCrossoverBase: Cannot apply crossover to real vectors of different length");
      }

      double[] result = Cross(scope, random, parents);
      scope.AddVariable(new Variable(scope.TranslateName("RealVector"), new DoubleArrayData(result)));
    }

    /// <summary>
    /// Performs a crossover of multiple real vectors.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing all parent real vectors.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected abstract double[] Cross(IScope scope, IRandom random, double[][] parents);
  }
}
