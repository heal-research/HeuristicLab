#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.RealVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for PolynomialOnePositionManipulator and is intended
  ///to contain all PolynomialOnePositionManipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class PolynomialOnePositionManipulatorTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    [TestCategory("Encodings.RealVector")]
    [TestProperty("Time", "short")]
    public void PolynomialOnePositionManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      RealVector parent, expected;
      DoubleValue contiguity, maxManipulation;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.IntNumbers = new int[] { 3 };
      random.DoubleNumbers = new double[] { 0.2 };
      parent = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      expected = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.1261980542102, 0.1 });
      contiguity = new DoubleValue(0.2);
      maxManipulation = new DoubleValue(0.7);
      PolynomialOnePositionManipulator.Apply(random, parent, contiguity, maxManipulation);
      Assert.IsTrue(Auxiliary.RealVectorIsAlmostEqualByPosition(expected, parent));
      // The following test is not based on published examples
      exceptionFired = false;
      random.Reset();
      random.IntNumbers = new int[] { 3 };
      random.DoubleNumbers = new double[] { 0.2 };
      parent = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      contiguity = new DoubleValue(-1); //Contiguity value < 0
      maxManipulation = new DoubleValue(0.2);
      try {
        PolynomialOnePositionManipulator.Apply(random, parent, contiguity, maxManipulation);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
