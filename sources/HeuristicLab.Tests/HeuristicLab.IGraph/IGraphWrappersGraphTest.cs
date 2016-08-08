#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.IGraph.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class IGraphWrappersGraphTest {
    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersGraphConstructionAndFinalization() {
      var graph = new Graph(5, new[] {
        Tuple.Create(0, 1),
        Tuple.Create(0, 2),
        Tuple.Create(1, 2),
        Tuple.Create(2, 3),
        Tuple.Create(2, 4),
        Tuple.Create(3, 4),
      });
      Assert.AreEqual(5, graph.Vertices);
      Assert.IsFalse(graph.IsDirected);

      graph = new Graph(3, new[] {
        Tuple.Create(0, 1),
        Tuple.Create(0, 2),
        Tuple.Create(1, 2),
      }, directed: true);
      Assert.AreEqual(3, graph.Vertices);
      Assert.IsTrue(graph.IsDirected);
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void TestDensity() {
      var graph = new Graph(5, new[] {
        Tuple.Create(0, 1),
        Tuple.Create(0, 2),
        Tuple.Create(1, 2),
        Tuple.Create(2, 3),
        Tuple.Create(2, 4),
        Tuple.Create(3, 4),
      });

      var density = graph.Density();
      // in un-directed graphs edges count twice
      Assert.IsTrue(density.IsAlmost(12 / 20.0));

      graph.Dispose();

      graph = new Graph(5, new[] {
        Tuple.Create(0, 1),
        Tuple.Create(0, 2),
        Tuple.Create(1, 2),
        Tuple.Create(2, 3),
        Tuple.Create(2, 4),
        Tuple.Create(3, 4),
      }, directed: true);

      density = graph.Density();
      // in directed graphs edges count twice
      Assert.IsTrue(density.IsAlmost(6 / 20.0));
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void TestPageRank() {
      var graph = new Graph(4, new[] {
        Tuple.Create(0, 1),
        Tuple.Create(0, 2),
        Tuple.Create(1, 2),
        Tuple.Create(2, 0),
        Tuple.Create(3, 2),
      }, directed: true);
      var ranks = graph.PageRank();
      Assert.AreEqual(4, ranks.Length);
      Assert.AreEqual(0.372, ranks[0], 0.01);
      Assert.AreEqual(0.195, ranks[1], 0.01);
      Assert.AreEqual(0.394, ranks[2], 0.01);
      Assert.AreEqual(0.037, ranks[3], 0.01);

      graph = new Graph(4, new[] {
        Tuple.Create(0, 1),
        Tuple.Create(1, 2),
        Tuple.Create(2, 3),
        Tuple.Create(3, 0),
      }, directed: true);
      ranks = graph.PageRank();
      Assert.AreEqual(4, ranks.Length);
      Assert.AreEqual(0.250, ranks[0], 0.01);
      Assert.AreEqual(0.250, ranks[1], 0.01);
      Assert.AreEqual(0.250, ranks[2], 0.01);
      Assert.AreEqual(0.250, ranks[3], 0.01);

      graph = new Graph(4, new[] {
        Tuple.Create(0, 1),
        Tuple.Create(0, 2),
        Tuple.Create(0, 3),
        Tuple.Create(1, 0),
        Tuple.Create(2, 0),
        Tuple.Create(3, 0),
      }, directed: true);
      ranks = graph.PageRank();
      Assert.AreEqual(4, ranks.Length);
      Assert.AreEqual(0.480, ranks[0], 0.01);
      Assert.AreEqual(0.173, ranks[1], 0.01);
      Assert.AreEqual(0.173, ranks[2], 0.01);
      Assert.AreEqual(0.173, ranks[3], 0.01);
    }
  }
}
