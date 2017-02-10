#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.LinearLinkageEncoding.Tests {
  [TestClass()]
  public class ConversionsTest {
    private readonly int[][] validForwardEncoding = new[]{
      new int[0], new[] {0},
      new[] {0, 1}, new[] {1, 1},
      new[] {0, 1, 2}, new[] {1, 1, 2}, new[] {2, 1, 2},
      new[] {0, 2, 2}, new[] {1, 2, 2},
      new[] {0, 1, 2, 3}, new[] {1, 1, 2, 3}, new[] {2, 1, 2, 3}, new[] {3, 1, 2, 3},
      new[] {0, 1, 3, 3}, new[] {1, 2, 2, 3}, new[] {2, 1, 3, 3}, new[] {3, 2, 2, 3},
      new[] {0, 2, 2, 3}, new[] {1, 3, 2, 3},
      new[] {0, 2, 3, 3}
    };
    private readonly int[][] invalidForwardEncoding = new[]{
      new [] {-1}, new [] {1}, new [] {1, 0},
      new [] {0, 0, 0}, new [] {0, 0, 1}, new [] {0, 0, 2},
      new [] {0, 1, 0},
      new [] {0, 2, 0}, new [] {0, 2, 1},
      new [] {1, 0, 0}, new [] {1, 0, 1}, new [] {1, 0, 2},
      new [] {1, 1, 0}, new [] {1, 1, 1},
      new [] {1, 2, 0}, new [] {1, 2, 1},
      new [] {2, 0, 0}, new [] {2, 0, 1}, new [] {2, 0, 2},
      new [] {2, 1, 0}, new [] {2, 1, 1},
      new [] {2, 2, 0}, new [] {2, 2, 1}, new [] {2, 2, 2},
    };
    private readonly int[][] invalidEndEncoding = new[]{
      new [] {-1}, new [] {1}, new [] {1, 0},
      new [] {0, 0, 1}, new [] {0, 0, 2},
      new [] {0, 1, 0},
      new [] {0, 2, 0}, new [] {0, 2, 1},
      new [] {1, 0, 0}, new [] {1, 0, 1}, new [] {1, 0, 2},
      new [] {1, 1, 0}, new [] {1, 1, 1},
      new [] {1, 2, 0}, new [] {1, 2, 1},
      new [] {2, 0, 0}, new [] {2, 0, 1}, new [] {2, 0, 2},
      new [] {2, 1, 0}, new [] {2, 1, 1},
      new [] {2, 2, 0}, new [] {2, 2, 1},
    };

    [TestMethod()]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void EndLinksRoundtripTest() {
      foreach (var values in validForwardEncoding) {
        var expected = LinearLinkage.FromForwardLinks(values);
        var llee = expected.ToEndLinks();
        var actual = LinearLinkage.FromEndLinks(llee);
        Assert.IsTrue(Auxiliary.LinearLinkageIsEqualByPosition(expected, actual),
          "[{0}] did not roundtrip successfully!", string.Join(", ", values));
      }
    }

    [TestMethod()]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void GroupsRoundtripTest() {
      foreach (var values in validForwardEncoding) {
        var expected = LinearLinkage.FromForwardLinks(values);
        var groups = expected.GetGroups();
        var actual = LinearLinkage.FromGroups(expected.Length, groups);
        Assert.IsTrue(Auxiliary.LinearLinkageIsEqualByPosition(expected, actual),
          "[{0}] did not roundtrip successfully!", string.Join(", ", values));
      }
    }

    [TestMethod()]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void InvalidForwardLinksTest() {
      foreach (var values in invalidForwardEncoding) {
        var isValid = true;
        try {
          var lle = LinearLinkage.FromForwardLinks(values);
        } catch (ArgumentException e) {
          isValid = false;
        }
        Assert.IsFalse(isValid, "[{0}] is invalid and should throw ArgumentException!", string.Join(", ", values));
      }
    }

    [TestMethod()]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void InvalidEndLinksTest() {
      foreach (var values in invalidEndEncoding) {
        var isValid = true;
        try {
          var lle = LinearLinkage.FromEndLinks(values);
        } catch (ArgumentException e) {
          isValid = false;
        }
        Assert.IsFalse(isValid, "{0} should be invalid", string.Join(", ", values));
      }
    }

    [TestMethod]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void ConvertFromLLEtoLLEe() {
      var random = new MersenneTwister(42);
      var lle = MaxGroupsLinearLinkageCreator.Apply(random, 32, 8);
      Assert.IsTrue(lle.SequenceEqual(LinearLinkage.FromEndLinks(lle.ToEndLinks())));
    }

    [TestMethod]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void ConvertFromLLEtoLLEb() {
      var random = new MersenneTwister(42);
      var lle = MaxGroupsLinearLinkageCreator.Apply(random, 32, 8);
      Assert.IsTrue(lle.SequenceEqual(LinearLinkage.FromBackLinks(lle.ToBackLinks())));
    }
  }
}
