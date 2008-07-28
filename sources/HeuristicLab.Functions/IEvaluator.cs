using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Functions {
  public interface IEvaluator {
    void ResetEvaluator(IFunctionTree functionTree);
    double Evaluate(int sampleIndex);
  }
}
