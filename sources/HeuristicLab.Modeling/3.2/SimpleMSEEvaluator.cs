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
      try {
        return Calculate(values);
      }
      catch (ArgumentException) {
        return double.PositiveInfinity;
      }
    }

    public static double Calculate(double[,] values) {
      double sse = 0;
      double cnt = 0;
      for (int i = 0; i < values.GetLength(0); i++) {
        double estimated = values[i, ESTIMATION_INDEX];
        double target = values[i, ORIGINAL_INDEX];
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(target) && !double.IsInfinity(target)) {
          double error = estimated - target;
          sse += error * error;
          cnt++;
        }
      }
      if (cnt > 0) {
        double mse = sse / cnt;
        return mse;
      } else {
        throw new ArgumentException("Mean squared errors is not defined for input vectors of NaN or Inf");
      }
    }
  }
}
