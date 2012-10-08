using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition;
using HeuristicLab.Encodings.ScheduleEncoding.ScheduleEncoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.ScheduleEncoding.Tests_33.Tests {


  /// <summary>
  ///This is a test class for DirectScheduleGTCrossoverTest and is intended
  ///to contain all DirectScheduleGTCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class DirectScheduleGTCrossoverTest {


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
      IRandom random = new TestRandom(new int[] { 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1 }, null);
      Schedule parent1 = TestUtils.CreateTestSchedule1();
      Schedule parent2 = TestUtils.CreateTestSchedule2();
      ItemList<Job> jobData = TestUtils.CreateJobData();
      double mutProp = 0.05;
      Schedule actual;
      actual = DirectScheduleGTCrossover.Apply(random, parent1, parent2, jobData, mutProp);
      Schedule expected = DirectScheduleRandomCreator.Apply(3, 3, new PWREncoding(3, 3, new TestRandom(new int[] { 0, 2, 1, 1, 0, 2, 1, 2, 0 }, null)), TestUtils.CreateJobData());
      Assert.IsTrue(actual.Equals(expected));
    }
  }
}
