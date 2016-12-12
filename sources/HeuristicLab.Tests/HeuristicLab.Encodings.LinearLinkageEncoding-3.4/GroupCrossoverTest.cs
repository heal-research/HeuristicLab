using System;
using HeuristicLab.Random;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.LinearLinkageEncoding.Tests {
  [TestClass()]
  public class GroupCrossoverTest {
    [TestMethod()]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void EqualParentsTest() {
      var random = new FastRandom(0);
      var parent = LinearLinkage.SingleElementGroups(10);
      var child = GroupCrossover.Apply(random, parent, parent);
      Assert.IsTrue(Auxiliary.LinearLinkageIsEqualByPosition(parent, child));

      parent = RandomLinearLinkageCreator.Apply(random, 10);
      child = GroupCrossover.Apply(random, parent, parent);
      Assert.IsTrue(Auxiliary.LinearLinkageIsEqualByPosition(parent, child));
    }

    // Example from paper:
    // Multi-objective Genetic Algorithms for grouping problems. Korkmaz, E.E. Applied Intelligence (2010) 33: 179. 
    [TestMethod()]
    [TestCategory("Encodings.LinearLinkage")]
    [TestProperty("Time", "short")]
    public void ExampleFromPaperTest() {
      var parent1 = LinearLinkage.FromEndLinks(new[] { 3, 4, 3, 3, 4 });

      var parent2 = LinearLinkage.FromEndLinks(new[] { 2, 2, 2, 4, 4 });

      CheckGroupCrossover(parent1, parent2, new[] { 3, 4, 2, 3, 4 }, new[] { 0.0, 0.0, 0.0, 0.0 }); // (i)
      CheckGroupCrossover(parent1, parent2, new[] { 2, 4, 2, 3, 4 }, new[] { 0.0, 0.0, 0.0, 0.9 }); // (iv)
      CheckGroupCrossover(parent1, parent2, new[] { 3, 2, 2, 3, 4 }, new[] { 0.0, 0.0, 0.9, 0.0 }); // (iii)
      CheckGroupCrossover(parent1, parent2, new[] { 2, 2, 2, 3, 4 }, new[] { 0.0, 0.0, 0.9, 0.9 }); // (ii)
      CheckGroupCrossover(parent1, parent2, new[] { 3, 4, 3, 3, 4 }, new[] { 0.0, 0.9 });
      CheckGroupCrossover(parent1, parent2, new[] { 2, 4, 2, 4, 4 }, new[] { 0.9, 0.0, 0.0 });
      CheckGroupCrossover(parent1, parent2, new[] { 2, 2, 2, 4, 4 }, new[] { 0.9, 0.0, 0.9 });
      CheckGroupCrossover(parent1, parent2, new[] { 4, 4, 4, 4, 4 }, new[] { 0.9, 0.9, 0.0, 0.0 });
      CheckGroupCrossover(parent1, parent2, new[] { 4, 4, 4, 4, 4 }, new[] { 0.9, 0.9, 0.0, 0.9 });
      CheckGroupCrossover(parent1, parent2, new[] { 2, 4, 2, 4, 4 }, new[] { 0.9, 0.9, 0.9, 0.0 });
      CheckGroupCrossover(parent1, parent2, new[] { 4, 4, 2, 4, 4 }, new[] { 0.9, 0.9, 0.9, 0.9 });
    }

    private void CheckGroupCrossover(LinearLinkage parent1, LinearLinkage parent2, int[] expectedllee, double[] randomNumbers) {
      var expected = LinearLinkage.FromEndLinks(expectedllee);
      var random = new TestRandom() { DoubleNumbers = randomNumbers };
      var child = GroupCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(Auxiliary.LinearLinkageIsEqualByPosition(expected, child), "Expected [{0}] but was [{1}]!", string.Join(", ", expected), string.Join(", ", child));
    }
  }
}
