using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleMSEEvaluator : OperatorBase {

    public SimpleMSEEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "Target vs predicted values", typeof(DoubleMatrixData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MSE", "Mean squarred error", typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      DoubleMatrixData values = GetVariableValue<DoubleMatrixData>("Values", scope, true);
      double sse = 0;
      double cnt = 0;
      for (int i = 0; i < values.Data.GetLength(0); i++) {
        double estimated = values.Data[i, 0];
        double target = values.Data[i, 1];
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(target) && !double.IsInfinity(target)) {
          double error = estimated - target;
          sse += error * error;
          cnt++;
        }
      }

      double mse = sse / cnt;
      scope.AddVariable(new Variable(scope.TranslateName("MSE"), new DoubleData(mse)));
      return null;
    }
  }
}
