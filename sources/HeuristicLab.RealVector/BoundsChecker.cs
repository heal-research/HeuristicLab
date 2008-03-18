using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  class BoundsChecker : OperatorBase {
    public override string Description {
      get { return "Checks if all elements of a real vector are inside a given minimum and maximum value. If not, elements are corrected."; }
    }

    public BoundsChecker()
      : base() {
      AddVariableInfo(new VariableInfo("RealVector", "Real vector to check", typeof(DoubleArrayData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Minimum", "Minimum value of each vector element (included).", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum value of each vector element (included).", typeof(DoubleData), VariableKind.In));
    }

    public static double[] Apply(double min, double max, double[] vector) {
      int length = vector.Length;
      double[] result = (double[])vector.Clone();

      for (int i = 0; i < length; i++) {
        if (result[i] < min) result[i] = min;
        if (result[i] > max) result[i] = max;
      }
      return result;
    }

    public override IOperation Apply(IScope scope) {
      DoubleArrayData vector = GetVariableValue<DoubleArrayData>("RealVector", scope, false);
      double min = GetVariableValue<DoubleData>("Minimum", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Maximum", scope, true).Data;
      vector.Data = Apply(min, max, vector.Data);
      return null;
    }
  }
}
