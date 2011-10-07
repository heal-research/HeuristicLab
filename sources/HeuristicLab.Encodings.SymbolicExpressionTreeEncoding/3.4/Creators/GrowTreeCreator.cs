#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("GrowTreeCreator", "An operator that creates new symbolic expression trees using the 'Grow' method")]
  public class GrowTreeCreator : SymbolicExpressionTreeCreator,
                                 ISymbolicExpressionTreeSizeConstraintOperator,
                                 ISymbolicExpressionTreeGrammarBasedOperator {
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    private const string SymbolicExpressionTreeGrammarParameterName = "SymbolicExpressionTreeGrammar";
    private const string ClonedSymbolicExpressionTreeGrammarParameterName = "ClonedSymbolicExpressionTreeGrammar";

    #region Parameter Properties
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }

    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }

    public IValueLookupParameter<ISymbolicExpressionGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueLookupParameter<ISymbolicExpressionGrammar>)Parameters[SymbolicExpressionTreeGrammarParameterName]; }
    }

    public ILookupParameter<ISymbolicExpressionGrammar> ClonedSymbolicExpressionTreeGrammarParameter {
      get {
        return
            (ILookupParameter<ISymbolicExpressionGrammar>)
            Parameters[ClonedSymbolicExpressionTreeGrammarParameterName];
      }
    }

    #endregion
    #region Properties
    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; }
    }

    public ISymbolicExpressionGrammar SymbolicExpressionTreeGrammar {
      get { return ClonedSymbolicExpressionTreeGrammarParameter.ActualValue; }
    }

    #endregion

    [StorableConstructor]
    protected GrowTreeCreator(bool deserializing) : base(deserializing) { }
    protected GrowTreeCreator(GrowTreeCreator original, Cloner cloner) : base(original, cloner) { }

    public GrowTreeCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "The maximal length (number of nodes) of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
      Parameters.Add(new ValueLookupParameter<ISymbolicExpressionGrammar>(SymbolicExpressionTreeGrammarParameterName, "The tree grammar that defines the correct syntax of symbolic expression trees that should be created."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionGrammar>(ClonedSymbolicExpressionTreeGrammarParameterName, "An immutable clone of the concrete grammar that is actually used to create and manipulate trees."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GrowTreeCreator(this, cloner);
    }

    public override IOperation Apply() {
      if (ClonedSymbolicExpressionTreeGrammarParameter.ActualValue == null) {
        SymbolicExpressionTreeGrammarParameter.ActualValue.ReadOnly = true;
        IScope globalScope = ExecutionContext.Scope;
        while (globalScope.Parent != null)
          globalScope = globalScope.Parent;

        globalScope.Variables.Add(new Variable(ClonedSymbolicExpressionTreeGrammarParameterName, (ISymbolicExpressionGrammar)SymbolicExpressionTreeGrammarParameter.ActualValue.Clone()));
      }
      return base.Apply();
    }

    protected override ISymbolicExpressionTree Create(IRandom random) {
      return Create(random, SymbolicExpressionTreeGrammar, MaximumSymbolicExpressionTreeDepth.Value);
    }

    /// <summary>
    /// Create a symbolic expression tree using the 'Grow' method.
    /// All symbols are allowed for nodes, so the resulting trees can be of any shape and size. 
    /// </summary>
    /// <param name="random">Random generator</param>
    /// <param name="grammar">Available tree grammar</param>
    /// <param name="maxTreeDepth">Maximum tree depth</param>
    /// <returns></returns>
    public static ISymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeDepth) {
      var tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
      rootNode.SetGrammar(new SymbolicExpressionTreeGrammar(grammar));
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);

      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      startNode.SetGrammar(new SymbolicExpressionTreeGrammar(grammar));
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);

      rootNode.AddSubtree(startNode);

      Grow(random, startNode, maxTreeDepth - 2);
      tree.Root = rootNode;
      return tree;
    }

    public static void Grow(IRandom random, ISymbolicExpressionTreeNode seedNode, int maxDepth) {
      // make sure it is possible to create a trees smaller than maxDepth
      if (seedNode.Grammar.GetMinimumExpressionDepth(seedNode.Symbol) > maxDepth)
        throw new ArgumentException("Cannot create trees of depth " + maxDepth + " or smaller because of grammar constraints.", "maxDepth");

      var arity = SampleArity(random, seedNode);
      if (arity <= 0) return; // maybe throw an exception here? But this should never happen.

      var possibleSymbols = seedNode.Grammar.GetAllowedChildSymbols(seedNode.Symbol).Where(s => s.InitialFrequency > 0.0).ToList();

      for (var i = 0; i != arity; ++i) {
        var selectedSymbol = possibleSymbols.SelectRandom(random);
        var tree = selectedSymbol.CreateTreeNode();
        if (tree.HasLocalParameters) tree.ResetLocalParameters(random);
        seedNode.AddSubtree(tree);
      }

      foreach (var subTree in seedNode.Subtrees) {
        RecursiveGrow(random, subTree, 0, maxDepth);
      }
    }

    public static void RecursiveGrow(IRandom random, ISymbolicExpressionTreeNode root, int currentDepth, int maxDepth) {
      var arity = SampleArity(random, root);
      if (arity <= 0) return;

      var possibleSymbols = currentDepth < maxDepth ?
        root.Grammar.GetAllowedChildSymbols(root.Symbol).Where(s => s.InitialFrequency > 0.0).ToList() :
        root.Grammar.GetAllowedChildSymbols(root.Symbol).Where(s => s.InitialFrequency > 0.0 && root.Grammar.GetMaximumSubtreeCount(s) == 0).ToList();

      for (var i = 0; i != arity; ++i) {
        var selectedSymbol = possibleSymbols.SelectRandom(random);
        var tree = selectedSymbol.CreateTreeNode();
        if (tree.HasLocalParameters) tree.ResetLocalParameters(random);
        root.AddSubtree(tree);
      }

      foreach (var subTree in root.Subtrees) {
        RecursiveGrow(random, subTree, currentDepth + 1, maxDepth);
      }
    }

    private static int SampleArity(IRandom random, ISymbolicExpressionTreeNode node) {
      var minArity = node.Grammar.GetMinimumSubtreeCount(node.Symbol);
      var maxArity = node.Grammar.GetMaximumSubtreeCount(node.Symbol);

      return random.Next(minArity, maxArity + 1);
    }
  }
}