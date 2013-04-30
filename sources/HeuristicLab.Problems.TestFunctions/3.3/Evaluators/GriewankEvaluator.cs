#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// The Griewank function is introduced in Griewank, A.O. 1981. Generalized descent for global optimization. Journal of Optimization Theory and Applications 34, pp. 11-39.
  /// It is a multimodal fitness function in the range [-600,600]^n.
  /// Here it is implemented as described (without the modifications) in Locatelli, M. 2003. A note on the Griewank test function. Journal of Global Optimization 25, pp. 169-174, Springer.
  /// </summary>
  [Item("GriewankEvaluator", "Evaluates the Griewank function on a given point. The optimum of this function is 0 at the origin. It is introduced by Griewank A.O. 1981 and implemented as described (without the modifications) in Locatelli, M. 2003. A note on the Griewank test function. Journal of Global Optimization 25, pp. 169-174, Springer.")]
  [StorableClass]
  public class GriewankEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    /// <summary>
    /// Returns false as the Griewank function is a minimization problem.
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
      get { return new DoubleMatrix(new double[,] { { -600, 600 } }); }
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
    protected GriewankEvaluator(bool deserializing) : base(deserializing) { }
    protected GriewankEvaluator(GriewankEvaluator original, Cloner cloner) : base(original, cloner) { }
    public GriewankEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GriewankEvaluator(this, cloner);
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      return new RealVector(dimension);
    }
    /// <summary>
    /// If dimension of the problem is less or equal than 100 the values of Math.Sqrt(i + 1) are precomputed.
    /// </summary>
    private double[] sqrts;

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Griewank function at the given point.</returns>
    public static double Apply(RealVector point) {
      double result = 0;
      double val = 0;

      for (int i = 0; i < point.Length; i++)
        result += point[i] * point[i];
      result = result / 4000;

      val = Math.Cos(point[0]);
      for (int i = 1; i < point.Length; i++)
        val *= Math.Cos(point[i] / Math.Sqrt(i + 1));

      result = result - val + 1;
      return result;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>. It uses an array of precomputed values for Math.Sqrt(i + 1) with i = 0..N
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <param name="sqrts">The precomputed array of square roots.</param>
    /// <returns>The result value of the Griewank function at the given point.</returns>
    private static double Apply(RealVector point, double[] sqrts) {
      double result = 0;
      double val = 0;

      for (int i = 0; i < point.Length; i++)
        result += point[i] * point[i];
      result = result / 4000;

      val = Math.Cos(point[0]);
      for (int i = 1; i < point.Length; i++)
        val *= Math.Cos(point[i] / sqrts[i]);

      result = result - val + 1;
      return result;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Griewank function at the given point.</returns>
    public override double Evaluate(RealVector point) {
      if (point.Length > 100)
        return Apply(point);
      else {
        if (sqrts == null || sqrts.Length < point.Length) ComputeSqrts(point.Length);
        return Apply(point, sqrts);
      }
    }

    private void ComputeSqrts(int length) {
      sqrts = new double[length];
      for (int i = 0; i < length; i++) sqrts[i] = Math.Sqrt(i + 1);
    }
  }
}
