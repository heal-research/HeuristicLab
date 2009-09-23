using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleDirectionalSymmetryEvaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "DirectionalSymmetry";
      }
    }

    public override double Evaluate(double[,] values) {
      return Calculate(values);
    }

    public static double Calculate(double[,] values) {
      int n = 0;
      int nCorrect = 0;
      for (int i = 1; i < values.GetLength(0); i++) {
        double prevOriginal = values[i - 1, ORIGINAL_INDEX];
        double original = values[i, ORIGINAL_INDEX];
        double prevEstimated = values[i - 1, ESTIMATION_INDEX];
        double estimated = values[i, ESTIMATION_INDEX];

        if (!(double.IsNaN(original) || double.IsInfinity(original) || double.IsNaN(prevOriginal) || double.IsInfinity(prevOriginal))) {
          n++;
          if (!(double.IsNaN(estimated) || double.IsInfinity(estimated) || double.IsNaN(prevEstimated) || double.IsInfinity(prevEstimated))) {
            if ((original - prevOriginal) * (estimated - prevEstimated) >= 0) nCorrect++;
          }
        }
      }
      return (double)nCorrect / (double)n * 100.0;
    }
  }
}
