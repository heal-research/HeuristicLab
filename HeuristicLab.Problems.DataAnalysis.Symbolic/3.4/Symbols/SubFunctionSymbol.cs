using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("C6C2FBEE-8F4B-4FF2-80F5-D566BE55ED69")]
  [Item("SubFunction", "Symbol that represents a sub function.")]
  public sealed class SubFunctionSymbol : Symbol {
    public override int MinimumArity => 0;
    public override int MaximumArity => 1;

    public SubFunctionSymbol() : base("SubFunction", "Symbol that represents a sub function.") { }

    private SubFunctionSymbol(SubFunctionSymbol original, Cloner cloner) : base(original, cloner) { }

    [StorableConstructor]
    private SubFunctionSymbol(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) =>
      new SubFunctionSymbol(this, cloner);

    public override ISymbolicExpressionTreeNode CreateTreeNode() => new SubFunctionTreeNode(this);
  }
}
