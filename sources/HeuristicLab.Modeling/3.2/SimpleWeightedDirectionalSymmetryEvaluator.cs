using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleWeightedDirectionalSymmetryEvaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "WeightedDirectionalSymmetry";
      }
    }

    public override double Evaluate(double[,] values) {
      return Calculate(values);
    }

    public static double Calculate(double[,] values) {
      double correctSum = 0.0;
      double incorrectSum = 0.0;
      for (int i = 1; i < values.GetLength(0); i++) {
        double prevOriginal = values[i - 1, ORIGINAL_INDEX];
        double original = values[i, ORIGINAL_INDEX];
        double prevEstimated = values[i - 1, ESTIMATION_INDEX];
        double estimated = values[i, ESTIMATION_INDEX];
        if (!(double.IsNaN(original) || double.IsInfinity(original) || double.IsNaN(prevOriginal) || double.IsInfinity(prevOriginal))) {
          if (!(double.IsNaN(estimated) || double.IsInfinity(estimated) || double.IsNaN(prevEstimated) || double.IsInfinity(prevEstimated))) {
            double error = Math.Abs(estimated - original);
            if ((original - prevOriginal) * (estimated - prevEstimated) >= 0) correctSum += error;
            else incorrectSum += error;
          }
        }
      }
      return incorrectSum / correctSum;
    }
  }
}
