using System;
using System.Linq;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests.HeuristicLab.Encodings.LinearLinkageEncoding_3._3 {
  [TestClass]
  public class ConversionTests {
    [TestMethod]
    public void ConvertFromLLEtoLLEe() {
      var random = new MersenneTwister(42);
      var lle = MaxGroupsLinearLinkageCreator.Apply(random, 32, 8);
      Assert.IsTrue(lle.SequenceEqual(LinearLinkage.FromEndLinks(lle.ToEndLinks())));
    }

    [TestMethod]
    public void ConvertFromLLEtoLLEb() {
      var random = new MersenneTwister(42);
      var lle = MaxGroupsLinearLinkageCreator.Apply(random, 32, 8);
      Assert.IsTrue(lle.SequenceEqual(LinearLinkage.FromBackLinks(lle.ToBackLinks())));
    }
  }
}
