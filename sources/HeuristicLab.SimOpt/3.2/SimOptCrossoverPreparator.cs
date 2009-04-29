using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class SimOptCrossoverPreparator : OperatorBase {
    public override string Description {
      get {
        return @"Prepares the parent parameters for crossing";
      }
    }

    public SimOptCrossoverPreparator()
      : base() {
    }

    public override IOperation Apply(IScope scope) {
      int parents = scope.SubScopes.Count;
      int parameters = scope.SubScopes[0].SubScopes.Count;
      // remove the parents and add them to a temporary list
      IList<IScope> parentsScopes = new List<IScope>();
      while (scope.SubScopes.Count > 0) {
        IScope tmp = scope.SubScopes[0];
        scope.RemoveSubScope(tmp);
        parentsScopes.Add(tmp);
      }

      for (int i = 0; i < parameters; i++) {
        scope.AddSubScope(new Scope("Parameters_" + (i + 1).ToString()));
        for (int k = 0; k < parents; k++) {
          IScope param = parentsScopes[k].SubScopes[0];
          parentsScopes[k].RemoveSubScope(param);
          scope.SubScopes[i].AddSubScope(param);
        }
      }
      return null;
    }
  }
}
