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

using System.Linq;
using HeuristicLab.Algorithms.ParameterlessPopulationPyramid;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ParameterlessPopulationPyramid.Test {
  [TestClass]
  public class LinkageTreeTest {
    private static int Length = 9;
    private static bool[][] solutions = new bool[][] {
    new bool[] { true, true, false, false, false, false, false, false, false }, // 110000000
    new bool[] { false, false, true, true, false, false, false, false, false }, // 001100000
    new bool[] { false, false, false, false, true, true, false, false, false }, // 000011000
    new bool[] { false, false, false, false, false, false, true, true, true },  // 000000111
    new bool[] { true, true, true, true, false, false, false, false, false },   // 111100000
    new bool[] { true, true, true, true, true, true, true, true, true },        // 111111111
    };

    // These are the clusters that should be built using "solutions" and the seed 123
    private static int[][] correctClusters = new int[][] {
      new int[] { 4, 5 },
      new int[] { 2, 3 },
      new int[] { 0, 1 },
      new int[] { 6, 7, 8 },
      new int[] { 0, 1, 2, 3},
      new int[] { 4, 5, 6, 7, 8},
    };

    [TestMethod]
    [TestProperty("Time", "short")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void LinkageTreeTestAdd() {
      MersenneTwister rand = new MersenneTwister();
      LinkageTree tree = new LinkageTree(Length, rand);
      tree.Add(solutions[0]);
      tree.Add(solutions[1]);
      PrivateObject hidden = new PrivateObject(tree);
      int[][][] result = (int[][][])hidden.GetField("occurances");
      Assert.AreEqual(1, result[1][0][0]); // Positions 0 and 1 had value 00 exactly once
      Assert.AreEqual(2, result[Length - 1][Length - 2][0]); // Positions 0 and 1 had value 00 exactly once
      Assert.AreEqual(0, result[Length - 1][Length - 2][1]); // Positions 7 and 8 never had value 10
      Assert.AreEqual(1, result[1][0][3]); // Positions 0 and 1 had value 11 exactly once
    }

    [TestMethod]
    [TestProperty("Time", "short")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void LinkageTreeTestEntropyDistance() {
      MersenneTwister rand = new MersenneTwister();
      LinkageTree tree = new LinkageTree(Length, rand);
      PrivateObject hidden = new PrivateObject(tree);
      // No information should result in a distance of 0
      Assert.AreEqual((double)0, hidden.Invoke("EntropyDistance", new object[] { 0, 1 }));
      foreach (var solution in solutions) {
        tree.Add(solution);
      }
      // Check that 0 and 1 are closer than 0 and 2
      var linked = (double)hidden.Invoke("EntropyDistance", new object[] { 0, 1 });
      var unlinked = (double)hidden.Invoke("EntropyDistance", new object[] { 0, 2 });
      Assert.IsTrue(linked < unlinked);

      // Reversing the arguments should not change the result
      var forward = hidden.Invoke("EntropyDistance", new object[] { Length - 1, Length - 2 });
      var backward = hidden.Invoke("EntropyDistance", new object[] { Length - 2, Length - 1 });
      Assert.AreEqual(forward, backward);
    }

    [TestMethod]
    [TestProperty("Time", "short")]
    [TestCategory("Algorithms.ParameterlessPopulationPyramid")]
    public void LinkageTreeTestRebuild() {
      // The seed matters as equal sized clusters can appear in any order
      MersenneTwister rand = new MersenneTwister(123);
      LinkageTree tree = new LinkageTree(Length, rand);
      foreach (var solution in solutions) {
        tree.Add(solution);
      }

      // Check if the clusters created contain the expected variables.
      var found = tree.Clusters.ToArray();
      Assert.AreEqual(correctClusters.Length, found.Length);
      for (int i = 0; i < found.Length; i++) {
        found[i].Sort();
        Assert.IsTrue(found[i].SequenceEqual(correctClusters[i]), string.Format("Failed On Cluster {0}", i));
      }
    }
  }
}
