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
using HeuristicLab.Evolutionary;

namespace HeuristicLab.SimOpt {
  public class SimOptDiscreteMultiCrossover : CrossoverBase {

    public override string Description {
      get { return @"This operator applies a discrete recombination on the variables defined"; }
    }

    public SimOptDiscreteMultiCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Item", "The item list to be recombined", typeof(ConstrainedItemList), VariableKind.In));
    }

    protected override void Cross(IScope scope, IRandom random) {
      ICollection<IConstraint> violated;

      ConstrainedItemList[] p = new ConstrainedItemList[scope.SubScopes.Count];
      for (int i = 0; i < p.Length; i++) {
        p[i] = scope.SubScopes[i].GetVariableValue<ConstrainedItemList>("Item", false);
        if (i > 0 && p[i].Count != p[i-1].Count) throw new InvalidOperationException("ERROR: the lists do not contain the same number of items");
      }

      ConstrainedItemList childList = (ConstrainedItemList)p[0].Clone();
      if (childList.Count > 1) {
        int iter = 0;
        do {
          childList.BeginCombinedOperation();
          for (int i = 0; i < childList.Count; i++) {
            int nextParent = random.Next(0, scope.SubScopes.Count);
            if (nextParent > 0) childList.TrySetAt(i, (IItem)p[nextParent].Clone(), out violated);
          }
        } while (!childList.EndCombinedOperation(out violated) && ++iter < 100);
        if (violated.Count == 0) {
          scope.AddVariable(new Variable(scope.SubScopes[0].TranslateName("Item"), childList));
        }
      }
    }
  }
}