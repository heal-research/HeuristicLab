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
        tree = Parser.Parse(template);
        GetSubFunctions(Tree);
        OnChanged();
      } 
    }

    [Storable]
    private ISymbolicExpressionTree tree;
    public ISymbolicExpressionTree Tree => tree;

    [Storable]
    public IReadOnlyDictionary<string, SubFunction> SubFunctions { get; private set; } = new Dictionary<string, SubFunction>();

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
      var subFunctions = new Dictionary<string, SubFunction>();
      foreach (var node in tree.IterateNodesPrefix())
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
      SubFunctions = subFunctions;
    }

    private void OnSubFunctionChanged(object sender, EventArgs e) => OnChanged();
  }
}
