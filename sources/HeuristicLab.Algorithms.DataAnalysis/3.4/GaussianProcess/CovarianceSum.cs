using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Algorithms.DataAnalysis.GaussianProcess {
  public class CovarianceSum : ICovarianceFunction {
    private IList<ICovarianceFunction> covariances;

    public int NumberOfParameters {
      get { return covariances.Sum(c => c.NumberOfParameters); }
    }

    public CovarianceSum(IEnumerable<ICovarianceFunction> covariances) {
      this.covariances = covariances.ToList();
    }

    public void SetMatrix(double[,] x) {
      foreach (var covariance in covariances) {
        covariance.SetMatrix(x, x);
      }
    }

    public void SetMatrix(double[,] x, double[,] xt) {
      foreach (var covariance in covariances) {
        covariance.SetMatrix(x, xt);
      }
    }

    public void SetHyperparamter(double[] hyp) {
      int i = 0;
      foreach (var covariance in covariances) {
        int n = covariance.NumberOfParameters;
        covariance.SetHyperparamter(hyp.Skip(i).Take(n).ToArray());
        i += n;
      }
    }

    public double GetCovariance(int i, int j) {
      return covariances.Select(c => c.GetCovariance(i, j)).Sum();
    }


    public double[] GetDiagonalCovariances() {
      return covariances
        .Select(c => c.GetDiagonalCovariances())
        .Aggregate((s, d) => s.Zip(d, (a, b) => a + b).ToArray())
        .ToArray();
    }

    public double[] GetDerivatives(int i, int j) {
      return covariances
        .Select(c => c.GetDerivatives(i, j))
        .Aggregate(Enumerable.Empty<double>(), (h0, h1) => h0.Concat(h1))
        .ToArray();
    }
  }
}
