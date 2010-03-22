#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// Schwefel Function (Sine Root)<br/>
  /// Domain:  [-500.0 , 500.0]^n<br/>
  /// Optimum: 0.0 at (420.968746453712, 420.968746453712, ..., 420.968746453712)
  /// </summary>
  [Item("SchwefelEvaluator", "Evaluates the Schwefel function on a given point. The optimum of this function is 0 at (420.968746453712,420.968746453712,...,420.968746453712).")]
  [StorableClass]
  public class SchwefelEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    /// <summary>
    /// Returns false as the Rosenbrock function is a minimization problem.
    /// </summary>
    public override bool Maximization {
      get { return false; }
    }
    /// <summary>
    /// Gets the optimum function value (0).
    /// </summary>
    public override double BestKnownQuality {
      get { return 0; }
    }
    /// <summary>
    /// Gets the lower and upper bound of the function.
    /// </summary>
    public override DoubleMatrix Bounds {
      get { return new DoubleMatrix(new double[,] { { -500, 500 } }); }
    }
    /// <summary>
    /// Gets the minimum problem size (1).
    /// </summary>
    public override int MinimumProblemSize {
      get { return 1; }
    }
    /// <summary>
    /// Gets the (theoretical) maximum problem size (2^31 - 1).
    /// </summary>
    public override int MaximumProblemSize {
      get { return int.MaxValue; }
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Schwefel function at the given point.</returns>
    public static double Apply(RealVector point) {
      double result = 418.982887272433 * point.Length;
      for (int i = 0; i < point.Length; i++)
        result -= point[i] * Math.Sin(Math.Sqrt(Math.Abs(point[i])));
      return (result);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Schwefel function at the given point.</returns>
    protected override double EvaluateFunction(RealVector point) {
      return Apply(point);
    }
  }
}
