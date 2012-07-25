
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.DataAnalysis.GaussianProcess {
  public interface IMeanFunction : IItem {
    int GetNumberOfParameters(int numberOfVariables);
    void SetParameter(double[] hyp, double[,] x);
    double[] GetMean();
    double[] GetGradients(int k);
  }
}
