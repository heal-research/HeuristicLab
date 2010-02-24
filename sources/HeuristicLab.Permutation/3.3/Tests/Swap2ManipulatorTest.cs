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

namespace HeuristicLab.Permutation.Tests {


  /// <summary>
  ///This is a test class for Swap2Manipulator and is intended
  ///to contain all Swap2Manipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class Swap2ManipulatorTest {


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
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    public void Swap2ManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent, expected;
      // The following test is based on an example from Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg, p. 45
      random.Reset();
      random.IntNumbers = new int[] { 1, 4 };
      parent = new Permutation(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
      Assert.IsTrue(parent.Validate());

      expected = new Permutation(new int[] { 0, 4, 2, 3, 1, 5, 6, 7, 8 });
      Assert.IsTrue(expected.Validate());
      Swap2Manipulator.Apply(random, parent);
      Assert.IsTrue(parent.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, parent));
     
      // perform a test when the two permutations are of unequal length
      random.Reset();
      bool exceptionFired = false;
      try {
        CyclicCrossover.Apply(random, new Permutation(8), new Permutation(6));
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Swap2Manipulator Constructor
    ///</summary>
    [TestMethod()]
    public void Swap2ManipulatorConstructorTest() {
      Swap2Manipulator target = new Swap2Manipulator();
    }
  }
}
