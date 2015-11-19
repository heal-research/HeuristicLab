#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Tests {
  [TestClass]
  public class ProbabilisticTreeCreatorTest {
    private const int POPULATION_SIZE = 10000;
    private const int MAX_TREE_LENGTH = 100;
    private const int MAX_TREE_DEPTH = 10;

    [TestMethod()]
    [TestCategory("Encodings.SymbolicExpressionTree")]
    [TestProperty("Time", "long")]
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
      //mkommend: commented due to performance issues on the builder
      // Assert.IsTrue(Math.Round(1000.0 / (msPerRandomTreeCreation)) > 250); // must achieve more than 250 random trees / s
    }

    [TestMethod]
    [TestCategory("Encodings.SymbolicExpressionTree")]
    [TestProperty("Time", "short")]
    public void ProbabilisticTreeCreatorSpecificTreeSizesTest() {
      var grammar = Grammars.CreateSimpleArithmeticGrammar();
      var random = new MersenneTwister(31415);

      for (int targetTreeSize = 3; targetTreeSize <= 100; targetTreeSize++) {
        var tree = ProbabilisticTreeCreator.CreateExpressionTree(random, grammar, targetTreeSize, ((int)Math.Log(targetTreeSize, 2)) + 2);
        Assert.AreEqual(targetTreeSize, tree.Length);
      }
    }
  }
}
