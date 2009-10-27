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

using HeuristicLab.GP.StructureIdentification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.DataAnalysis;
using System;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Random;
using HeuristicLab.GP.Operators;
using System.Collections.Generic;
using System.Text;
namespace HeuristicLab.GP.Test {


  [TestClass()]
  public class ProbabilisticTreeCreatorTest {
    private const int N = 1000;
    private TestContext testContextInstance;
    private static IFunctionTree[] randomTrees;

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

    [ClassInitialize()]
    public static void CreateRandomTrees(TestContext testContext) {
      MersenneTwister twister = new MersenneTwister();
      Dataset ds = Util.CreateRandomDataset(twister, 1, 20);
      randomTrees = Util.CreateRandomTrees(twister, ds, N, 1, 100);
    }

    [TestMethod()]
    public void SizeDistributionTest() {
      int[] histogram = new int[105 / 5];
      for (int i = 0; i < randomTrees.Length; i++) {
        histogram[randomTrees[i].GetSize() / 5]++;
      }
      StringBuilder strBuilder = new StringBuilder();
      for (int i = 0; i < histogram.Length; i++) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append("< "); strBuilder.Append((i + 1) * 5);
        strBuilder.Append(": "); strBuilder.Append(histogram[i]);
      }
      Assert.Inconclusive("Size distribution of ProbabilisticTreeCreator: " + strBuilder);
    }

    [TestMethod()]
    public void FunctionDistributionTest() {
      Dictionary<IFunction, int> occurances = new Dictionary<IFunction, int>();
      for (int i = 0; i < randomTrees.Length; i++) {
        foreach (var node in FunctionTreeIterator.IteratePrefix(randomTrees[i])) {
          if (node.SubTrees.Count > 0) {
            if (!occurances.ContainsKey(node.Function))
              occurances[node.Function] = 0;
            occurances[node.Function]++;
          }
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var function in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(function.Name); strBuilder.Append(": ");
        strBuilder.Append(occurances[function]);
      }
      Assert.Inconclusive("Function distribution of ProbabilisticTreeCreator: " + strBuilder);
    }

    [TestMethod()]
    public void TerminalDistributionTest() {
      Dictionary<IFunction, int> occurances = new Dictionary<IFunction, int>();
      for (int i = 0; i < randomTrees.Length; i++) {
        foreach (var node in FunctionTreeIterator.IteratePrefix(randomTrees[i])) {
          if (node.SubTrees.Count == 0) {
            if (!occurances.ContainsKey(node.Function))
              occurances[node.Function] = 0;
            occurances[node.Function]++;
          }
        }
      }
      StringBuilder strBuilder = new StringBuilder();
      foreach (var function in occurances.Keys) {
        strBuilder.Append(Environment.NewLine);
        strBuilder.Append(function.Name); strBuilder.Append(": ");
        strBuilder.Append(occurances[function]);
      }
      Assert.Inconclusive("Terminal distribution of ProbabilisticTreeCreator: " + strBuilder);
    }
  }
}
