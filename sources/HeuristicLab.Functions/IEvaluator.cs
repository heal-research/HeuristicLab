using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public interface IEvaluator {
    void ResetEvaluator(IFunctionTree functionTree, Dataset dataset);
    double Evaluate(int sampleIndex);
  }
}
