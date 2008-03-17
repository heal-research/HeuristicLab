using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  class ContinuousCrossover : RealVectorCrossoverBase {
    public override string Description {
      get { return "Continuous crossover for real vectors."; }
    }

    public static double[] Apply(IRandom random, double[] parent1, double[] parent2) {
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < 0.5) {
          result[i] = (parent1[i] + parent2[i]) / 2;
        } else {
          result[i] = parent1[i];
        }
      }
      return result;
    }

    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
