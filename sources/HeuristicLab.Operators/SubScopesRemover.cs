using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  public class SubScopesRemover : OperatorBase {
    public override string Description {
      get {
        return @"This operator either removes the subscope specified in the variable SubScopeIndex or if this variable is absent, removes all subscopes.";
      }
    }

    public SubScopesRemover()
      : base() {
      AddVariableInfo(new VariableInfo("SubScopeIndex", "(Optional) the index of the subscope to remove", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IntData index = GetVariableValue<IntData>("SubScopeIndex", scope, true, false);
      if (index == null) { // remove all scopes
        while (scope.SubScopes.Count > 0) {
          scope.RemoveSubScope(scope.SubScopes[0]);
        }
      } else {
        if (index.Data < 0 && index.Data >= scope.SubScopes.Count) throw new InvalidOperationException("ERROR: no scope with index " + index.Data + " exists");
        scope.RemoveSubScope(scope.SubScopes[index.Data]);
      }
      return null;
    }
  }
}
