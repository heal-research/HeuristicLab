using System;
using System.Linq;
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
        template = value;
        Tree = Parser.Parse(template);
        OnChanged();
      } 
    }

    [Storable]
    private ISymbolicExpressionTree treeWithoutLinearScaling;
    [Storable]
    private ISymbolicExpressionTree treeWithLinearScaling;
    public ISymbolicExpressionTree Tree {
      get => ApplyLinearScaling ? treeWithLinearScaling : treeWithoutLinearScaling;
      private set {
        treeWithLinearScaling = AddLinearScalingTerms(value);
        treeWithoutLinearScaling = value;
        SubFunctions = GetSubFunctions();
      }
    }

    [Storable]
    public IReadOnlyDictionary<string, SubFunction> SubFunctions { get; private set; }

    [Storable]
    private bool applyLinearScaling = false;
    public bool ApplyLinearScaling {
      get => applyLinearScaling;
      set {
        applyLinearScaling = value;
        OnChanged();
      }
    }

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

    private Dictionary<string, SubFunction> GetSubFunctions() {
      var subFunctions = new Dictionary<string, SubFunction>();
      foreach (var node in Tree.IterateNodesPrefix())
        if (node is SubFunctionTreeNode subFunctionTreeNode) {
          if (!subFunctionTreeNode.Arguments.Any())
            throw new ArgumentException($"The sub-function '{subFunctionTreeNode}' requires inputs (e.g. {subFunctionTreeNode.Name}(var1, var2)).");

          if (subFunctions.TryGetValue(subFunctionTreeNode.Name, out SubFunction v)) {
            if(!v.Arguments.SequenceEqual(subFunctionTreeNode.Arguments))
              throw new ArgumentException(
                $"The sub-function '{v.Name}' has (at least two) different signatures " + 
                $"({v.Name}({string.Join(",", v.Arguments)}) <> {subFunctionTreeNode.Name}({string.Join(",", subFunctionTreeNode.Arguments)})).");
          } else {
            var subFunction = new SubFunction() {
              Name = subFunctionTreeNode.Name,
              Arguments = subFunctionTreeNode.Arguments
            };
            subFunction.Changed += OnSubFunctionChanged;
            subFunctions.Add(subFunction.Name, subFunction);
          }
        }
      return subFunctions;
    }

    private ISymbolicExpressionTree AddLinearScalingTerms(ISymbolicExpressionTree tree) {
      var clonedTree = (ISymbolicExpressionTree)tree.Clone();
      var startNode = clonedTree.Root.Subtrees.First();
      var template = startNode.Subtrees.First();

      var add = new Addition();
      var addNode = add.CreateTreeNode();

      var mul = new Multiplication();
      var mulNode = mul.CreateTreeNode();

      var c1 = new Constant();
      var c1Node = (ConstantTreeNode)c1.CreateTreeNode();
      c1Node.Value = 0.0;
      var c2 = new Constant();
      var c2Node = (ConstantTreeNode)c2.CreateTreeNode();
      c2Node.Value = 1.0;
      
      addNode.AddSubtree(c1Node);
      addNode.AddSubtree(mulNode);
      mulNode.AddSubtree(c2Node);
        
      startNode.RemoveSubtree(0);
      startNode.AddSubtree(addNode);
      mulNode.AddSubtree(template);
      return clonedTree;
    }

    private void OnSubFunctionChanged(object sender, EventArgs e) => OnChanged();
  }
}
