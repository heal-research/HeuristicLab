using HeuristicLab.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Encodings.PermutationEncoding_33.Tests {
    /// <summary>
    ///This is a test class for CyclicCrossover2Test and is intended
    ///to contain all CyclicCrossover2Test Unit Tests
    ///</summary>
  [TestClass()]
  public class CyclicCrossover2Test {


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
    ///A test for Cross
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HeuristicLab.Encodings.Permutation-3.3.dll")]
    public void CyclicCrossover2CrossTest() {
      TestRandom random = new TestRandom();
      CyclicCrossover2_Accessor target = new CyclicCrossover2_Accessor(new PrivateObject(typeof(CyclicCrossover2)));
      // perform a test with more than two parents
      random.Reset();
      bool exceptionFired = false;
      try {
        target.Cross(random, new ItemArray<Permutation>(new Permutation[] { 
          new Permutation(4), new Permutation(4), new Permutation(4)}));
      } catch (System.InvalidOperationException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    public void CyclicCrossover2ApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent1, parent2, expected, actual;
      // The following test is based on an example from Affenzeller, M. et al. 2009. Genetic Algorithms and Genetic Programming - Modern Concepts and Practical Applications. CRC Press. pp. 134.
      random.Reset();
      random.IntNumbers = new int[] { 0 };
      parent1 = new Permutation(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
      Assert.IsTrue(parent1.Validate());
      parent2 = new Permutation(new int[] { 2, 5, 6, 0, 7, 1, 3, 8, 4, 9 });
      Assert.IsTrue(parent2.Validate());
      expected = new Permutation(new int[] { 0, 5, 2, 3, 7, 1, 6, 8, 4, 9 });
      Assert.IsTrue(expected.Validate());
      actual = CyclicCrossover2.Apply(random, parent1, parent2);
      Assert.IsTrue(actual.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, actual));

      // perform a test when the two permutations are of unequal length
      random.Reset();
      bool exceptionFired = false;
      try {
        CyclicCrossover.Apply(random, new Permutation(8), new Permutation(6));
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
