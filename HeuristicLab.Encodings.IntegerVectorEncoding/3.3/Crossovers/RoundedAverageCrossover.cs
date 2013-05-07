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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Rounded average crossover for integer vectors.
  /// </summary>
  [Item("RoundedAverageCrossover", "Average crossover for integer vectors.")]
  [StorableClass]
  public class RoundedAverageCrossover : BoundedIntegerVectorCrossover, IBoundedIntegerVectorOperator {

    [StorableConstructor]
    protected RoundedAverageCrossover(bool deserializing) : base(deserializing) { }
    protected RoundedAverageCrossover(RoundedAverageCrossover original, Cloner cloner) : base(original, cloner) { }
    public RoundedAverageCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedAverageCrossover(this, cloner);
    }

    /// <summary>
    /// Performs an average crossover of the two given parent integer vectors.
    /// The average is rounded and mapped to the nearest valid value (e.g. if step size is > 1)
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">The parents for crossover.</param>
    /// <param name="bounds">The bounds matrix that contains for each dimension one row with minimum (inclusive), maximum (exclusive), and step size columns.
    /// If the number of rows is smaller than the number of dimensions the matrix is cycled.</param>
    /// <returns>The newly created integer vector, resulting from the single point crossover.</returns>
    public static IntegerVector Apply(IRandom random, ItemArray<IntegerVector> parents, IntMatrix bounds) {
      int length = parents[0].Length, parentsCount = parents.Length;
      if (parents.Length < 2) throw new ArgumentException("RoundedAverageCrossover: The number of parents is less than 2.", "parents");
      if (bounds == null || bounds.Rows < 1 || bounds.Columns < 2) throw new ArgumentException("AverageCrossover: Invalid bounds specified.", "bounds");

      var result = new IntegerVector(length);
      try {
        double avg;
        for (int i = 0; i < length; i++) {
          avg = 0;
          for (int j = 0; j < parentsCount; j++)
            avg += parents[j][i];
          avg /= parentsCount;
          int min = bounds[i % bounds.Rows, 0], max = bounds[i % bounds.Rows, 1], step = 1;
          if (bounds.Columns > 2) step = bounds[i % bounds.Rows, 2];
          max = FloorFeasible(min, max, step, max - 1);
          result[i] = RoundFeasible(min, max, step, avg);
        }
      } catch (IndexOutOfRangeException) {
        throw new ArgumentException("RoundedAverageCrossover: The parents' vectors are of different length.", "parents");
      }

      return result;
    }

    /// <summary>
    /// Performs an average crossover between two parent integer vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two integer vectors that should be crossed.</param>
    /// <param name="bounds">The bounds and step size for each dimension (will be cycled in case there are less rows than elements in the parent vectors).</param>
    /// <returns>The newly created integer vector, resulting from the average crossover.</returns>
    protected override IntegerVector CrossBounded(IRandom random, ItemArray<IntegerVector> parents, IntMatrix bounds) {
      return Apply(random, parents, bounds);
    }
  }
}
