using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests_33.Tests {


  /// <summary>
  ///This is a test class for JSMSXXCrossoverTest and is intended
  ///to contain all JSMSXXCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class JSMSXXCrossoverTest {


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
      IRandom random = new TestRandom(new int[] { 3 }, null);
      JSMEncoding p1 = TestUtils.CreateTestJSM1();
      JSMEncoding p2 = TestUtils.CreateTestJSM2();
      JSMEncoding expected = new JSMEncoding();
      ItemList<Permutation> jsm = new ItemList<Permutation>();
      for (int i = 0; i < 6; i++) {
        jsm.Add(new Permutation(PermutationTypes.Absolute, new int[] { 2, 1, 0, 3, 4, 5 }));
      }
      expected.JobSequenceMatrix = jsm;

      JSMEncoding actual;
      actual = JSMSXXCrossover.Apply(random, p1, p2);

      Assert.IsTrue(expected.Equals(actual));
    }
  }
}
