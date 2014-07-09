#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Tests {
  [TestClass]
  public class ArgumentCreaterTest {
    private const int POPULATION_SIZE = 1000;
    private const int MAX_TREE_LENGTH = 100;
    private const int MAX_TREE_DEPTH = 10;

    [TestMethod]
    [TestCategory("Encodings.SymbolicExpressionTree")]
    [TestProperty("Time", "long")]
    public void ArgumentCreaterDistributionsTest() {
      var trees = new List<ISymbolicExpressionTree>();
      var grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister(31415);
      int failedOps = 0;
      for (int i = 0; i < POPULATION_SIZE; i++) {
        ISymbolicExpressionTree tree;
        do {
          tree = ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_LENGTH, MAX_TREE_DEPTH);
          SubroutineCreater.CreateSubroutine(random, tree, MAX_TREE_LENGTH, MAX_TREE_DEPTH, 3, 3);
        } while (!TreeHasAdfWithParameter(tree, 3));
        var success = ArgumentCreater.CreateNewArgument(random, tree, 60000, 100, 3, 3);
        if (!success) failedOps++;
        Util.IsValid(tree);
        trees.Add(tree);
      }
      // difficult to make sure that create argument operations succeed because trees are macro-expanded can potentially become very big 
      // => just test if only a small proportion fails
      Assert.IsTrue(failedOps < POPULATION_SIZE * 0.05); // only 5% may fail
      Console.WriteLine("ArgumentCreator: " + Environment.NewLine +
        "Failed operations: " + failedOps * 100.0 / POPULATION_SIZE + " %" + Environment.NewLine +
        Util.GetSizeDistributionString(trees, 200, 20) + Environment.NewLine +
        Util.GetFunctionDistributionString(trees) + Environment.NewLine +
        Util.GetNumberOfSubtreesDistributionString(trees) + Environment.NewLine +
        Util.GetTerminalDistributionString(trees) + Environment.NewLine
        );
    }

    private bool TreeHasAdfWithParameter(ISymbolicExpressionTree tree, int maxParameters) {
      if (tree.Root.Subtrees.Count() != 2) return false;
      var firstAdf = tree.Root.GetSubtree(1);
      return firstAdf.Grammar.GetAllowedChildSymbols(firstAdf.Symbol, 0).Where(x => x is Argument).Count() < maxParameters;
    }
  }
}
