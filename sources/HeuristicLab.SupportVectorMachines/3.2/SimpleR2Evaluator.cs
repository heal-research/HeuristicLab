using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.SupportVectorMachines {
  public class SimpleR2Evaluator : OperatorBase{

    public SimpleR2Evaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "Target vs predicted values", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("R2", "Coefficient of determination", typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ItemList values = GetVariableValue<ItemList>("Values", scope, true);

      double targetMean = 0;
      double sse = 0;
      double cnt = 0;
      foreach (ItemList row in values) {                                
        double estimated = ((DoubleData)row[0]).Data;
        double target = ((DoubleData)row[1]).Data;
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(target) && !double.IsInfinity(target)) {
          targetMean += target;
          double error = estimated - target;
          sse += error * error;
          cnt++;
        }
      }
      targetMean /= cnt;

      double targetDeviationTotalSumOfSquares = 0;
      foreach (ItemList row in values) {
        double target = ((DoubleData)row[1]).Data;
        if (!double.IsNaN(target) && !double.IsInfinity(target)) {
          target = target - targetMean;
          target = target * target;
          targetDeviationTotalSumOfSquares += target;
        }
      }
      double quality = 1 - sse / targetDeviationTotalSumOfSquares;
      if (quality > 1)
        throw new InvalidProgramException();

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("R2"), new DoubleData(quality)));
      return null;
    }
  }
}
