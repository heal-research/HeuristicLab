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
using HeuristicLab.Operators;

namespace HeuristicLab.TestFunctions {
  /// <summary>
  /// Rosenbrock Function<br/>
  /// Domain:  [-2.048 , 2.048]^n<br/>
  /// Optimum: 0.0 at (1, 1, ..., 1)
  /// </summary>
  public class RosenbrockEvaluator : TestFunctionEvaluatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return
@"Rosenbrock Function

Domain:  [-2.048 , 2.048]^n
Optimum: 0.0 at (1, 1, ..., 1)";
          }
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Rosenbrock function at the given point.</returns>
    public static double Apply(double[] point) {
      double result = 0;
      for (int i = 0; i < point.Length - 1; i++) {
        result += 100 * (point[i + 1] - point[i] * point[i]) * (point[i + 1] - point[i] * point[i]);
        result += (1 - point[i]) * (1 - point[i]);
      }
      return result;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Rosenbrock function at the given point.</returns>
    protected override double EvaluateFunction(double[] point) {
      return Apply(point);
    }
  }
}
