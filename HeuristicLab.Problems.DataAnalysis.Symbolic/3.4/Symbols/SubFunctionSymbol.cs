using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("C6C2FBEE-8F4B-4FF2-80F5-D566BE55ED69")]
  [Item("SubFunctionSymbol", "Symbol that represents a sub function.")]
  public class SubFunctionSymbol : Symbol {
    public override int MinimumArity => 1;
    public override int MaximumArity => byte.MaxValue;

    public SubFunctionSymbol() : base("SubFunctionSymbol", "Symbol that represents a sub function.") { }

    private SubFunctionSymbol(SubFunctionSymbol original, Cloner cloner) : base(original, cloner) { }

    [StorableConstructor]
    public SubFunctionSymbol(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) =>
      new SubFunctionSymbol(this, cloner);

    public override ISymbolicExpressionTreeNode CreateTreeNode() => new SubFunctionTreeNode(this);
  }
}
