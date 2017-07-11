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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.IGraph.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class IGraphWrappersVectorTest {
    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorConstructionAndFinalizationTest() {
      using (var vector = new Vector(7)) {
        Assert.AreEqual(7, vector.Length);
        Assert.AreEqual(0, vector[0]);
        vector[0] = 4;
        using (var other = new Vector(vector)) {
          Assert.AreEqual(7, other.Length);
          Assert.AreEqual(4, other[0]);
        }
      }
      var myvec = new double[] { 1, 2, 3 };
      using (var vector = new Vector(myvec)) {
        Assert.AreEqual(3, vector.Length);
        Assert.AreEqual(myvec[0], vector[0]);
        Assert.AreEqual(myvec[1], vector[1]);
        Assert.AreEqual(myvec[2], vector[2]);
      }
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorGetSetTest() {
      using (var vector = new Vector(5)) {
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
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorFillTest() {
      using (var vector = new Vector(5)) {
        vector.Fill(2.3);
        Assert.IsTrue(new[] { 2.3, 2.3, 2.3, 2.3, 2.3 }.SequenceEqual(vector.ToArray()));
      }
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorReverseTest() {
      using (var vector = new Vector(5)) {
        vector[0] = vector[1] = 4;
        vector[2] = 3;
        vector[3] = 1.5;
        vector[4] = -0.5;
        vector.Reverse();
        Assert.IsTrue(new[] { -0.5, 1.5, 3, 4, 4 }.SequenceEqual(vector.ToArray()));
      }
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorShuffleTest() {
      var different = new HashSet<RealVector>(new RealVectorEqualityComparer());
      for (var i = 0; i < 100; i++) {
        using (var vector = new Vector(5)) {
          vector[0] = vector[1] = 4;
          vector[2] = 3;
          vector[3] = 1.5;
          vector[4] = -0.5;
          vector.Shuffle();
          var result = vector.ToArray();
          different.Add(new RealVector(result));
          Assert.AreEqual(2, result.Count(x => x == 4));
          Assert.AreEqual(1, result.Count(x => x == 3));
          Assert.AreEqual(1, result.Count(x => x == 1.5));
          Assert.AreEqual(1, result.Count(x => x == -0.5));
        }
      }
      // There should be reasonable low probability that all 100 shuffles result in exactly the same vector
      Assert.IsTrue(different.Count > 1);
      Assert.IsTrue(different.Count <= 60); // there are a total of 60 different shuffles 5! / 2!
      Console.WriteLine("Shuffle produced " + different.Count + " unique vectors");
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersVectorScaleTest() {
      using (var vector = new Vector(5)) {
        vector[0] = vector[1] = 4;
        vector[2] = 3;
        vector[3] = 1.5;
        vector[4] = -0.5;
        vector.Scale(2);
        Assert.IsTrue(new double[] { 8, 8, 6, 3, -1 }.SequenceEqual(vector.ToArray()));
      }
    }
  }
}
