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

using System.Linq;
using HeuristicLab.IGraph.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class IGraphWrappersVectorTest {
    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorConstructionAndFinalization() {
      var vector = new Vector(7);
      Assert.AreEqual(7, vector.Length);
      Assert.AreEqual(0, vector[0]);
      vector[0] = 4;
      var other = new Vector(vector);
      Assert.AreEqual(7, other.Length);
      Assert.AreEqual(4, other[0]);

      var myvec = new double[] { 1, 2, 3 };
      vector = new Vector(myvec);
      Assert.AreEqual(3, vector.Length);
      Assert.AreEqual(myvec[0], vector[0]);
      Assert.AreEqual(myvec[1], vector[1]);
      Assert.AreEqual(myvec[2], vector[2]);
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorGetSetTest() {
      var vector = new Vector(5);
      vector[0] = vector[1] = 4;
      vector[2] = 3;
      vector[3] = 1.5;
      vector[4] = -0.5;
      Assert.AreEqual(4, vector[0]);
      Assert.AreEqual(4, vector[1]);
      Assert.AreEqual(3, vector[2]);
      Assert.AreEqual(1.5, vector[3]);
      Assert.AreEqual(-0.5, vector[4]);

      var netmat = vector.ToArray();
      Assert.AreEqual(5, netmat.Length);
      for (var i = 0; i < netmat.Length; i++)
        Assert.AreEqual(vector[i], netmat[i]);
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorFillTest() {
      var vector = new Vector(5);
      vector.Fill(2.3);
      Assert.IsTrue(new[] { 2.3, 2.3, 2.3, 2.3, 2.3 }.SequenceEqual(vector.ToArray()));
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorReverseTest() {
      var vector = new Vector(5);
      vector[0] = vector[1] = 4;
      vector[2] = 3;
      vector[3] = 1.5;
      vector[4] = -0.5;
      vector.Reverse();
      Assert.IsTrue(new[] { -0.5, 1.5, 3, 4, 4 }.SequenceEqual(vector.ToArray()));
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorShuffleTest() {
      var vector = new Vector(5);
      vector[0] = vector[1] = 4;
      vector[2] = 3;
      vector[3] = 1.5;
      vector[4] = -0.5;
      vector.Shuffle();
      Assert.IsFalse(new[] { -0.5, 1.5, 3, 4, 4 }.SequenceEqual(vector.ToArray()));
      Assert.IsFalse(new[] { 4, 4, 3, 1.5, -0.5 }.SequenceEqual(vector.ToArray()));
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorScaleTest() {
      var vector = new Vector(5);
      vector[0] = vector[1] = 4;
      vector[2] = 3;
      vector[3] = 1.5;
      vector[4] = -0.5;
      vector.Scale(2);
      Assert.IsTrue(new double[] { 8, 8, 6, 3, -1 }.SequenceEqual(vector.ToArray()));
    }
  }
}
