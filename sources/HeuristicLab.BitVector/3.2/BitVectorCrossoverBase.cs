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

namespace HeuristicLab.BitVector {
  /// <summary>
  /// Base class for all bit vector crossover operators.
  /// </summary>
  public abstract class BitVectorCrossoverBase : CrossoverBase {
    /// <summary>
    /// Initializes a new instance of <see cref="BitVectorCrossoverBase"/> with one variable info
    /// (<c>BitVector</c>).
    /// </summary>
    public BitVectorCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("BitVector", "Parent and child bit vector", typeof(BoolArrayData), VariableKind.In | VariableKind.New));
    }

    /// <summary>
    /// Performs a crossover by calling <see cref="Cross(HeuristicLab.Core.IScope, HeuristicLab.Core.IRandom, bool[][]"/>
    /// and adds the created bit vector to the current scope.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the parents have different lengths.</exception>
    /// <param name="scope">The current scope which represents a new child.</param>
    /// <param name="random">A random number generator.</param>
    protected sealed override void Cross(IScope scope, IRandom random) {
      bool[][] parents = new bool[scope.SubScopes.Count][];
      int length = -1;
      for (int i = 0; i < scope.SubScopes.Count; i++) {
        parents[i] = scope.SubScopes[i].GetVariableValue<BoolArrayData>("BitVector", false).Data;
        if (i == 0) length = parents[i].Length;
        else if (parents[i].Length != length) throw new InvalidOperationException("ERROR in BitVectorCrossoverBase: Cannot apply crossover to bit vectors of different length");
      }

      bool[] result = Cross(scope, random, parents);
      scope.AddVariable(new Variable(scope.TranslateName("BitVector"), new BoolArrayData(result)));
    }

    /// <summary>
    /// Performs a crossover of multiple bit vectors.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing all parent bit vectors.</param>
    /// <returns>The newly created bit vector, resulting from the crossover operation.</returns>
    protected abstract bool[] Cross(IScope scope, IRandom random, bool[][] parents);
  }
}
