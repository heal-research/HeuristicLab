#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// Single point crossover for integer vectors.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("SinglePointCrossover", "Single point crossover for integer vectors. It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableType("0B985803-CC2D-466B-B5F2-9A1F7C415735")]
  public class SinglePointCrossover : IntegerVectorCrossover {
    [StorableConstructor]
    protected SinglePointCrossover(StorableConstructorFlag _) : base(_) { }
    protected SinglePointCrossover(SinglePointCrossover original, Cloner cloner) : base(original, cloner) { }
    public SinglePointCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SinglePointCrossover(this, cloner);
    }

    /// <summary>
    /// Performs a single point crossover at a randomly chosen position of the two 
    /// given parent integer vectors.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created integer vector, resulting from the single point crossover.</returns>
    public static IntegerVector Apply(IRandom random, IntegerVector parent1, IntegerVector parent2) {
      if (parent1.Length != parent2.Length)
        throw new ArgumentException("SinglePointCrossover: The parents are of different length.");

      int length = parent1.Length;
      int[] result = new int[length];
      int breakPoint = random.Next(1, length);

      for (int i = 0; i < breakPoint; i++)
        result[i] = parent1[i];
      for (int i = breakPoint; i < length; i++)
        result[i] = parent2[i];

      return new IntegerVector(result);
    }

    /// <summary>
    /// Performs a single point crossover at a randomly chosen position of two 
    /// given parent integer vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two integer vectors that should be crossed.</param>
    /// <returns>The newly created integer vector, resulting from the single point crossover.</returns>
    protected override IntegerVector Cross(IRandom random, ItemArray<IntegerVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("ERROR in SinglePointCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
