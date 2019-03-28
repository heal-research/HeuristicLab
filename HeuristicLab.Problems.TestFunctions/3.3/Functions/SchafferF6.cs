#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// The Schaffer F6 function y = 0.5 + (Sin^2(Sqrt(x^2 + y^2)) - 0.5) / (1 + 0.001 * (x^2 + y^2))^2 is a multimodal function that has its optimal value 0 at the origin.
  /// </summary
  [Item("SchafferF6", "Evaluates the Schaffer F6 function y = 0.5 + (Sin^2(Sqrt(x^2 + y^2)) - 0.5) / (1 + 0.001 * (x^2 + y^2))^2 on a given point. The optimum of this function is 0 at the origin.")]
  [StorableType("FC160F97-DB25-403E-882F-7BEBA0F01E01")]
  public class SchafferF6 : SingleObjectiveTestFunction {
    /// <summary>
    /// Returns false as the Schaffer F6 function is a minimization problem.
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
      get { return new DoubleMatrix(new double[,] { { -100, 100 } }); }
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

    public override RealVector GetBestKnownSolution(int dimension) {
      return new RealVector(dimension);
    }

    [StorableConstructor]
    protected SchafferF6(StorableConstructorFlag _) : base(_) { }
    protected SchafferF6(SchafferF6 original, Cloner cloner) : base(original, cloner) { }
    public SchafferF6() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SchafferF6(this, cloner);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Schaffer F6 function at the given point.</returns>
    public static double Apply(RealVector point) {
      if (point.Length != 2) throw new ArgumentException("The SchafferF6 can only be evaluated for two dimenional vectors");
      var sumSquare = point[0] * point[0] + point[1] * point[1];
      var sin = Math.Sin(Math.Sqrt(sumSquare));
      var nom = sin * sin - 0.5;
      var denom = (1 + 0.001 * sumSquare) * (1 + 0.001 * sumSquare);
      return 0.5 + nom / denom;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Rastrigin function at the given point.</returns>
    public override double Evaluate(RealVector point) {
      return Apply(point);
    }
  }
}
