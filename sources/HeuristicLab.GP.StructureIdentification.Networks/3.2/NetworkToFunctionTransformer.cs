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
        yield return TransformExpression(boundExpression, targetVariable, targetVariables.Except(new string[] { targetVariable }));
      }
    }

    /// <summary>
    /// applies all tree-transforming meta functions (= cycle and flip)
    /// precondition: root is a F2 function (possibly cycle) and the tree contains 0 or n flip functions, each branch has an openparameter symbol in the bottom left
    /// postconditon: root is any F2 function (but cycle) and the tree doesn't contains any flips, each branch has an openparameter symbol in the bottom left
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static IFunctionTree ApplyMetaFunctions(IFunctionTree tree) {
      return ApplyFlips(ApplyCycles(tree));
    }

    private static IFunctionTree ApplyFlips(IFunctionTree tree) {
      if (tree.SubTrees.Count == 0) {
        return tree;
      } else if (tree.Function is Flip) {
        var partiallyAppliedBranch = ApplyFlips(tree.SubTrees[0]);
        if (partiallyAppliedBranch.Function is OpenParameter) return partiallyAppliedBranch;
        else return InvertChain(partiallyAppliedBranch);
      } else {
        List<IFunctionTree> subTrees = new List<IFunctionTree>(tree.SubTrees);
        while (tree.SubTrees.Count > 0) tree.RemoveSubTree(0);
        foreach (var subTree in subTrees) {
          tree.AddSubTree(ApplyFlips(subTree));
        }
        return tree;
      }
    }

    /// <summary>
    ///  inverts and reverses chain of functions.
    ///  precondition: tree is any F1 non-terminal that ends with an openParameter
    ///  postcondition: tree is inverted and reversed chain of F1 non-terminals and ends with an openparameter.
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static IFunctionTree InvertChain(IFunctionTree tree) {
      List<IFunctionTree> currentChain = new List<IFunctionTree>(IterateChain(tree));
      // get a list of function trees from bottom to top
      List<IFunctionTree> reversedChain = new List<IFunctionTree>(currentChain.Reverse<IFunctionTree>().Skip(1));
      IFunctionTree openParam = currentChain.Last();

      // build new tree by inverting every function in the reversed chain and keeping f0 branches untouched.
      IFunctionTree parent = reversedChain[0];
      IFunctionTree invParent = GetInvertedFunction(parent.Function).GetTreeNode();
      for (int j = 1; j < parent.SubTrees.Count; j++) {
        invParent.AddSubTree(parent.SubTrees[j]);
      }
      IFunctionTree root = invParent;
      for (int i = 1; i < reversedChain.Count(); i++) {
        IFunctionTree child = reversedChain[i];
        IFunctionTree invChild = GetInvertedFunction(child.Function).GetTreeNode();
        invParent.InsertSubTree(0, invChild);

        parent = child;
        invParent = invChild;
        for (int j = 1; j < parent.SubTrees.Count; j++) {
          invParent.AddSubTree(parent.SubTrees[j]);
        }
      }
      // append open param at the end
      invParent.InsertSubTree(0, openParam);
      return root;
    }

    private static IEnumerable<IFunctionTree> IterateChain(IFunctionTree tree) {
      while (tree.SubTrees.Count > 0) {
        yield return tree;
        tree = tree.SubTrees[0];
      }
      yield return tree;
    }


    private static Dictionary<Type, IFunction> invertedFunction = new Dictionary<Type, IFunction>() {
      { typeof(AdditionF1), new SubtractionF1() },
      { typeof(SubtractionF1), new AdditionF1() },
      { typeof(MultiplicationF1), new DivisionF1() },
      { typeof(DivisionF1), new MultiplicationF1() },
      { typeof(OpenLog), new OpenExp() },
      { typeof(OpenExp), new OpenLog() },
      //{ typeof(OpenSqr), new OpenSqrt() },
      //{ typeof(OpenSqrt), new OpenSqr() },
      { typeof(Flip), new Flip()},
      { typeof(Addition), new Subtraction()},
      { typeof(Subtraction), new Addition()},
      { typeof(Multiplication), new Division()},
      { typeof(Division), new Multiplication()},
      { typeof(Exponential), new Logarithm()},
      { typeof(Logarithm), new Exponential()}
    };
    private static IFunction GetInvertedFunction(IFunction function) {
      return invertedFunction[function.GetType()];
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



    private static IFunctionTree AppendLeft(IFunctionTree tree, IFunctionTree node) {
      IFunctionTree originalTree = tree;
      while (!IsBottomLeft(tree)) tree = tree.SubTrees[0];
      tree.InsertSubTree(0, node);
      return originalTree;
    }

    private static bool IsBottomLeft(IFunctionTree tree) {
      if (tree.SubTrees.Count == 0) return true;
      else if (tree.SubTrees[0].Function is Variable) return true;
      else if (tree.SubTrees[0].Function is Constant) return true;
      else return false;
    }

    /// <summary>
    /// recieves a function tree transforms it into a function-tree for the given target variable
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="targetVariable"></param>
    /// <returns></returns>
    private static IFunctionTree TransformExpression(IFunctionTree tree, string targetVariable, IEnumerable<string> parameters) {
      if (tree.Function is Variable || tree.Function is Constant || tree.Function is Differential) return tree;
      if (tree.Function is Addition || tree.Function is Subtraction ||
          tree.Function is Multiplication || tree.Function is Division ||
          tree.Function is Exponential || tree.Function is Logarithm) {
        var occuringVariables = from x in FunctionTreeIterator.IteratePrefix(tree)
                                where x is VariableFunctionTree
                                let name = ((VariableFunctionTree)x).VariableName
                                select name;
        var openParameters = (new string[] { targetVariable }).Concat(parameters);
        var missingVariables = openParameters.Except(occuringVariables);
        if (missingVariables.Count() > 0) {
          VariableFunctionTree varTree = (VariableFunctionTree)(new Variable()).GetTreeNode();
          varTree.VariableName = missingVariables.First();
          varTree.SampleOffset = 0;
          varTree.Weight = 1.0;
          tree = (IFunctionTree)tree.Clone();
          tree.InsertSubTree(0, varTree);
        }
      }
      int targetIndex = -1;
      IFunctionTree combinator = null;
      List<IFunctionTree> subTrees = new List<IFunctionTree>(tree.SubTrees);
      if (HasTargetVariable(subTrees[0], targetVariable)) {
        targetIndex = 0;
        combinator = FunctionFromCombinator(tree).GetTreeNode();
      } else {
        for (int i = 1; i < subTrees.Count; i++) {
          if (HasTargetVariable(subTrees[i], targetVariable)) {
            targetIndex = i;
            combinator = GetInvertedFunction(FunctionFromCombinator(tree)).GetTreeNode();
            break;
          }
        }
      }
      if (targetIndex == -1) {
        // target variable was not found
        return tree;
      } else {
        // target variable was found
        for (int i = 0; i < subTrees.Count; i++) {
          if (i != targetIndex)
            combinator.AddSubTree(subTrees[i]);
        }
        if (subTrees[targetIndex].Function is Variable) return combinator;
        else {
          IFunctionTree bottomLeft;
          IFunctionTree targetChain = InvertF0Chain(subTrees[targetIndex], out bottomLeft);
          bottomLeft.InsertSubTree(0, combinator);
          return targetChain;
        }
      }
    }

    // inverts a chain of F0 functions 
    // precondition: left bottom is a variable (the selected target variable)
    // postcondition: the chain is inverted. the target variable is removed
    private static IFunctionTree InvertF0Chain(IFunctionTree tree, out IFunctionTree bottomLeft) {
      List<IFunctionTree> currentChain = IterateChain(tree).ToList();

      List<IFunctionTree> reversedChain = currentChain.Reverse<IFunctionTree>().Skip(1).ToList();

      // build new tree by inverting every function in the reversed chain and keeping f0 branches untouched.
      IFunctionTree parent = reversedChain[0];
      IFunctionTree invParent = GetInvertedFunction(parent.Function).GetTreeNode();
      for (int j = 1; j < parent.SubTrees.Count; j++) {
        invParent.AddSubTree(parent.SubTrees[j]);
      }
      IFunctionTree root = invParent;
      for (int i = 1; i < reversedChain.Count(); i++) {
        IFunctionTree child = reversedChain[i];
        IFunctionTree invChild = GetInvertedFunction(child.Function).GetTreeNode();
        invParent.InsertSubTree(0, invChild);
        parent = child;
        invParent = invChild;
        for (int j = 1; j < parent.SubTrees.Count; j++) {
          invParent.AddSubTree(parent.SubTrees[j]);
        }
      }
      bottomLeft = invParent;
      return root;
    }



    //private static IFunctionTree InvertCombinator(IFunctionTree tree) {
    //  if (tree.Function is OpenAddition) {
    //    return (new OpenSubtraction()).GetTreeNode();
    //  } else if (tree.Function is OpenSubtraction) {
    //    return (new OpenAddition()).GetTreeNode();
    //  } else if (tree.Function is OpenMultiplication) {
    //    return (new OpenDivision()).GetTreeNode();
    //  } else if (tree.Function is OpenDivision) {
    //    return (new OpenMultiplication()).GetTreeNode();
    //  } else throw new InvalidOperationException();
    //}

    private static Dictionary<Type, IFunction> combinatorFunction = new Dictionary<Type, IFunction>() {
      { typeof(OpenAddition), new Addition()},
      { typeof(OpenSubtraction), new Subtraction()},
      { typeof(OpenDivision), new Division()},
      { typeof(OpenMultiplication), new Multiplication()},
      { typeof(Addition), new Addition()},
      { typeof(Subtraction), new Subtraction()},
      { typeof(Division), new Division()},
      { typeof(Multiplication), new Multiplication()},
      { typeof(Logarithm), new Logarithm()},
      { typeof(Exponential), new Exponential()},
    };
    private static IFunction FunctionFromCombinator(IFunctionTree tree) {
      return combinatorFunction[tree.Function.GetType()];
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

    private static Dictionary<Type, IFunction> closedForm = new Dictionary<Type, IFunction>() {
      {typeof(OpenAddition), new OpenAddition()},
      {typeof(OpenSubtraction), new OpenSubtraction()},
      {typeof(OpenMultiplication), new OpenMultiplication()},
      {typeof(OpenDivision), new OpenDivision()},
      {typeof(AdditionF1), new Addition()},
      {typeof(SubtractionF1), new Subtraction()},
      {typeof(MultiplicationF1), new Multiplication()},
      {typeof(DivisionF1), new Division()},
      {typeof(OpenExp), new Exponential()},
      {typeof(OpenLog), new Logarithm()},
      //{typeof(OpenSqr), new Power()},
      //{typeof(OpenSqrt), new Sqrt()},
      {typeof(OpenParameter), new Variable()},
    };

    /// <summary>
    /// transforms a tree that contains F2 and F1 functions into a tree composed of F2 and F0 functions.
    /// precondition: the tree doesn't contains cycle or flip symbols. the tree has openparameters in the bottom left
    /// postcondition: all F1 and functions are replaced by matching F0 functions
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="targetVariables"></param>
    /// <returns></returns>
    private static IFunctionTree BindVariables(IFunctionTree tree, IEnumerator<string> targetVariables) {
      if (!closedForm.ContainsKey(tree.Function.GetType())) return tree;
      IFunction matchingFunction = closedForm[tree.Function.GetType()];
      IFunctionTree matchingTree = matchingFunction.GetTreeNode();
      if (matchingFunction is Variable) {
        targetVariables.MoveNext();
        var varTreeNode = (VariableFunctionTree)matchingTree;
        varTreeNode.VariableName = targetVariables.Current;
        varTreeNode.SampleOffset = ((OpenParameterFunctionTree)tree).SampleOffset;
        varTreeNode.Weight = 1.0;
        return varTreeNode;
        //} else if (matchingFunction is Power) {
        //  matchingTree.AddSubTree(BindVariables(tree.SubTrees[0], targetVariables));
        //  var const2 = (ConstantFunctionTree)(new Constant()).GetTreeNode();
        //  const2.Value = 2.0;
        //  matchingTree.AddSubTree(const2);
      } else {
        foreach (IFunctionTree subTree in tree.SubTrees) {
          matchingTree.AddSubTree(BindVariables(subTree, targetVariables));
        }
      }

      return matchingTree;
    }
  }
}
