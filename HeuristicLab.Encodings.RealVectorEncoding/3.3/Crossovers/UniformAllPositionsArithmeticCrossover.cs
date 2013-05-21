#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// The uniform all positions arithmetic crossover constructs an offspring by calculating x = alpha * p1 + (1-alpha) * p2 for every position x in the vector.
  /// </summary>
  /// <remarks>
  /// By setting alpha = 0.5 it is the same as the <see cref="AverageCrossover"/>, but only on two parents.
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("UniformAllPositionsArithmeticCrossover", "The uniform all positions arithmetic crossover constructs an offspring by calculating x = alpha * p1 + (1-alpha) * p2 for every position x in the vector. Note that for alpha = 0.5 it is the same as the AverageCrossover (except that the AverageCrossover is defined for more than 2 parents). It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class UniformAllPositionsArithmeticCrossover : RealVectorCrossover {
    /// <summary>
    /// The alpha parameter needs to be in the interval [0;1] and specifies how close the resulting offspring should be either to parent1 (alpha -> 0) or parent2 (alpha -> 1).
    /// </summary>
    public ValueLookupParameter<DoubleValue> AlphaParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Alpha"]; }
    }

    [StorableConstructor]
    protected UniformAllPositionsArithmeticCrossover(bool deserializing) : base(deserializing) { }
    protected UniformAllPositionsArithmeticCrossover(UniformAllPositionsArithmeticCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance with one parameter (<c>Alpha</c>).
    /// </summary>
    public UniformAllPositionsArithmeticCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Alpha", "The alpha value in the range [0;1]", new DoubleValue(0.33)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformAllPositionsArithmeticCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the arithmetic crossover on all positions by calculating x = alpha * p1 + (1 - alpha) * p2.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the parent vectors are of different length or alpha is outside the range [0;1].</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent vector.</param>
    /// <param name="parent2">The second parent vector.</param>
    /// <param name="alpha">The alpha parameter (<see cref="AlphaParameter"/>).</param>
    /// <returns>The vector resulting from the crossover.</returns>
    public static RealVector Apply(IRandom random, RealVector parent1, RealVector parent2, DoubleValue alpha) {
      int length = parent1.Length;
      if (length != parent2.Length) throw new ArgumentException("UniformAllPositionsArithmeticCrossover: The parent vectors are of different length.", "parent1");
      if (alpha.Value < 0 || alpha.Value > 1) throw new ArgumentException("UniformAllPositionsArithmeticCrossover: Parameter alpha must be in the range [0;1]", "alpha");
      RealVector result = new RealVector(length);
      for (int i = 0; i < length; i++) {
        result[i] = alpha.Value * parent1[i] + (1 - alpha.Value) * parent2[i];
      }
      return result;
    }

    /// <summary>
    /// Checks that there are exactly 2 parents, that the alpha parameter is not null and fowards the call to <see cref="Apply(IRandom, RealVector, DoubleArrrayData, DoubleValue)"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when there are not exactly two parents.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the alpha parmeter could not be found.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The collection of parents (must be of size 2).</param>
    /// <returns>The vector resulting from the crossover.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("UniformAllPositionsArithmeticCrossover: There must be exactly two parents.", "parents");
      if (AlphaParameter.ActualValue == null) throw new InvalidOperationException("UniformAllPositionsArithmeticCrossover: Parameter " + AlphaParameter.ActualName + " could not be found.");
      return Apply(random, parents[0], parents[1], AlphaParameter.ActualValue);
    }
  }
}
