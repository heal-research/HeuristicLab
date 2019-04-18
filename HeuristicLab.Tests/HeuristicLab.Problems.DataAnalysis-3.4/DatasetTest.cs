#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.DataAnalysis.Tests {
  [TestClass]
  public class DatasetTest {
    private TestContext testContextInstance;
    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get { return testContextInstance; }
      set { testContextInstance = value; }
    }

    [TestMethod]
    [TestCategory("Problems.DataAnalysis")]
    [TestProperty("Time", "short")]
    public void TestImportRowiseData() {
      var matrix = new double[,] { { 1, 2, 3 }, { 2, 3, 1 }, { 3, 1, 2 }, { 1, 2, 3 }, { 2, 3, 1 } };
      var ds = Dataset.FromRowData(new[] { "X1", "X2", "X3" }, matrix);
      Assert.AreEqual(3, ds.Columns);
      Assert.AreEqual(5, ds.Rows);
      Assert.IsTrue(ds.GetDoubleValues("X1").SequenceEqual(new double[] { 1, 2, 3, 1, 2 }));

      var listoflits = new List<List<double>>() {
        new List<double>() { 1, 2, 3 },
        new List<double>() { 2, 3, 1 },
        new List<double>() { 3, 1, 2 },
        new List<double>() { 1, 2, 3 },
        new List<double>() { 2, 3, 1 }
      };
      ds = Dataset.FromRowData(new[] { "X1", "X2", "X3" }, listoflits);
      Assert.AreEqual(3, ds.Columns);
      Assert.AreEqual(5, ds.Rows);
      Assert.IsTrue(ds.GetDoubleValues("X1").SequenceEqual(new double[] { 1, 2, 3, 1, 2 }));

      var complexData = new List<IList>() {
        new object[] { 1.0, new DateTime(2019, 1, 1), "a" },
        new object[] { 2.0, new DateTime(2019, 1, 2), "b" },
        new object[] { 3.0, new DateTime(2019, 1, 3), "c" },
        new object[] { 4.0, new DateTime(2019, 1, 4), "d" },
        new object[] { 5.0, new DateTime(2019, 1, 5), "e" },
      };
      ds = Dataset.FromRowData(new[] { "X1", "X2", "X3" }, complexData);
      Assert.AreEqual(3, ds.Columns);
      Assert.AreEqual(5, ds.Rows);
      Assert.IsTrue(ds.GetDoubleValues("X1").SequenceEqual(new double[] { 1, 2, 3, 4, 5 }));
      Assert.IsTrue(ds.GetDateTimeValues("X2").SequenceEqual(Enumerable.Range(1, 5).Select(d => new DateTime(2019, 1, d))));
      Assert.IsTrue(ds.GetStringValues("X3").SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));
    }
  }
}
