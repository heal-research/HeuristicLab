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

namespace HeuristicLab.TestFunctions {
  /// <summary>
  /// Zakharov Function<br/>
  /// Domain:  [-5.0 , 10.0]^n<br/>
  /// Optimum: 0.0 at (0.0, 0.0, ..., 0.0)
  /// </summary>
  public class ZakharovEvaluator : TestFunctionEvaluatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return
@"Zakharov Function

Domain:  [-5.0 , 10.0]^n
Optimum: 0.0 at (0.0, 0.0, ..., 0.0)";
          }
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Zakharov function at the given point.</returns>
    public static double Apply(double[] point) {
      int length = point.Length;
      double s1 = 0;
      double s2 = 0;

      for (int i = 0; i < length; i++) {
        s1 = s1 + point[i] * point[i];
        s2 = s2 + 0.5 * i * point[i];
      }
      return s1 + s2 * s2 + s2 * s2 * s2 * s2;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Zakharov function at the given point.</returns>
    protected override double EvaluateFunction(double[] point) {
      return Apply(point);
    }
  }
}
