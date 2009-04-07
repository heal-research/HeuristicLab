using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class SubScopeMessageComposer : OperatorBase {
    public override string Description {
      get {
        return @"Takes a message skeleton and fills it with variables from the subscope";
      }
    }

    public SubScopeMessageComposer()
      : base() {
      AddVariableInfo(new VariableInfo("Message", "The message to fill", typeof(Message), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      Message message = GetVariableValue<Message>("Message", scope, false);
      foreach (IVariable var in message.Give) {
        int done = 0;
        foreach (IScope subscope in scope.SubScopes) {
          IItem item = subscope.GetVariableValue(var.Name, false, false);
          if (item != null) {
            done = 1;
            if (var.Value.GetType().Equals(item.GetType())) {
              var.Value = (IItem)item.Clone();
              done = 2;
            }
          }
        }
        if (done == 0) throw new InvalidOperationException("ERROR in SubScopeMessageComposer: Parameter not found");
        if (done == 1) throw new InvalidOperationException("ERROR in SubScopeMessageComposer: Parameter did not have the right type");
      }
      return null;
    }
  }
}
