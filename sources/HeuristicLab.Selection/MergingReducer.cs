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
using HeuristicLab.Operators;

namespace HeuristicLab.Selection {
  public class MergingReducer : ReducerBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    protected override ICollection<IScope> Reduce(IScope scope) {
      List<IScope> subScopes = new List<IScope>();

      for (int i = 0; i < scope.SubScopes.Count; i++) {
        for (int j = 0; j < scope.SubScopes[i].SubScopes.Count; j++)
          subScopes.Add(scope.SubScopes[i].SubScopes[j]);
      }
      return subScopes;
    }
  }
}
