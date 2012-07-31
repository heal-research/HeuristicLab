using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis.GaussianProcess {
  [StorableClass]
  [Item(Name = "CovarianceLinear", Description = "Linear covariance function with for Gaussian processes.")]
  public class CovarianceLinear : Item, ICovarianceFunction {
    [Storable]
    private double[,] x;
    [Storable]
    private double[,] xt;


    private double[,] k;
    private bool symmetric;

    public int GetNumberOfParameters(int numberOfVariables) {
      return 0;
    }
    [StorableConstructor]
    protected CovarianceLinear(bool deserializing) : base(deserializing) { }
    protected CovarianceLinear(CovarianceLinear original, Cloner cloner)
      : base(original, cloner) {
      // note: using shallow copies here!
      this.x = original.x;
      this.xt = original.xt;

    }
    public CovarianceLinear()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceLinear(this, cloner);
    }

    public void SetParameter(double[] hyp, double[,] x) {
      if (hyp.Length > 0) throw new ArgumentException();
      SetParameter(hyp, x, x);
      this.symmetric = true;
    }

    public void SetParameter(double[] hyp, double[,] x, double[,] xt) {
      this.x = x;
      this.xt = xt;
      this.symmetric = false;

      k = null;
    }

    public double GetCovariance(int i, int j) {
      if (k == null) CalculateInnerProduct();
      return k[i, j];
    }


    public double[] GetDiagonalCovariances() {
      if (x != xt) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = x.GetLength(1);
      var k = new double[rows];
      for (int i = 0; i < rows; i++) {
        k[i] = 0;
        for (int j = 0; j < cols; j++) {
          k[i] += x[i, j] * x[i, j];
        }
      }
      return k;
    }

    public double[] GetGradient(int i, int j) {
      throw new NotSupportedException();
    }


    private void CalculateInnerProduct() {
      if (x.GetLength(1) != xt.GetLength(1)) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = xt.GetLength(0);
      k = new double[rows, cols];
      if (symmetric) {
        for (int i = 0; i < rows; i++) {
          for (int j = i; j < cols; j++) {
            k[i, j] = Util.ScalarProd(Util.GetRow(x, i),
                                      Util.GetRow(x, j));
            k[j, i] = k[i, j];
          }
        }
      } else {
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < cols; j++) {
            k[i, j] = Util.ScalarProd(Util.GetRow(x, i),
                                      Util.GetRow(xt, j));
          }
        }
      }
    }
  }
}
