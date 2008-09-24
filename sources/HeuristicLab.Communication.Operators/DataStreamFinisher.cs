using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class DataStreamFinisher : OperatorBase {
    public override string Description {
      get {
        return @"TODO";
      }
    }

    public DataStreamFinisher() {
      AddVariableInfo(new VariableInfo("DataStream", "", typeof(IDataStream), VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      IDataStream datastream = GetVariableValue<IDataStream>("DataStream", scope, true);

      datastream.Close();

      IVariableInfo info = GetVariableInfo("DataStream");
      if (info.Local)
        RemoveVariable(info.ActualName);
      else
        scope.RemoveVariable(scope.TranslateName(info.FormalName));

      return null;
    }
  }
}
