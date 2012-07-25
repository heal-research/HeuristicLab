using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Algorithms.DataAnalysis.GaussianProcess {
  public class CovarianceNNOne : ICovarianceFunction {
    private double[,] x;
    private double[,] xt;
    private double sf2;
    private double l2;
    private double[,] S;
    private double[] sx;
    private double[] sz;
    private double sxsx;
    private double sxsz;

    public int NumberOfParameters {
      get { return 2; }
    }

    public void SetMatrix(double[,] x) {
      SetMatrix(x, x);
    }

    public void SetMatrix(double[,] x, double[,] xt) {
      this.x = x;
      this.xt = xt;
      S = null;
      sx = null;
      sz = null;
    }

    public void SetHyperparamter(double[] hyp) {
      if (hyp.Length != 2) throw new ArgumentException();
      this.l2 = Math.Exp(2 * hyp[0]);
      this.sf2 = Math.Exp(2 * hyp[1]);
      S = null;
      sx = null;
      sz = null;
    }

    public double GetCovariance(int i, int j) {
      if (S == null) CalculateVectorProducts();
      if (sx == null) CalculateSx();
      bool symmetric = x == xt;
      double k;
      if (symmetric) {
        k = S[i, j] / sxsx;
      } else {
        k = S[i, j] / sxsz;
      }
      return sf2 * Math.Asin(k);
    }


    public double[] GetDiagonalCovariances() {
      if (x != xt) throw new InvalidOperationException();
      if (sx == null) CalculateSx();
      int rows = x.GetLength(0);
      var k = new double[rows];
      for (int i = 0; i < rows; i++) {
        k[i] = sx[i] / (sx[i] + l2);
        k[i] = sf2 * Math.Asin(k[i]);
      }
      return k;
    }

    public double[] GetDerivatives(int i, int j) {
      double[] dhyp = new double[NumberOfParameters];
      double[] vx = sx.Select(e => e / (l2 + e) / 2).ToArray();

      double k;
      double v;
      if (x == xt) {
        k = S[i, j] / sxsx;
        v = vx[i] + vx[j];
      } else {
        double[] vz = sz.Select(e => e / (l2 + e) / 2).ToArray();
        v = vx[i] + vz[j];
        k = S[i, j] / sxsz;
      }
      dhyp[0] = -2 * sf2 * (k - k * v) / Math.Sqrt(1 - k * k);
      dhyp[1] = 2.0 * sf2 * Math.Asin(k);
      return dhyp;
    }

    private void CalculateSx() {
      this.sx = new double[x.GetLength(0)];
      for (int i = 0; i < sx.Length; i++) {
        sx[i] = 1 + Product(GetRow(x, i), GetRow(x, i));
      }
      this.sz = new double[xt.GetLength(0)];
      for (int i = 0; i < sz.Length; i++) {
        sz[i] = 1 + Product(GetRow(xt, i), GetRow(xt, i));
      }

      sxsx = Product(sx.Select(e => Math.Sqrt(l2 + e)), sx.Select(e => Math.Sqrt(l2 + e)));
      sxsz = Product(sx.Select(e => Math.Sqrt(l2 + e)), sz.Select(e => Math.Sqrt(l2 + e)));
    }

    private void CalculateVectorProducts() {
      if (x.GetLength(1) != xt.GetLength(1)) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = xt.GetLength(0);
      S = new double[rows, cols];
      bool symmetric = x == xt;
      for (int i = 0; i < rows; i++) {
        for (int j = i; j < rows; j++) {
          S[i, j] = 1 + Product(GetRow(x, i), GetRow(xt, j));
          if (symmetric) {
            S[j, i] = S[i, j];
          } else {
            S[j, i] = 1 + Product(GetRow(x, j), GetRow(xt, i));
          }
        }
      }
    }


    private double Product(IEnumerable<double> x, IEnumerable<double> y) {
      return x.Zip(y, (a, b) => a * b).Sum();
    }
    private static IEnumerable<double> GetRow(double[,] x, int r) {
      int cols = x.GetLength(1);
      return Enumerable.Range(0, cols).Select(c => x[r, c]);
    }
  }
}
