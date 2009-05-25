using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleR2Evaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "R2";
      }
    }

    public override double Evaluate(double[,] values) {
      return Calculate(values);
    }

    public static double Calculate(double[,] values) {
      double targetMean = 0;
      double sse = 0;
      double cnt = 0;
      for (int i = 0; i < values.GetLength(0); i++) {
        double estimated = values[i, 0];
        double target = values[i, 1];
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(target) && !double.IsInfinity(target)) {
          targetMean += target;
          double error = estimated - target;
          sse += error * error;
          cnt++;
        }
      }
      targetMean /= cnt;

      double targetDeviationTotalSumOfSquares = 0;
      for (int i = 0; i < values.GetLength(0); i++) {
        double target = values[i, 1];
        if (!double.IsNaN(target) && !double.IsInfinity(target)) {
          target = target - targetMean;
          target = target * target;
          targetDeviationTotalSumOfSquares += target;
        }
      }
      double quality = 1 - sse / targetDeviationTotalSumOfSquares;
      if (quality > 1)
        throw new InvalidProgramException();

      return quality;
    }
  }
}
