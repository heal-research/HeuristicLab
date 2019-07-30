#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class IGraphLayoutTest {
    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersLayoutFruchtermanReingoldTest() {
      using (var graph = new Graph(5, new[] {
        Tuple.Create(0, 1),
        Tuple.Create(0, 2),
        Tuple.Create(1, 2),
        Tuple.Create(2, 3),
        Tuple.Create(2, 4),
        Tuple.Create(3, 4),
      })) {
        Assert.AreEqual(5, graph.Vertices);
        try {
          using (var matrix = graph.LayoutWithFruchtermanReingold()) {
            Assert.AreEqual(5, matrix.Rows);
            Assert.AreEqual(2, matrix.Columns);
          }
        } catch { Assert.Fail("Layouting with fruchterman-reingold using default parameters failed."); }

        try {
          using (var matrix = new Matrix(1, 1)) {
            graph.LayoutWithFruchtermanReingold(50, 2, matrix);
          }
          Assert.Fail("Layouting with fruchterman-reingold using too little pre-initialized coordinates failed.");
        } catch (ArgumentException) { }

        try {
          using (var matrix = new Matrix(7, 3)) {
            graph.LayoutWithFruchtermanReingold(50, 2, matrix);
            Assert.Fail("Layouting with fruchterman-reingold using too many pre-initialized coordinates failed.");
          }
        } catch (ArgumentException) { }

        try {
          using (var matrix = new Matrix(5, 2)) {
            matrix[0, 0] = matrix[0, 1] = 1;
            matrix[1, 0] = matrix[1, 1] = 2;
            matrix[2, 0] = matrix[2, 1] = 3;
            matrix[3, 0] = matrix[3, 1] = 4;
            matrix[4, 0] = matrix[4, 1] = 5;
            graph.LayoutWithFruchtermanReingold(50, 2, matrix);
            Assert.AreEqual(5, matrix.Rows);
            Assert.AreEqual(2, matrix.Columns);
            Assert.IsFalse(
                 matrix[0, 0].IsAlmost(1) && matrix[0, 1].IsAlmost(1)
              && matrix[1, 0].IsAlmost(2) && matrix[1, 1].IsAlmost(2)
              && matrix[2, 0].IsAlmost(2) && matrix[2, 1].IsAlmost(2)
              && matrix[3, 0].IsAlmost(2) && matrix[3, 1].IsAlmost(2)
              && matrix[4, 0].IsAlmost(2) && matrix[4, 1].IsAlmost(2));
          }
        } catch { Assert.Fail("Layouting with fruchterman-reingold using pre-initialized coordinates failed."); }
      }
    }
  }
}
