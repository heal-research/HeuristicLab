#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Crossovers;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  [TestClass]
  public class AllArchitectureAlteringOperatorsTest {
    private const int POPULATION_SIZE = 1000;
    private const int N_ITERATIONS = 20;
    private const int MAX_TREE_SIZE = 100;
    private const int MAX_TREE_HEIGHT = 10;
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
    public void AllArchitectureAlteringOperatorsDistributionTest() {
      var trees = new List<SymbolicExpressionTree>();
      var newTrees = new List<SymbolicExpressionTree>();
      var grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister(31415);
      IntValue maxTreeSize = new IntValue(MAX_TREE_SIZE);
      IntValue maxTreeHeigth = new IntValue(MAX_TREE_HEIGHT);
      IntValue maxDefuns = new IntValue(3);
      IntValue maxArgs = new IntValue(3);
      for (int i = 0; i < POPULATION_SIZE; i++) {
        var tree = ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_SIZE, MAX_TREE_HEIGHT, 3, 3);
        Util.IsValid(tree);
        trees.Add(tree);
      }
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      var combinedAAOperator = new MultiSymbolicExpressionTreeArchitectureManipulator();
      int failedEvents = 0;
      for (int g = 0; g < N_ITERATIONS; g++) {
        for (int i = 0; i < POPULATION_SIZE; i++) {
          if (random.NextDouble() < 0.5) {
            // manipulate
            var selectedTree = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
            var op = combinedAAOperator.Operators.SelectRandom(random);
            bool success = false;
            op.ModifyArchitecture(random, selectedTree, grammar, maxTreeSize, maxTreeHeigth, maxDefuns, maxArgs, out success);
            if (!success) failedEvents++; // architecture manipulation might fail
            Util.IsValid(selectedTree);
            newTrees.Add(selectedTree);
          } else {
            // crossover
            SymbolicExpressionTree par0 = null;
            SymbolicExpressionTree par1 = null;
            do {
              par0 = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
              par1 = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
            } while (par0.Size > MAX_TREE_SIZE || par1.Size > MAX_TREE_SIZE);
            bool success;
            newTrees.Add(SubtreeCrossover.Cross(random, par0, par1, 0.9, MAX_TREE_SIZE, MAX_TREE_HEIGHT, out success));
            Assert.IsTrue(success); // crossover must succeed
          }
        }
        trees = newTrees;
      }
      stopwatch.Stop();
      var msPerOperation = stopwatch.ElapsedMilliseconds / (double)POPULATION_SIZE / (double)N_ITERATIONS;
      Console.WriteLine("AllArchitectureAlteringOperators: " + Environment.NewLine +
        "Operations / s: ~" + Math.Round(1000.0 / (msPerOperation)) + "operations / s)" + Environment.NewLine +
        "Failed events: " + failedEvents * 100.0 / (double)(POPULATION_SIZE * N_ITERATIONS * 2.0) + "%" + Environment.NewLine +
        Util.GetSizeDistributionString(trees, 200, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(trees) + Environment.NewLine +
        Util.GetNumberOfSubTreesDistributionString(trees) + Environment.NewLine +
        Util.GetTerminalDistributionString(trees) + Environment.NewLine
        );

      Assert.IsTrue(failedEvents * 100.0 / (POPULATION_SIZE * N_ITERATIONS * 2.0) < 25.0); // 75% of architecture operations must succeed
      Assert.IsTrue(Math.Round(1000.0 / (msPerOperation)) > 1000); // must achieve more than 1000 ops per second
    }
  }
}
