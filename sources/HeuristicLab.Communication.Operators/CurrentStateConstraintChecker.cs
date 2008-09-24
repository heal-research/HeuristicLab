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
