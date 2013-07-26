#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.BinaryVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for SinglePositionBitflipManipulator and is intended
  ///to contain all SinglePositionBitflipManipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class SinglePositionBitflipManipulatorTest {


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
    [TestMethod]
    public void SinglePositionBitflipManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      BinaryVector parent, expected;
      // The following test is based on Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg, p. 21.
      random.Reset();
      random.IntNumbers = new int[] { 4 };
      parent = new BinaryVector(new bool[] { true, true, true, false, false, false, false, false, false, 
        false, true, true, true, true, true, true, false, false, false, true, false, true});
      expected = new BinaryVector(new bool[] { true, true, true, false, true, false, false, false, false, 
        false, true, true, true, true, true, true, false, false, false, true, false, true});
      SinglePositionBitflipManipulator.Apply(random, parent);
      Assert.IsTrue(Auxiliary.BinaryVectorIsEqualByPosition(expected, parent));
    }

    /// <summary>
    ///A test for SinglePositionBitflipManipulator Constructor
    ///</summary>
    [TestMethod]
    public void SinglePositionBitflipManipulatorConstructorTest() {
      SinglePositionBitflipManipulator target = new SinglePositionBitflipManipulator();
    }
  }
}
