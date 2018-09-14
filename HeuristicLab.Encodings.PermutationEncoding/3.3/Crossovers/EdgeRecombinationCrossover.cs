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

namespace HeuristicLab.Encodings.PermutationEncoding {
  /// <summary>
  /// Performs a cross over permutation between two permutation arrays by calculating the edges (neighbours)
  /// of each element. Starts at a randomly chosen position, the next element is a neighbour with the least 
  /// number of neighbours, the next again a neighbour and so on.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Whitley et.al. 1991, The Traveling Salesman and Sequence Scheduling, in Davis, L. (Ed.), Handbook of Genetic Algorithms, New York, pp. 350-372.<br />
  /// The operator first determines all cycles in the permutation and then composes the offspring by alternating between the cycles of the two parents.
  /// </remarks>
  [Item("EdgeRecombinationCrossover", "An operator which performs the edge recombination crossover on two permutations. It is implemented as described in Whitley et.al. 1991, The Traveling Salesman and Sequence Scheduling, in Davis, L. (Ed.), Handbook of Genetic Algorithms, New York, pp. 350-372.")]
  [StorableClass]
  public class EdgeRecombinationCrossover : PermutationCrossover {
    [StorableConstructor]
    protected EdgeRecombinationCrossover(bool deserializing) : base(deserializing) { }
    protected EdgeRecombinationCrossover(EdgeRecombinationCrossover original, Cloner cloner) : base(original, cloner) { }
    public EdgeRecombinationCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EdgeRecombinationCrossover(this, cloner);
    }

    /// <summary>
    /// Performs a cross over permutation of <paramref name="parent1"/> and <paramref name="2"/>
    /// by calculating the edges of each element. Starts at a randomly chosen position, 
    /// the next element is a neighbour with the least 
    /// number of neighbours, the next again a neighbour and so on.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="parent1"/> and <paramref name="parent2"/> are not of equal length.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the permutation lacks a number.
    /// </exception>
    /// <param name="random">The random number generator.</param>
    /// <param name="parent1">The parent scope 1 to cross over.</param>
    /// <param name="parent2">The parent scope 2 to cross over.</param>
    /// <returns>The created cross over permutation as int array.</returns>
    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("EdgeRecombinationCrossover: The parent permutations are of unequal length.");
      int length = parent1.Length;
      int[] result = new int[length];
      int[,] edgeList = new int[length, 4];
      bool[] remainingNumbers = new bool[length];
      int index, currentEdge, currentNumber, nextNumber, currentEdgeCount, minEdgeCount;

      for (int i = 0; i < length; i++) {  // generate edge list for every number
        remainingNumbers[i] = true;

        index = 0;
        while ((index < length) && (parent1[index] != i)) {  // search edges in parent1
          index++;
        }
        if (index == length) {
          throw (new InvalidOperationException("Permutation doesn't contain number " + i + "."));
        } else {
          edgeList[i, 0] = parent1[(index - 1 + length) % length];
          edgeList[i, 1] = parent1[(index + 1) % length];
        }
        index = 0;
        while ((index < length) && (parent2[index] != i)) {  // search edges in parent2
          index++;
        }
        if (index == length) {
          throw (new InvalidOperationException("Permutation doesn't contain number " + i + "."));
        } else {
          currentEdge = parent2[(index - 1 + length) % length];
          if ((edgeList[i, 0] != currentEdge) && (edgeList[i, 1] != currentEdge)) {  // new edge found ?
            edgeList[i, 2] = currentEdge;
          } else {
            edgeList[i, 2] = -1;
          }
          currentEdge = parent2[(index + 1) % length];
          if ((edgeList[i, 0] != currentEdge) && (edgeList[i, 1] != currentEdge)) {  // new edge found ?
            edgeList[i, 3] = currentEdge;
          } else {
            edgeList[i, 3] = -1;
          }
        }
      }

      currentNumber = random.Next(length);  // get number to start
      for (int i = 0; i < length; i++) {
        result[i] = currentNumber;
        remainingNumbers[currentNumber] = false;

        for (int j = 0; j < 4; j++) {  // remove all edges to / from currentNumber
          if (edgeList[currentNumber, j] != -1) {
            for (int k = 0; k < 4; k++) {
              if (edgeList[edgeList[currentNumber, j], k] == currentNumber) {
                edgeList[edgeList[currentNumber, j], k] = -1;
              }
            }
          }
        }

        minEdgeCount = 5;  // every number hasn't more than 4 edges
        nextNumber = -1;
        for (int j = 0; j < 4; j++) {  // find next number with least edges
          if (edgeList[currentNumber, j] != -1) {  // next number found
            currentEdgeCount = 0;
            for (int k = 0; k < 4; k++) {  // count edges of next number
              if (edgeList[edgeList[currentNumber, j], k] != -1) {
                currentEdgeCount++;
              }
            }
            if ((currentEdgeCount < minEdgeCount) ||
              ((currentEdgeCount == minEdgeCount) && (random.NextDouble() < 0.5))) {
              nextNumber = edgeList[currentNumber, j];
              minEdgeCount = currentEdgeCount;
            }
          }
        }
        currentNumber = nextNumber;
        if (currentNumber == -1) {  // current number has no more edge
          index = 0;
          while ((index < length) && (!remainingNumbers[index])) {  // choose next remaining number
            index++;
          }
          if (index < length) {
            currentNumber = index;
          }
        }
      }
      return new Permutation(parent1.PermutationType, result);
    }

    /// <summary>
    /// Checks number of parents and calls <see cref="Apply(IRandom, Permutation, Permutation)"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if there are not exactly two parents.</exception>
    /// <param name="random">A random number generator.</param>
    /// <param name="parents">An array containing the two permutations that should be crossed.</param>
    /// <returns>The newly created permutation, resulting from the crossover operation.</returns>
    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("ERROR in EdgeRecombinationCrossover: The number of parents is not equal to 2");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
