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

namespace HeuristicLab.ES {
  public class ESRandomSelector : OperatorBase {
    public override string Description {
      get {
        return @"Selects rho*lambda individuals with each rho successing individuals being selected from the parent population without repetitions.";
      }
    }

    public ESRandomSelector()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Lambda", "The amount of children to select", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Rho", "The amount of parents per child", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IScope source = new Scope("Remaining");
      while (scope.SubScopes.Count > 0) {
        IScope s = scope.SubScopes[0];
        scope.RemoveSubScope(s);
        source.AddSubScope(s);
      }
      scope.AddSubScope(source);
      IScope target = new Scope("Selected");
      scope.AddSubScope(target);

      Select(source, target);

      return null;
    }

    protected virtual void Select(IScope source, IScope target) {
      IRandom random = GetVariableValue<IRandom>("Random", source, true);
      int children = GetVariableValue<IntData>("Lambda", source, true).Data;
      int parents = GetVariableValue<IntData>("Rho", source, true).Data;
      int parentsAvailable = source.SubScopes.Count;

      if (parents > parentsAvailable)
        throw new InvalidOperationException("Cannot select more parents per child than there are parents available");

      IList<int> selectedParents = new List<int>(parentsAvailable);
      for (int i = 0; i < children; i++) {
        selectedParents.Clear();
        for (int j = 0; j < parents; j++) {
          int nextParent = j; // will be used in case parents == parentsAvailable
          if (parents < parentsAvailable) {
            do {
              nextParent = random.Next(parentsAvailable);
            } while (selectedParents.Contains(nextParent));
          }

          target.AddSubScope((IScope)source.SubScopes[nextParent].Clone());
          selectedParents.Add(nextParent);
        }
      }
    }
  }
}
