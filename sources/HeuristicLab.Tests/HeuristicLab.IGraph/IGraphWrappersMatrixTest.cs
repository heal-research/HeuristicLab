#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.IGraph.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  [TestClass]
  public class IGraphWrappersMatrixTest {
    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersMatrixConstructionAndFinalizationTest() {
      using (var matrix = new Matrix(3, 2)) {
        Assert.AreEqual(3, matrix.Rows);
        Assert.AreEqual(2, matrix.Columns);
        Assert.AreEqual(0, matrix[0, 0]);
        matrix[0, 0] = 4;
        var other = new Matrix(matrix);
        Assert.AreEqual(3, other.Rows);
        Assert.AreEqual(2, other.Columns);
        Assert.AreEqual(4, other[0, 0]);
      }
      var mat = new double[,] {
        { 1, 2, 3 },
        { 4, 5, 6}
      };
      using (var matrix = new Matrix(mat)) {
        Assert.AreEqual(2, matrix.Rows);
        Assert.AreEqual(3, matrix.Columns);
        var test = matrix.ToMatrix();
        for (var i = 0; i < matrix.Rows; i++)
          for (var j = 0; j < matrix.Columns; j++) {
            Assert.AreEqual(mat[i, j], matrix[i, j]);
            Assert.AreEqual(mat[i, j], test[i, j]);
          }
      }
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersMatrixGetSetTest() {
      using (var matrix = new Matrix(3, 2)) {
        matrix[0, 0] = matrix[0, 1] = 4;
        matrix[1, 0] = 3;
        matrix[1, 1] = 2;
        matrix[2, 0] = 1.5;
        matrix[2, 1] = -0.5;
        Assert.AreEqual(4, matrix[0, 0]);
        Assert.AreEqual(4, matrix[0, 1]);
        Assert.AreEqual(3, matrix[1, 0]);
        Assert.AreEqual(2, matrix[1, 1]);
        Assert.AreEqual(1.5, matrix[2, 0]);
        Assert.AreEqual(-0.5, matrix[2, 1]);

        var netmat = matrix.ToMatrix();
        Assert.AreEqual(3, netmat.GetLength(0));
        Assert.AreEqual(2, netmat.GetLength(1));
        for (var i = 0; i < netmat.GetLength(0); i++)
          for (var j = 0; j < netmat.GetLength(1); j++)
            Assert.AreEqual(matrix[i, j], netmat[i, j]);
      }
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersMatrixFillTest() {
      using (var matrix = new Matrix(3, 2)) {
        matrix.Fill(2.6);
        Assert.AreEqual(2.6, matrix[0, 0]);
        Assert.AreEqual(2.6, matrix[0, 1]);
        Assert.AreEqual(2.6, matrix[1, 0]);
        Assert.AreEqual(2.6, matrix[1, 1]);
        Assert.AreEqual(2.6, matrix[2, 0]);
        Assert.AreEqual(2.6, matrix[2, 1]);
      }
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersMatrixTransposeTest() {
      using (var matrix = new Matrix(3, 2)) {
        matrix.Transpose();
        Assert.AreEqual(2, matrix.Rows);
        Assert.AreEqual(3, matrix.Columns);
      }
    }

    [TestMethod]
    [TestCategory("ExtLibs")]
    [TestCategory("ExtLibs.igraph")]
    [TestProperty("Time", "short")]
    public void IGraphWrappersMatrixScaleTest() {
      using (var matrix = new Matrix(3, 2)) {
        matrix[0, 0] = matrix[0, 1] = 4;
        matrix[1, 0] = 3;
        matrix[1, 1] = 2;
        matrix[2, 0] = 1.5;
        matrix[2, 1] = -0.5;
        matrix.Scale(2);
        Assert.AreEqual(8, matrix[0, 0]);
        Assert.AreEqual(8, matrix[0, 1]);
        Assert.AreEqual(6, matrix[1, 0]);
        Assert.AreEqual(4, matrix[1, 1]);
        Assert.AreEqual(3, matrix[2, 0]);
        Assert.AreEqual(-1, matrix[2, 1]);
      }
    }
  }
}
