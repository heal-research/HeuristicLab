#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  ///This is a test class for DiscreteCrossoverTest and is intended
  ///to contain all DiscreteCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class DiscreteCrossoverTest {
    /// <summary>
    ///A test for Cross
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.IntegerVector")]
    [TestProperty("Time", "short")]
    public void DiscreteCrossoverCrossTest() {
      DiscreteCrossover_Accessor target = new DiscreteCrossover_Accessor(new PrivateObject(typeof(DiscreteCrossover)));
      ItemArray<IntegerVector> parents;
      TestRandom random = new TestRandom();
      bool exceptionFired;
      // The following test checks if there is an exception when there are less than 2 parents
      random.Reset();
      parents = new ItemArray<IntegerVector>(new IntegerVector[] { new IntegerVector(4) });
      exceptionFired = false;
      try {
        IntegerVector actual;
        actual = target.Cross(random, parents);
      } catch (System.ArgumentException) {
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
    public void DiscreteCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      IntegerVector parent1, parent2, expected, actual;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.IntNumbers = new int[] { 0, 0, 1, 0, 1 };
      parent1 = new IntegerVector(new int[] { 2, 2, 3, 5, 1 });
      parent2 = new IntegerVector(new int[] { 4, 1, 3, 2, 8 });
      expected = new IntegerVector(new int[] { 2, 2, 3, 5, 8 });
      actual = DiscreteCrossover.Apply(random, new ItemArray<IntegerVector>(new IntegerVector[] { parent1, parent2 }));
      Assert.IsTrue(Auxiliary.IntegerVectorIsEqualByPosition(actual, expected));

      // The following test is not based on published examples
      random.Reset();
      random.IntNumbers = new int[] { 0, 0, 1, 0, 1 };
      parent1 = new IntegerVector(new int[] { 2, 2, 3, 5, 1, 9 }); // this parent is longer
      parent2 = new IntegerVector(new int[] { 4, 1, 3, 2, 8 });
      exceptionFired = false;
      try {
        actual = DiscreteCrossover.Apply(random, new ItemArray<IntegerVector>(new IntegerVector[] { parent1, parent2 }));
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
