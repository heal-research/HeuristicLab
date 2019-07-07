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

using HeuristicLab.Data;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.RealVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for SimulatedBinaryCrossoverTest and is intended
  ///to contain all SimulatedBinaryCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SimulatedBinaryCrossoverTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    [TestCategory("Encodings.RealVector")]
    [TestProperty("Time", "short")]
    public void SimulatedBinaryCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      RealVector parent1, parent2, expected, actual;
      DoubleValue contiguity;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.3, 0.9, 0.7, 0.2, 0.8, 0.1, 0.2, 0.3, 0.4, 0.8, 0.7 };
      contiguity = new DoubleValue(0.3);
      parent1 = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      parent2 = new RealVector(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      expected = new RealVector(new double[] { 0.644880972204315, 0.0488239539275703, 0.3, 0.5, 0.1 });
      actual = SimulatedBinaryCrossover.Apply(random, parent1, parent2, contiguity);
      Assert.IsTrue(Auxiliary.RealVectorIsAlmostEqualByPosition(actual, expected));
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.3, 0.9, 0.7, 0.2, 0.8, 0.1, 0.2, 0.3, 0.4, 0.8, 0.7 };
      contiguity = new DoubleValue(0.3);
      parent1 = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1, 0.9 }); // this parent is longer
      parent2 = new RealVector(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      exceptionFired = false;
      try {
        actual = SimulatedBinaryCrossover.Apply(random, parent1, parent2, contiguity);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.3, 0.9, 0.7, 0.2, 0.8, 0.1, 0.2, 0.3, 0.4, 0.8, 0.7 };
      contiguity = new DoubleValue(-0.3);  //  contiguity < 0
      parent1 = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      parent2 = new RealVector(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      exceptionFired = false;
      try {
        actual = SimulatedBinaryCrossover.Apply(random, parent1, parent2, contiguity);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
