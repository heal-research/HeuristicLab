using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleMSEEvaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "MSE";
      }
    }
    public override double Evaluate(double[,] values) {
      return Calculate(values);
    }

    public static double Calculate(double[,] values) {
      double sse = 0;
      double cnt = 0;
      for (int i = 0; i < values.GetLength(0); i++) {
        double estimated = values[i, 0];
        double target = values[i, 1];
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(target) && !double.IsInfinity(target)) {
          double error = estimated - target;
          sse += error * error;
          cnt++;
        }
      }

      double mse = sse / cnt;
      return mse;
    }
  }
}
