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
  public class UnidirectionalRingMigrator : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public override IOperation Apply(IScope scope) {
      IList<IScope> emigrantsList = new List<IScope>();

      for (int i = 0; i < scope.SubScopes.Count; i++) {
        IScope emigrants = scope.SubScopes[i].SubScopes[1];
        scope.SubScopes[i].RemoveSubScope(emigrants);
        emigrantsList.Add(emigrants);
      }

      // shift first emigrants to end of list
      emigrantsList.Add(emigrantsList[0]);
      emigrantsList.RemoveAt(0);

      for (int i = 0; i < scope.SubScopes.Count; i++)
        scope.SubScopes[i].AddSubScope(emigrantsList[i]);

      return null;
    }
  }
}
