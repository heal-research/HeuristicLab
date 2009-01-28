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
    /// Performs a crossover of two given parents.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <param name="child">The resulting child scope.</param>
    protected sealed override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      DoubleArrayData vector1 = parent1.GetVariableValue<DoubleArrayData>("RealVector", false);
      DoubleArrayData vector2 = parent2.GetVariableValue<DoubleArrayData>("RealVector", false);

      if (vector1.Data.Length != vector2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to real vectors of different length.");

      double[] result = Cross(scope, random, vector1.Data, vector2.Data);
      child.AddVariable(new Variable(child.TranslateName("RealVector"), new DoubleArrayData(result)));
    }

    /// <summary>
    /// Performs a crossover of two given parents.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected abstract double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2);
  }
}
