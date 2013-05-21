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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// The local crossover for real vectors is similar to the <see cref="UniformAllPositionsArithmeticCrossover"/>, but where the factor alpha is chosen randomly in the interval [0;1) for each position.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Dumitrescu, D. et al. (2000), Evolutionary computation, CRC Press, Boca Raton, FL, p. 194.
  /// </remarks>
  [Item("LocalCrossover", @"The local crossover is similar to the arithmetic all positions crossover, but uses a random alpha for each position x = alpha * p1 + (1-alpha) * p2. It is implemented as described in Dumitrescu, D. et al. (2000), Evolutionary computation, CRC Press, Boca Raton, FL., p. 194.")]
  [StorableClass]
  public class LocalCrossover : RealVectorCrossover {
    [StorableConstructor]
    protected LocalCrossover(bool deserializing) : base(deserializing) { }
    protected LocalCrossover(LocalCrossover original, Cloner cloner) : base(original, cloner) { }
    public LocalCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LocalCrossover(this, cloner);
    }
    
    /// <summary>
    /// Performs a local crossover on the two given parent vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when two parents are not of the same length.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <returns>The newly created real vector, resulting from the local crossover.</returns>
    public static RealVector Apply(IRandom random, RealVector parent1, RealVector parent2) {
      if (parent1.Length != parent2.Length)
        throw new ArgumentException("LocalCrossover: the two parents are not of the same length");

      double factor;
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        factor = random.NextDouble();
        result[i] = (factor * parent1[i]) + ((1 - factor) * parent2[i]);
      }
      return new RealVector(result);
    }

    /// <summary>
    /// Performs a local crossover operation for two given parent real vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two real vectors that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("LocalCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
