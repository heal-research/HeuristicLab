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
using System.Diagnostics;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class LocalProcessInitiator : OperatorBase {

    public override string Description {
      get { return @"This operator will initialize and start a process with the specified configuration."; }
    }
    public LocalProcessInitiator() {
      AddVariableInfo(new VariableInfo("DriverConfiguration", "", typeof(LocalProcessDriverConfiguration), VariableKind.In));
      AddVariableInfo(new VariableInfo("Process", "", typeof(ProcessData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      LocalProcessDriverConfiguration config = GetVariableValue<LocalProcessDriverConfiguration>("DriverConfiguration", scope, true);
      ProcessData proc = new ProcessData();
      proc.Initialize(config);

      IVariableInfo info = GetVariableInfo("Process");
      if (info.Local)
        AddVariable(new Variable(info.ActualName, proc));
      else
        scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), proc));

      return null;
    }
  }
}
