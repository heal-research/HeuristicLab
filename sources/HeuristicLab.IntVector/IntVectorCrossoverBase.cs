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

namespace HeuristicLab.IntVector {
  /// <summary>
  /// Base class for all integer vector crossover operators.
  /// </summary>
  public abstract class IntVectorCrossoverBase : CrossoverBase {
    /// <summary>
    /// Initializes a new instance of <see cref="IntVectorCrossoverBase"/> with one variable info
    /// (<c>IntVector</c>).
    /// </summary>
    public IntVectorCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("IntVector", "Parent and child integer vector", typeof(IntArrayData), VariableKind.In | VariableKind.New));
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
      IVariableInfo intVectorInfo = GetVariableInfo("IntVector");
      IntArrayData vector1 = parent1.GetVariableValue<IntArrayData>(intVectorInfo.FormalName, false);
      IntArrayData vector2 = parent2.GetVariableValue<IntArrayData>(intVectorInfo.FormalName, false);

      if (vector1.Data.Length != vector2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to integer vectors of different length.");

      int[] result = Cross(scope, random, vector1.Data, vector2.Data);
      child.AddVariable(new Variable(child.TranslateName(intVectorInfo.FormalName), new IntArrayData(result)));
    }

    /// <summary>
    /// Performs a crossover of two given parents.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created integer vector, resulting from the crossover operation.</returns>
    protected abstract int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2);
  }
}
