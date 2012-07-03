using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.PermutationEncoding_33.Tests {


  /// <summary>
  ///This is a test class for UniformLikeCrossoverTest and is intended
  ///to contain all UniformLikeCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class UniformLikeCrossoverTest {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    public void UniformLikeCrossoverApplyTest() {
      // test from the paper
      IRandom random = new TestRandom(new int[] { 0 }, new double[] { 0.2, 0.7, 0.2, 0.2 }); // TODO: Initialize to an appropriate value
      Permutation parent1 = new Permutation(PermutationTypes.Absolute,
        new int[] { 3, 2, 0, 7, 5, 4, 1, 6 });
      Assert.IsTrue(parent1.Validate());
      Permutation parent2 = new Permutation(PermutationTypes.Absolute,
        new int[] { 5, 0, 4, 7, 1, 3, 2, 6 });
      Assert.IsTrue(parent2.Validate());
      Permutation expected = new Permutation(PermutationTypes.Absolute,
        new int[] { 3, 0, 4, 7, 5, 2, 1, 6 });
      Assert.IsTrue(expected.Validate());
      Permutation actual;
      actual = UniformLikeCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(actual.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, actual));
    }

    /// <summary>
    ///A test for Cross
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HeuristicLab.Encodings.PermutationEncoding-3.3.dll")]
    public void UniformLikeCrossoverCrossTest() {
      UniformLikeCrossover_Accessor target = new UniformLikeCrossover_Accessor();
      IRandom random = new TestRandom(new int[] { }, new double[] { 0.1, 0.2, 0.3, 0.4 });
      random.Reset();
      bool exceptionFired = false;
      try {
        target.Cross(random, new ItemArray<Permutation>(new Permutation[] { 
          new Permutation(PermutationTypes.RelativeUndirected, 4), new Permutation(PermutationTypes.RelativeUndirected, 4), new Permutation(PermutationTypes.RelativeUndirected, 4)}));
      } catch (System.InvalidOperationException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
