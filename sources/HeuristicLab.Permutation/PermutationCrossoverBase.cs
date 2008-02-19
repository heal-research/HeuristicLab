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
using HeuristicLab.Evolutionary;

namespace HeuristicLab.Permutation {
  public abstract class PermutationCrossoverBase : CrossoverBase {
    public PermutationCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("Permutation", "Parent and child permutations", typeof(Permutation), VariableKind.In | VariableKind.New));
    }

    protected sealed override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      IVariableInfo permutationInfo = GetVariableInfo("Permutation");
      Permutation perm1 = parent1.GetVariableValue<Permutation>(permutationInfo.ActualName, false);
      Permutation perm2 = parent2.GetVariableValue<Permutation>(permutationInfo.ActualName, false);

      if (perm1.Data.Length != perm2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to permutations of different length.");

      int[] result = Cross(scope, random, perm1.Data, perm2.Data);
      child.AddVariable(new Variable(permutationInfo.ActualName, new Permutation(result)));
    }

    protected abstract int[] Cross(IScope scope, IRandom random, int[] parent1, int[] parent2);
  }
}
