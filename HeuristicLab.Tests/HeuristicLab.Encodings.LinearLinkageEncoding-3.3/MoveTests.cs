using System;
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
      var lle = MaxGroupsLinearLinkageCreator.Apply(random, 32, 8);
      var before = (LinearLinkage)lle.Clone();
      var moves = 0;
      foreach (var move in MoveGenerator.Generate(lle)) {
        move.Apply(lle);
        Assert.IsFalse(before.SequenceEqual(lle));
        move.Undo(lle);
        Assert.IsTrue(before.SequenceEqual(lle));
        moves++;
      }
      Assert.IsTrue(moves > 0);
    }
  }
}
