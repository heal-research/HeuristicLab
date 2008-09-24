using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.SimOpt {
  public class SimOptSquentialSubOperatorCrossover : OperatorBase {
    public override string Description {
      get { return @"Takes the parameter vector of two items and on each index applies the respectively indexed suboperator"; }
    }

    public SimOptSquentialSubOperatorCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Items", "The parameter vector", typeof(ConstrainedItemList), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo("Parents", "The number of parents per child", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      int parents = GetVariableValue<IntData>("Parents", scope, true).Data;

      int subScopesCount = scope.SubScopes.Count;
      if (subScopesCount < parents || (subScopesCount % parents) != 0)
        throw new InvalidOperationException("Size of mating pool is not a multiple (>1) of the number of parents per child");
      int children = subScopesCount / parents;

      CompositeOperation co = new CompositeOperation();
      for (int i = 0; i < SubOperators.Count; i++) {
        if (SubOperators[i].GetVariable("Index") != null) {
          SubOperators[i].GetVariable("Index").Value = new IntData(i);
        }
        if (SubOperators[i].GetVariableInfo("Items") != null) {
          SubOperators[i].GetVariableInfo("Items").ActualName = GetVariableInfo("Items").ActualName;
        }
      }
      for (int i = 0; i < children; i++) {
        IScope child = (IScope)scope.SubScopes[0].Clone();
        for (int j = 0; j < parents; j++) {
          IScope parent = scope.SubScopes[0];
          child.AddSubScope(parent);
          scope.RemoveSubScope(parent);
        }
        scope.AddSubScope(child);
        for (int n = 0 ; n < SubOperators.Count ; n++)
          co.AddOperation(new AtomicOperation(SubOperators[n], child));
        co.AddOperation(new AtomicOperation(new Operators.SubScopesRemover(), child));
      }
      return co;
    }
  }
}
