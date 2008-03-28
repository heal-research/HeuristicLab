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
using HeuristicLab.Operators;

namespace HeuristicLab.Evolutionary {
  public abstract class MultiCrossoverBase : OperatorBase {
    public MultiCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("Parents", "Number of parents that should be crossed", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      int parents = GetVariableValue<IntData>("Parents", scope, true).Data;

      if ((scope.SubScopes.Count % parents) != 0)
        throw new InvalidOperationException("Size of mating pool and number of parents don't match");

      int children = scope.SubScopes.Count / parents;
      for (int i = 0; i < children; i++) {
        IScope[] parentScopes = new IScope[parents];
        for (int j = 0; j < parentScopes.Length; j++)
          parentScopes[j] = scope.SubScopes[j];

        IScope child = new Scope(i.ToString());
        scope.AddSubScope(child);
        Cross(scope, random, parentScopes, child);

        for (int j = 0; j < parentScopes.Length; j++)
          scope.RemoveSubScope(parentScopes[j]);
      }
      return null;
    }

    protected abstract void Cross(IScope scope, IRandom random, IScope[] parents, IScope child);
  }
}
