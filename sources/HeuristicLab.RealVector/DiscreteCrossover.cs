using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.RealVector {
  class DiscreteCrossover : RealVectorCrossoverBase {
    public override string Description {
      get { return "Discrete crossover for real vectors."; }
    }

    public static double[] Apply(IRandom random, double[] parent1, double[] parent2) {
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < 0.5) {
          result[i] = parent1[i];
        } else {
          result[i] = parent2[i];
        }
      }

      return result;
    }

    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
