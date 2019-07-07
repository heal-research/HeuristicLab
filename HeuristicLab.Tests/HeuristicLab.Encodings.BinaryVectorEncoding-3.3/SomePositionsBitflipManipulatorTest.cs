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

namespace HeuristicLab.Encodings.BinaryVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for SomePositionsBitflipManipulator and is intended
  ///to contain all SomePositionsBitflipManipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class SomePositionsBitflipManipulatorTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.BinaryVector")]
    [TestProperty("Time", "short")]
    public void SomePositionsBitflipManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      BinaryVector parent, expected;
      DoubleValue pm;
      // The following test is based on Michalewicz, Z. 1999. Genetic Algorithms + Data Structures = Evolution Programs. Third, Revised and Extended Edition, Spring-Verlag Berlin Heidelberg, p. 21.
      random.Reset();
      random.DoubleNumbers = new double[] { 0.3, 0.3, 0.1, 0.1, 0.3, 0.3, 0.3, 0.1, 0.3 };
      pm = new DoubleValue(0.2);
      parent = new BinaryVector(new bool[] { true, false, true, false, false, false, false, true, false });
      expected = new BinaryVector(new bool[] { true, false, false, true, false, false, false, false, false });
      SomePositionsBitflipManipulator.Apply(random, parent, pm);
      Assert.IsTrue(Auxiliary.BinaryVectorIsEqualByPosition(expected, parent));
    }
  }
}
