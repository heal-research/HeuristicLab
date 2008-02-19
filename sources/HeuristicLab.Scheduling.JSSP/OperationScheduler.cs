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
using System.Xml; 
using HeuristicLab.Core;
using HeuristicLab.Data; 

namespace HeuristicLab.Scheduling.JSSP {
  public class OperationScheduler : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public OperationScheduler()
      : base() {
      AddVariableInfo(new VariableInfo("PartialSchedule", "(Partial) schedule", typeof(Schedule), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Operation", "Operation to be scheduled. Start date is updated after scheduling.", typeof(Operation), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      Schedule pSchedule = GetVariableValue<Schedule>("PartialSchedule", scope, true);
      Operation op = GetVariableValue<Operation>("Operation", scope, true);
      int operationStartDate = pSchedule.ScheduleOperation(op);
      op.Start = operationStartDate; 
      return null;
    }
  }
}
