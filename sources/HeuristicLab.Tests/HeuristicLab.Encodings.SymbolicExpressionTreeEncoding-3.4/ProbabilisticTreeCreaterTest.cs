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
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._4.Tests {
  [TestClass]
  public class ProbabilisticTreeCreaterTest {
    private const int POPULATION_SIZE = 10000;
    private const int MAX_TREE_LENGTH = 100;
    private const int MAX_TREE_DEPTH = 10;
    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    [TestMethod()]
    public void ProbabilisticTreeCreaterDistributionsTest() {
      var randomTrees = new List<ISymbolicExpressionTree>();
      var grammar = Grammars.CreateSimpleArithmeticGrammar();
      var random = new MersenneTwister(31415);
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i < POPULATION_SIZE; i++) {
        randomTrees.Add(ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_LENGTH, MAX_TREE_DEPTH));
      }
      stopwatch.Stop();

      int count = 0;
      int depth = 0;
      foreach (var tree in randomTrees) {
        Util.IsValid(tree);
        count += tree.Length;
        depth += tree.Depth;
      }
      double msPerRandomTreeCreation = stopwatch.ElapsedMilliseconds / (double)POPULATION_SIZE;

      Console.WriteLine("ProbabilisticTreeCreator: " + Environment.NewLine +
        msPerRandomTreeCreation + " ms per random tree (~" + Math.Round(1000.0 / (msPerRandomTreeCreation)) + "random trees / s)" + Environment.NewLine +
        Util.GetSizeDistributionString(randomTrees, 105, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(randomTrees) + Environment.NewLine +
        Util.GetNumberOfSubtreesDistributionString(randomTrees) + Environment.NewLine +
        Util.GetTerminalDistributionString(randomTrees) + Environment.NewLine +
        "Average tree depth: " + depth / POPULATION_SIZE + Environment.NewLine +
        "Average tree length: " + count / POPULATION_SIZE + Environment.NewLine +
        "Total nodes created: " + count + Environment.NewLine
        );
      Assert.IsTrue(Math.Round(1000.0 / (msPerRandomTreeCreation)) > 250); // must achieve more than 250 random trees / s
    }

    [TestMethod]
    public void ProbabilisticTreeCreatorSpecificTreeSizesTest() {
      var trees = new List<ISymbolicExpressionTree>();
      var grammar = Grammars.CreateSimpleArithmeticGrammar();
      var random = new MersenneTwister(31415);
      var treeGrammarType = SymbolicExpressionTreeGrammar_Accessor.ShadowedType.ReferencedType;


      for (int targetTreeSize = 1; targetTreeSize <= 100; targetTreeSize++) {
        var tree = new SymbolicExpressionTree();
        var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
        rootNode.SetGrammar((ISymbolicExpressionTreeGrammar)Activator.CreateInstance(treeGrammarType, grammar));
        if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
        var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
        startNode.SetGrammar((ISymbolicExpressionTreeGrammar)Activator.CreateInstance(treeGrammarType, grammar));
        if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);
        rootNode.AddSubtree(startNode);

        ProbabilisticTreeCreator_Accessor.TryCreateFullTreeFromSeed(random, startNode, targetTreeSize, ((int)Math.Log(targetTreeSize, 2)) + 1);
        tree.Root = rootNode;
        Assert.AreEqual(targetTreeSize + 2, tree.Length);  //the root and start node must be additionally added
      }
    }
  }
}
