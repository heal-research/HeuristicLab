#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.RealVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for HeuristicCrossoverTest and is intended
  ///to contain all HeuristicCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class HeuristicCrossoverTest {
    /// <summary>
    ///A test for Cross
    ///</summary>
    [TestMethod()]
    [TestCategory("Encodings.RealVector")]
    [TestProperty("Time", "short")]
    public void HeuristicCrossoverCrossTest() {
      HeuristicCrossover_Accessor target = new HeuristicCrossover_Accessor(new PrivateObject(typeof(HeuristicCrossover)));
      ItemArray<RealVector> parents;
      TestRandom random = new TestRandom();
      bool exceptionFired;
      // The following test checks if there is an exception when there are more than 2 parents
      random.Reset();
      parents = new ItemArray<RealVector>(new RealVector[] { new RealVector(5), new RealVector(6), new RealVector(4) });
      exceptionFired = false;
      try {
        RealVector actual;
        actual = target.Cross(random, parents);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
      // The following test checks if there is an exception when there are less than 2 parents
      random.Reset();
      parents = new ItemArray<RealVector>(new RealVector[] { new RealVector(4) });
      exceptionFired = false;
      try {
        RealVector actual;
        actual = target.Cross(random, parents);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    [TestCategory("Encodings.RealVector")]
    [TestProperty("Time", "short")]
    public void HeuristicCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      RealVector parent1, parent2, expected, actual;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.3 };
      parent1 = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      parent2 = new RealVector(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      expected = new RealVector(new double[] { 0.14, 0.23, 0.3, 0.59, -0.11 });
      actual = HeuristicCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(Auxiliary.RealVectorIsAlmostEqualByPosition(actual, expected));
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.3 };
      parent1 = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1, 0.9 }); // this parent is longer
      parent2 = new RealVector(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      exceptionFired = false;
      try {
        actual = HeuristicCrossover.Apply(random, parent1, parent2);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
