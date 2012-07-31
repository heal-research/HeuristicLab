
using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanLinear", Description = "Linear mean function for Gaussian processes.")]
  public class MeanLinear : Item, IMeanFunction {
    [Storable]
    private double[] alpha;
    [Storable]
    private int n;
    public int GetNumberOfParameters(int numberOfVariables) {
      return numberOfVariables;
    }
    [StorableConstructor]
    protected MeanLinear(bool deserializing) : base(deserializing) { }
    protected MeanLinear(MeanLinear original, Cloner cloner)
      : base(original, cloner) {
      if (original.alpha != null) {
        this.alpha = new double[original.alpha.Length];
        Array.Copy(original.alpha, alpha, original.alpha.Length);
      }
      this.n = original.n;
    }
    public MeanLinear()
      : base() {
    }

    public void SetParameter(double[] hyp, double[,] x) {
      if (hyp.Length != x.GetLength(1)) throw new ArgumentException("Number of hyper-parameters must match the number of variables.", "hyp");
      this.alpha = new double[hyp.Length];
      Array.Copy(hyp, alpha, hyp.Length);
      this.n = x.GetLength(0);
    }

    public double[] GetMean(double[,] x) {
      int cols = x.GetLength(1);
      return (from i in Enumerable.Range(0, n)
              let rowVector = from j in Enumerable.Range(0, cols)
                              select x[i, j]
              select Util.ScalarProd(alpha, rowVector))
        .ToArray();
    }

    public double[] GetGradients(int k, double[,] x) {
      int cols = x.GetLength(1);
      if (k > cols) throw new ArgumentException();
      return (from r in Enumerable.Range(0, n)
              select x[r, k]).ToArray();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanLinear(this, cloner);
    }
  }
}
