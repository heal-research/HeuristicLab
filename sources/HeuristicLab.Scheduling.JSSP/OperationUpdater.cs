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
using System.Collections; 

namespace HeuristicLab.Scheduling.JSSP {
  public class OperationUpdater : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public OperationUpdater()
      : base() {
      AddVariableInfo(new VariableInfo("Operation", "Scheduled operation and unscheduled operation which shall be updated.", typeof(Operation), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      Operation scheduledOp = GetVariableValue<Operation>("Operation", scope, false);
      for(int i = 0; i < scope.SubScopes.Count; i++) {
        Operation op = GetVariableValue<Operation>("Operation", scope.SubScopes[i], false);
        if((op.Job == scheduledOp.Job) && (op.Predecessors != null)) {
          int index = -1; 
          for(int j = 0; j < op.Predecessors.Count; j++) {
            if(((IntData)op.Predecessors[j]).Data == scheduledOp.OperationIndex) {
              index = j; 
            }
          }
          if(index != -1) {
            op.Predecessors.RemoveAt(index); // remove scheduled op from predecessor list
            op.Start = scheduledOp.Start + scheduledOp.Duration; // new earliest start date
          }
        }
      }
      return null; 
    }
  }
}
