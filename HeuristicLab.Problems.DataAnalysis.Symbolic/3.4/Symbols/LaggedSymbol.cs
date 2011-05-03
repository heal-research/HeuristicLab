using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("LaggedSymbol", "Represents a symblol whose evaluation is shifted.")]
  public abstract class LaggedSymbol : Symbol {
    [Storable]
    private int minLag;
    public virtual int MinLag {
      get { return minLag; }
      set { minLag = value; }
    }
    [Storable]
    private int maxLag;
    public virtual int MaxLag {
      get { return maxLag; }
      set { maxLag = value; }
    }

    [StorableConstructor]
    protected LaggedSymbol(bool deserializing) : base(deserializing) { }
    protected LaggedSymbol(LaggedSymbol original, Cloner cloner)
      : base(original, cloner) {
      minLag = original.minLag;
      maxLag = original.maxLag;
    }
    protected LaggedSymbol()
      : base("LaggedSymbol", "Represents a symbol whose evaluation is shifted.") {
      minLag = -1; maxLag = -1;
    }
    protected LaggedSymbol(string name, string description)
      : base(name, description) {
      minLag = -1; maxLag = -1;
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new LaggedTreeNode(this);
    }
  }
}
