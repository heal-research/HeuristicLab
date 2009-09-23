using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling {
  public class SimpleNMSEEvaluator : SimpleEvaluatorBase {

    public override string OutputVariableName {
      get {
        return "NMSE";
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
      double mse = SimpleMSEEvaluator.Calculate(values);
      double mean = Statistics.Mean(Matrix<double>.GetColumn(values, ORIGINAL_INDEX));
      double ssd = 0;
      int n = 0;
      for (int i = 0; i < values.GetLength(0); i++) {
        double original = values[i, ORIGINAL_INDEX];
        if (!(double.IsNaN(original) || double.IsInfinity(original))) {
          double dev = original - mean;
          ssd += dev * dev;
          n++;
        }
      }
      double variance = ssd / (n - 1);
      return mse / variance;
    }
  }
}
