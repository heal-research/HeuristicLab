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

using HeuristicLab.Core;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.IntegerVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for SinglePointCrossoverTest and is intended
  ///to contain all SinglePointCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SinglePointCrossoverTest {
    /// <summary>
    ///A test for Cross
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.IntegerVector")]
    [TestProperty("Time", "short")]
    public void SinglePointCrossoverCrossTest() {
      var target = new PrivateObject(typeof(SinglePointCrossover));
      ItemArray<IntegerVector> parents;
      TestRandom random = new TestRandom();
      bool exceptionFired;
      // The following test checks if there is an exception when there are more than 2 parents
      random.Reset();
      parents = new ItemArray<IntegerVector>(new IntegerVector[] { new IntegerVector(5), new IntegerVector(6), new IntegerVector(4) });
      exceptionFired = false;
      try {
        IntegerVector actual;
        actual = (IntegerVector)target.Invoke("Cross", random, parents);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
      // The following test checks if there is an exception when there are less than 2 parents
      random.Reset();
      parents = new ItemArray<IntegerVector>(new IntegerVector[] { new IntegerVector(4) });
      exceptionFired = false;
      try {
        IntegerVector actual;
        actual = (IntegerVector)target.Invoke("Cross", random, parents);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.IntegerVector")]
    [TestProperty("Time", "short")]
    public void SinglePointCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      IntegerVector parent1, parent2, expected, actual;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.IntNumbers = new int[] { 3 };
      parent1 = new IntegerVector(new int[] { 2, 2, 3, 5, 1 });
      parent2 = new IntegerVector(new int[] { 4, 1, 3, 2, 8 });
      expected = new IntegerVector(new int[] { 2, 2, 3, 2, 8 });
      actual = SinglePointCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(Auxiliary.IntegerVectorIsEqualByPosition(actual, expected));
      // The following test is not based on published examples
      random.Reset();
      random.IntNumbers = new int[] { 2 };
      parent1 = new IntegerVector(new int[] { 2, 2, 3, 5, 1, 9 }); // this parent is longer
      parent2 = new IntegerVector(new int[] { 4, 1, 3, 2, 8 });
      exceptionFired = false;
      try {
        actual = SinglePointCrossover.Apply(random, parent1, parent2);
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
