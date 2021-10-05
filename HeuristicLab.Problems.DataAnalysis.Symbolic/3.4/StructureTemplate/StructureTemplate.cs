using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("E3C038DB-C6AA-457D-9F65-AF16C44CCE22")]
  [Item("StructureTemplate", "Structure Template")]
  public class StructureTemplate : Item {

    [Storable]
    private string template = "";
    public string Template {
      get => template; 
      set {
        if(template != value) {
          template = value;
          tree = Parser.Parse(template);
          subFunctions = GetSubFunctions(Tree);
        }
      } 
    }

    [Storable]
    private ISymbolicExpressionTree tree;
    public ISymbolicExpressionTree Tree => tree;

    [Storable]
    private IEnumerable<SubFunction> subFunctions;
    public IEnumerable<SubFunction> SubFunctions => subFunctions;

    protected InfixExpressionParser Parser { get; set; } = new InfixExpressionParser();


    #region Constructors
    public StructureTemplate() { }

    [StorableConstructor]
    protected StructureTemplate(StorableConstructorFlag _) : base(_) { }

    protected StructureTemplate(StructureTemplate original, Cloner cloner) { }
    #endregion

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) =>
      new StructureTemplate(this, cloner);
    #endregion

    private IEnumerable<SubFunction> GetSubFunctions(ISymbolicExpressionTree tree) {
      int count = 1;
      foreach (var node in tree.IterateNodesPrefix())
        if (node.Symbol is SubFunctionSymbol)
          yield return new SubFunction() { Name = $"f{count++}" };
    }
  }
}
