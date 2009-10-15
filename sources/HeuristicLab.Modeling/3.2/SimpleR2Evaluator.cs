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
      try {
        return Calculate(values);
      }
      catch (ArgumentException) {
        return double.NegativeInfinity;
      }
    }

    public static double Calculate(double[,] values) {
      double targetMean = 0;
      double sse = 0;
      double cnt = 0;
      for (int i = 0; i < values.GetLength(0); i++) {
        double estimated = values[i, ESTIMATION_INDEX];
        double target = values[i, ORIGINAL_INDEX];
        if (!double.IsNaN(estimated) && !double.IsInfinity(estimated) &&
            !double.IsNaN(target) && !double.IsInfinity(target)) {
          targetMean += target;
          double error = estimated - target;
          sse += error * error;
          cnt++;
        }
      }

      if (cnt > 0) {
        targetMean /= cnt;

        double targetDeviationTotalSumOfSquares = 0;
        for (int i = 0; i < values.GetLength(0); i++) {
          double target = values[i, ORIGINAL_INDEX];
          if (!double.IsNaN(target) && !double.IsInfinity(target)) {
            double targetDiff = target - targetMean;
            targetDeviationTotalSumOfSquares += targetDiff * targetDiff;
          }
        }
        double quality = 1 - sse / targetDeviationTotalSumOfSquares;
        if (quality > 1)
          throw new InvalidProgramException();

        return quality;
      } else {
        throw new ArgumentException("Coefficient of determination is not defined for input vectors of NaN or Inf");
      }
    }
  }
}
