using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  class AverageCrossover : RealVectorCrossoverBase {
    public override string Description {
      get { return "Average (Continuous) crossover for real vectors."; }
    }

    public static double[] Apply(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      int length = parent1.Length;
      double[] result = new double[length];
      double min = scope.GetVariableValue<DoubleData>("Minimum", true).Data;
      double max = scope.GetVariableValue<DoubleData>("Maximum", true).Data;

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < 0.5) {
          result[i] = (parent1[i] + parent2[i]) / 2;
        } else {
          result[i] = parent1[i];
        }

        // check borders
        if (result[i] < min) { result[i] = min; }
        if (result[i] > max) { result[i] = max; }
      }

      return result;
    }

    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      return Apply(scope, random, parent1, parent2);
    }
  }
}
