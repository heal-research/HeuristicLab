using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  class RandomConvexCrossover : RealVectorCrossoverBase {
    public override string Description {
      get { return "Random convex crossover for real vectors."; }
    }

    public static double[] Apply(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      double factor;
      int length = parent1.Length;
      double[] result = new double[length];
      double min = scope.GetVariableValue<DoubleData>("Minimum", true).Data;
      double max = scope.GetVariableValue<DoubleData>("Maximum", true).Data;

      factor = random.NextDouble();

      for (int i = 0; i < length; i++) {
        result[i] = (factor * parent1[i]) + ((1 - factor) * parent2[i]);

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
