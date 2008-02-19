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
using HeuristicLab.Selection;

namespace HeuristicLab.Scheduling.JSSP {
  public class ConditionalSelector : SelectorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public ConditionalSelector()
      : base() {
      AddVariableInfo(new VariableInfo("Condition", "Input condition", typeof(BoolData), VariableKind.In));
    }

    private int GetFirstSelectedScopeIndex(IScope source) {
      for (int i = 0; i < source.SubScopes.Count; i++) {
        bool condition = GetVariableValue<BoolData>("Condition", source.SubScopes[i], true).Data;
        if (condition) {
          return i;
        }
      }
      return -1;
    }

    protected override void Select(IScope source, IScope target, bool copySelected) {
      if (copySelected) {
        for (int i = 0; i < source.SubScopes.Count; i++) {
          IScope selectedScope = source.SubScopes[i];
          bool condition = GetVariableValue<BoolData>("Condition", source.SubScopes[i], true).Data;
          if (condition) {
            target.AddSubScope((IScope)selectedScope.Clone());
          }
        }
      } else {
        int removeAt = GetFirstSelectedScopeIndex(source);
        while (removeAt != -1) {
          IScope selectedScope = source.SubScopes[removeAt];
          target.AddSubScope(selectedScope);
          source.RemoveSubScope(selectedScope);
          removeAt = GetFirstSelectedScopeIndex(source);
        }
      }
    }
  }
}
