using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleStableCorrelationCoefficientEvaluator : SimpleEvaluatorBase {

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
      double sum_sq_x = 0.0;
      double sum_sq_y = 0.0;
      double sum_coproduct = 0.0;
      if (IsInvalidValue(values[0, ORIGINAL_INDEX]) || IsInvalidValue(values[0, ESTIMATION_INDEX])) {
        throw new ArgumentException("Correlation coefficient is not defined for variables with NaN or infinity values.");
      }
      double mean_x = values[0, ORIGINAL_INDEX];
      double mean_y = values[0, ESTIMATION_INDEX];
      for (int i = 1; i < values.GetLength(0); i++) {
        double sweep = (i - 1.0) / i;
        if (IsInvalidValue(values[i, ORIGINAL_INDEX]) || IsInvalidValue(values[i, ESTIMATION_INDEX])) {
          throw new ArgumentException("Correlation coefficient is not defined for variables with NaN or infinity values.");
        }
        double delta_x = values[i, ORIGINAL_INDEX] - mean_x;
        double delta_y = values[i, ESTIMATION_INDEX] - mean_y;
        sum_sq_x += delta_x * delta_x * sweep;
        sum_sq_y += delta_y * delta_y * sweep;
        sum_coproduct += delta_x * delta_y * sweep;
        mean_x += delta_x / i;
        mean_y += delta_y / i;
      }
      double pop_sd_x = Math.Sqrt(sum_sq_x / values.GetLength(0));
      double pop_sd_y = Math.Sqrt(sum_sq_y / values.GetLength(0));
      double cov_x_y = sum_coproduct / values.GetLength(0);
      double r = cov_x_y / (pop_sd_x * pop_sd_y);
      return r * r;
    }

    private static bool IsInvalidValue(double d) {
      return double.IsNaN(d) || double.IsInfinity(d);
    }
  }
}
