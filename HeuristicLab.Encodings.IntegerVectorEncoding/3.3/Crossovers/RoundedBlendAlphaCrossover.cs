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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Rounded blend alpha crossover for integer vectors (BLX-a). Creates a new offspring by selecting a random value 
  /// from the interval between the two alleles of the parent solutions and rounding the result to the next feasible
  /// integer. The interval is increased in both directions by the factor alpha.
  /// </summary>
  [Item("RoundedBlendAlphaCrossover", "The rounded blend alpha crossover (BLX-a) for integer vectors creates new offspring by sampling a new value in the range [min_i - d * alpha, max_i + d * alpha) at each position i and rounding the result to the next feasible integer. Here min_i and max_i are the smaller and larger value of the two parents at position i and d is max_i - min_i.")]
  [StorableClass]
  public class RoundedBlendAlphaCrossover : BoundedIntegerVectorCrossover {
    /// <summary>
    /// The alpha parameter specifies how much the interval between the parents should be extended to the left and right.
    /// The value of this parameter also determines the name of the operator: BLX-0.0 for example means alpha is set to 0.
    /// When Alpha is 0, then the offspring will only be chosen in between the parents, the bigger alpha is the more it will be possible to choose
    /// values left and right of the min and max value for each gene.
    /// </summary>
    public ValueLookupParameter<DoubleValue> AlphaParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Alpha"]; }
    }

    [StorableConstructor]
    protected RoundedBlendAlphaCrossover(bool deserializing) : base(deserializing) { }
    protected RoundedBlendAlphaCrossover(RoundedBlendAlphaCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="RoundedBlendAlphaCrossover"/> with one parameter (<c>Alpha</c>).
    /// </summary>
    public RoundedBlendAlphaCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Alpha", "The Alpha parameter controls the extension of the range beyond the two parents. It must be >= 0. A value of 0.5 means that half the range is added to both sides of the intervals.", new DoubleValue(0.5)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedBlendAlphaCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the rounded blend alpha crossover (BLX-a) of two integer vectors.<br/>
    /// It creates new offspring by sampling a new value in the range [min_i - d * alpha, max_i + d * alpha) at each position i
    /// and rounding the result to the next integer.
    /// Here min_i and max_i are the smaller and larger value of the two parents at position i and d is max_i - min_i.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are of different length or<br/>
    /// when <paramref name="alpha"/> is less than 0.
    /// </exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <param name="alpha">The alpha value for the crossover.</param>
    /// <returns>The newly created integer vector resulting from the crossover operation.</returns>
    public static IntegerVector Apply(IRandom random, IntegerVector parent1, IntegerVector parent2, IntMatrix bounds, DoubleValue alpha) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("RoundedBlendAlphaCrossover: The parents' vectors are of different length.", "parent1");
      if (alpha.Value < 0) throw new ArgumentException("RoundedBlendAlphaCrossover: Paramter alpha must be greater or equal than 0.", "alpha");
      if (bounds == null || bounds.Rows < 1 || bounds.Columns < 2) throw new ArgumentException("RoundedBlendAlphaCrossover: Invalid bounds specified.", "bounds");

      int length = parent1.Length;
      var result = new IntegerVector(length);
      double max = 0, min = 0, d = 0, resMin = 0, resMax = 0;
      int minBound, maxBound, step = 1;

      for (int i = 0; i < length; i++) {
        minBound = bounds[i % bounds.Rows, 0];
        maxBound = bounds[i % bounds.Rows, 1];
        if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];
        maxBound = FloorFeasible(minBound, maxBound, step, maxBound - 1);

        max = Math.Max(parent1[i], parent2[i]);
        min = Math.Min(parent1[i], parent2[i]);
        d = Math.Abs(max - min);
        resMin = FloorFeasible(minBound, maxBound, step, min - d * alpha.Value);
        resMax = CeilingFeasible(minBound, maxBound, step, max + d * alpha.Value);

        result[i] = RoundFeasible(minBound, maxBound, step, resMin + random.NextDouble() * Math.Abs(resMax - resMin));
      }
      return result;
    }

    /// <summary>
    /// Checks that the number of parents is equal to 2 and forwards the call to <see cref="Apply(IRandom, RealVector, RealVector, DoubleValue)"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the number of parents is not equal to 2.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the parameter alpha could not be found.</exception>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="parents">The collection of parents (must be of size 2).</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <returns>The integer vector resulting from the crossover operation.</returns>
    protected override IntegerVector CrossBounded(IRandom random, ItemArray<IntegerVector> parents, IntMatrix bounds) {
      if (parents.Length != 2) throw new ArgumentException("RoundedBlendAlphaCrossover: The number of parents is not equal to 2", "parents");
      if (AlphaParameter.ActualValue == null) throw new InvalidOperationException("RoundedBlendAlphaCrossover: Parameter " + AlphaParameter.ActualName + " could not be found.");
      return Apply(random, parents[0], parents[1], bounds, AlphaParameter.ActualValue);
    }
  }
}
