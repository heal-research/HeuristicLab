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
  public class GrowTreeCreatorTest {
    private const int POPULATION_SIZE = 10000;
    private const int MAX_TREE_DEPTH = 10;
    private const int MAX_TREE_LENGTH = 50;
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
    public void GrowTreeCreatorDistributionsTest() {
      var randomTrees = new List<ISymbolicExpressionTree>();
      var grammar = Grammars.CreateSimpleArithmeticGrammar();
      var random = new MersenneTwister(31415);
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i != POPULATION_SIZE; i++) {
        randomTrees.Add(GrowTreeCreator.Create(random, grammar, MAX_TREE_LENGTH, MAX_TREE_DEPTH));
      }
      stopwatch.Stop();

      int count = 0;
      int depth = 0;
      int maxLength = 0;
      foreach (var tree in randomTrees) {
        Util.IsValid(tree);
        if (maxLength < tree.Length)
          maxLength = tree.Length;
        count += tree.Length;
        depth += tree.Depth;
      }

      double msPerRandomTreeCreation = stopwatch.ElapsedMilliseconds / (double)POPULATION_SIZE;

      Console.WriteLine("GrowTreeCreator: " + Environment.NewLine +
        msPerRandomTreeCreation + " ms per random tree (~" + Math.Round(1000.0 / (msPerRandomTreeCreation)) + "random trees / s)" + Environment.NewLine +
        Util.GetSizeDistributionString(randomTrees, maxLength, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(randomTrees) + Environment.NewLine +
        Util.GetNumberOfSubtreesDistributionString(randomTrees) + Environment.NewLine +
        Util.GetTerminalDistributionString(randomTrees) + Environment.NewLine +
        "Average tree depth: " + depth / POPULATION_SIZE + Environment.NewLine +
        "Average tree length: " + count / POPULATION_SIZE + Environment.NewLine +
        "Total nodes created: " + count + Environment.NewLine
        );
      Assert.IsTrue(Math.Round(1000.0 / (msPerRandomTreeCreation)) > 300); // must achieve more than 300 random trees / s
    }
  }
}
