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

namespace HeuristicLab.Selection {
  public abstract class SelectorBase : OperatorBase {
    public SelectorBase()
      : base() {
      AddVariableInfo(new VariableInfo("CopySelected", "Copy or move selected sub-scopes", typeof(BoolData), VariableKind.In));
      GetVariableInfo("CopySelected").Local = true;
      AddVariable(new Variable("CopySelected", new BoolData(false)));
    }

    public override IOperation Apply(IScope scope) {
      BoolData copySelected = GetVariableValue<BoolData>("CopySelected", scope, true);

      IScope source = new Scope("Remaining");
      while (scope.SubScopes.Count > 0) {
        IScope s = scope.SubScopes[0];
        scope.RemoveSubScope(s);
        source.AddSubScope(s);
      }
      scope.AddSubScope(source);
      IScope target = new Scope("Selected");
      scope.AddSubScope(target);

      Select(source, target, copySelected.Data);

      return null;
    }

    protected abstract void Select(IScope source, IScope target, bool copySelected);
  }
}
