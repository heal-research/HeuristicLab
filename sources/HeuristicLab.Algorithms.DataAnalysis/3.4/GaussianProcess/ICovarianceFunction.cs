

using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public interface ICovarianceFunction : IItem {
    int GetNumberOfParameters(int numberOfVariables);
    void SetParameter(double[] hyp, double[,] x);
    void SetParameter(double[] hyp, double[,] x, double[,] xt);

    double GetCovariance(int i, int j);
    double[] GetDiagonalCovariances();
    double[] GetGradient(int i, int j);
  }
}
