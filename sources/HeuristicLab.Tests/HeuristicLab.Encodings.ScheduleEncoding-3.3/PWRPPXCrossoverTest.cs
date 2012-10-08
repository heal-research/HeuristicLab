using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests_33.Tests {


  /// <summary>
  ///This is a test class for PWRPPXCrossoverTest and is intended
  ///to contain all PWRPPXCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class PWRPPXCrossoverTest {


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
    public void ApplyTest() {
      IRandom random = new TestRandom(new int[] { 1, 1, 0, 0, 1, 1, 0, 0, 1 }, null);
      PWREncoding parent1 = TestUtils.CreateTestPWR1();
      PWREncoding parent2 = TestUtils.CreateTestPWR2();
      PWREncoding expected = new PWREncoding();
      expected.PermutationWithRepetition = new IntegerVector(new int[] { 1, 0, 1, 0, 1, 2, 0, 2, 2 });
      PWREncoding actual;
      actual = PWRPPXCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(actual.Equals(expected));
    }
  }
}
