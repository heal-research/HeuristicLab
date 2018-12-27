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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// Uniform crossover for binary vectors.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("UniformCrossover", "Uniform crossover for binary vectors. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public sealed class UniformCrossover : BinaryVectorCrossover {

    [StorableConstructor]
    private UniformCrossover(bool deserializing) : base(deserializing) { }
    private UniformCrossover(UniformCrossover original, Cloner cloner) : base(original, cloner) { }
    public UniformCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformCrossover(this, cloner);
    }

    /// <summary>
    /// Performs a uniform crossover between two binary vectors.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created binary vector, resulting from the uniform crossover.</returns>
    public static BinaryVector Apply(IRandom random, BinaryVector parent1, BinaryVector parent2) {
      if (parent1.Length != parent2.Length)
        throw new ArgumentException("UniformCrossover: The parents are of different length.");

      int length = parent1.Length;
      bool[] result = new bool[length];

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < 0.5)
          result[i] = parent1[i];
        else
          result[i] = parent2[i];
      }

      return new BinaryVector(result);
    }

    /// <summary>
    /// Performs a uniform crossover at a randomly chosen position of two 
    /// given parent binary vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two binary vectors that should be crossed.</param>
    /// <returns>The newly created binary vector, resulting from the uniform crossover.</returns>
    protected override BinaryVector Cross(IRandom random, ItemArray<BinaryVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("ERROR in UniformCrossover: The number of parents is not equal to 2");

      return Apply(random, parents[0], parents[1]);
    }
  }
}
