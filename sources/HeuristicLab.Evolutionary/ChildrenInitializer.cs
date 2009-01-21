#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ChildrenInitializer : OperatorBase {
    public override string Description {
      get {
        return @"This operator prepares the individuals for crossover.";
      }
    }

    public ChildrenInitializer()
      : base() {
      AddVariableInfo(new VariableInfo("ParentsPerChild", "Denotes the number of parents that should be crossed per child. Note that some of the typical crossover operators can only work with 2 parents.", typeof(IntData), VariableKind.In));
      GetVariableInfo("ParentsPerChild").Local = true;
      AddVariable(new Variable("ParentsPerChild", new IntData(2)));
    }

    public override IOperation Apply(IScope scope) {
      int parents = GetVariableValue<IntData>("ParentsPerChild", scope, true).Data;
      int children = scope.SubScopes.Count;
      if (children % parents > 0) throw new InvalidOperationException("ERROR in ChildrenInitializer: The number of selected parents is not a natural multiple of ParentsPerChild");
      children /= parents;

      for (int i = 0; i < children; i++) {
        IScope child = new Scope(i.ToString());
        for (int y = 0; y < parents; y++) {
          IScope parenty = scope.SubScopes[0];
          scope.RemoveSubScope(parenty);
          child.AddSubScope(parenty);
        }
        scope.AddSubScope(child);
      }

      return null;
    }
  }
}
