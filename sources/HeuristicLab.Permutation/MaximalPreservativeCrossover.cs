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
  public class MaximalPreservativeCrossover : PermutationCrossoverBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] numberCopied = new bool[length];
      int breakPoint1, breakPoint2, subsegmentLength, index;

      if (length >= 20) {  // length of subsegment must be >= 10 and <= length / 2
        breakPoint1 = random.Next(length - 9);
        subsegmentLength = random.Next(10, Math.Min((int)(length / 2), length - breakPoint1) + 1);
        breakPoint2 = breakPoint1 + subsegmentLength - 1;
      } else {
        breakPoint1 = random.Next(length - 1);
        breakPoint2 = random.Next(breakPoint1 + 1, length);
      }

      index = 0;
      for (int i = breakPoint1; i <= breakPoint2; i++) {  // copy part of first permutation
        result[index] = parent1[i];
        numberCopied[result[index]] = true;
        index++;
      }

      for (int i = 0; i < parent2.Length; i++) {  // copy remaining part of second permutation
        if (!numberCopied[parent2[i]]) {
          result[index] = parent2[i];
          index++;
        }
      }
      return result;
    }

    protected override int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
