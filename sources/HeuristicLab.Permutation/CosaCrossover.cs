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
  public class CosaCrossover : PermutationCrossoverBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
      int length = parent1.Length;
      int[] result = new int[length];
      int crossPoint, startIndex, endIndex;

      crossPoint = random.Next(length);
      startIndex = (crossPoint + 1) % length;

      int i = 0;
      while ((i < parent2.Length) && (parent2[i] != parent1[startIndex])) {  // find index of start value in second permutation
        i++;
      }
      i = (i + 1) % length;
      int j = 0;
      while ((j < parent1.Length) && (parent1[j] != parent2[i])) {  // find index of parent2[i] in first permutation
        j++;
      }
      endIndex = (j - 1 + length) % length;

      i = endIndex;
      j = 0;
      do {  // permutation from end to crosspoint (backwards)
        result[j] = parent1[i];
        i = (i - 1 + length) % length;
        j++;
      } while (i != crossPoint);

      i = (endIndex + 1) % length;
      while (i != crossPoint) {  // permutation from end to crosspoint (forwards)
        result[j] = parent1[i];
        i = (i + 1) % length;
        j++;
      }
      result[j] = parent1[crossPoint];  // last station: crosspoint
      return result;
    }

    protected override int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
