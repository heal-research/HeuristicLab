#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Modeling;
using HeuristicLab.DataAnalysis;
using System.Diagnostics;

namespace HeuristicLab.GP.StructureIdentification.Networks {
  public class NetworkToFunctionTransformer : OperatorBase {
    public NetworkToFunctionTransformer()
      : base() {
      AddVariableInfo(new VariableInfo("Network", "The network (open expression)", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariables", "Name of the target variables", typeof(ItemList<StringData>), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree with all targetvaribales", typeof(IGeneticProgrammingModel), VariableKind.New));
    }

    public override string Description {
      get { return "Extracts the network (function tree with unbound parameters) and creates a closed form function tree for each target variable."; }
    }

    public override IOperation Apply(IScope scope) {
      IGeneticProgrammingModel model = GetVariableValue<IGeneticProgrammingModel>("Network", scope, true);
      ItemList<StringData> targetVariables = GetVariableValue<ItemList<StringData>>("TargetVariables", scope, true);
      // clear old sub-scopes
      while (scope.SubScopes.Count > 0) scope.RemoveSubScope(scope.SubScopes[0]);

      // create a new sub-scope for each target variable with the transformed expression
      foreach (IFunctionTree transformedTree in Transform(model.FunctionTree, targetVariables.Select(x => x.Data))) {
        Scope exprScope = new Scope();
        scope.AddSubScope(exprScope);
        exprScope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FunctionTree"), new GeneticProgrammingModel(transformedTree)));
      }

      return null;
    }

    private static IEnumerable<IFunctionTree> Transform(IFunctionTree networkDescription, IEnumerable<string> targetVariables) {
      // bind open parameters of network to target variables
      //IFunctionTree openExpression = RemoveOpenParameters(networkDescription);
      IFunctionTree paritallyEvaluatedOpenExpression = ApplyMetaFunctions((IFunctionTree)networkDescription.Clone());
      IFunctionTree boundExpression = BindVariables(paritallyEvaluatedOpenExpression, targetVariables.GetEnumerator());

      // create a new sub-scope for each target variable with the transformed expression
      foreach (var targetVariable in targetVariables) {
        yield return TransformExpression(boundExpression, targetVariable);
      }
    }

    private static IFunctionTree ApplyMetaFunctions(IFunctionTree tree) {
      IFunctionTree root = ApplyCycles(tree);
      List<IFunctionTree> subTrees = new List<IFunctionTree>(root.SubTrees);
      while (root.SubTrees.Count > 0) root.RemoveSubTree(0);

      foreach (IFunctionTree subTree in subTrees) {
        root.AddSubTree(ApplyFlips(subTree));
      }
      return root;
    }

    private static IFunctionTree ApplyFlips(IFunctionTree tree) {
      if (tree.SubTrees.Count == 0) {
        return tree;
      } else if (tree.Function is Flip) {
        return InvertFunction(tree.SubTrees[0]);
      } else {
        IFunctionTree tmp = ApplyFlips(tree.SubTrees[0]);
        tree.RemoveSubTree(0); tree.AddSubTree(tmp);
        return tree;
      }
    }

    private static IFunctionTree ApplyCycles(IFunctionTree tree) {
      int nRotations = 0;
      while (tree.Function is Cycle) {
        nRotations++;
        tree = tree.SubTrees[0];
      }
      if (nRotations > 0 && nRotations % tree.SubTrees.Count > 0) {
        IFunctionTree[] subTrees = tree.SubTrees.ToArray();
        while (tree.SubTrees.Count > 0) tree.RemoveSubTree(0);

        nRotations = nRotations % subTrees.Length;
        Array.Reverse(subTrees, 0, nRotations);
        Array.Reverse(subTrees, nRotations, subTrees.Length - nRotations);
        Array.Reverse(subTrees, 0, subTrees.Length);

        for (int i = 0; i < subTrees.Length; i++) {
          tree.AddSubTree(subTrees[i]);
        }
      }
      return tree;
    }

