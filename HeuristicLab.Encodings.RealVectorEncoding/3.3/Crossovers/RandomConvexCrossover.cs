#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// The random convex crossover is similar to the <see cref="LocalCrossover"/>, but chooses just one random alpha for all positions.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Dumitrescu, D. et al. (2000), Evolutionary computation, CRC Press, Boca Raton, FL, pp. 193 - 194.
  /// </remarks>
  [Item("RandomConvexCrossover", "The random convex crossover acts like the local crossover, but with just one randomly chosen alpha for all crossed positions. It is implementes as described in Dumitrescu, D. et al. (2000), Evolutionary computation, CRC Press, Boca Raton, FL, pp. 193 - 194.")]
  [StorableClass]
  public class RandomConvexCrossover : RealVectorCrossover {
    [StorableConstructor]
    protected RandomConvexCrossover(bool deserializing) : base(deserializing) { }
    protected RandomConvexCrossover(RandomConvexCrossover original, Cloner cloner) : base(original, cloner) { }
    public RandomConvexCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomConvexCrossover(this, cloner);
    }
    
    /// <summary>
    /// Performs a random convex crossover on the two given parents.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when two parents are not of the same length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent vector for the crossover.</param>
    /// <param name="parent2">The second parent vector for the crossover.</param>
    /// <returns>The newly created real vector, resulting from the random convex crossover.</returns>
    public static RealVector Apply(IRandom random, RealVector parent1, RealVector parent2) {
      if (parent1.Length != parent2.Length)
        throw new ArgumentException("ERROR in RandomConvexCrossover: the two parents are not of the same length");

      int length = parent1.Length;
      double[] result = new double[length];
      double factor = random.NextDouble();

      for (int i = 0; i < length; i++)
        result[i] = (factor * parent1[i]) + ((1 - factor) * parent2[i]);
      return new RealVector(result);
    }

    /// <summary>
    /// Performs a random convex crossover operation for two given parent real vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two real vectors that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("ERROR in RandomConvexCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
