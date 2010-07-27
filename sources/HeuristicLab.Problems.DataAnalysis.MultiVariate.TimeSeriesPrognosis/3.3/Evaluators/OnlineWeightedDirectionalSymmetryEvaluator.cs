using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis {
  public class OnlineWeightedDirectionalSymmetryEvaluator : IOnlineTimeSeriesPrognosisEvaluator {
    private double prevEstimated;
    private double prevOriginal;
    private double incorrectSum;
    private double correctSum;

    public double WeightedDirectionalSymmetry {
      get {
        return incorrectSum / correctSum;
      }
    }

    public OnlineWeightedDirectionalSymmetryEvaluator() {
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return WeightedDirectionalSymmetry; }
    }

    public void Add(double original, double estimated) {
      if (double.IsInfinity(original) || double.IsNaN(original) || double.IsInfinity(estimated) || double.IsNaN(estimated)) {
        throw new ArgumentException("Weighted directional symmetry is not defined for series containing NaN or infinity values.");
      }

      // not the first element and a valid estimated value
      if (!double.IsNaN(prevOriginal)) {
        double error = Math.Abs(estimated - original);
        if ((original - prevOriginal) * (estimated - prevEstimated) >= 0.0)
          correctSum += error;
        else
          incorrectSum += error;
      }
    }

    public void Reset() {
      correctSum = 0;
      incorrectSum = 0;
      prevOriginal = double.NaN;
      prevEstimated = double.NaN;
    }

    #endregion

    #region IOnlineTimeSeriesPrognosisEvaluator Members

    public void StartNewPredictionWindow(double referenceOriginalValue) {
      prevOriginal = referenceOriginalValue;
      prevEstimated = referenceOriginalValue;
    }

    #endregion
  }
}
