#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  /// <summary>
  /// Base class for a test function evaluator.
  /// </summary>
  [Item("Multi-Objective Function", "Base class for multi objective functions.")]
  [StorableClass]
  public abstract class MultiObjectiveTestFunction : ParameterizedNamedItem, IMultiObjectiveTestFunction {
    /// <summary>
    /// These operators should not change their name through the GUI
    /// </summary>
    public override bool CanChangeName {
      get { return false; }
    }

    /// <summary>
    /// Gets the minimum problem size.
    /// </summary>
    [Storable]
    public int MinimumSolutionLength { get; private set; }
    /// <summary>
    /// Gets the maximum problem size.
    /// </summary>
    [Storable]
    public int MaximumSolutionLength { get; private set; }


    /// <summary>
    /// Gets the minimum solution size.
    /// </summary>
    [Storable]
    public int MinimumObjectives { get; private set; }
    /// <summary>
    /// Gets the maximum solution size.
    /// </summary>
    [Storable]
    public int MaximumObjectives { get; private set; }


    /// <summary>
    /// Returns whether the actual function constitutes a maximization or minimization problem.
    /// </summary>
    public bool[] Maximization(int objectives) {
      CheckObjectives(objectives);
      return GetMaximization(objectives);
    }
    protected abstract bool[] GetMaximization(int objectives);
    /// <summary>
    /// Gets the lower and upper bound of the function.
    /// </summary>
    public double[,] Bounds(int objectives) {
      CheckObjectives(objectives);
      return GetBounds(objectives);
    }
    protected abstract double[,] GetBounds(int objectives);

    /// <summary>
    /// retrieves the optimal pareto front (if known from a file)
    /// </summary>
    public IEnumerable<double[]> OptimalParetoFront(int objectives) {
      CheckObjectives(objectives);
      return GetOptimalParetoFront(objectives);
    }
    protected abstract IEnumerable<double[]> GetOptimalParetoFront(int objectives);

    /// <summary>
    /// returns a Reference Point for Hypervolume calculation (default=(11|11))
    /// </summary>
    public double[] ReferencePoint(int objectives) {
      CheckObjectives(objectives);
      return GetReferencePoint(objectives);
    }
    protected abstract double[] GetReferencePoint(int objectives);

    /// <summary>
    /// returns the best known Hypervolume for this test function   (default=-1) 
    /// </summary>     
    public virtual double OptimalHypervolume(int objectives) {
      CheckObjectives(objectives);
      return GetBestKnownHypervolume(objectives);
    }

    protected virtual double GetBestKnownHypervolume(int objectives) {
      return -1;
    }

    protected void CheckObjectives(int objectives) {
      if (objectives < MinimumObjectives) throw new ArgumentException(string.Format("There must be at least {0} objectives", MinimumObjectives));
      if (objectives > MaximumObjectives) throw new ArgumentException(string.Format("There must be at most {0} objectives", MaximumObjectives));
    }

    [StorableConstructor]
    protected MultiObjectiveTestFunction(bool deserializing) : base(deserializing) { }

    protected MultiObjectiveTestFunction(MultiObjectiveTestFunction original, Cloner cloner)
      : base(original, cloner) {
      MinimumObjectives = original.MinimumObjectives;
      MaximumObjectives = original.MaximumObjectives;
      MinimumSolutionLength = original.MinimumSolutionLength;
      MaximumSolutionLength = original.MaximumSolutionLength;
    }

    protected MultiObjectiveTestFunction(int minimumObjectives, int maximumObjectives, int minimumSolutionLength, int maximumSolutionLength)
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("Minimum Objectives",
        "The dimensionality of the problem instance (number of variables in the function).",
        (IntValue)new IntValue(minimumObjectives).AsReadOnly()) { GetsCollected = false });
      Parameters.Add(new FixedValueParameter<IntValue>("Maximum Objectives", "The dimensionality of the problem instance (number of variables in the function).", (IntValue)new IntValue(maximumObjectives).AsReadOnly()) { GetsCollected = false });
      Parameters.Add(new FixedValueParameter<IntValue>("Minimum SolutionLength", "The dimensionality of the problem instance (number of variables in the function).", (IntValue)new IntValue(minimumSolutionLength).AsReadOnly()) { GetsCollected = false });
      Parameters.Add(new FixedValueParameter<IntValue>("Maximum SolutionLength", "The dimensionality of the problem instance (number of variables in the function).", (IntValue)new IntValue(maximumSolutionLength).AsReadOnly()) { GetsCollected = false });

      MinimumObjectives = minimumObjectives;
      MaximumObjectives = maximumObjectives;
      MinimumSolutionLength = minimumSolutionLength;
      MaximumSolutionLength = maximumSolutionLength;
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result values of the function at the given point.</returns>
    public abstract double[] Evaluate(RealVector point, int objectives);
  }
}
