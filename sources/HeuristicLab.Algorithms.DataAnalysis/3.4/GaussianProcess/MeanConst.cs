
using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanConst", Description = "Constant mean function for Gaussian processes.")]
  public class MeanConst : Item, IMeanFunction {
    [Storable]
    private double c;
    [Storable]
    private int n;
    public int GetNumberOfParameters(int numberOfVariables) {
      return 1;
    }
    [StorableConstructor]
    protected MeanConst(bool deserializing) : base(deserializing) { }
    protected MeanConst(MeanConst original, Cloner cloner)
      : base(original, cloner) {
      this.c = original.c;
      this.n = original.n;
    }
    public MeanConst()
      : base() {
    }

    public void SetParameter(double[] hyp, double[,] x) {
      if (hyp.Length != 1) throw new ArgumentException("Only one hyper-parameter allowed for constant mean function.", "hyp");
      this.c = hyp[0];
      this.n = x.GetLength(0);
    }

    public double[] GetMean(double[,] x) {
      return Enumerable.Repeat(c, n).ToArray();
    }

    public double[] GetGradients(int k, double[,] x) {
      if (k > 0) throw new ArgumentException();
      return Enumerable.Repeat(1.0, n).ToArray();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanConst(this, cloner);
    }
  }
}
