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
  /// Checks if all elements of a real vector are inside a given minimum and maximum value. 
  /// If not, the elements are corrected.
  /// </summary>
  public class BoundsChecker : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Checks if all elements of a real vector are inside a given minimum and maximum value. If not, elements are corrected."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BoundsChecker"/> with three variable infos
    /// (<c>RealVector</c>, <c>Minimum</c> and <c>Maximum</c>).
    /// </summary>
    public BoundsChecker()
      : base() {
      AddVariableInfo(new VariableInfo("RealVector", "Real vector to check", typeof(DoubleArrayData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Minimum", "Minimum value of each vector element (included).", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum value of each vector element (included).", typeof(DoubleData), VariableKind.In));
    }

    /// <summary>
    /// Checks if all elements of the given <paramref name="vector"/> are inside the given minimum 
    /// and maximum value and if not they are corrected.
    /// </summary>
    /// <param name="min">The minimum value of the range (inclusive).</param>
    /// <param name="max">The maximum value of the range (inclusive).</param>
    /// <param name="vector">The vector to check.</param>
    /// <returns>The corrected real vector.</returns>
    public static double[] Apply(double min, double max, double[] vector) {
      int length = vector.Length;
      double[] result = (double[])vector.Clone();

      for (int i = 0; i < length; i++) {
        if (result[i] < min) result[i] = min;
        if (result[i] > max) result[i] = max;
      }
      return result;
    }

    /// <summary>
    /// Checks if all elements of the given <paramref name="vector"/> are inside the given minimum 
    /// and maximum value and if not they are corrected.
    /// </summary>
    /// <remarks>Calls <see cref="Apply(double, double, double[])"/>.</remarks>
    /// <param name="scope">The current scope.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      DoubleArrayData vector = GetVariableValue<DoubleArrayData>("RealVector", scope, false);
      double min = GetVariableValue<DoubleData>("Minimum", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Maximum", scope, true).Data;
      vector.Data = Apply(min, max, vector.Data);
      return null;
    }
  }
}
