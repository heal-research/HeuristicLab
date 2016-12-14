using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests.HeuristicLab.Encodings.LinearLinkageEncoding {
  [TestClass]
  public class LinearLinkageMoveTests {
    [TestMethod]
    public void TestApplyUndo() {
      var random = new MersenneTwister(42);
      for (var p = 0; p < 7; p++) {
        var lle = ExactGroupsLinearLinkageCreator.Apply(random, 64, (int)Math.Pow(2, p));
        var before = (LinearLinkage)lle.Clone();
        var beforeb = before.ToBackLinks();
        var moves = 0;
        foreach (var move in MoveGenerator.Generate(lle)) {
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
    public void TestApply() {
      var random = new MersenneTwister(42);
      for (var p = 0; p < 7; p++) {
        var lle = ExactGroupsLinearLinkageCreator.Apply(random, 64, (int)Math.Pow(2, p));
        var lleb = lle.ToBackLinks();
        var groupItems = new List<int>() { 0 };
        var moves = 0;
        for (var i = 1; i < lle.Length; i++) {
          var move = MoveGenerator.GenerateForItem(i, groupItems, lle, lleb).SampleRandom(random);
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
