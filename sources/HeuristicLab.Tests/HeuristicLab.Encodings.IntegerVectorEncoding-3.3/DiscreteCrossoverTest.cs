#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.IntegerVectorEncoding_33.Tests {


  /// <summary>
  ///This is a test class for DiscreteCrossoverTest and is intended
  ///to contain all DiscreteCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class DiscreteCrossoverTest {


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

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion

    /// <summary>
    ///A test for Cross
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HeuristicLab.Encodings.IntegerVectorEncoding-3.3.dll")]
    public void DiscreteCrossoverCrossTest() {
      DiscreteCrossover_Accessor target = new DiscreteCrossover_Accessor(new PrivateObject(typeof(DiscreteCrossover)));
      ItemArray<IntegerVector> parents;
      TestRandom random = new TestRandom();
      bool exceptionFired;
      // The following test checks if there is an exception when there are less than 2 parents
      random.Reset();
      parents = new ItemArray<IntegerVector>(new IntegerVector[] { new IntegerVector(4) });
      exceptionFired = false;
      try {
        IntegerVector actual;
        actual = target.Cross(random, parents);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    public void DiscreteCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      IntegerVector parent1, parent2, expected, actual;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0, 0, 0.9, 0, 0.9 };
      parent1 = new IntegerVector(new int[] { 2, 2, 3, 5, 1 });
      parent2 = new IntegerVector(new int[] { 4, 1, 3, 2, 8 });
      expected = new IntegerVector(new int[] { 2, 2, 3, 5, 8 });
      actual = DiscreteCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(Auxiliary.IntegerVectorIsEqualByPosition(actual, expected));

      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0, 0, 0.9, 0, 0.9 };
      parent1 = new IntegerVector(new int[] { 2, 2, 3, 5, 1, 9 }); // this parent is longer
      parent2 = new IntegerVector(new int[] { 4, 1, 3, 2, 8 });
      exceptionFired = false;
      try {
        actual = DiscreteCrossover.Apply(random, parent1, parent2);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for DiscreteCrossover Constructor
    ///</summary>
    [TestMethod()]
    public void DiscreteCrossoverConstructorTest() {
      DiscreteCrossover target = new DiscreteCrossover();
    }
  }
}
