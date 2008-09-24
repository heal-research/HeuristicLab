using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.SimOpt {
  public class SubScopesRemover : OperatorBase {
    public override IOperation Apply(IScope scope) {
      while (scope.SubScopes.Count > 0) {
        scope.RemoveSubScope(scope.SubScopes[0]);
      }
      return null;
    }
  }
}
