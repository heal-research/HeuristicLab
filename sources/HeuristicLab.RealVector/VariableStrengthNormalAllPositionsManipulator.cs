using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.RealVector {
  public class VariableStrengthNormalAllPositionsManipulator : RealVectorManipulatorBase {
    public override string Description {
      get { return @"Adds a N(0, 1*factor) distributed random variable to each dimension in the real vector"; }
    }

    public VariableStrengthNormalAllPositionsManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("ShakingFactor", "The factor determining the strength of the mutation", typeof(DoubleData), VariableKind.In));
    }

    public static double[] Apply(double shakingFactor, IRandom random, double[] vector) {
      NormalDistributedRandom N = new NormalDistributedRandom(random, 0.0, 1.0 * shakingFactor);
      for (int i = 0; i < vector.Length; i++) {
        vector[i] = vector[i] + N.NextDouble();
      }
      return vector;
    }

    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double shakingFactor = scope.GetVariableValue<DoubleData>("ShakingFactor", true).Data;
      return Apply(shakingFactor, random, vector);
    }
  }
}
