using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  class RandomLocalCrossover : RealVectorCrossoverBase {
    public override string Description {
      get { return "Random local crossover for real vectors."; }
    }

    public static double[] Apply(IRandom random, double[] parent1, double[] parent2) {
      double factor;
      int length = parent1.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        factor = random.NextDouble();
        result[i] = (factor * parent1[i]) + ((1 - factor) * parent2[i]);
      }
      return result;
    }

    protected override double[] Cross(IScope scope, IRandom random, double[] parent1, double[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
