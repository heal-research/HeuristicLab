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
  /// Uniformly distributed change of all positions of a real vector.
  /// </summary>
  public class UniformAllPositionsManipulator : RealVectorManipulatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Uniformly distributed change of all positions of a real vector."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="UniformAllPositionsManipulator"/> with two variable infos
    /// (<c>Minimum</c> and <c>Maximum</c>).
    /// </summary>
    public UniformAllPositionsManipulator() {
      AddVariableInfo(new VariableInfo("Minimum", "Minimum of the sampling range for the vector element (included)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum of the sampling range for the vector element (excluded)", typeof(DoubleData), VariableKind.In));
    }

    /// <summary>
    /// Changes all positions in the given real <paramref name="vector"/>.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <param name="min">The minimum value of the sampling range for each vector element (inclusive).</param>
    /// <param name="max">The maximum value of the sampling range for each vector element (exclusive).</param>
    /// <returns>The new real vector which has been manipulated.</returns>
    public static double[] Apply(IRandom random, double[] vector, double min, double max) {
      double[] result = (double[])vector.Clone();

      for (int i = 0; i < result.Length; i++) {
        result[i] = min + random.NextDouble() * (max - min);
      }

      return result;
    }

    /// <summary>
    /// Changes all positions in the given real <paramref name="vector"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <returns>The new real vector which has been manipulated.</returns>
    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double min = GetVariableValue<DoubleData>("Minimum", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Maximum", scope, true).Data;
      return Apply(random, vector, min, max);
    }
  }
}
