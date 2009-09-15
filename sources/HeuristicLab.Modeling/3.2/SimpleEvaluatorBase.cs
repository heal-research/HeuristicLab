using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public abstract class SimpleEvaluatorBase : OperatorBase {
    protected const int ORIGINAL_INDEX = 0;
    protected const int ESTIMATION_INDEX = 1;

    public virtual string OutputVariableName {
      get { return "Quality"; }
    }
    public SimpleEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "Target vs predicted values", typeof(DoubleMatrixData), VariableKind.In));
      AddVariableInfo(new VariableInfo(OutputVariableName, OutputVariableName, typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      DoubleMatrixData values = GetVariableValue<DoubleMatrixData>("Values", scope, true);
      DoubleData quality = GetVariableValue<DoubleData>(OutputVariableName, scope, false, false);
      if (quality == null) {
        quality = new DoubleData();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(OutputVariableName), quality));
      }

      quality.Data = Evaluate(values.Data);
      return null;
    }

    public abstract double Evaluate(double[,] values);
  }
}
