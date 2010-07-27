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
  public interface IOnlineTimeSeriesPrognosisEvaluator : IOnlineEvaluator {
    void StartNewPredictionWindow(double referenceOriginalValue);
  }
}
