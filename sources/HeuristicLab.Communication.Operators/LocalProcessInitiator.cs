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
