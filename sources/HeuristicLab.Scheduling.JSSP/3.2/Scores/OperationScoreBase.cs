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
  public abstract class OperationScoreBase : OperatorBase {
    public OperationScoreBase() : base() {
      AddVariableInfo(new VariableInfo("Schedule", "Partial Schedule", typeof(Schedule), VariableKind.In));
      AddVariableInfo(new VariableInfo("Index", "Index of operation from schedulable set to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Operation", "Schedulable Operation to evaluate", typeof(Operation), VariableKind.In));
      AddVariableInfo(new VariableInfo("Result", "Result of the operator", typeof(DoubleData), VariableKind.Out));
    }

    public abstract double GetOpScore(IScope unscheduled, Operation op, Schedule s);

    public override IOperation Apply(IScope scope) {
      IntData i = this.GetVariableValue<IntData>("Index", scope, false);
      Operation op = null; 
      if((scope.SubScopes.Count == 2) && (scope.SubScopes[1].SubScopes.Count > 0)) {
        op = this.GetVariableValue<Operation>("Operation", scope.SubScopes[1].SubScopes[i.Data], false);
      } else {
        throw new Exception("SubScope structure not valid!");
      }
      Schedule s = this.GetVariableValue<Schedule>("Schedule", scope, false);
      
      GetVariableValue<DoubleData>("Result", scope, true).Data = GetOpScore(scope.SubScopes[0], op, s);

      return null;
    }
  }
}
