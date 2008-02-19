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

namespace HeuristicLab.Evolutionary {
  public class SASEGASAReunificator : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public override IOperation Apply(IScope scope) {
      int subScopes = scope.SubScopes.Count;
      if (subScopes <= 1)
        throw new InvalidOperationException("SASEGASA reunification requires 2 or more sub-scopes");

      // get all sub-sub-scopes
      IList<IScope> subSubScopes = new List<IScope>();
      for (int i = 0; i < scope.SubScopes.Count; i++) {
        while (scope.SubScopes[i].SubScopes.Count > 0) {
          subSubScopes.Add(scope.SubScopes[i].SubScopes[0]);
          scope.SubScopes[i].RemoveSubScope(scope.SubScopes[i].SubScopes[0]);
        }
      }

      // reduce number of sub-scopes by 1 and partition sub-sub-scopes again
      scope.RemoveSubScope(scope.SubScopes[scope.SubScopes.Count - 1]);
      subScopes--;
      int subSubScopesCount = subSubScopes.Count / subScopes;
      for (int i = 0; i < subScopes; i++) {
        for (int j = 0; j < subSubScopesCount; j++) {
          scope.SubScopes[i].AddSubScope(subSubScopes[0]);
          subSubScopes.RemoveAt(0);
        }
      }

      // add remaining sub-sub-scopes to last sub-scope
      while (subSubScopes.Count > 0) {
        scope.SubScopes[scope.SubScopes.Count - 1].AddSubScope(subSubScopes[0]);
        subSubScopes.RemoveAt(0);
      }

      return null;
    }
  }
}
