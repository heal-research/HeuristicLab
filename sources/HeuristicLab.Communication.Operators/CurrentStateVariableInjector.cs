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
  public class CurrentStateVariableInjector : OperatorBase {
    public override string Description {
      get { return @"TODO\r\n add description"; }
    }

    public CurrentStateVariableInjector() {
      AddVariableInfo(new VariableInfo("CurrentState", "The current state when working up a protocol", typeof(ProtocolState), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      ConstrainedItemList data = currentState.SendingData;
      for (int i = 0 ; i < data.Count ; i++) {
        scope.AddVariable((Variable)data[i].Clone(new Dictionary<Guid, object>()));
      }

      return null;
    }
  }
}
