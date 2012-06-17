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
  /// The rounded uniform arithmetic crossover (continuous recombination) constructs an offspring by calculating x = alpha * p1 + (1-alpha) * p2 for a position x in the vector with a given probability (otherwise p1 is taken at this position).
  /// </summary>
  [Item("RoundedUniformSomePositionsArithmeticCrossover", "The uniform some positions arithmetic crossover (continuous recombination) constructs an offspring by calculating x = alpha * p1 + (1-alpha) * p2 for a position x in the vector with a given probability (otherwise p1 is taken at this position).")]
  [StorableClass]
  public class RoundedUniformArithmeticCrossover : BoundedIntegerVectorCrossover, IBoundedIntegerVectorOperator {

    /// <summary>
    /// The alpha parameter needs to be in the interval (0;1) and specifies how close the resulting offspring should be either to parent1 (alpha -> 0) or parent2 (alpha -> 1).
    /// </summary>
    public ValueLookupParameter<DoubleValue> AlphaParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Alpha"]; }
    }
    /// <summary>
    /// The probability in the range (0;1] for each position in the vector to be crossed.
    /// </summary>
    public ValueLookupParameter<DoubleValue> ProbabilityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Probability"]; }
    }

    [StorableConstructor]
    protected RoundedUniformArithmeticCrossover(bool deserializing) : base(deserializing) { }
    protected RoundedUniformArithmeticCrossover(RoundedUniformArithmeticCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance with two parameters (<c>Alpha</c> and <c>Probability</c>).
    /// </summary>
    public RoundedUniformArithmeticCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Alpha", "The alpha value in the range (0;1) that defines whether the point should be close to parent1 (->1) or parent2 (->0)", new DoubleValue(0.5)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Probability", "The probability for crossing a position in the range (0;1]", new DoubleValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedUniformArithmeticCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the arithmetic crossover on some positions by taking either x = alpha * p1 + (1 - alpha) * p2 or x = p1 depending on the probability for a gene to be crossed.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent vector.</param>
    /// <param name="parent2">The second parent vector.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <param name="alpha">The alpha parameter (<see cref="AlphaParameter"/>).</param>
    /// <param name="probability">The probability parameter (<see cref="ProbabilityParameter"/>).</param>
    /// <returns>The vector resulting from the crossover.</returns>
    public static IntegerVector Apply(IRandom random, IntegerVector parent1, IntegerVector parent2, IntMatrix bounds, DoubleValue alpha, DoubleValue probability) {
      int length = parent1.Length;
      if (length != parent2.Length) throw new ArgumentException("RoundedUniformArithmeticCrossover: The parent vectors are of different length.", "parent1");
      if (alpha.Value < 0 || alpha.Value > 1) throw new ArgumentException("RoundedUniformArithmeticCrossover: Parameter alpha must be in the range [0;1]", "alpha");
      if (probability.Value < 0 || probability.Value > 1) throw new ArgumentException("RoundedUniformArithmeticCrossover: Parameter probability must be in the range [0;1]", "probability");

      var result = new IntegerVector(length);
      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < probability.Value) {
          int min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1], step = 1;
          if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];
          double value = alpha.Value * parent1[i] + (1 - alpha.Value) * parent2[i];
          result[i] = RoundFeasible(min, max, step, value);
        } else result[i] = parent1[i];
      }
      return result;
    }

    /// <summary>
    /// Checks that there are exactly 2 parents, that the alpha and the probability parameter are not null and fowards the call to the static Apply method.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when there are not exactly two parents.</exception>
    /// <exception cref="InvalidOperationException">Thrown when either the alpha parmeter or the probability parameter could not be found.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The collection of parents (must be of size 2).</param>
    /// /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <returns>The vector resulting from the crossover.</returns>
    protected override IntegerVector CrossBounded(IRandom random, ItemArray<IntegerVector> parents, IntMatrix bounds) {
      if (parents.Length != 2) throw new ArgumentException("RoundedUniformArithmeticCrossover: There must be exactly two parents.", "parents");
      if (AlphaParameter.ActualValue == null) throw new InvalidOperationException("RoundedUniformArithmeticCrossover: Parameter " + AlphaParameter.ActualName + " could not be found.");
      if (ProbabilityParameter.ActualValue == null) throw new InvalidOperationException("RoundedUniformArithmeticCrossover: Parameter " + ProbabilityParameter.ActualName + " could not be found.");
      return Apply(random, parents[0], parents[1], bounds, AlphaParameter.ActualValue, ProbabilityParameter.ActualValue);
    }
  }
}
