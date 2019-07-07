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
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// The generalized Rastrigin function y = Sum((x_i)^2 + A * (1 - Cos(2pi*x_i))) is a highly multimodal function that has its optimal value 0 at the origin.
  /// It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.
  /// </summary
  [Item("RastriginEvaluator", "Evaluates the generalized Rastrigin function y = Sum((x_i)^2 + A * (1 - Cos(2pi*x_i))) on a given point. The optimum of this function is 0 at the origin. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.")]
  [StorableType("DAC41E30-0487-4400-A089-4B57DFAA329A")]
  public class RastriginEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    public override string FunctionName { get { return "Rastrigin"; } }
    /// <summary>
    /// Returns false as the Rastrigin function is a minimization problem.
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
      get { return new DoubleMatrix(new double[,] { { -5.12, 5.12 } }); }
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
    /// The parameter A is a parameter of the objective function y = Sum((x_i)^2 + A * (1 - Cos(2pi*x_i))). Default is A = 10.
    /// </summary>
    public ValueParameter<DoubleValue> AParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["A"]; }
    }
    /// <summary>
    /// The parameter A is a parameter of the objective function y = Sum((x_i)^2 + A * (1 - Cos(2pi*x_i))). Default is A = 10.
    /// </summary>
    public DoubleValue A {
      get { return AParameter.Value; }
      set { if (value != null) AParameter.Value = value; }
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      return new RealVector(dimension);
    }

    [StorableConstructor]
    protected RastriginEvaluator(StorableConstructorFlag _) : base(_) { }
    protected RastriginEvaluator(RastriginEvaluator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of the RastriginEvaluator with one parameter (<c>A</c>).
    /// </summary>
    public RastriginEvaluator()
      : base() {
      Parameters.Add(new ValueParameter<DoubleValue>("A", "The parameter A is a parameter of the objective function y = Sum((x_i)^2 + A * (1 - Cos(2pi*x_i))). Default is A = 10.", new DoubleValue(10)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RastriginEvaluator(this, cloner);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Rastrigin function at the given point.</returns>
    public static double Apply(RealVector point, double a) {
      double result = a * point.Length;
      for (int i = 0; i < point.Length; i++) {
        result += point[i] * point[i];
        result -= a * Math.Cos(2 * Math.PI * point[i]);
      }
      return (result);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Rastrigin function at the given point.</returns>
    public override double Evaluate(RealVector point) {
      return Apply(point, A.Value);
    }
  }
}
