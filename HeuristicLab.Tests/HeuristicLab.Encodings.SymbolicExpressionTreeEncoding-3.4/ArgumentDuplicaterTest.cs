#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ArgumentDuplicaterTest {
    private const int POPULATION_SIZE = 1000;
    private const int MAX_TREE_LENGTH = 100;
    private const int MAX_TREE_DEPTH = 10;

    [TestMethod]
    [TestCategory("Encodings.SymbolicExpressionTree")]
    [TestProperty("Time", "long")]
    public void ArgumentDuplicaterDistributionsTest() {
      var trees = new List<ISymbolicExpressionTree>();
      var grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister(31415);
      for (int i = 0; i < POPULATION_SIZE; i++) {
        ISymbolicExpressionTree tree = null;
        do {
          tree = ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_LENGTH, MAX_TREE_DEPTH);
          SubroutineCreater.CreateSubroutine(random, tree, MAX_TREE_LENGTH, MAX_TREE_DEPTH, 3, 3);
        } while (!HasAdfWithArguments(tree));
        var success = ArgumentDuplicater.DuplicateArgument(random, tree, 3, 3);
        Assert.IsTrue(success);
        Util.IsValid(tree);
        trees.Add(tree);
      }
      Console.WriteLine("ArgumentDuplicater: " + Environment.NewLine +
        Util.GetSizeDistributionString(trees, 200, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(trees) + Environment.NewLine +
        Util.GetNumberOfSubtreesDistributionString(trees) + Environment.NewLine +
        Util.GetTerminalDistributionString(trees) + Environment.NewLine
        );
    }

    private bool HasAdfWithArguments(ISymbolicExpressionTree tree) {
      if (tree.Root.Subtrees.Count() != 2) return false;
      var firstAdf = tree.Root.GetSubtree(1);
      return firstAdf.Grammar.GetAllowedChildSymbols(firstAdf.Symbol, 0).Where(x => x is Argument).Count() == 1;
    }
  }
}
