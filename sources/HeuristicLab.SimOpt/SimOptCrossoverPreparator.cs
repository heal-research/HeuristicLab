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
      AddVariableInfo(new VariableInfo("Parents", "Number of parents per child", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      int parents = GetVariableValue<IntData>("Parents", scope, true).Data;
      int populationSize = scope.SubScopes.Count;
      int childrenSize = populationSize / parents;
      if (populationSize % parents > 0) throw new ArgumentException("ERROR in SimOptCrossoverPreparator: The number of subscopes is not a multiple of the number of parents per child");
      for (int i = 0; i < childrenSize; i++) {
        IScope child = new Scope(i.ToString());
        int parameters = scope.SubScopes[0].SubScopes.Count;
        for (int k = 0; k < parameters; k++) {
          child.AddSubScope(new Scope("Parameter_" + (k+1).ToString()));
          for (int j = 0; j < parents; j++) {
            IScope param = scope.SubScopes[j].SubScopes[0]; // take scope containing the parameter from the parent
            child.SubScopes[k].AddSubScope(param); // add it to the child
            scope.SubScopes[j].RemoveSubScope(param);
          }
        }
        for (int j = 0; j < parents; j++)
          scope.RemoveSubScope(scope.SubScopes[0]); // remove the parent
        scope.SubScopes.Add(child); // add the child to the end of the scope list
      }
      return null;
    }
  }
}
