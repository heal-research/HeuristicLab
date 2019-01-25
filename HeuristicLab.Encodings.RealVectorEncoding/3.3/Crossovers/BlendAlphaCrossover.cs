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
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// Blend alpha crossover for real vectors (BLX-a). Creates a new offspring by selecting a random value 
  /// from the interval between the two alleles of the parent solutions. The interval is increased 
  /// in both directions by the factor alpha.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Takahashi, M. and Kita, H. 2001. A crossover operator using independent component analysis for real-coded genetic algorithms Proceedings of the 2001 Congress on Evolutionary Computation, pp. 643-649.<br/>
  /// The default value for alpha is 0.5.
  /// </remarks>
  [Item("BlendAlphaCrossover", "The blend alpha crossover (BLX-a) for real vectors creates new offspring by sampling a new value in the range [min_i - d * alpha, max_i + d * alpha) at each position i. Here min_i and max_i are the smaller and larger value of the two parents at position i and d is max_i - min_i. It is implemented as described in Takahashi, M. and Kita, H. 2001. A crossover operator using independent component analysis for real-coded genetic algorithms Proceedings of the 2001 Congress on Evolutionary Computation, pp. 643-649.")]
  [StorableType("A56EA6B8-79A2-4762-B651-BEBAB95F8E8E")]
  public class BlendAlphaCrossover : RealVectorCrossover {
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
    protected BlendAlphaCrossover(StorableConstructorFlag _) : base(_) { }
    protected BlendAlphaCrossover(BlendAlphaCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="BlendAlphaCrossover"/> with one parameter (<c>Alpha</c>).
    /// </summary>
    public BlendAlphaCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Alpha", "The Alpha parameter controls the extension of the range beyond the two parents. It must be >= 0. A value of 0.5 means that half the range is added to both sides of the intervals.", new DoubleValue(0.5)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BlendAlphaCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the blend alpha crossover (BLX-a) of two real vectors.<br/>
    /// It creates new offspring by sampling a new value in the range [min_i - d * alpha, max_i + d * alpha) at each position i.
    /// Here min_i and max_i are the smaller and larger value of the two parents at position i and d is max_i - min_i.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are of different length or<br/>
    /// when <paramref name="alpha"/> is less than 0.
    /// </exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <param name="alpha">The alpha value for the crossover.</param>
    /// <returns>The newly created real vector resulting from the crossover operation.</returns>
    public static RealVector Apply(IRandom random, RealVector parent1, RealVector parent2, DoubleValue alpha) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("BlendAlphaCrossover: The parents' vectors are of different length.", "parent1");
      if (alpha.Value < 0) throw new ArgumentException("BlendAlphaCrossover: Paramter alpha must be greater or equal than 0.", "alpha");
      int length = parent1.Length;
      RealVector result = new RealVector(length);
      double max = 0, min = 0, d = 0, resMin = 0, resMax = 0;

      for (int i = 0; i < length; i++) {
        max = Math.Max(parent1[i], parent2[i]);
        min = Math.Min(parent1[i], parent2[i]);
        d = Math.Abs(max - min);
        resMin = min - d * alpha.Value;
        resMax = max + d * alpha.Value;

        result[i] = resMin + random.NextDouble() * Math.Abs(resMax - resMin);
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
    /// <returns>The real vector resulting from the crossover operation.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("BlendAlphaCrossover: The number of parents is not equal to 2", "parents");
      if (AlphaParameter.ActualValue == null) throw new InvalidOperationException("BlendAlphaCrossover: Parameter " + AlphaParameter.ActualName + " could not be found.");
      return Apply(random, parents[0], parents[1], AlphaParameter.ActualValue);
    }
  }
}
