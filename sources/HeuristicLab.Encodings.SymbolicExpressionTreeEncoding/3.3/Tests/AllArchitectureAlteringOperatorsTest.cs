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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureAlteringOperators;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Crossovers;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators;

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
      int failedEvents = 0;
      IntValue maxTreeSize = new IntValue(100);
      IntValue maxTreeHeigth = new IntValue(10);
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
      for (int g = 0; g < N_ITERATIONS; g++) {
        for (int i = 0; i < POPULATION_SIZE; i++) {
          if (random.NextDouble() < 0.5) {
            // manipulate
            var selectedTree = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
            var op = combinedAAOperator.Operators.SelectRandom(random);
            bool success = false;
            op.ModifyArchitecture(random, selectedTree, grammar, maxTreeSize, maxTreeHeigth, maxDefuns, maxArgs, out success);
            if (!success) failedEvents++;
            Util.IsValid(selectedTree);
            newTrees.Add(selectedTree);
          } else {
            // crossover
            var par0 = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
            var par1 = (SymbolicExpressionTree)trees.SelectRandom(random).Clone();
            bool success;
            newTrees.Add(SubtreeCrossover.Cross(random, par0, par1, 0.9, 100, 10, out success));
            if (!success) failedEvents++;
          }
        }
        trees = newTrees;
      }
      stopwatch.Stop();
      var msPerOperation = stopwatch.ElapsedMilliseconds / (double)POPULATION_SIZE / (double)N_ITERATIONS;
      Assert.Inconclusive("AllArchitectureAlteringOperators: " + Environment.NewLine +
        "Failed events: " + failedEvents / (double)POPULATION_SIZE / N_ITERATIONS * 100 + " %" + Environment.NewLine +
        "Operations / s: ~" + Math.Round(1000.0 / (msPerOperation)) + "operations / s)" + Environment.NewLine +
        Util.GetSizeDistributionString(trees, 200, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(trees) + Environment.NewLine +
        Util.GetNumberOfSubTreesDistributionString(trees) + Environment.NewLine +
        Util.GetTerminalDistributionString(trees) + Environment.NewLine
        );
    }
  }
}