    private static IFunctionTree InvertFunction(IFunctionTree tree) {
      IFunctionTree invertedNode = null;
      if (tree.Function is OpenParameter || tree.Function is Variable) {
        return tree;
      } else if (tree.Function is AdditionF1) {
        invertedNode = (new SubtractionF1()).GetTreeNode();
        invertedNode.AddSubTree(tree.SubTrees[1]);
      } else if (tree.Function is DivisionF1) {
        invertedNode = (new MultiplicationF1()).GetTreeNode();
        invertedNode.AddSubTree(tree.SubTrees[1]);
      } else if (tree.Function is MultiplicationF1) {
        invertedNode = (new DivisionF1()).GetTreeNode();
        invertedNode.AddSubTree(tree.SubTrees[1]);
      } else if (tree.Function is SubtractionF1) {
        invertedNode = (new AdditionF1()).GetTreeNode();
        invertedNode.AddSubTree(tree.SubTrees[1]);
      } else if (tree.Function is OpenExp) {
        invertedNode = (new OpenLog()).GetTreeNode();
      } else if (tree.Function is OpenLog) {
        invertedNode = (new OpenLog()).GetTreeNode();
      } else if (tree.Function is OpenSqrt) {
        invertedNode = (new OpenSqr()).GetTreeNode();
      } else {
        throw new ArgumentException();
      }
      IFunctionTree invertedTail = ApplyFlips(tree.SubTrees[0]);
      if (invertedTail.Function is OpenParameter || invertedTail.Function is Variable) {
        invertedNode.InsertSubTree(0, invertedTail);
        return invertedNode;
      } else {
        return AppendLeft(invertedTail, invertedNode);
      }
    }

    private static IFunctionTree AppendLeft(IFunctionTree tree, IFunctionTree node) {
      IFunctionTree originalTree = tree;
      while (tree.SubTrees[0].SubTrees.Count > 0) tree = tree.SubTrees[0];
      tree.InsertSubTree(0, node);
      return originalTree;
    }

    private static IFunctionTree TransformExpression(IFunctionTree tree, string targetVariable) {
      if (tree.SubTrees.Count >= 3) {
        int targetIndex = -1;
        IFunctionTree combinator;
        List<IFunctionTree> subTrees = new List<IFunctionTree>(tree.SubTrees);
        //while (tree.SubTrees.Count > 0) tree.RemoveSubTree(0);
        if (HasTargetVariable(subTrees[0], targetVariable)) {
          targetIndex = 0;
          combinator = FunctionFromCombinator(tree);
        } else {
          for (int i = 1; i < subTrees.Count; i++) {
            if (HasTargetVariable(subTrees[i], targetVariable)) {
              targetIndex = i;
              break;
            }
          }
          combinator = FunctionFromCombinator(InvertCombinator(tree));
        }
        // not found
        if (targetIndex == -1) throw new InvalidOperationException();
        IFunctionTree targetChain = TransformToFunction(InvertFunction(subTrees[targetIndex]));
        for (int i = 0; i < subTrees.Count; i++) {
          if (i != targetIndex)
            combinator.AddSubTree(TransformToFunction(subTrees[i]));
        }
        if (targetChain.Function is Variable) return combinator;
        else {
          AppendLeft(targetChain, combinator);
          return targetChain;
        }
      }
      throw new NotImplementedException();
    }

