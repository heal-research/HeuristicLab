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

namespace HeuristicLab.Scheduling.JSSP {
  public class IsSchedulable : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public IsSchedulable()
      : base() {
      AddVariableInfo(new VariableInfo("Operation", "One operation of JSSP job", typeof(Operation), VariableKind.In));
      AddVariableInfo(new VariableInfo("Schedulable", "Is the operation schedulable?", typeof(BoolData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      Operation op = GetVariableValue<Operation>("Operation", scope, true);
      if(scope.GetVariable(GetVariableInfo("Schedulable").ActualName) != null) {
        BoolData isSchedulable = GetVariableValue<BoolData>("Schedulable", scope, false);
        isSchedulable.Data = (op.Predecessors.Count == 0);
      } else {
        scope.AddVariable(new Variable(GetVariableInfo("Schedulable").ActualName, new BoolData((op.Predecessors.Count == 0))));
      }
      return null;
    }

  }
}
