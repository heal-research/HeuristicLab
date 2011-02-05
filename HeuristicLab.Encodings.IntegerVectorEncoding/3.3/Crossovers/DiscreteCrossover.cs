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

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Discrete crossover for integer vectors.
  /// </summary>
  /// 
  /// It is implemented as described in Gwiazda, T.D. 2006. Genetic algorithms reference Volume I Crossover for single-objective numerical optimization problems, p.17.
  /// </remarks>
  [Item("DiscreteCrossover", "Discrete crossover for integer vectors. It is implemented as described in Gwiazda, T.D. 2006. Genetic algorithms reference Volume I Crossover for single-objective numerical optimization problems, p.17.")]
  [StorableClass]
  public class DiscreteCrossover : IntegerVectorCrossover {
    [StorableConstructor]
    protected DiscreteCrossover(bool deserializing) : base(deserializing) { }
    protected DiscreteCrossover(DiscreteCrossover original, Cloner cloner) : base(original, cloner) { }
    public DiscreteCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DiscreteCrossover(this, cloner);
    }

    /// <summary>
    /// Performs a discrete crossover operation of the two given parents.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the vectors of the parents are of different length.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for the crossover operation.</param>
    /// <param name="parent2">The second parent for the crossover operation.</param>
    /// <returns>The newly created integer vector, resulting from the crossover operation.</returns>
    public static IntegerVector Apply(IRandom random, IntegerVector parent1, IntegerVector parent2) {
      if (parent1.Length != parent2.Length)
        throw new ArgumentException("DiscreteCrossover: The parents are of different length.");

      int length = parent1.Length;
      int[] result = new int[length];

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < 0.5)
          result[i] = parent1[i];
        else
          result[i] = parent2[i];
      }
      return new IntegerVector(result);
    }

    /// <summary>
    /// Performs a discrete crossover operation for two given parent integer vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two integer vectors that should be crossed.</param>
    /// <returns>The newly created integer vector, resulting from the crossover operation.</returns>
    protected override IntegerVector Cross(IRandom random, ItemArray<IntegerVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("ERROR in DiscreteCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
