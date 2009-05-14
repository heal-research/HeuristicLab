using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.SupportVectorMachines {
  public class SimpleMSEEvaluator : OperatorBase{

    public SimpleMSEEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "Target vs predicted values", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("MSE", "Mean squarred error", typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ItemList values = GetVariableValue<ItemList>("Values", scope, true);
      double target;
      double estimated;
      double error;
      double sse = 0;
      double cnt = 0;
      foreach (ItemList row in values) {
        estimated = ((DoubleData)row[0]).Data;
        target = ((DoubleData)row[1]).Data;
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(target) && !double.IsInfinity(target)) {
          error = estimated - target;
          sse += error * error;
          cnt++;
        }
      }

      double mse = sse / cnt;
      scope.AddVariable(new Variable(scope.TranslateName("MSE"),new DoubleData(mse)));
      return null;
    }
  }
}
