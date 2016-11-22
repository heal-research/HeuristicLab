#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.RealVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for RandomConvexCrossoverTest and is intended
  ///to contain all RandomConvexCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class RandomConvexCrossoverTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    [TestCategory("Encodings.RealVector")]
    [TestProperty("Time", "short")]
    public void RandomConvexCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      RealVector parent1, parent2, expected, actual;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.3 };
      parent1 = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      parent2 = new RealVector(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      expected = new RealVector(new double[] { 0.34, 0.13, 0.3, 0.29, 0.59 });
      actual = RandomConvexCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(Auxiliary.RealVectorIsAlmostEqualByPosition(actual, expected));
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.3 };
      parent1 = new RealVector(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1, 0.9 }); // this parent is longer
      parent2 = new RealVector(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      exceptionFired = false;
      try {
        actual = RandomConvexCrossover.Apply(random, parent1, parent2);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
