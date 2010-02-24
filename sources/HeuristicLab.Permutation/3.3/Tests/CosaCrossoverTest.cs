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

using HeuristicLab.Permutation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Core;

namespace HeuristicLab.Permutation.Tests
{
    
    
    /// <summary>
    ///This is a test class for CosaCrossoverTest and is intended
    ///to contain all CosaCrossoverTest Unit Tests
    ///</summary>
  [TestClass()]
  public class CosaCrossoverTest {


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
    [DeploymentItem("HeuristicLab.Permutation-3.3.dll")]
    public void CosaCrossoverCrossTest() {
      TestRandom random = new TestRandom();
      CosaCrossover_Accessor target = 
        new CosaCrossover_Accessor(new PrivateObject(typeof(CosaCrossover)));
      // perform a test with more than two parents
      random.Reset();
      bool exceptionFired = false;
      try {
        target.Cross(random, new ItemArray<Permutation>(new Permutation[] { 
          new Permutation(4), new Permutation(4), new Permutation(4)}));
      } catch (System.InvalidOperationException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    public void CosaCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent1, parent2, expected, actual;
      random.Reset();
      random.IntNumbers = new int[] { 4 };
      parent1 = new Permutation(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
      Assert.IsTrue(parent1.Validate());
      parent2 = new Permutation(new int[] { 1, 3, 5, 7, 6, 4, 2, 0 });
      Assert.IsTrue(parent2.Validate());
      expected = new Permutation(new int[] { 6, 5, 7, 0, 1, 2, 3, 4 });
      Assert.IsTrue(expected.Validate());
      actual = CosaCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(actual.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, actual));

      // perform a test when the two permutations are of unequal length
      random.Reset();
      bool exceptionFired = false;
      try {
        CosaCrossover.Apply(random, new Permutation(8), new Permutation(6));
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for CosaCrossover Constructor
    ///</summary>
    [TestMethod()]
    public void CosaCrossoverConstructorTest() {
      CosaCrossover target = new CosaCrossover();
    }
  }
}
