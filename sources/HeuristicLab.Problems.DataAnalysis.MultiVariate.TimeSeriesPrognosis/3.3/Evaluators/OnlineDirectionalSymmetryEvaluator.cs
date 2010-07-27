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
  public class OnlineDirectionalSymmetryEvaluator : IOnlineTimeSeriesPrognosisEvaluator {
    private double prevEstimated;
    private double prevOriginal;
    private int n;
    private int nCorrect;

    public double DirectionalSymmetry {
      get {
        if (n < 1) return 0.0;
        return (double)nCorrect / n * 100.0;
      }
    }

    public OnlineDirectionalSymmetryEvaluator() {
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return DirectionalSymmetry; }
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(original) || double.IsInfinity(original) || double.IsNaN(estimated) || double.IsInfinity(estimated)) {
        throw new ArgumentException("Directional symmetry is not defined for series containing NaN or infinity values.");
      }
      if (!double.IsNaN(prevOriginal)) {
        if ((original - prevOriginal) * (estimated - prevEstimated) >= 0.0) {
          nCorrect++;
        } else {
          n++;
        }
      }
    }

    public void Reset() {
      n = 0;
      nCorrect = 0;
      prevOriginal = double.NaN;
      prevEstimated = double.NaN;
    }

    #endregion

    #region IOnlineTimeSeriesPrognosisEvaluator Members

    public void StartNewPredictionWindow(double lastKnownOriginalValue) {
      prevEstimated = lastKnownOriginalValue;
      prevOriginal = lastKnownOriginalValue;
    }

    #endregion
  }
}
