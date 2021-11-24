using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("44E03792-5E65-4C70-99B2-7849B8927E28")]
  [Item("RealConstant", "Represents a number.")]
  public sealed class RealConstant : Symbol{
    [Storable]
    private double minValue;
    public double MinValue {
      get { return minValue; }
      set {
        if (value != minValue) {
          minValue = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double maxValue;
    public double MaxValue {
      get { return maxValue; }
      set {
        if (value != maxValue) {
          maxValue = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    private const int minimumArity = 0;
    private const int maximumArity = 0;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    [StorableConstructor]
    private RealConstant(StorableConstructorFlag _) : base(_) { }

    private RealConstant(RealConstant original, Cloner cloner) : base(original, cloner) {
      minValue = original.minValue;
      maxValue = original.maxValue;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealConstant(this, cloner);
    }

    public RealConstant() : base("RealConstant", "Represents a number.") {
      minValue = -20.0;
      maxValue = 20.0;
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new RealConstantTreeNode(this);
    }
  }
}
