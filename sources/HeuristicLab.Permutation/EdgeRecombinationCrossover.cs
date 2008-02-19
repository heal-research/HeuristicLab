#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Permutation {
  public class EdgeRecombinationCrossover : PermutationCrossoverBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
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
      return result;
    }

    protected override int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
