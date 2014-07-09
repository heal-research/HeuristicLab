#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// N point crossover for binary vectors.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg..
  /// </remarks>
  [Item("NPointCrossover", "N point crossover for binary vectors. It is implemented as described in Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg.")]
  [StorableClass]
  public sealed class NPointCrossover : BinaryVectorCrossover {
    /// <summary>
    /// Number of crossover points.
    /// </summary>
    public ValueLookupParameter<IntValue> NParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["N"]; }
    }

    [StorableConstructor]
    private NPointCrossover(bool deserializing) : base(deserializing) { }
    private NPointCrossover(NPointCrossover original, Cloner cloner) : base(original, cloner) { }
    /// <summary>
    /// Initializes a new instance of <see cref="NPointCrossover"/>
    /// </summary>
    public NPointCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("N", "Number of crossover points", new IntValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NPointCrossover(this, cloner);
    }

    /// <summary>
    /// Performs a N point crossover at randomly chosen positions of the two 
    /// given parent binary vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the value for N is invalid or when the parent vectors are of different length.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <param name="n">Number of crossover points.</param>
    /// <returns>The newly created binary vector, resulting from the N point crossover.</returns>
    public static BinaryVector Apply(IRandom random, BinaryVector parent1, BinaryVector parent2, IntValue n) {
      if (parent1.Length != parent2.Length)
        throw new ArgumentException("NPointCrossover: The parents are of different length.");

      if (n.Value > parent1.Length)
        throw new ArgumentException("NPointCrossover: There cannot be more breakpoints than the size of the parents.");

      if (n.Value < 1)
        throw new ArgumentException("NPointCrossover: N cannot be < 1.");

      int length = parent1.Length;
      bool[] result = new bool[length];
      int[] breakpoints = new int[n.Value];

      //choose break points
      List<int> breakpointPool = new List<int>();

      for (int i = 0; i < length; i++)
        breakpointPool.Add(i);

      for (int i = 0; i < n.Value; i++) {
        int index = random.Next(breakpointPool.Count);
        breakpoints[i] = breakpointPool[index];
        breakpointPool.RemoveAt(index);
      }

      Array.Sort(breakpoints);

      //perform crossover
      int arrayIndex = 0;
      int breakPointIndex = 0;
      bool firstParent = true;

      while (arrayIndex < length) {
        if (breakPointIndex < breakpoints.Length &&
          arrayIndex == breakpoints[breakPointIndex]) {
          breakPointIndex++;
          firstParent = !firstParent;
        }

        if (firstParent)
          result[arrayIndex] = parent1[arrayIndex];
        else
          result[arrayIndex] = parent2[arrayIndex];

        arrayIndex++;
      }

      return new BinaryVector(result);
    }

    /// <summary>
    /// Performs a N point crossover at a randomly chosen position of two 
    /// given parent binary vectors.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if there are not exactly two parents.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the N parameter could not be found.</description></item>
    /// </exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two binary vectors that should be crossed.</param>
    /// <returns>The newly created binary vector, resulting from the N point crossover.</returns>
    protected override BinaryVector Cross(IRandom random, ItemArray<BinaryVector> parents) {
      if (parents.Length != 2) throw new ArgumentException("ERROR in NPointCrossover: The number of parents is not equal to 2");

      if (NParameter.ActualValue == null) throw new InvalidOperationException("NPointCrossover: Parameter " + NParameter.ActualName + " could not be found.");

      return Apply(random, parents[0], parents[1], NParameter.Value);
    }
  }
}
