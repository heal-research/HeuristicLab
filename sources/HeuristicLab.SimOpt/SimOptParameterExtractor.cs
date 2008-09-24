using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class SimOptParameterExtractor : OperatorBase {
    public override string Description {
      get { return @"Injects the contents of a ConstrainedItemList into the scope"; }
    }

    public SimOptParameterExtractor()
      : base() {
      AddVariableInfo(new VariableInfo("Items", "The ConstrainedItemList to be extracted", typeof(ConstrainedItemList), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      ConstrainedItemList cil = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      for (int i = 0; i < cil.Count; i++) {
        IVariable var = scope.GetVariable(((Variable)cil[i]).Name);
        if (var == null) scope.AddVariable((IVariable)cil[i].Clone());
        else var.Value = (IItem)((Variable)cil[i]).Value.Clone();
      }
      return null;
    }
  }
}
