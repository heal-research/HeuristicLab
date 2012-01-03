#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._4.Tests {
  [TestClass]
  public class AllArchitectureAlteringOperatorsTest {
    private const int POPULATION_SIZE = 1000;
    private const int N_ITERATIONS = 200;
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
    [Timeout(3600000)]
    public void AllArchitectureAlteringOperatorsDistributionTest() {
      var trees = new List<ISymbolicExpressionTree>();
      var newTrees = new List<ISymbolicExpressionTree>();
      var grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister(31415);
      SymbolicExpressionTreeStringFormatter formatter = new SymbolicExpressionTreeStringFormatter();
      IntValue maxTreeSize = new IntValue(MAX_TREE_LENGTH);
      IntValue maxTreeHeigth = new IntValue(MAX_TREE_DEPTH);
      IntValue maxDefuns = new IntValue(3);
      IntValue maxArgs = new IntValue(3);
      for (int i = 0; i < POPULATION_SIZE; i++) {
        var tree = ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_LENGTH, MAX_TREE_DEPTH);
        Util.IsValid(tree);
        trees.Add(tree);
      }
      Stopwatch stopwatch = new Stopwatch();
      int failedEvents = 0;
      for (int g = 0; g < N_ITERATIONS; g++) {
        for (int i = 0; i < POPULATION_SIZE; i++) {
          if (random.NextDouble() < 0.5) {
            // manipulate
            stopwatch.Start();
            var selectedTree = (ISymbolicExpressionTree)trees.SelectRandom(random).Clone();
            var oldTree = (ISymbolicExpressionTree)selectedTree.Clone();
            bool success = false;
            int sw = random.Next(6);
            switch (sw) {
              case 0: success = ArgumentCreater.CreateNewArgument(random, selectedTree, MAX_TREE_LENGTH, MAX_TREE_DEPTH, 3, 3); break;
              case 1: success = ArgumentDeleter.DeleteArgument(random, selectedTree, 3, 3); break;
              case 2: success = ArgumentDuplicater.DuplicateArgument(random, selectedTree, 3, 3); break;
              case 3: success = SubroutineCreater.CreateSubroutine(random, selectedTree, MAX_TREE_LENGTH, MAX_TREE_DEPTH, 3, 3); break;
              case 4: success = SubroutineDuplicater.DuplicateSubroutine(random, selectedTree, 3, 3); break;
              case 5: success = SubroutineDeleter.DeleteSubroutine(random, selectedTree, 3, 3); break;
            }
            stopwatch.Stop();
            if (!success) failedEvents++;
            Util.IsValid(selectedTree);
            newTrees.Add(selectedTree);
          } else {
            stopwatch.Start();
            // crossover
            SymbolicExpressionTree par0 = null;
            SymbolicExpressionTree par1 = null;
            do {
              par0 = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
              par1 = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
            } while (par0.Length > MAX_TREE_LENGTH || par1.Length > MAX_TREE_LENGTH);
            var newTree = SubtreeCrossover.Cross(random, par0, par1, 0.9, MAX_TREE_LENGTH, MAX_TREE_DEPTH);
            stopwatch.Stop();
            Util.IsValid(newTree);
            newTrees.Add(newTree);
          }
        }
        trees = new List<ISymbolicExpressionTree>(newTrees);
        newTrees.Clear();
      }
      var msPerOperation = stopwatch.ElapsedMilliseconds / ((double)POPULATION_SIZE * (double)N_ITERATIONS);
      Console.WriteLine("AllArchitectureAlteringOperators: " + Environment.NewLine +
        "Operations / s: ~" + Math.Round(1000.0 / (msPerOperation)) + "operations / s)" + Environment.NewLine +
        "Failed events: " + failedEvents * 100.0 / (double)(POPULATION_SIZE * N_ITERATIONS / 2.0) + "%" + Environment.NewLine +
        Util.GetSizeDistributionString(trees, 200, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(trees) + Environment.NewLine +
        Util.GetNumberOfSubtreesDistributionString(trees) + Environment.NewLine +
        Util.GetTerminalDistributionString(trees) + Environment.NewLine
        );

      Assert.IsTrue(failedEvents * 100.0 / (POPULATION_SIZE * N_ITERATIONS / 2.0) < 75.0); // 25% of architecture operations must succeed
      Assert.IsTrue(Math.Round(1000.0 / (msPerOperation)) > 800); // must achieve more than 800 ops per second
    }
  }
}
