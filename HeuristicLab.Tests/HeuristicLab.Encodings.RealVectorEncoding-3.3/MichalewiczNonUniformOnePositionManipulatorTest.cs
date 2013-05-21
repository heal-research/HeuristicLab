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

using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.RealVectorEncoding_33.Tests {
  /// <summary>
  ///This is a test class for MichalewiczNonUniformOnePositionManipulator and is intended
  ///to contain all MichalewiczNonUniformOnePositionManipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class MichalewiczNonUniformOnePositionManipulatorTest {


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
    public void MichalewiczNonUniformOnePositionManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      RealVector parent, expected;
      DoubleValue generationsDependency;
      DoubleMatrix bounds;
      IntValue currentGeneration, maximumGenerations;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.IntNumbers = new int[] { 3 };
      random.DoubleNumbers = new double[] { 0.2, 0.7 };
      parent = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      expected = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.34, 0.1 });
      bounds = new DoubleMatrix(new double[,] { { 0.3, 0.7 } });
      generationsDependency = new DoubleValue(0.1);
      currentGeneration = new IntValue(1);
      maximumGenerations = new IntValue(4);
      MichalewiczNonUniformOnePositionManipulator.Apply(random, parent, bounds, currentGeneration, maximumGenerations, generationsDependency);
      Assert.IsTrue(Auxiliary.RealVectorIsAlmostEqualByPosition(expected, parent));
      // The following test is not based on published examples
      exceptionFired = false;
      random.Reset();
      random.IntNumbers = new int[] { 3 };
      random.DoubleNumbers = new double[] { 0.2, 0.7 };
      parent = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      bounds = new DoubleMatrix(new double[,] { { 0.3, 0.7 } });
      generationsDependency = new DoubleValue(0.1);
      currentGeneration = new IntValue(5); //current generation > max generation
      maximumGenerations = new IntValue(4);
      try {
        MichalewiczNonUniformOnePositionManipulator.Apply(random, parent, bounds, currentGeneration, maximumGenerations, generationsDependency);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for MichalewiczNonUniformOnePositionManipulator Constructor
    ///</summary>
    [TestMethod()]
    public void MichalewiczNonUniformOnePositionManipulatorConstructorTest() {
      MichalewiczNonUniformOnePositionManipulator target = new MichalewiczNonUniformOnePositionManipulator();
    }
  }
}
