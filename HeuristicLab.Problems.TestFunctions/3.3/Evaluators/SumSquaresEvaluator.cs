#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// The Sum Squares function is defined as sum(i * x_i * x_i) for i = 1..n
  /// </summary>
  [Item("SumSquaresEvaluator", "Evaluates the sum squares function on a given point. The optimum of this function is 0 at the origin. The Sum Squares function is defined as sum(i * x_i * x_i) for i = 1..n.")]
  [StorableClass]
  public class SumSquaresEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    public override string FunctionName { get { return "SumSquares"; } }
    /// <summary>
    /// Returns false as the Sum Squares function is a minimization problem.
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
      get { return new DoubleMatrix(new double[,] { { -10, 10 } }); }
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

    [StorableConstructor]
    protected SumSquaresEvaluator(bool deserializing) : base(deserializing) { }
    protected SumSquaresEvaluator(SumSquaresEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SumSquaresEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SumSquaresEvaluator(this, cloner);
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      return new RealVector(dimension);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Sum Squares function at the given point.</returns>
    public static double Apply(RealVector point) {
      double result = 0;
      for (int i = 0; i < point.Length; i++) {
        result += (i + 1) * point[i] * point[i];
      }
      return result;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Sum Squares function at the given point.</returns>
    public override double Evaluate(RealVector point) {
      return Apply(point);
    }
  }
}
