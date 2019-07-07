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

using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Random.Tests {

  [TestClass()]
  public class RandomEnumerableSampleTest {
    [TestMethod]
    [TestCategory("General")]
    [TestProperty("Time", "short")]
    public void SampleProportionalWithoutRepetitionTest() {
      {
        // select 1 of 100 uniformly (weights = 0)
        var items = Enumerable.Range(0, 100);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(0.0, 100);
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 1, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 1);
        }
      }
      {
        // select 1 of 1 uniformly (weights = 0)
        var items = Enumerable.Range(0, 1);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(0.0, 1);
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 1, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 1);
        }
      }
      {
        // select 1 of 2 non-uniformly (weights = 1, 2)
        var items = Enumerable.Range(0, 2);
        var random = new MersenneTwister(31415);
        var weights = new double[] { 1.0, 2.0 };
        var zeroSelected = 0;
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 1, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 1);
          if (sample[0] == 0) zeroSelected++;
        }
        Assert.IsTrue(zeroSelected > 0 && zeroSelected < 1000);
      }
      {
        // select 2 of 2 non-uniformly (weights = 1, 1000)
        var items = Enumerable.Range(0, 2);
        var random = new MersenneTwister(31415);
        var weights = new double[] { 1.0, 1000.0 };
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 2, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 2);
          Assert.AreEqual(sample.Distinct().Count(), 2);
        }
      }
      {
        // select 2 from 1 uniformly (weights = 0), this does not throw an exception but instead returns a sample with 1 element!
        var items = Enumerable.Range(0, 1);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(0.0, 1);
        var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 2, weights, false, false).ToArray();
        Assert.AreEqual(sample.Count(), 1);
      }

      {
        // select 10 of 100 uniformly (weights = 0)
        var items = Enumerable.Range(0, 100);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(0.0, 100);
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 10, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 10);
          Assert.AreEqual(sample.Distinct().Count(), 10);
        }
      }

      {
        // select 100 of 100 uniformly (weights = 0)
        var items = Enumerable.Range(0, 100);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(0.0, 100);
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 100, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 100);
          Assert.AreEqual(sample.Distinct().Count(), 100);
        }
      }

      {
        // select 10 of 10 uniformly (weights = 1)
        var items = Enumerable.Range(0, 10);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(1.0, 10);
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 10, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 10);
          Assert.AreEqual(sample.Distinct().Count(), 10);
        }
      }

      {
        // select 10 of 10 uniformly (weights = 1)
        var items = Enumerable.Range(0, 10);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(1.0, 10);
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 10, weights, true, false).ToArray();
          Assert.AreEqual(sample.Count(), 10);
          Assert.AreEqual(sample.Distinct().Count(), 10);
        }
      }

      {
        // select 10 of 10 uniformly (weights = 1)
        var items = Enumerable.Range(0, 10);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(1.0, 10);
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 10, weights, true, true).ToArray();
          Assert.AreEqual(sample.Count(), 10);
          Assert.AreEqual(sample.Distinct().Count(), 10);
        }
      }

      {
        // select 5 of 10 uniformly (weights = 0..n)
        var items = Enumerable.Range(0, 10);
        var random = new MersenneTwister(31415);
        var weights = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 5, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 5);
          Assert.AreEqual(sample.Distinct().Count(), 5);
        }
      }

      {
        // select 5 of 10 uniformly (weights = 0..n)
        var items = Enumerable.Range(0, 10);
        var random = new MersenneTwister(31415);
        var weights = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 5, weights, true, false).ToArray();
          Assert.AreEqual(sample.Count(), 5);
          Assert.AreEqual(sample.Distinct().Count(), 5);
        }
      }

      {
        // select 5 of 10 uniformly (weights = 0..n)
        var items = Enumerable.Range(0, 10);
        var random = new MersenneTwister(31415);
        var weights = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < 1000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 5, weights, true, true).ToArray();
          Assert.AreEqual(sample.Count(), 5);
          Assert.AreEqual(sample.Distinct().Count(), 5);
        }
      }

      {
        // select 10 of 100 uniformly (weights = 1)
        // repeat 1000000 times and calculate statistics
        var items = Enumerable.Range(0, 100);
        var random = new MersenneTwister(31415);
        var weights = Enumerable.Repeat(1.0, 100);
        var selectionCount = new int[100, 100]; // frequency of selecting item at pos
        for (int i = 0; i < 1000000; i++) {
          var sample = RandomEnumerable.SampleProportionalWithoutRepetition(items, random, 100, weights, false, false).ToArray();
          Assert.AreEqual(sample.Count(), 100);
          Assert.AreEqual(sample.Distinct().Count(), 100);

          int pos = 0;
          foreach (var item in sample) {
            selectionCount[item, pos]++;
            pos++;
          }
        }
        var sb = new StringBuilder();
        for (int item = 0; item < 100; item++) {
          for (int pos = 0; pos < 100; pos++) {
            sb.AppendFormat("{0} ", selectionCount[item, pos]);
          }
          sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
      }
    }
  }
}