    private static IFunctionTree TransformToFunction(IFunctionTree tree) {
      if (tree.SubTrees.Count == 0) return tree;
      else if (tree.Function is AdditionF1) {
        var addTree = (new Addition()).GetTreeNode();
        foreach (var subTree in tree.SubTrees) {
          addTree.AddSubTree(TransformToFunction(subTree));
        }
        return addTree;
      } else if (tree.Function is SubtractionF1) {
        var sTree = (new Subtraction()).GetTreeNode();
        foreach (var subTree in tree.SubTrees) {
          sTree.AddSubTree(TransformToFunction(subTree));
        }
        return sTree;
      } else if (tree.Function is MultiplicationF1) {
        var mulTree = (new Multiplication()).GetTreeNode();
        foreach (var subTree in tree.SubTrees) {
          mulTree.AddSubTree(TransformToFunction(subTree));
        }
        return mulTree;
      } else if (tree.Function is DivisionF1) {
        var divTree = (new Division()).GetTreeNode();
        foreach (var subTree in tree.SubTrees) {
          divTree.AddSubTree(TransformToFunction(subTree));
        }
        return divTree;
      } else if (tree.Function is OpenExp) {
        var expTree = (new Exponential()).GetTreeNode();
        expTree.AddSubTree(TransformToFunction(tree.SubTrees[0]));
        return expTree;
      } else if (tree.Function is OpenLog) {
        var logTree = (new Logarithm()).GetTreeNode();
        logTree.AddSubTree(TransformToFunction(tree.SubTrees[0]));
        return logTree;
      } else if (tree.Function is OpenSqr) {
        var powTree = (new Power()).GetTreeNode();
        powTree.AddSubTree(TransformToFunction(tree.SubTrees[0]));
        var const2 = (ConstantFunctionTree)(new Constant()).GetTreeNode();
        const2.Value = 2.0;
        powTree.AddSubTree(const2);
        return powTree;
      } else if (tree.Function is OpenSqrt) {
        var sqrtTree = (new Sqrt()).GetTreeNode();
        sqrtTree.AddSubTree(TransformToFunction(tree.SubTrees[0]));
        return sqrtTree;
      }
      throw new ArgumentException();
    }

    private static IFunctionTree InvertCombinator(IFunctionTree tree) {
      if (tree.Function is OpenAddition) {
        return (new OpenSubtraction()).GetTreeNode();
      } else if (tree.Function is OpenSubtraction) {
        return (new OpenAddition()).GetTreeNode();
      } else if (tree.Function is OpenMultiplication) {
        return (new OpenDivision()).GetTreeNode();
      } else if (tree.Function is OpenDivision) {
        return (new OpenMultiplication()).GetTreeNode();
      } else throw new InvalidOperationException();
    }

    private static IFunctionTree FunctionFromCombinator(IFunctionTree tree) {
      if (tree.Function is OpenAddition) {
        return (new Addition()).GetTreeNode();
      } else if (tree.Function is OpenSubtraction) {
        return (new Subtraction()).GetTreeNode();
      } else if (tree.Function is OpenMultiplication) {
        return (new Multiplication()).GetTreeNode();
      } else if (tree.Function is OpenDivision) {
        return (new Division()).GetTreeNode();
      } else throw new InvalidOperationException();
    }

    private static bool HasTargetVariable(IFunctionTree tree, string targetVariable) {
      if (tree.SubTrees.Count == 0) {
        var varTree = tree as VariableFunctionTree;
        if (varTree != null) return varTree.VariableName == targetVariable;
        else return false;
      } else return (from x in tree.SubTrees
                     where HasTargetVariable(x, targetVariable)
                     select true).Any();
    }

    private static IFunctionTree BindVariables(IFunctionTree tree, IEnumerator<string> targetVariables) {
      if (tree.Function is OpenParameter && targetVariables.MoveNext()) {
        var varTreeNode = (VariableFunctionTree)(new Variable()).GetTreeNode();
        varTreeNode.VariableName = targetVariables.Current;
        varTreeNode.SampleOffset = ((OpenParameterFunctionTree)tree).SampleOffset;
        varTreeNode.Weight = 1.0;
        return varTreeNode;
      } else {
        IList<IFunctionTree> subTrees = new List<IFunctionTree>(tree.SubTrees);
        while (tree.SubTrees.Count > 0) tree.RemoveSubTree(0);
        foreach (IFunctionTree subTree in subTrees) {
          tree.AddSubTree(BindVariables(subTree, targetVariables));
        }
        return tree;
      }
    }
  }
}
