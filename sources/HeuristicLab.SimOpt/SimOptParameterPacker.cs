using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class SimOptParameterPacker : OperatorBase {
    public override string Description {
      get { return @"Updates a ConstrainedItemList with the variables in the scope and removes them afterwards."; }
    }

    public SimOptParameterPacker()
      : base() {
      AddVariableInfo(new VariableInfo("Items", "The ConstrainedItemList to be updated", typeof(ConstrainedItemList), VariableKind.In | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ConstrainedItemList cil = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      for (int i = 0; i < cil.Count; i++) {
        IVariable var = scope.GetVariable(((Variable)cil[i]).Name);
        if (var == null) throw new InvalidOperationException("ERROR in SimOptParameterPacker: Cannot find variable " + ((Variable)cil[i]).Name + " in scope");
        else {
          ((Variable)cil[i]).Value = (IItem)var.Value.Clone();
          scope.RemoveVariable(var.Name);
        }
      }
      return null;
    }
  }
}
