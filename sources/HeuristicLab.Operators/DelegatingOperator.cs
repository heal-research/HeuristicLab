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

namespace HeuristicLab.Operators {
  public abstract class DelegatingOperator : OperatorBase {
    public override IOperation Execute(IScope scope) {
      myCanceled = false;

      if(scope.GetVariable(Guid.ToString()) == null) { // contained operator not yet executed
        // add marker
        scope.AddVariable(new Variable(Guid.ToString(), new NullData()));

        // add aliases and local variables
        foreach (IVariableInfo variableInfo in VariableInfos) {
          scope.AddAlias(variableInfo.FormalName, variableInfo.ActualName);
          if (variableInfo.Local)
            scope.AddVariable(GetVariable(variableInfo.ActualName));
        }

        CompositeOperation next = new CompositeOperation();
        next.AddOperation(Apply(scope));
        // execute combined operator again after contained operators have been executed
        next.AddOperation(new AtomicOperation(this, scope));

        OnExecuted();
        return next;
      } else {  // contained operator already executed
        // remove marker
        scope.RemoveVariable(Guid.ToString());

        // remove aliases and local variables
        foreach (IVariableInfo variableInfo in VariableInfos) {
          scope.RemoveAlias(variableInfo.FormalName);
          if (variableInfo.Local)
            scope.RemoveVariable(variableInfo.ActualName);
        }

        OnExecuted();
        return null;
      }
    }
  }
}
