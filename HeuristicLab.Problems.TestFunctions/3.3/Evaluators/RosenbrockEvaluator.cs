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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HEAL.Fossil;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// The Rosenbrock function features a flat valley in which the global optimum is located.
  /// It is implemented as generalized Rosenbrock function as for example given in Shang, Y.-W. and Qiu, Y.-H. 2006. A Note on the Extended Rosenbrock Function. Evolutionary Computation 14, pp. 119-126, MIT Press.
  /// </summary>
  [Item("RosenbrockEvaluator", @"The Rosenbrock function features a flat valley in which the global optimum is located.
For 2 and 3 dimensions the single minimum of this function is 0 at (1,1,...,1), for 4 to 30 dimensions there is an additional local minimum close to (-1,1,...,1).
It is unknown how many local minima there are for dimensions greater than 30.
It is implemented as generalized Rosenbrock function for which the 2 dimensional function is a special case, as for example given in Shang, Y.-W. and Qiu, Y.-H. 2006. A Note on the Extended Rosenbrock Function. Evolutionary Computation 14, pp. 119-126, MIT Press.")]
  [StorableType("20C31FDB-4F7D-4563-B955-F4F8B34DFF3E")]
  public class RosenbrockEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    public override string FunctionName { get { return "Rosenbrock"; } }
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
      get { return new DoubleMatrix(new double[,] { { -2.048, 2.048 } }); }
    }
    /// <summary>
    /// Gets the minimum problem size (2).
    /// </summary>
    public override int MinimumProblemSize {
      get { return 2; }
    }
    /// <summary>
    /// Gets the (theoretical) maximum problem size (2^31 - 1).
    /// </summary>
    public override int MaximumProblemSize {
      get { return int.MaxValue; }
    }

    [StorableConstructor]
    protected RosenbrockEvaluator(StorableConstructorFlag _) : base(_) { }
    protected RosenbrockEvaluator(RosenbrockEvaluator original, Cloner cloner) : base(original, cloner) { }
    public RosenbrockEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RosenbrockEvaluator(this, cloner);
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      if (dimension < 2) throw new ArgumentException(Name + ": This function is not defined for 1 dimension.");
      RealVector result = new RealVector(dimension);
      for (int i = 0; i < dimension; i++) result[i] = 1;
      return result;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Rosenbrock function at the given point.</returns>
    public static double Apply(RealVector point) {
      double result = 0;
      for (int i = 0; i < point.Length - 1; i++) {
        result += 100 * (point[i] * point[i] - point[i + 1]) * (point[i] * point[i] - point[i + 1]);
        result += (point[i] - 1) * (point[i] - 1);
      }
      return result;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Rosenbrock function at the given point.</returns>
    public override double Evaluate(RealVector point) {
      return Apply(point);
    }
  }
}
