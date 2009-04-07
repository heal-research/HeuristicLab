using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class SubScopeMessageDecomposer : OperatorBase {
    public override string Description {
      get {
        return @"Takes a message and extracts the contents into subscopes";
      }
    }

    public SubScopeMessageDecomposer()
      : base() {
      AddVariableInfo(new VariableInfo("Message", "The message to decompose", typeof(Message), VariableKind.In | VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      Message message = GetVariableValue<Message>("Message", scope, false);
      if (message.Give.Count > scope.SubScopes.Count) throw new InvalidOperationException("ERROR in SubScopeMessageDecomposer: There are not enough subscopes to put the contents of the message into");
      int i = 0;
      foreach (IVariable var in message.Give) {
        scope.SubScopes[i].AddVariable(var);
        i++;
      }
      IVariableInfo info = GetVariableInfo("Message");
      if (info.Local) {
        RemoveVariable(info.ActualName);
      } else {
        scope.RemoveVariable(scope.TranslateName(info.FormalName));
      }
      return null;
    }
  }
}
