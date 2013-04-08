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
  /// The Booth function is implemented as described on http://www-optima.amp.i.kyoto-u.ac.jp/member/student/hedar/Hedar_files/TestGO_files/Page816.htm, last accessed April 12th, 2010.
  /// </summary>
  [Item("BoothEvaluator", "Evaluates the Booth function on a given point. The optimum of this function is 0 at (1,3). It is implemented as described on http://www-optima.amp.i.kyoto-u.ac.jp/member/student/hedar/Hedar_files/TestGO_files/Page816.htm, last accessed April 12th, 2010.")]
  [StorableClass]
  public class BoothEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    /// <summary>
    /// Returns false as the Booth function is a minimization problem.
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
    protected BoothEvaluator(bool deserializing) : base(deserializing) { }
    protected BoothEvaluator(BoothEvaluator original, Cloner cloner) : base(original, cloner) { }
    public BoothEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BoothEvaluator(this, cloner);
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      if (dimension != 2) throw new ArgumentException(Name + ": This function is only defined for 2 dimensions.", "dimension");
      return new RealVector(new double[] { 1, 3 });
    }
    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Booth function at the given point.</returns>
    public static double Apply(RealVector point) {
      return (point[0] + 2 * point[1] - 7) * (point[0] + 2 * point[1] - 7)
        + (2 * point[0] + point[1] - 5) * (2 * point[0] + point[1] - 5);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Booth function at the given point.</returns>
    public override double EvaluateFunction(RealVector point) {
      return Apply(point);
    }
  }
}
