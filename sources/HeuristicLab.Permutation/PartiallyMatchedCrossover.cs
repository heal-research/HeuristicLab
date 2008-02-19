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
  public class PartiallyMatchedCrossover : PermutationCrossoverBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public static int[] Apply(IRandom random, int[] parent1, int[] parent2) {
      int length = parent1.Length;
      int[] result = new int[length];
      bool[] numbersCopied = new bool[length];

      int breakPoint1 = random.Next(length - 1);
      int breakPoint2 = random.Next(breakPoint1 + 1, length);

      for (int j = breakPoint1; j <= breakPoint2; j++) {  // copy part of first permutation
        result[j] = parent1[j];
        numbersCopied[result[j]] = true;
      }

      int index = breakPoint1;
      int i = (breakPoint1 == 0 ? (breakPoint2 + 1) : 0);
      while (i < length) {  // copy rest from second permutation
        if (!numbersCopied[parent2[i]]) {  // copy directly
          result[i] = parent2[i];
        } else {  // copy from area between breakpoints
          while (numbersCopied[parent2[index]]) {  // find next valid index
            index++;
          }
          result[i] = parent2[index];
          index++;
        }

        i++;
        if (i == breakPoint1) {  // skip area between breakpoints
          i = breakPoint2 + 1;
        }
      }
      return result;
    }

    protected override int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
