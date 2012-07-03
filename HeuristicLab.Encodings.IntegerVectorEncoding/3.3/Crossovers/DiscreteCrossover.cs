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
    /// Performs a discrete crossover operation of any number of given parents.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the vectors of the parents are of different length or when there are less than 2 parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">The list of parents for the crossover operation.</param>
    /// <returns>The newly created integer vector, resulting from the crossover operation.</returns>
    public static IntegerVector Apply(IRandom random, ItemArray<IntegerVector> parents) {
      if (parents.Length < 2) throw new ArgumentException("DiscreteCrossover: There are less than two parents to cross.");
      int length = parents[0].Length;

      for (int i = 0; i < parents.Length; i++) {
        if (parents[i].Length != length)
          throw new ArgumentException("DiscreteCrossover: The parents' vectors are of different length.", "parents");
      }

      var result = new IntegerVector(length);
      for (int i = 0; i < length; i++) {
        result[i] = parents[random.Next(parents.Length)][i];
      }

      return result;
    }

    /// <summary>
    /// Performs a discrete crossover operation for any number of given parent integer vectors.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing integer vectors that should be crossed.</param>
    /// <returns>The newly created integer vector, resulting from the crossover operation.</returns>
    protected override IntegerVector Cross(IRandom random, ItemArray<IntegerVector> parents) {
      return Apply(random, parents);
    }
  }
}
