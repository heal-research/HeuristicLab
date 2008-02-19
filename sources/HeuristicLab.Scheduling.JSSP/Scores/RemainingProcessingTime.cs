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

namespace HeuristicLab.Scheduling.JSSP {
  public class RemainingProcessingTime : OperationScoreBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public override double GetOpScore(IScope unscheduled, Operation op, Schedule s) {
      int remainingTime = op.Duration; 
      for(int i = 0; i < unscheduled.SubScopes.Count; i++) { 
        Operation unscheduledOp = this.GetVariableValue<Operation>("Operation", unscheduled.SubScopes[i], false); 
        if (unscheduledOp.Job == op.Job) {
          remainingTime += unscheduledOp.Duration; 
        }
      }
      return remainingTime; 
    }
  }
}
