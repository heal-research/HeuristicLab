using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("F91000E6-B041-4648-A9E8-595228F957FA")]
  public sealed class RealConstantTreeNode : SymbolicExpressionTreeTerminalNode{

    public new RealConstant Symbol {
      get { return (RealConstant) base.Symbol; }
    }

    public double Value { get; set; }

    [StorableConstructor]
    private RealConstantTreeNode(StorableConstructorFlag _) : base(_) { }

    private RealConstantTreeNode(RealConstantTreeNode original, Cloner cloner)
      : base(original, cloner) {
      Value = original.Value;
    }

    private RealConstantTreeNode() : base() { }
    public RealConstantTreeNode(RealConstant realConstantSymbol) : base(realConstantSymbol) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealConstantTreeNode(this, cloner);
    }

    public override string ToString() {
      return $"{Value:E4}";
    }
  }
}