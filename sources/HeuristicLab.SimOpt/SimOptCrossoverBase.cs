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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public abstract class SimOptCrossoverBase : OperatorBase {
    public SimOptCrossoverBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "The PRNG to use", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Items", "Paramter vector", typeof(ConstrainedItemList), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Index", "Which parameter to cross", typeof(IntData), VariableKind.In));
      GetVariableInfo("Index").Local = true;
      AddVariable(new Variable("Index", new IntData(-1)));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      ConstrainedItemList childVector = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      int index = GetVariableValue<IntData>("Index", scope, false).Data;
      if (index < 0 || index >= childVector.Count) throw new InvalidOperationException("ERROR in SimOptCrossover: Index is out of range of the parameter vector.");

      for (int i = 1; i < scope.SubScopes.Count; i++) {
        object parent1, parent2;
        if (i == 1) {
          IItem p1 = scope.SubScopes[0].GetVariableValue<ConstrainedItemList>("Items", false)[index];
          if (p1 is IVariable)
            parent1 = ((((IVariable)p1).Value as IObjectData).Data as ICloneable).Clone();
          else
            parent1 = ((p1 as IObjectData).Data as ICloneable).Clone();
        } else {
          IItem p1 = childVector[index];
          if (p1 is IVariable)
            parent1 = (((IVariable)p1).Value as IObjectData).Data;
          else
            parent1 = ((IObjectData)p1).Data;
        }
        IItem p2 = scope.SubScopes[i].GetVariableValue<ConstrainedItemList>("Items", false)[index];
        if (p2 is IVariable)
          parent2 = (((IVariable)p2).Value as IObjectData).Data;
        else
          parent2 = ((IObjectData)p2).Data;
        
        object child = Cross(random, parent1, parent2, scope);

        IItem parameter = childVector[index];
        if (parameter is IVariable)
          (((IVariable)parameter).Value as IObjectData).Data = child;
        else {
          ((IObjectData)parameter).Data = child;
        }
      }
      return null;
    }

    protected abstract object Cross(IRandom random, object parent1, object parent2, IScope scope);
  }
}
