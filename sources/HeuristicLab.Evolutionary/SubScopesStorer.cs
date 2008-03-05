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
  public class SubScopesStorer : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public SubScopesStorer()
      : base() {
      AddVariableInfo(new VariableInfo("SubScopes", "Number of sub-scopes that should be available", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SubScopesStore", "Temporarily stored sub-scopes", typeof(ItemList<IScope>), VariableKind.New | VariableKind.In | VariableKind.Out | VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      IntData subScopes = GetVariableValue<IntData>("SubScopes", scope, true);
      ItemList<IScope> subScopesStore = GetVariableValue<ItemList<IScope>>("SubScopesStore", scope, false);

      if (subScopesStore == null) {
        subScopesStore = new ItemList<IScope>();
        IVariableInfo info = GetVariableInfo("SubScopesStore");
        if (info.Local)
          AddVariable(new Variable(info.ActualName, subScopesStore));
        else
          scope.AddVariable(new Variable(info.ActualName, subScopesStore));
      }

      IScope left = scope.SubScopes[0];
      IScope right = scope.SubScopes[1];

      if (right.SubScopes.Count + subScopesStore.Count
          >= subScopes.Data) {  // enough sub-scopes available
        // restore sub-scopes
        for (int i = 0; i < subScopesStore.Count; i++) {
          right.AddSubScope(subScopesStore[i]);
          IVariableInfo info = GetVariableInfo("SubScopesStore");
          if (info.Local)
            RemoveVariable(info.ActualName);
          else
            scope.RemoveVariable(info.ActualName);
        }
        return null;
      } else {  // not enough sub-scopes available
        // store sub-scopes
        for (int i = 0; i < right.SubScopes.Count; i++) {
          IScope s = right.SubScopes[0];
          right.RemoveSubScope(s);
          subScopesStore.Add(s);
        }

        // shift left on level up
        scope.RemoveSubScope(right);
        scope.RemoveSubScope(left);
        for (int i = 0; i < left.SubScopes.Count; i++)
          scope.AddSubScope(left.SubScopes[i]);

        return new AtomicOperation(SubOperators[0], scope);
      }
    }
  }
}
