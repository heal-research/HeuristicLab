#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// The Zakharov function is implemented as described in Hedar, A. & Fukushima, M. 2004. Heuristic pattern search and its hybridization with simulated annealing for nonlinear global optimization. Optimization Methods and Software 19, pp. 291-308, Taylor & Francis.
  /// </summary>
  [Item("ZakharovEvaluator", "Evaluates the Zakharov function on a given point. The optimum of this function is 0 at the origin. It is implemented as described in Hedar, A. & Fukushima, M. 2004. Heuristic pattern search and its hybridization with simulated annealing for nonlinear global optimization. Optimization Methods and Software 19, pp. 291-308, Taylor & Francis.")]
  [StorableClass]
  public class ZakharovEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    /// <summary>
    /// Returns false as the Zakharov function is a minimization problem.
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
      get { return new DoubleMatrix(new double[,] { { -5, 10 } }); }
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

    public override RealVector GetBestKnownSolution(int dimension) {
      return new RealVector(dimension);
    }

    [StorableConstructor]
    protected ZakharovEvaluator(bool deserializing) : base(deserializing) { }
    protected ZakharovEvaluator(ZakharovEvaluator original, Cloner cloner) : base(original, cloner) { }
    public ZakharovEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZakharovEvaluator(this, cloner);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Zakharov function at the given point.</returns>
    public static double Apply(RealVector point) {
      int length = point.Length;
      double s1 = 0;
      double s2 = 0;

      for (int i = 0; i < length; i++) {
        s1 += point[i] * point[i];
        s2 += 0.5 * i * point[i];
      }
      return s1 + (s2 * s2) + (s2 * s2 * s2 * s2);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Zakharov function at the given point.</returns>
    public override double Evaluate(RealVector point) {
      return Apply(point);
    }
  }
}
