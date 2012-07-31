
using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis.GaussianProcess {
  [StorableClass]
  [Item(Name = "MeanZero", Description = "Constant zero mean function for Gaussian processes.")]
  public class MeanZero : Item, IMeanFunction {
    [Storable]
    private int n;
    public int GetNumberOfParameters(int numberOfVariables) {
      return 0;
    }
    [StorableConstructor]
    protected MeanZero(bool deserializing) : base(deserializing) { }
    protected MeanZero(MeanZero original, Cloner cloner)
      : base(original, cloner) {
      this.n = original.n;
    }
    public MeanZero() {
    }

    public void SetParameter(double[] hyp, double[,] x) {
      if (hyp.Length > 0) throw new ArgumentException("No hyper-parameters allowed for zero mean function.", "hyp");
      this.n = x.GetLength(0);
    }

    public double[] GetMean(double[,] x) {
      return Enumerable.Repeat(0.0, n).ToArray();
    }

    public double[] GetGradients(int k, double[,] x) {
      if (k > 0) throw new ArgumentException();
      return Enumerable.Repeat(0.0, n).ToArray();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanZero(this, cloner);
    }
  }
}
