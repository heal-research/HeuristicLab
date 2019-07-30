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

using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.BinaryVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for SinglePointCrossoverTest and is intended
  ///to contain all SinglePointCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class UniformCrossoverTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.BinaryVector")]
    [TestProperty("Time", "short")]
    public void SinglePointCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      BinaryVector parent1, parent2, expected, actual;
      bool exceptionFired;
      // The following test is based on Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg, p. 49
      random.Reset();
      random.DoubleNumbers = new double[] { 0.35, 0.62, 0.18, 0.42, 0.83, 0.76, 0.39, 0.51, 0.36 };
      parent1 = new BinaryVector(new bool[] { false, false, false, false, true, false, false, false, false });
      parent2 = new BinaryVector(new bool[] { true, true, false, true, false, false, false, false, true });
      expected = new BinaryVector(new bool[] { false, true, false, false, false, false, false, false, false });
      actual = UniformCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(Auxiliary.BinaryVectorIsEqualByPosition(actual, expected));

      // The following test is based on Eiben, A.E. and Smith, J.E. 2003. Introduction to Evolutionary Computation. Natural Computing Series, Springer-Verlag Berlin Heidelberg, p. 49
      random.Reset();
      random.DoubleNumbers = new double[] { 0.35, 0.62, 0.18, 0.42, 0.83, 0.76, 0.39, 0.51, 0.36 };
      parent2 = new BinaryVector(new bool[] { false, false, false, false, true, false, false, false, false });
      parent1 = new BinaryVector(new bool[] { true, true, false, true, false, false, false, false, true });
      expected = new BinaryVector(new bool[] { true, false, false, true, true, false, false, false, true });
      actual = UniformCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(Auxiliary.BinaryVectorIsEqualByPosition(actual, expected));

      // The following test is not based on any published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.35, 0.62, 0.18, 0.42, 0.83, 0.76, 0.39, 0.51, 0.36 };
      parent1 = new BinaryVector(new bool[] { false, true, true, false, false }); // this parent is longer
      parent2 = new BinaryVector(new bool[] { false, true, true, false });
      exceptionFired = false;
      try {
        actual = UniformCrossover.Apply(random, parent1, parent2);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
