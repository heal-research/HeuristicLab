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

namespace HeuristicLab.Permutation {
  public class InsertionManipulator : PermutationManipulatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public static int[] Apply(IRandom random, int[] permutation) {
      int[] result = (int[])permutation.Clone();
      int cutIndex, insertIndex, number;

      cutIndex = random.Next(permutation.Length);
      insertIndex = random.Next(permutation.Length);
      number = permutation[cutIndex];

      int i = 0;  // index in new permutation
      int j = 0;  // index in old permutation
      while (i < permutation.Length) {
        if (j == cutIndex) {
          j++;
        }
        if (i == insertIndex) {
          result[i] = number;
          i++;
        }
        if ((i < permutation.Length) && (j < permutation.Length)) {
          result[i] = permutation[j];
          i++;
          j++;
        }
      }
      return result;
    }

    protected override int[] Manipulate(IScope scope, IRandom random, int[] permutation) {
      return Apply(random, permutation);
    }
  }
}
