#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("E3C038DB-C6AA-457D-9F65-AF16C44CCE22")]
  [Item("StructureTemplate", "Structure Template")]
  public class StructureTemplate : Item {

    #region Properties
    [Storable]
    private string template;
    public string Template {
      get => template;
      set {
        if (value == template) return;

        var parsedTree = Parser.Parse(value);
        //assignment must be done after successfully parsing the tree
        template = value;

        if (applyLinearScaling)
          parsedTree = LinearScaling.AddLinearScalingTerms(parsedTree);
        Tree = parsedTree;
        OnChanged();
      }
    }

    private ISymbolicExpressionTree _oldTree;
    [Storable(OldName = "treeWithoutLinearScaling")]
    private ISymbolicExpressionTree _OldTreeW {
      set => _oldTree = value;
    }

    [Storable]
    private ISymbolicExpressionTree tree;
    public ISymbolicExpressionTree Tree {
      get => tree;
      private set {
        containsNumericParameters = null;

        var newFunctions = CreateSubFunctions(value);
        var oldFunctions = subFunctions?.Intersect(newFunctions)
                           ?? Enumerable.Empty<SubFunction>();
        // adds new functions and keeps the old ones (if they match)
        var functionsToAdd = newFunctions.Except(oldFunctions);
        subFunctions = functionsToAdd.Concat(oldFunctions).ToList();
        RegisterSubFunctionEventHandlers(functionsToAdd);

        tree = value;
      }
    }

    private bool? containsNumericParameters;
    public bool ContainsNumericParameters {
      get {
        if (!containsNumericParameters.HasValue)
          containsNumericParameters = Tree.IterateNodesPrefix().OfType<NumberTreeNode>().Any();

        return containsNumericParameters.Value;
      }
    }

    [Storable]
    private IList<SubFunction> subFunctions = new List<SubFunction>();
    public IEnumerable<SubFunction> SubFunctions => subFunctions;

    [Storable]
    private bool applyLinearScaling;
    public bool ApplyLinearScaling {
      get => applyLinearScaling;
      set {
        if (value == applyLinearScaling) return;

        applyLinearScaling = value;
        if (applyLinearScaling) LinearScaling.AddLinearScalingTerms(Tree);
        else LinearScaling.RemoveLinearScalingTerms(Tree);
        containsNumericParameters = null; // reset cached value
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
    public StructureTemplate() {
      Reset();
    }

    [StorableConstructor]
    protected StructureTemplate(StorableConstructorFlag _) : base(_) { }

    protected StructureTemplate(StructureTemplate original, Cloner cloner) : base(original, cloner) {
      this.tree = cloner.Clone(original.tree);
      this.template = original.Template;
      this.applyLinearScaling = original.ApplyLinearScaling;
      this.subFunctions = original.subFunctions.Select(cloner.Clone).ToList();
      RegisterSubFunctionEventHandlers(SubFunctions);
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (Tree == null && _oldTree != null) {
        if (ApplyLinearScaling) _oldTree = LinearScaling.AddLinearScalingTerms(_oldTree);
        Tree = _oldTree;
        _oldTree = null;
      }

      RegisterSubFunctionEventHandlers(SubFunctions);
    }
    #endregion

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) =>
      new StructureTemplate(this, cloner);
    #endregion

    public void Reset() {
      subFunctions = new List<SubFunction>();
      tree = null;
      containsNumericParameters = null;
      Template = "f(_)";
    }

    private static IList<SubFunction> CreateSubFunctions(ISymbolicExpressionTree tree) {
      var subFunctions = new List<SubFunction>();
      foreach (var node in tree.IterateNodesPrefix())
        if (node is SubFunctionTreeNode subFunctionTreeNode) {
          if (!subFunctionTreeNode.Arguments.Any())
            throw new ArgumentException($"The sub-function '{subFunctionTreeNode}' requires inputs (e.g. {subFunctionTreeNode.Name}(var1, var2)).");

          var existingSubFunction = subFunctions.Where(x => x.Name == subFunctionTreeNode.Name).FirstOrDefault();
          if (existingSubFunction != null) {
            // an existing subFunction must have the same signature
            if (!existingSubFunction.Arguments.SequenceEqual(subFunctionTreeNode.Arguments))
              throw new ArgumentException(
                $"The sub-function '{existingSubFunction.Name}' has (at least two) different signatures " +
                $"({existingSubFunction.Name}({string.Join(",", existingSubFunction.Arguments)}) <> " +
                $"{subFunctionTreeNode.Name}({string.Join(",", subFunctionTreeNode.Arguments)})).");
          } else {
            var subFunction = new SubFunction() {
              Name = subFunctionTreeNode.Name,
              Arguments = subFunctionTreeNode.Arguments
            };
            subFunctions.Add(subFunction);
          }
        }
      return subFunctions;
    }

    private void RegisterSubFunctionEventHandlers(IEnumerable<SubFunction> subFunctions) {
      foreach (var sf in subFunctions) {
        sf.Changed += (o, e) => OnChanged();
      }
    }
  }
}
