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
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// Base class for a test function evaluator.
  /// </summary>
  [Item("Single-Objective Function", "Base class for single objective functions.")]
  [StorableType("2775A51F-C97B-4D2E-9B25-9E2591A961CB")]
  public abstract class SingleObjectiveTestFunction : ParameterizedNamedItem, ISingleObjectiveTestFunction {
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

    [StorableConstructor]
    protected SingleObjectiveTestFunction(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectiveTestFunction(SingleObjectiveTestFunction original, Cloner cloner) : base(original, cloner) { }
    protected SingleObjectiveTestFunction() : base() { }

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
