#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// The rounded local crossover for integer vectors is similar to the <see cref="UniformArithmeticCrossover"/>, but where the factor alpha is chosen randomly in the interval [0;1) for each position.
  /// </summary>cribed in Dumitrescu, D. et al. (2000), Evolutionary computation, CRC Press, Boca Raton, FL, p. 194.
  /// </remarks>
  [Item("RoundedLocalCrossover", @"The runded local crossover is similar to the arithmetic crossover, but uses a random alpha for each position x = alpha * p1 + (1-alpha) * p2.")]
  [StorableClass]
  public class RoundedLocalCrossover : BoundedIntegerVectorCrossover {
    [StorableConstructor]
    protected RoundedLocalCrossover(bool deserializing) : base(deserializing) { }
    protected RoundedLocalCrossover(RoundedLocalCrossover original, Cloner cloner) : base(original, cloner) { }
    public RoundedLocalCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedLocalCrossover(this, cloner);
    }

    /// <summary>
    /// Performs a local crossover on the two given parent vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when two parents are not of the same length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <returns>The newly created integer vector, resulting from the local crossover.</returns>
    public static IntegerVector Apply(IRandom random, IntegerVector parent1, IntegerVector parent2, IntMatrix bounds) {
      if (parent1.Length != parent2.Length)
        throw new ArgumentException("RoundedLocalCrossover: the two parents are not of the same length");

      double factor;
      int length = parent1.Length;
      var result = new IntegerVector(length);

      int min, max, step = 1;
      for (int i = 0; i < length; i++) {
        min = bounds[i % bounds.Rows, 0];
        max = bounds[i % bounds.Rows, 1];
        if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];
        max = FloorFeasible(min, max, step, max - 1);
        factor = random.NextDouble();
        result[i] = RoundFeasible(min, max, step, (factor * parent1[i]) + ((1 - factor) * parent2[i]));
      }
      return result;
    }

    /// <summary>
    /// Performs a local crossover operation for two given parent integer vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two real vectors that should be crossed.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <returns>The newly created integer vector, resulting from the crossover operation.</returns>
    protected override IntegerVector CrossBounded(IRandom random, ItemArray<IntegerVector> parents, IntMatrix bounds) {
      if (parents.Length != 2) throw new ArgumentException("RoundedLocalCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1], bounds);
    }
  }
}
