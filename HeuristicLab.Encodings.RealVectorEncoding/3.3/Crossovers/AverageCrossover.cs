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
using HEAL.Attic;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// The average crossover (intermediate recombination) calculates the average or centroid of a number of parent vectors.
  /// </summary>
  /// <remarks>
  /// It is implemented as described by Beyer, H.-G. and Schwefel, H.-P. 2002. Evolution Strategies - A Comprehensive Introduction Natural Computing, 1, pp. 3-52.
  /// </remarks>
  [Item("AverageCrossover", "The average crossover (intermediate recombination) produces a new offspring by calculating in each position the average of a number of parents. It is implemented as described by Beyer, H.-G. and Schwefel, H.-P. 2002. Evolution Strategies - A Comprehensive Introduction Natural Computing, 1, pp. 3-52.")]
  [StorableType("D182B4D8-DFB1-467C-B6C8-8A8BC7540304")]
  public class AverageCrossover : RealVectorCrossover {
    [StorableConstructor]
    protected AverageCrossover(StorableConstructorFlag _) : base(_) { }
    protected AverageCrossover(AverageCrossover original, Cloner cloner) : base(original, cloner) { }
    public AverageCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AverageCrossover(this, cloner);
    }
    
    /// <summary>
    /// Performs the average crossover (intermediate recombination) on a list of parents.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when there is just one parent or when the parent vectors are of different length.</exception>
    /// <remarks>
    /// There can be more than two parents.
    /// </remarks>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The list of parents.</param>
    /// <returns>The child vector (average) of the parents.</returns>
    public static RealVector Apply(IRandom random, ItemArray<RealVector> parents) {
      int length = parents[0].Length, parentsCount = parents.Length;
      if (parents.Length < 2) throw new ArgumentException("AverageCrossover: The number of parents is less than 2.", "parents");
      RealVector result = new RealVector(length);
      try {
        double avg;
        for (int i = 0; i < length; i++) {
          avg = 0;
          for (int j = 0; j < parentsCount; j++)
            avg += parents[j][i];
          result[i] = avg / (double)parentsCount;
        }
      }
      catch (IndexOutOfRangeException) {
        throw new ArgumentException("AverageCrossover: The parents' vectors are of different length.", "parents");
      }

      return result;
    }

    /// <summary>
    /// Forwards the call to <see cref="Apply(IRandom, ItemArray<RealVector>)"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The list of parents.</param>
    /// <returns>The child vector (average) of the parents.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      return Apply(random, parents);
    }
  }
}
