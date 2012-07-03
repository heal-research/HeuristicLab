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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// The sphere function is a unimodal function that has its optimum at the origin.
  /// It is implemented as described in Beyer, H.-G. and Schwefel, H.-P. 2002. Evolution Strategies - A Comprehensive Introduction Natural Computing, 1, pp. 3-52.
  /// </summary>
  [Item("SphereEvaluator", "Evaluates the Sphere function y = C * ||X||^Alpha on a given point. The optimum of this function is 0 at the origin. It is implemented as described in Beyer, H.-G. and Schwefel, H.-P. 2002. Evolution Strategies - A Comprehensive Introduction Natural Computing, 1, pp. 3-52.")]
  [StorableClass]
  public class SphereEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    /// <summary>
    /// Returns false as the Sphere function is a minimization problem.
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

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SphereEvaluator(this, cloner);
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      return new RealVector(dimension);
    }

    /// <summary>
    /// The parameter C modifies the steepness of the objective function y = C * ||X||^Alpha. Default is C = 1.
    /// </summary>
    public ValueParameter<DoubleValue> CParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["C"]; }
    }
    /// <summary>
    /// The parameter Alpha modifies the steepness of the objective function y = C * ||X||^Alpha. Default is Alpha = 2.
    /// </summary>
    public ValueParameter<DoubleValue> AlphaParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["Alpha"]; }
    }
    /// <summary>
    /// The parameter C modifies the steepness of the objective function y = C * ||X||^Alpha. Default is C = 1.
    /// </summary>
    public DoubleValue C {
      get { return CParameter.Value; }
      set { if (value != null) CParameter.Value = value; }
    }
    /// <summary>
    /// The parameter Alpha modifies the steepness of the objective function y = C * ||X||^Alpha. Default is Alpha = 2.
    /// </summary>
    public DoubleValue Alpha {
      get { return AlphaParameter.Value; }
      set { if (value != null) AlphaParameter.Value = value; }
    }

    [StorableConstructor]
    protected SphereEvaluator(bool deserializing) : base(deserializing) { }
    protected SphereEvaluator(SphereEvaluator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of the SphereEvaluator with two parameters (<c>C</c> and <c>Alpha</c>).
    /// </summary>
    public SphereEvaluator()
      : base() {
      Parameters.Add(new ValueParameter<DoubleValue>("C", "The parameter C modifies the steepness of the objective function y = C * ||X||^Alpha. Default is C = 1.", new DoubleValue(1)));
      Parameters.Add(new ValueParameter<DoubleValue>("Alpha", "The parameter Alpha modifies the steepness of the objective function y = C * ||X||^Alpha. Default is Alpha = 2.", new DoubleValue(2)));
    }
    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Sphere function at the given point.</returns>
    public static double Apply(RealVector point, double c, double alpha) {
      double result = 0;
      for (int i = 0; i < point.Length; i++)
        result += point[i] * point[i];
      if (alpha != 2) result = Math.Pow(Math.Sqrt(result), alpha);
      return c * result;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <remarks>Calls <see cref="Apply"/>.</remarks>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the Sphere function at the given point.</returns>
    protected override double EvaluateFunction(RealVector point) {
      return Apply(point, C.Value, Alpha.Value);
    }
  }
}
