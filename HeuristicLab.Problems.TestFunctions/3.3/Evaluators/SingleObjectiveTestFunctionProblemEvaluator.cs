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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// Base class for a test function evaluator.
  /// </summary>
  [Item("Evaluator", "Base calls for single objective test function evaluators.")]
  [StorableType("2775A51F-C97B-4D2E-9B25-9E2591A961CB")]
  public abstract class SingleObjectiveTestFunctionProblemEvaluator : InstrumentedOperator, ISingleObjectiveTestFunctionProblemEvaluator {
    /// <summary>
    /// The name of the function
    /// </summary>
    public abstract string FunctionName { get; }
    /// <summary>
    /// These operators should not change their name through the GUI
    /// </summary>
    public override bool CanChangeName {
      get { return false; }
    }
    /// <summary>
    /// Returns whether the actual function constitutes a maximization or minimization problem.
    /// </summary>
    public abstract bool Maximization { get; }
    /// <summary>
    /// Gets the lower and upper bound of the function.
    /// </summary>
    public abstract DoubleMatrix Bounds { get; }
    /// <summary>
    /// Gets the optimum function value.
    /// </summary>
    public abstract double BestKnownQuality { get; }
    /// <summary>
    /// Gets the minimum problem size.
    /// </summary>
    public abstract int MinimumProblemSize { get; }
    /// <summary>
    /// Gets the maximum problem size.
    /// </summary>
    public abstract int MaximumProblemSize { get; }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<RealVector> PointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Point"]; }
    }

    [StorableConstructor]
    protected SingleObjectiveTestFunctionProblemEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectiveTestFunctionProblemEvaluator(SingleObjectiveTestFunctionProblemEvaluator original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="SingleObjectiveTestFunctionEvaluator"/> with two parameters
    /// (<c>Quality</c> and <c>Point</c>).
    /// </summary>
    public SingleObjectiveTestFunctionProblemEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "Result of the evaluation of a solution."));
      Parameters.Add(new LookupParameter<RealVector>("Point", "The point at which the function should be evaluated."));
    }

    public override IOperation InstrumentedApply() {
      RealVector point = PointParameter.ActualValue;
      double quality = Evaluate(point);
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.InstrumentedApply();
    }

    public virtual double Evaluate2D(double x, double y) {
      return Evaluate(new RealVector(new double[] { x, y }));
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the function at the given point.</returns>
    public abstract double Evaluate(RealVector point);

    /// <summary>
    /// Gets the best known solution for this function.
    /// </summary>
    public abstract RealVector GetBestKnownSolution(int dimension);
  }
}
