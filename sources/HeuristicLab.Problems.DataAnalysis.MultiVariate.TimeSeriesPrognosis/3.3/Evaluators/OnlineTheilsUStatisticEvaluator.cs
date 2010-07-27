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
  public class OnlineTheilsUStatisticEvaluator : IOnlineTimeSeriesPrognosisEvaluator {
    private OnlineMeanAndVarianceCalculator squaredErrorMeanCalculator;
    private OnlineMeanAndVarianceCalculator unbiasedEstimatorMeanCalculator;
    private double prevOriginal;

    public double TheilsUStatistic {
      get {
        return Math.Sqrt(squaredErrorMeanCalculator.Mean) / Math.Sqrt(unbiasedEstimatorMeanCalculator.Mean);
      }
    }

    public OnlineTheilsUStatisticEvaluator() {
      squaredErrorMeanCalculator = new OnlineMeanAndVarianceCalculator();
      unbiasedEstimatorMeanCalculator = new OnlineMeanAndVarianceCalculator();
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return TheilsUStatistic; }
    }

    public void Add(double original, double estimated) {
      if (double.IsInfinity(original) || double.IsNaN(original) || double.IsInfinity(estimated) || double.IsNaN(estimated)) {
        throw new ArgumentException("Theil's U-statistic is not defined for series containing NaN or infinity values.");
      }

      if (!double.IsNaN(prevOriginal)) {
        // error of predicted change
        double errorEstimatedChange = (estimated - original);
        squaredErrorMeanCalculator.Add(errorEstimatedChange * errorEstimatedChange);

        // error of naive model y(t+1) = y(t)
        double errorNoChange = (original - prevOriginal);
        unbiasedEstimatorMeanCalculator.Add(errorNoChange * errorNoChange);
      }
    }

    public void Reset() {
      prevOriginal = double.NaN;
      squaredErrorMeanCalculator.Reset();
      unbiasedEstimatorMeanCalculator.Reset();
    }

    #endregion

    #region IOnlineTimeSeriesPrognosisEvaluator Members

    public void StartNewPredictionWindow(double referenceOriginalValue) {
      prevOriginal = referenceOriginalValue;
    }

    #endregion
  }
}
