using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Algorithms.DataAnalysis.GaussianProcess {
  public class CovariancePeriodic : ICovarianceFunction {
    private double[,] x;
    private double[,] xt;
    private double sf2;
    private double l;
    private double[,] sd;
    private double p;

    public int NumberOfParameters {
      get { return 2; }
    }

    public CovariancePeriodic(double p) {
      this.p = p;
    }

    public void SetMatrix(double[,] x) {
      SetMatrix(x, x);
    }

    public void SetMatrix(double[,] x, double[,] xt) {
      this.x = x;
      this.xt = xt;
      sd = null;
    }

    public void SetHyperparamter(double[] hyp) {
      if (hyp.Length != 2) throw new ArgumentException();
      this.l = Math.Exp(hyp[0]);
      this.sf2 = Math.Exp(2 * hyp[1]);
      sd = null;
    }

    public double GetCovariance(int i, int j) {
      if (sd == null) CalculateSquaredDistances();
      double k = sd[i, j];
      k = Math.PI * k / p;
      k = Math.Sin(k) / l;
      k = k * k;

      return sf2 * Math.Exp(-2.0 * k);
    }


    public double[] GetDiagonalCovariances() {
      if (x != xt) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      var cov = new double[rows];
      for (int i = 0; i < rows; i++) {
        double k = Math.Sqrt(SqrDist(GetRow(x, i), GetRow(xt, i)));
        k = Math.PI * k / p;
        k = Math.Sin(k) / l;
        k = k * k;
        cov[i] = sf2 * Math.Exp(-2.0 * k);
      }
      return cov;
    }

    public double[] GetDerivatives(int i, int j) {

      var res = new double[2];
      double k = sd[i, j];
      k = Math.PI * k / p;
      k = Math.Sin(k) / l;
      k = k * k;
      res[0] = 4 * sf2 * Math.Exp(-2 * k) * k;
      res[1] = 2 * sf2 * Math.Exp(-2 * k);
      return res;
    }

    private void CalculateSquaredDistances() {
      if (x.GetLength(1) != xt.GetLength(1)) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = xt.GetLength(0);
      sd = new double[rows, cols];
      bool symmetric = x == xt;
      for (int i = 0; i < rows; i++) {
        for (int j = i; j < rows; j++) {
          sd[i, j] = Math.Sqrt(SqrDist(GetRow(x, i), GetRow(xt, j)));
          if (symmetric) {
            sd[j, i] = sd[i, j];
          } else {
            sd[j, i] = Math.Sqrt(SqrDist(GetRow(x, j), GetRow(xt, i)));
          }
        }
      }
    }


    private double SqrDist(IEnumerable<double> x, IEnumerable<double> y) {
      var d0 = x.Zip(y, (a, b) => (a - b) * (a - b));
      return Math.Max(0, d0.Sum());
    }
    private static IEnumerable<double> GetRow(double[,] x, int r) {
      int cols = x.GetLength(1);
      return Enumerable.Range(0, cols).Select(c => x[r, c]);
    }
  }
}
