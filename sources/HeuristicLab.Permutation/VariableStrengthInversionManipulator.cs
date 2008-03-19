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
  public class VariableStrengthInversionManipulator : VariableStrengthPermutationManipulatorBase {
    public override string Description {
      get { return @"Applies a Lin-2-Opt manipulation which inverts the permutation between a randomly selected interval"; }
    }

    public static int[] Apply(IRandom random, int[] permutation) {
      int[] result = (int[])permutation.Clone();
      int breakPoint1, breakPoint2;

      breakPoint1 = random.Next(result.Length - 1);
      do {
        breakPoint2 = random.Next(result.Length - 1);
      } while (breakPoint2 == breakPoint1);
      if (breakPoint2 < breakPoint1) { int h = breakPoint1; breakPoint1 = breakPoint2; breakPoint2 = h; }

      for (int i = 0; i <= (breakPoint2 - breakPoint1); i++) {  // reverse permutation between breakpoints
        result[breakPoint1 + i] = permutation[breakPoint2 - i];
      }
      return result;
    }

    protected override int[] Manipulate(IScope scope, IRandom random, int[] permutation) {
      return Apply(random, permutation);
    }
  }
}
