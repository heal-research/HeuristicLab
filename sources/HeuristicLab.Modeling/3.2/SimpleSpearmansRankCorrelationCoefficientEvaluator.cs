using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleSpearmansRankCorrelationCoefficientEvaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "SpearmansRankCorrelationCoefficient";
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
      return alglib.correlation.spearmanrankcorrelation(
        Matrix<double>.GetColumn(values, ESTIMATION_INDEX),
        Matrix<double>.GetColumn(values, ORIGINAL_INDEX),
        values.GetLength(0));
    }
  }
}
