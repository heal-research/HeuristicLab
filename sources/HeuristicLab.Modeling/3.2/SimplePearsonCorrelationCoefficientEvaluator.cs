using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimplePearsonCorrelationCoefficientEvaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "R2";
      }
    }

    public override double Evaluate(double[,] values) {
      return Calculate(values);
    }

    public static double Calculate(double[,] values) {
      double[] estimated = Matrix<double>.GetColumn(values, ESTIMATION_INDEX);
      double[] original = Matrix<double>.GetColumn(values, ORIGINAL_INDEX);
      double r = alglib.correlation.pearsoncorrelation(
        ref estimated,
        ref original,
        values.GetLength(0));
      return r * r;
    }
  }
}
