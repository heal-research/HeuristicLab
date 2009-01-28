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
  /// Base class for all real vector multiple crossover operators.
  /// </summary>
  public abstract class RealVectorMultiCrossoverBase : MultiCrossoverBase {
    /// <summary>
    /// Initializes a new instance of <see cref="RealVectorMultiCrossoverBase"/> with one variable info
    /// (<c>RealVector</c>).
    /// </summary>
    public RealVectorMultiCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
    }

    /// <summary>
    /// Performs a crossover of a number of given parents.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">The parents for crossover.</param>
    /// <param name="child">The resulting child scope.</param>
    protected sealed override void Cross(IScope scope, IRandom random, IScope[] parents, IScope child) {
      IList<double[]> parentsList = new List<double[]>(parents.Length);

      for (int i = 0; i < parents.Length; i++)
        parentsList.Add(parents[i].GetVariableValue<DoubleArrayData>("RealVector", false).Data);

      double[] result = Cross(scope, random, parentsList);
      child.AddVariable(new Variable(child.TranslateName("RealVector"), new DoubleArrayData(result)));
    }

    /// <summary>
    /// Performs a crossover of a number of given parents.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">The parents for crossover.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected abstract double[] Cross(IScope scope, IRandom random, IList<double[]> parents);
  }
}
