using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis.GaussianProcess {
  [StorableClass]
  [Item(Name = "CovarianceSum",
    Description = "Sum covariance function for Gaussian processes.")]
  public class CovarianceSum : Item, ICovarianceFunction {
    [Storable]
    private ItemList<ICovarianceFunction> terms;

    [Storable]
    private int numberOfVariables;
    public ItemList<ICovarianceFunction> Terms {
      get { return terms; }
    }

    [StorableConstructor]
    protected CovarianceSum(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceSum(CovarianceSum original, Cloner cloner)
      : base(original, cloner) {
      this.terms = cloner.Clone(terms);
    }

    public CovarianceSum()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSum(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      this.numberOfVariables = numberOfVariables;
      return terms.Select(t => t.GetNumberOfParameters(numberOfVariables)).Sum();
    }

    public void SetParameter(double[] hyp, double[,] x) {
      int offset = 0;
      foreach (var t in terms) {
        t.SetParameter(hyp.Skip(offset).Take(t.GetNumberOfParameters(numberOfVariables)), x);
        offset += numberOfVariables;
      }
    }


    public void SetParameter(double[] hyp, double[,] x, double[,] xt) {
      this.l = Math.Exp(hyp[0]);
      this.sf2 = Math.Exp(2 * hyp[1]);

      this.symmetric = false;
      this.x = x;
      this.xt = xt;
      sd = null;
    }

    public double GetCovariance(int i, int j) {
      if (sd == null) CalculateSquaredDistances();
      return sf2 * Math.Exp(-sd[i, j] / 2.0);
    }


    public double[] GetDiagonalCovariances() {
      if (x != xt) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      var sd = new double[rows];
      for (int i = 0; i < rows; i++) {
        sd[i] = Util.SqrDist(Util.GetRow(x, i).Select(e => e / l), Util.GetRow(xt, i).Select(e => e / l));
      }
      return sd.Select(d => sf2 * Math.Exp(-d / 2.0)).ToArray();
    }


    public double[] GetGradient(int i, int j) {
      var res = new double[2];
      res[0] = sf2 * Math.Exp(-sd[i, j] / 2.0) * sd[i, j];
      res[1] = 2.0 * sf2 * Math.Exp(-sd[i, j] / 2.0);
      return res;
    }

    private void CalculateSquaredDistances() {
      if (x.GetLength(1) != xt.GetLength(1)) throw new InvalidOperationException();
      int rows = x.GetLength(0);
      int cols = xt.GetLength(0);
      sd = new double[rows, cols];
      if (symmetric) {
        for (int i = 0; i < rows; i++) {
          for (int j = i; j < rows; j++) {
            sd[i, j] = Util.SqrDist(Util.GetRow(x, i).Select(e => e / l), Util.GetRow(xt, j).Select(e => e / l));
            sd[j, i] = sd[i, j];
          }
        }
      } else {
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < cols; j++) {
            sd[i, j] = Util.SqrDist(Util.GetRow(x, i).Select(e => e / l), Util.GetRow(xt, j).Select(e => e / l));
          }
        }
      }
    }
  }
}
