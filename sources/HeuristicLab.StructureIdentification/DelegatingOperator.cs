using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.StructureIdentification {
  public abstract class DelegatingOperator : OperatorBase {
    public override IOperation Execute(IScope scope) {
      myCanceled = false;

      // copied from CombinedOperator (gkronber 22.3.08)
      if(scope.GetVariable(Guid.ToString()) == null) { // contained operator not yet executed
        // add marker
        scope.AddVariable(new Variable(Guid.ToString(), new NullData()));

        // add aliases
        foreach(IVariableInfo variableInfo in VariableInfos)
          scope.AddAlias(variableInfo.FormalName, variableInfo.ActualName);

        CompositeOperation next = new CompositeOperation();
        next.AddOperation(Apply(scope));
        // execute combined operator again after contained operators have been executed
        next.AddOperation(new AtomicOperation(this, scope));

        OnExecuted();
        return next;
      } else {  // contained operator already executed
        // remove marker
        scope.RemoveVariable(Guid.ToString());

        // remove aliases
        foreach(IVariableInfo variableInfo in VariableInfos)
          scope.RemoveAlias(variableInfo.FormalName);

        OnExecuted();
        return null;
      }
    }
  }
}
