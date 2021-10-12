using System;
using System.Collections.Generic;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("E3C038DB-C6AA-457D-9F65-AF16C44CCE22")]
  [Item("StructureTemplate", "Structure Template")]
  public class StructureTemplate : Item {

    #region Properties
    [Storable]
    private string template = "";
    public string Template {
      get => template; 
      set {
        if(template != value) {
          template = value;
          tree = Parser.Parse(template);
          GetSubFunctions(Tree);
          OnChanged();
        }
      } 
    }

    [Storable]
    private ISymbolicExpressionTree tree;
    public ISymbolicExpressionTree Tree => tree;

    [Storable]
    public IDictionary<SubFunctionTreeNode, SubFunction> SubFunctions { get; private set; } = new Dictionary<SubFunctionTreeNode, SubFunction>();

    protected InfixExpressionParser Parser { get; set; } = new InfixExpressionParser();
    #endregion

    #region Events
    public event EventHandler Changed;
    
    private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);
    #endregion

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

    private void GetSubFunctions(ISymbolicExpressionTree tree) {
      int count = 1;
      SubFunctions.Clear();
      foreach (var node in tree.IterateNodesPrefix())
        if (node is SubFunctionTreeNode subFunctionTreeNode) { 
          var subFunction = new SubFunction() { 
            Name = $"f{count++}({string.Join(",", subFunctionTreeNode.FunctionArguments)})", 
            FunctionArguments = subFunctionTreeNode.FunctionArguments 
          };
          subFunctionTreeNode.SubFunction = subFunction;
          SubFunctions.Add(subFunctionTreeNode, subFunction);
        }
    }
  }
}
