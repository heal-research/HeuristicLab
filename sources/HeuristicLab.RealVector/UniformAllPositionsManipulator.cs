using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  class UniformAllPositionsManipulator : RealVectorManipulatorBase {
    public override string Description {
      get { return "Uniformly distributed change of all positions of a real vector."; }
    }

    public UniformAllPositionsManipulator() {
      AddVariableInfo(new VariableInfo("Minimum", "Minimum of the sampling range for the vector element (included)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum of the sampling range for the vector element (excluded)", typeof(DoubleData), VariableKind.In));
    }

    public static double[] Apply(IRandom random, double[] vector, double min, double max) {
      double[] result = (double[])vector.Clone();

      for (int i = 0; i < result.Length; i++) {
        result[i] = min + random.NextDouble() * (max - min);
      }

      return result;
    }

    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double min = GetVariableValue<DoubleData>("Minimum", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Maximum", scope, true).Data;
      return Apply(random, vector, min, max);
    }
  }
}
