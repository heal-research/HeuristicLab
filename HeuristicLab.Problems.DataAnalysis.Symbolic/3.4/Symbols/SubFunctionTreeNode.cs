using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("05130B5F-0125-4367-A4E9-C42D1085024E")]
  public class SubFunctionTreeNode : SymbolicExpressionTreeNode {

    #region Properties
    public new SubFunctionSymbol Symbol => (SubFunctionSymbol)base.Symbol;

    public IEnumerable<string> FunctionArguments { get; set; } = Enumerable.Empty<string>();
    
    public SubFunction SubFunction { get; set; }
    #endregion

    #region Constructors
    public SubFunctionTreeNode(SubFunctionSymbol symbol) : base(symbol) { }

    [StorableConstructor]
    protected SubFunctionTreeNode(StorableConstructorFlag _) : base(_) { }

    protected SubFunctionTreeNode(SubFunctionTreeNode original, Cloner cloner) : base(original, cloner) {
      this.SubFunction = original.SubFunction;
    }
    #endregion

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) => new SubFunctionTreeNode(this, cloner);
    #endregion
  }
}
