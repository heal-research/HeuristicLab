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
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class CurrentStateConstraintChecker : OperatorBase {
    public override string Description {
      get {
        return @"TODO";
      }
    }

    public CurrentStateConstraintChecker() {
      AddVariableInfo(new VariableInfo("CurrentState", "", typeof(ProtocolState), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      ConstrainedItemList sendingData = (ConstrainedItemList)currentState.SendingData.Clone();
      sendingData.BeginCombinedOperation();
      for (int i = 0; i < sendingData.Count; i++) {
        Variable tmp = (Variable)sendingData[i];
        tmp.Value = scope.GetVariableValue(tmp.Name, false);
      }
      ICollection<IConstraint> violatedConstraints;
      bool result = sendingData.EndCombinedOperation(out violatedConstraints);
      if (result) return null;
      else return new AtomicOperation(SubOperators[0], scope);
    }
  }
}
