using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Constraints;

namespace HeuristicLab.SimOpt {
  public abstract class SimOptInitializationOperatorBase : OperatorBase {

    public SimOptInitializationOperatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "A (uniform distributed) pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Items", "The parameter vector to initialize", typeof(ConstrainedItemList), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Index", "Which index in the parameter vector to initialize", typeof(IntData), VariableKind.In));
      GetVariableInfo("Index").Local = true;
      AddVariable(new Variable("Index", new IntData(-1)));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      ConstrainedItemList parameterVector = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      IntData index = GetVariableValue<IntData>("Index", scope, false);
      int i = index.Data;
      if (i < 0 || i >= parameterVector.Count) throw new IndexOutOfRangeException("ERROR: Index is out of range of the parameter vector");
      IItem item = parameterVector[i];
      if (item is Variable) {
        item = ((Variable)item).Value;
      }
      Apply(scope, random, item);
      return null;
    }

    protected abstract void Apply(IScope scope, IRandom random, IItem item);

    #region helper functions
    protected bool IsIntegerConstrained(ConstrainedDoubleData data) {
      foreach (IConstraint constraint in data.Constraints) {
        if (constraint is IsIntegerConstraint) {
          return true;
        }
      }
      return false;
    }
    #endregion
  }
}
