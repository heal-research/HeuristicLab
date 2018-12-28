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
  /// The Beale function is defined for 2 dimensions with an optimum of 0 at (3, 0.5).
  /// It is implemented as described in Moré, J.J., Garbow, B., and Hillstrom, K. 1981. Testing unconstrained optimization software. ACM Transactions on Mathematical Software 7, pp. 136-140, ACM.
  /// </summary>
  [Item("BealeEvaluator", "Evaluates the Beale function on a given point. The optimum of this function is 0 at (3,0.5). It is implemented as described in Moré, J.J., Garbow, B., and Hillstrom, K. 1981. Testing unconstrained optimization software. ACM Transactions on Mathematical Software 7, pp. 136-140, ACM.")]
  [StorableType("EC1E155C-65ED-4603-A442-357ECC1E8F3D")]
  public class BealeEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    public override string FunctionName { get { return "Beale"; } }
    /// <summary>
    /// Returns false as the Beale function is a minimization problem.
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
      get { return new DoubleMatrix(new double[,] { { -4.5, 4.5 } }); }
    }
    /// <summary>
    /// Gets the minimum problem size (2).
    /// </summary>
    public override int MinimumProblemSize {
      get { return 2; }
    }
    /// <summary>
    /// Gets the maximum problem size (2).
    /// </summary>
    public override int MaximumProblemSize {
      get { return 2; }
    }

    [StorableConstructor]
    protected BealeEvaluator(StorableConstructorFlag _) : base(_) { }
    protected BealeEvaluator(BealeEvaluator original, Cloner cloner) : base(original, cloner) { }
    public BealeEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BealeEvaluator(this, cloner);
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      if (dimension != 2) throw new ArgumentException(Name + ": This function is only defined for 2 dimensions.", "dimension");
      return new RealVector(new double[] { 3, 0.5 });
    }
    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Beale function at the given point.</returns>
    public static double Apply(RealVector point) {
      double x1 = point[0], x2 = point[1];
      double f1 = 1.5 - x1 * (1 - x2);
      double f2 = 2.25 - x1 * (1 - x2 * x2);
      double f3 = 2.625 - x1 * (1 - x2 * x2 * x2);
      return (f1 * f1) + (f2 * f2) + (f3 * f3);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Beale function at the given point.</returns>
    public override double Evaluate(RealVector point) {
      return Apply(point);
    }
  }
}
