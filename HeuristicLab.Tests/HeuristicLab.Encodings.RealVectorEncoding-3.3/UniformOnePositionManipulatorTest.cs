#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.RealVectorEncoding_33.Tests {
  /// <summary>
  ///This is a test class for UniformOnePositionManipulator and is intended
  ///to contain all UniformOnePositionManipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class UniformOnePositionManipulatorTest {


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
    public void UniformOnePositionManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      RealVector parent, expected;
      DoubleMatrix bounds;
      // The following test is not based on published examples
      random.Reset();
      random.IntNumbers = new int[] { 3 };
      random.DoubleNumbers = new double[] { 0.2 };
      parent = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      expected = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.3, 0.1 });
      bounds = new DoubleMatrix(new double[,] { { 0.2, 0.7 } });
      UniformOnePositionManipulator.Apply(random, parent, bounds);
      Assert.IsTrue(Auxiliary.RealVectorIsAlmostEqualByPosition(expected, parent));
    }

    /// <summary>
    ///A test for UniformOnePositionManipulator Constructor
    ///</summary>
    [TestMethod()]
    public void UniformOnePositionManipulatorConstructorTest() {
      UniformOnePositionManipulator target = new UniformOnePositionManipulator();
    }
  }
}
