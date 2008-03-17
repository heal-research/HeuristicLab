using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.RealVector {
  class HeuristicCrossover : CrossoverBase {
    public HeuristicCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
    }

    public override string Description {
      get { return "Heuristic crossover for real vectors."; }
    }

    public static double[] Apply(IScope scope, IRandom random, double[] parent1, double[] parent2, double quality1, double quality2) {
      double factor;
      int length = parent1.Length;
      double[] result = new double[length];
      bool isMaximization = scope.GetVariableValue<BoolData>("Maximization", true).Data;
      double min = scope.GetVariableValue<DoubleData>("Minimum", true).Data;
      double max = scope.GetVariableValue<DoubleData>("Maximum", true).Data;

      factor = random.NextDouble();

      for (int i = 0; i < length; i++) {
        if (isMaximization) {
          // maximization problem
          if (quality1 > quality2) {
            result[i] = parent1[i] + factor * (parent1[i] - parent2[i]);
          } else {
            result[i] = parent2[i] + factor * (parent2[i] - parent1[i]);
          } // if
        } else {
          // minimization problem
          if (quality1 < quality2) {
            result[i] = parent1[i] + factor * (parent1[i] - parent2[i]);
          } else {
            result[i] = parent2[i] + factor * (parent2[i] - parent1[i]);
          } // if
        } // if

        // check borders
        if (result[i] < min) { result[i] = min; }
        if (result[i] > max) { result[i] = max; }
      } // for

      return result;
    }

    protected sealed override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      IVariableInfo realVectorInfo = GetVariableInfo("RealVector");
      IVariableInfo qualityInfo = GetVariableInfo("Quality");
      DoubleArrayData vector1 = parent1.GetVariableValue<DoubleArrayData>(realVectorInfo.ActualName, false);
      DoubleArrayData vector2 = parent2.GetVariableValue<DoubleArrayData>(realVectorInfo.ActualName, false);
      DoubleData quality1 = parent1.GetVariableValue<DoubleData>(qualityInfo.ActualName, false);
      DoubleData quality2 = parent2.GetVariableValue<DoubleData>(qualityInfo.ActualName, false);

      if (vector1.Data.Length != vector2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to real vectors of different length.");

      double[] result = Apply(scope, random, vector1.Data, vector2.Data, quality1.Data, quality2.Data);
      child.AddVariable(new Variable(realVectorInfo.ActualName, new DoubleArrayData(result)));
    }
  }
}
