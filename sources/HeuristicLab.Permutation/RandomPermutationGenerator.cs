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
  public class RandomPermutationGenerator : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public RandomPermutationGenerator()
      : base() {
      AddVariableInfo(new VariableInfo("Length", "Permutation length", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Permutation", "Created random permutation", typeof(Permutation), VariableKind.New));
    }

    public static int[] Apply(IRandom random, int length) {
      int[] perm = new int[length];

      for (int i = 0; i < length; i++)
        perm[i] = i;

      // Knuth shuffle
      int index, tmp;
      for (int i = 0; i < length - 1; i++) {
        index = random.Next(i, length);
        tmp = perm[i];
        perm[i] = perm[index];
        perm[index] = tmp;
      }
      return perm;
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      IntData length = GetVariableValue<IntData>("Length", scope, true);

      int[] perm = Apply(random, length.Data);
      scope.AddVariable(new Variable(scope.TranslateName("Permutation"), new Permutation(perm)));

      return null;
    }
  }
}
