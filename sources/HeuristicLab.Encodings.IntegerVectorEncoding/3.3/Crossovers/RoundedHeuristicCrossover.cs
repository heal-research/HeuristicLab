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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Heuristic crossover for integer vectors: Calculates the vector from the worse to the better parent and adds that to the better parent weighted with a factor in the interval [0;1).
  /// The result is then rounded to the next feasible integer.
  /// The idea is that going further in direction from the worse to the better leads to even better solutions (naturally this depends on the fitness landscape).
  /// </summary>
  [Item("RoundedHeuristicCrossover", "The heuristic crossover produces offspring that extend the better parent in direction from the worse to the better parent.")]
  [StorableClass]
  public class RoundedHeuristicCrossover : BoundedIntegerVectorCrossover {
    /// <summary>
    /// Whether the problem is a maximization or minimization problem.
    /// </summary>
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    /// <summary>
    /// The quality of the parents.
    /// </summary>
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    [StorableConstructor]
    protected RoundedHeuristicCrossover(bool deserializing) : base(deserializing) { }
    protected RoundedHeuristicCrossover(RoundedHeuristicCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="RoundedHeuristicCrossover"/> with two variable infos
    /// (<c>Maximization</c> and <c>Quality</c>).
    /// </summary>
    public RoundedHeuristicCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "Whether the problem is a maximization problem or not."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality values of the parents."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedHeuristicCrossover(this, cloner);
    }

    /// <summary>
    /// Perfomrs a heuristic crossover on the two given parents.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when two parents are not of the same length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="betterParent">The first parent for the crossover operation.</param>
    /// <param name="worseParent">The second parent for the crossover operation.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <returns>The newly created integer vector, resulting from the heuristic crossover.</returns>
    public static IntegerVector Apply(IRandom random, IntegerVector betterParent, IntegerVector worseParent, IntMatrix bounds) {
      if (betterParent.Length != worseParent.Length)
        throw new ArgumentException("HeuristicCrossover: the two parents are not of the same length");

      int length = betterParent.Length;
      var result = new IntegerVector(length);
      double factor = random.NextDouble();

      int min, max, step = 1;
      for (int i = 0; i < length; i++) {
        min = bounds[i % bounds.Rows, 0];
        max = bounds[i % bounds.Rows, 1];
        if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];
        max = FloorFeasible(min, max, step, max - 1);
        result[i] = RoundFeasible(min, max, step, betterParent[i] + factor * (betterParent[i] - worseParent[i]));
      }
      return result;
    }

    /// <summary>
    /// Performs a heuristic crossover operation for two given parent integer vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the number of parents is not equal to 2.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when either:<br/>
    /// <list type="bullet">
    /// <item><description>Maximization parameter could not be found.</description></item>
    /// <item><description>Quality parameter could not be found or the number of quality values is not equal to the number of parents.</description></item>
    /// </list>
    /// </exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two real vectors that should be crossed.</param>
    /// /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <returns>The newly created integer vector, resulting from the crossover operation.</returns>
    protected override IntegerVector CrossBounded(IRandom random, ItemArray<IntegerVector> parents, IntMatrix bounds) {
      if (parents.Length != 2) throw new ArgumentException("RoundedHeuristicCrossover: The number of parents is not equal to 2");

      if (MaximizationParameter.ActualValue == null) throw new InvalidOperationException("RoundedHeuristicCrossover: Parameter " + MaximizationParameter.ActualName + " could not be found.");
      if (QualityParameter.ActualValue == null || QualityParameter.ActualValue.Length != parents.Length) throw new InvalidOperationException("RoundedHeuristicCrossover: Parameter " + QualityParameter.ActualName + " could not be found, or not in the same quantity as there are parents.");

      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;

      if (maximization && qualities[0].Value >= qualities[1].Value || !maximization && qualities[0].Value <= qualities[1].Value)
        return Apply(random, parents[0], parents[1], bounds);
      else
        return Apply(random, parents[1], parents[0], bounds);
    }
  }
}
