using HeuristicLab.Encodings.IntVector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.IntVector_33.Tests {


  /// <summary>
  ///This is a test class for DiscreteCrossoverTest and is intended
  ///to contain all DiscreteCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class DiscreteCrossoverTest {


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
    [DeploymentItem("HeuristicLab.Encodings.IntVector-3.3.dll")]
    public void DiscreteCrossoverCrossTest() {
      DiscreteCrossover_Accessor target = new DiscreteCrossover_Accessor(new PrivateObject(typeof(DiscreteCrossover)));
      ItemArray<IntArrayData> parents;
      TestRandom random = new TestRandom();
      bool exceptionFired;
      // The following test checks if there is an exception when there are less than 2 parents
      random.Reset();
      parents = new ItemArray<IntArrayData>(new IntArrayData[] { new IntArrayData(4) });
      exceptionFired = false;
      try {
        IntArrayData actual;
        actual = target.Cross(random, parents);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    public void DiscreteCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      IntArrayData parent1, parent2, expected, actual;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0, 0, 0.9, 0, 0.9 };
      parent1 = new IntArrayData(new int[] { 2, 2, 3, 5, 1 });
      parent2 = new IntArrayData(new int[] { 4, 1, 3, 2, 8 });
      expected = new IntArrayData(new int[] { 2, 2, 3, 5, 8 });
      actual = DiscreteCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(Auxiliary.IntVectorIsEqualByPosition(actual, expected));

      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0, 0, 0.9, 0, 0.9 };
      parent1 = new IntArrayData(new int[] { 2, 2, 3, 5, 1, 9 }); // this parent is longer
      parent2 = new IntArrayData(new int[] { 4, 1, 3, 2, 8 });
      exceptionFired = false;
      try {
        actual = DiscreteCrossover.Apply(random, parent1, parent2);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for DiscreteCrossover Constructor
    ///</summary>
    [TestMethod()]
    public void DiscreteCrossoverConstructorTest() {
      DiscreteCrossover target = new DiscreteCrossover();
    }
  }
}
