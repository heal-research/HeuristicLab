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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Blend alpha-beta crossover for integer vectors (BLX-a-b). Creates a new offspring by selecting a 
  /// random value from the interval between the two alleles of the parent solutions and rounds the
  /// result to the nearest feasible value. The interval is increased in both directions as follows:
  /// Into the direction of the 'better' solution by the factor alpha, into the direction of the
  /// 'worse' solution by the factor beta.
  /// </summary>
  [Item("RoundedBlendAlphaBetaCrossover", "The rounded blend alpha beta crossover (BLX-a-b) for integer vectors is similar to the blend alpha crossover (BLX-a), but distinguishes between the better and worse of the parents. The interval from which to choose the new offspring can be extended beyond the better parent by specifying a higher alpha value, and beyond the worse parent by specifying a higher beta value. The new offspring is sampled uniformly in the extended range and rounded to the next feasible integer.")]
  [StorableClass]
  public class RoundedBlendAlphaBetaCrossover : BoundedIntegerVectorCrossover, ISingleObjectiveOperator {
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
    /// <summary>
    /// The Alpha parameter controls the extension of the range beyond the better parent. The value must be >= 0 and does not depend on Beta.
    /// </summary>
    public ValueLookupParameter<DoubleValue> AlphaParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Alpha"]; }
    }
    /// <summary>
    /// The Beta parameter controls the extension of the range beyond the worse parent. The value must be >= 0 and does not depend on Alpha.
    /// </summary>
    public ValueLookupParameter<DoubleValue> BetaParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Beta"]; }
    }

    [StorableConstructor]
    protected RoundedBlendAlphaBetaCrossover(bool deserializing) : base(deserializing) { }
    protected RoundedBlendAlphaBetaCrossover(RoundedBlendAlphaBetaCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="RoundedBlendAlphaBetaCrossover"/> with four additional parameters
    /// (<c>Maximization</c>, <c>Quality</c>, <c>Alpha</c> and <c>Beta</c>).
    /// </summary>
    public RoundedBlendAlphaBetaCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "Whether the problem is a maximization problem or not."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality values of the parents."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Alpha", "The Alpha parameter controls the extension of the range beyond the better parent. The value must be >= 0 and does not depend on Beta.", new DoubleValue(0.75)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Beta", "The Beta parameter controls the extension of the range beyond the worse parent. The value must be >= 0 and does not depend on Alpha.", new DoubleValue(0.25)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedBlendAlphaBetaCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the rounded blend alpha beta crossover (BLX-a-b) on two parent vectors.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when either:<br/>
    /// <list type="bullet">
    /// <item><description>The length of <paramref name="betterParent"/> and <paramref name="worseParent"/> is not equal.</description></item>
    /// <item><description>The parameter <paramref name="alpha"/> is smaller than 0.</description></item>
    /// <item><description>The parameter <paramref name="beta"/> is smaller than 0.</description></item>
    /// </list>
    /// </exception>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="betterParent">The better of the two parents with regard to their fitness.</param>
    /// <param name="worseParent">The worse of the two parents with regard to their fitness.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <param name="alpha">The parameter alpha.</param>
    /// <param name="beta">The parameter beta.</param>
    /// <returns>The integer vector that results from the crossover.</returns>
    public static IntegerVector Apply(IRandom random, IntegerVector betterParent, IntegerVector worseParent, IntMatrix bounds, DoubleValue alpha, DoubleValue beta) {
      if (betterParent.Length != worseParent.Length) throw new ArgumentException("RoundedBlendAlphaBetaCrossover: The parents' vectors are of different length.", "betterParent");
      if (alpha.Value < 0) throw new ArgumentException("RoundedBlendAlphaBetaCrossover: Parameter alpha must be greater or equal to 0.", "alpha");
      if (beta.Value < 0) throw new ArgumentException("RoundedBlendAlphaBetaCrossover: Parameter beta must be greater or equal to 0.", "beta");
      if (bounds == null || bounds.Rows < 1 || bounds.Columns < 2) throw new ArgumentException("RoundedBlendAlphaBetaCrossover: Invalid bounds specified.", "bounds");

      int length = betterParent.Length;
      double min, max, d;
      var result = new IntegerVector(length);
      int minBound, maxBound, step = 1;
      for (int i = 0; i < length; i++) {
        minBound = bounds[i % bounds.Rows, 0];
        maxBound = bounds[i % bounds.Rows, 1];
        if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];
        maxBound = FloorFeasible(minBound, maxBound, step, maxBound - 1);

        d = Math.Abs(betterParent[i] - worseParent[i]);
        if (betterParent[i] <= worseParent[i]) {
          min = FloorFeasible(minBound, maxBound, step, betterParent[i] - d * alpha.Value);
          max = CeilingFeasible(minBound, maxBound, step, worseParent[i] + d * beta.Value);
        } else {
          min = FloorFeasible(minBound, maxBound, step, worseParent[i] - d * beta.Value);
          max = CeilingFeasible(minBound, maxBound, step, betterParent[i] + d * alpha.Value);
        }
        result[i] = RoundFeasible(minBound, maxBound, step, min + random.NextDouble() * (max - min));
      }
      return result;
    }

    /// <summary>
    /// Checks if the number of parents is equal to 2, if all parameters are available and forwards the call to <see cref="Apply(IRandom, IntegerVector, IntegerVector, IntMatrix, DoubleValue, DoubleValue)"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the number of parents is not equal to 2.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when either:<br/>
    /// <list type="bullet">
    /// <item><description>Maximization parameter could not be found.</description></item>
    /// <item><description>Quality parameter could not be found or the number of quality values is not equal to the number of parents.</description></item>
    /// <item><description>Alpha parameter could not be found.</description></item>
    /// <item><description>Beta parameter could not be found.</description></item>
    /// </list>
    /// </exception>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="parents">The collection of parents (must be of size 2).</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <returns>The integer vector that results from the crossover.</returns>
    protected override IntegerVector CrossBounded(IRandom random, ItemArray<IntegerVector> parents, IntMatrix bounds) {
      if (parents.Length != 2) throw new ArgumentException("RoundedBlendAlphaBetaCrossover: Number of parents is not equal to 2.", "parents");
      if (MaximizationParameter.ActualValue == null) throw new InvalidOperationException("RoundedBlendAlphaBetaCrossover: Parameter " + MaximizationParameter.ActualName + " could not be found.");
      if (QualityParameter.ActualValue == null || QualityParameter.ActualValue.Length != parents.Length) throw new InvalidOperationException("RoundedBlendAlphaBetaCrossover: Parameter " + QualityParameter.ActualName + " could not be found, or not in the same quantity as there are parents.");
      if (AlphaParameter.ActualValue == null || BetaParameter.ActualValue == null) throw new InvalidOperationException("RoundedBlendAlphaBetaCrossover: Parameter " + AlphaParameter.ActualName + " or paramter " + BetaParameter.ActualName + " could not be found.");

      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      if (maximization && qualities[0].Value >= qualities[1].Value || !maximization && qualities[0].Value <= qualities[1].Value)
        return Apply(random, parents[0], parents[1], bounds, AlphaParameter.ActualValue, BetaParameter.ActualValue);
      else {
        return Apply(random, parents[1], parents[0], bounds, AlphaParameter.ActualValue, BetaParameter.ActualValue);
      }
    }
  }
}
