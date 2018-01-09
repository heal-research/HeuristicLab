#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Single point crossover for real vectors. The implementation is similar to the single point crossover for binary vectors.
  /// After a breakpoint is randomly chosen in the interval [1,N-1) with N = length of the vector, the first part is copied from parent1 the other part copied from parent2.
  /// The interval is chosen such that at least one position is taken from a different parent.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.
  /// </remarks>
  [Item("SinglePointCrossover", "Breaks both parent chromosomes at a randomly chosen point and assembles a child by taking one part of the first parent and the other part of the second pard. It is implemented as described in Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public class SinglePointCrossover : RealVectorCrossover {
    [StorableConstructor]
    protected SinglePointCrossover(bool deserializing) : base(deserializing) { }
    protected SinglePointCrossover(SinglePointCrossover original, Cloner cloner) : base(original, cloner) { }
    public SinglePointCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SinglePointCrossover(this, cloner);
    }
    
    /// <summary>
    /// Performs the single point crossover for real vectors. The implementation is similar to the single point crossover for binary vectors.
    /// After a breakpoint is randomly chosen in the interval [1,N-1) with N = length of the vector, the first part is copied from parent1 the other part copied from parent2.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the length of the vector is not the same in both parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created real vector, resulting from the single point crossover.</returns>
    public static RealVector Apply(IRandom random, RealVector parent1, RealVector parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("SinglePointCrossover: Parents are of unequal length");
      if (parent1.Length < 2) throw new ArgumentException("SinglePointCrossover: Cannot be applied to vectors with just one dimension.");
      int length = parent1.Length;
      RealVector result = new RealVector(length);
      int breakPoint;
      if (length == 2) breakPoint = 1;
      else breakPoint = random.Next(1, length - 1);

      for (int i = 0; i < breakPoint; i++)
        result[i] = parent1[i];
      for (int i = breakPoint; i < length; i++)
        result[i] = parent2[i];

      return result;
    }

    /// <summary>
    /// Checks number of parents and forwards the call to <see cref="Apply(IRandom, RealVector, RealVector)"/>.
    /// </summary> 
    /// <exception cref="ArgumentException">Thrown when the parents' vectors are of unequal length or when <paramref name="contiguity"/> is smaller than 0.</exception>
    /// <param name="random">The pseudo random number generator to use.</param>
    /// <param name="parents">The list of parents.</param>
    /// <returns>A new real vector.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("SinglePointCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
