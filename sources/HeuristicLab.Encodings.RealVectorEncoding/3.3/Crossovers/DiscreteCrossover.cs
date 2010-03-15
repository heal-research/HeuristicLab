#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Discrete crossover for real vectors: For each position in the new vector an allele
  /// of one of the parents is randomly selected.
  /// </summary>
  /// <remarks>
  /// This operator is also called dominant recombination and unlike other crossovers works on more than two parents also.<br />
  /// It is implemented as described in Beyer, H.-G. and Schwefel, H.-P. 2002. Evolution Strategies - A Comprehensive Introduction Natural Computing, 1, pp. 3-52.
  /// </remarks>
  [Item("DiscreteCrossover", "Discrete crossover for real vectors: Creates a new offspring by combining the alleles in the parents such that each allele is randomly selected from one parent. It is implemented as described in Beyer, H.-G. and Schwefel, H.-P. 2002. Evolution Strategies - A Comprehensive Introduction Natural Computing, 1, pp. 3-52.")]
  [StorableClass]
  public class DiscreteCrossover : RealVectorCrossover {
    /// <summary>
    /// Performs a discrete crossover operation on multiple parents.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the vectors of the parents are of different length.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the parents that should be crossed.</param>
    /// <returns>The newly created real vector, resulting from the crossover operation.</returns>
    public static DoubleArray Apply(IRandom random, ItemArray<DoubleArray> parents) {
      int length = parents[0].Length;
      
      for (int i = 0; i < parents.Length; i++) { 
        if(parents[i].Length != length)
          throw new ArgumentException("DiscreteCrossover: The parents' vectors are of different length.", "parents");
      }
      
      DoubleArray result = new DoubleArray(length);
      for (int i = 0; i < length; i++) {
        result[i] = parents[random.Next(parents.Length)][i];
      }        
      
      return result;
    }

    /// <summary>
    /// Checks number of parents and forwards the call to <see cref="Apply(IRandom, ItemArray<DoubleArray>)"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parents"/> contains less than 2 elements.</exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The collection of parents (must be of size 2 or greater).</param>
    /// <returns>The real vector resulting from the crossover.</returns>
    protected override DoubleArray Cross(IRandom random, ItemArray<DoubleArray> parents) {
      if (parents.Length < 2) throw new ArgumentException("DiscreteCrossover: The number of parents is less than 2.", "parents");
      return Apply(random, parents);
    }
  }
}
