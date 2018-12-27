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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// The Levy function is implemented as described on http://www-optima.amp.i.kyoto-u.ac.jp/member/student/hedar/Hedar_files/TestGO_files/Page2056.htm, last accessed April 12th, 2010.
  /// </summary>
  [Item("LevyEvaluator", "Evaluates the Levy function on a given point. The optimum of this function is 0 at (1,1,...,1). It is implemented as described on http://www-optima.amp.i.kyoto-u.ac.jp/member/student/hedar/Hedar_files/TestGO_files/Page2056.htm, last accessed April 12th, 2010.")]
  [StorableClass]
  public class LevyEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    public override string FunctionName { get { return "Levy"; } }
    /// <summary>
    /// Returns false as the Levy function is a minimization problem.
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
    /// Gets the (theoretical) maximum problem size (2^31 - 1).
    /// </summary>
    public override int MaximumProblemSize {
      get { return int.MaxValue; }
    }

    [StorableConstructor]
    protected LevyEvaluator(bool deserializing) : base(deserializing) { }
    protected LevyEvaluator(LevyEvaluator original, Cloner cloner) : base(original, cloner) { }
    public LevyEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LevyEvaluator(this, cloner);
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
    /// <returns>The result value of the Levy function at the given point.</returns>
    public static double Apply(RealVector point) {
      int length = point.Length;
      double[] z = new double[length];
      double s;

      for (int i = 0; i < length; i++) {
        z[i] = 1 + (point[i] - 1) / 4;
      }

      s = Math.Sin(Math.PI * z[0]);
      if (Math.Abs(s) < 1e-15) s = 0; // Math.Sin(Math.PI) == 0.00000000000000012246063538223773
      s *= s;

      for (int i = 0; i < length - 1; i++) {
        s += (z[i] - 1) * (z[i] - 1) * (1 + 10 * Math.Pow(Math.Sin(Math.PI * z[i] + 1), 2));
      }

      return s + Math.Pow(z[length - 1] - 1, 2) * (1 + Math.Pow(Math.Sin(2 * Math.PI * z[length - 1]), 2));
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Levy function at the given point.</returns>
    public override double Evaluate(RealVector point) {
      return Apply(point);
    }
  }
}
