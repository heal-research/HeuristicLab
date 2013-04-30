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
  /// The Schwefel function (sine root) is implemented as described in Affenzeller, M. and Wagner, S. 2005. Offspring Selection: A New Self-Adaptive Selection Scheme for Genetic Algorithms.  Ribeiro, B., Albrecht, R. F., Dobnikar, A., Pearson, D. W., and Steele, N. C. (eds.). Adaptive and Natural Computing Algorithms, pp. 218-221, Springer.
  /// </summary>
  [Item("SchwefelEvaluator", "Evaluates the Schwefel function (sine root) on a given point. In the given bounds [-500;500] the optimum of this function is close to 0 at (420.968746453712,420.968746453712,...,420.968746453712). It is implemented as described in Affenzeller, M. and Wagner, S. 2005. Offspring Selection: A New Self-Adaptive Selection Scheme for Genetic Algorithms.  Ribeiro, B., Albrecht, R. F., Dobnikar, A., Pearson, D. W., and Steele, N. C. (eds.). Adaptive and Natural Computing Algorithms, pp. 218-221, Springer.")]
  [StorableClass]
  public class SchwefelEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    /// <summary>
    /// Returns false as the Schwefel (sine root) function is a minimization problem.
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

    [StorableConstructor]
    protected SchwefelEvaluator(bool deserializing) : base(deserializing) { }
    protected SchwefelEvaluator(SchwefelEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SchwefelEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SchwefelEvaluator(this, cloner);
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      return null;
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
    public override double Evaluate(RealVector point) {
      return Apply(point);
    }
  }
}
