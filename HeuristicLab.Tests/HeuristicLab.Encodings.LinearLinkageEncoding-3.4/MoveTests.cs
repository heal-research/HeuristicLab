#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.LinearLinkageEncoding.Tests {
  [TestClass]
  public class LinearLinkageMoveTests {
    [TestMethod]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void TestLinearLinkageMoveApplyUndo() {
      var random = new MersenneTwister(42);
      for (var p = 0; p < 7; p++) {
        var lle = ExactGroupsLinearLinkageCreator.Apply(random, 64, (int)Math.Pow(2, p));
        var before = (LinearLinkage)lle.Clone();
        var beforeb = before.ToBackLinks();
        var moves = 0;
        foreach (var move in ExhaustiveEMSSMoveGenerator.Generate(lle)) {
          var lleb = lle.ToBackLinks();
          move.Apply(lle);
          move.ApplyToLLEb(lleb);
          Assert.IsFalse(before.SequenceEqual(lle));
          Assert.IsFalse(lleb.SequenceEqual(beforeb));
          Assert.IsTrue(lleb.SequenceEqual(lle.ToBackLinks()));
          Assert.IsTrue(lle.SequenceEqual(LinearLinkage.FromBackLinks(lleb)));
          move.Undo(lle);
          Assert.IsTrue(before.SequenceEqual(lle));
          moves++;
        }
        Assert.IsTrue(moves > 0);
      }
    }

    [TestMethod]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void TestLinearLinkageMoveApply() {
      var random = new MersenneTwister(42);
      for (var p = 0; p < 7; p++) {
        var lle = ExactGroupsLinearLinkageCreator.Apply(random, 64, (int)Math.Pow(2, p));
        var lleb = lle.ToBackLinks();
        var groupItems = new List<int>() { 0 };
        var moves = 0;
        for (var i = 1; i < lle.Length; i++) {
          var move = ExhaustiveEMSSMoveGenerator.GenerateForItem(i, groupItems, lle, lleb).SampleRandom(random);
          move.Apply(lle);
          move.ApplyToLLEb(lleb);
          Assert.IsTrue(lleb.SequenceEqual(lle.ToBackLinks()));
          Assert.IsTrue(lle.SequenceEqual(LinearLinkage.FromBackLinks(lleb)));

          if (lleb[i] != i)
            groupItems.Remove(lleb[i]);
          groupItems.Add(i);

          moves++;
        }
        Assert.IsTrue(moves == lle.Length - 1);
      }
    }
  }
}
