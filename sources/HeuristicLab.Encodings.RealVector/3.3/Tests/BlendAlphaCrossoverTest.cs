using HeuristicLab.Encodings.RealVector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.RealVector_33.Tests {


  /// <summary>
  ///This is a test class for BlendAlphaCrossoverTest and is intended
  ///to contain all BlendAlphaCrossoverTest Unit Tests
  ///</summary>
  [TestClass()]
  public class BlendAlphaCrossoverTest {


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
    [DeploymentItem("HeuristicLab.Encodings.RealVector-3.3.dll")]
    public void BlendAlphaCrossoverCrossTest() {
      BlendAlphaCrossover_Accessor target = new BlendAlphaCrossover_Accessor(new PrivateObject(typeof(BlendAlphaCrossover)));
      ItemArray<DoubleArrayData> parents;
      TestRandom random = new TestRandom();
      bool exceptionFired;
      // The following test checks if there is an exception when there are more than 2 parents
      random.Reset();
      parents = new ItemArray<DoubleArrayData>(new DoubleArrayData[] { new DoubleArrayData(5), new DoubleArrayData(6), new DoubleArrayData(4) });
      exceptionFired = false;
      try {
        DoubleArrayData actual;
        actual = target.Cross(random, parents);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
      // The following test checks if there is an exception when there are less than 2 parents
      random.Reset();
      parents = new ItemArray<DoubleArrayData>(new DoubleArrayData[] { new DoubleArrayData(4) });
      exceptionFired = false;
      try {
        DoubleArrayData actual;
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
    public void BlendAlphaCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      DoubleArrayData parent1, parent2, expected, actual;
      DoubleData alpha;
      bool exceptionFired;
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.5, 0.5, 0.5, 0.5, 0.5 };
      alpha = new DoubleData(0.5);
      parent1 = new DoubleArrayData(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      parent2 = new DoubleArrayData(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      expected = new DoubleArrayData(new double[] { 0.3, 0.15, 0.3, 0.35, 0.45 });
      actual = BlendAlphaCrossover.Apply(random, parent1, parent2, alpha);
      Assert.IsTrue(Auxiliary.RealVectorIsAlmostEqualByPosition(actual, expected));
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.25, 0.75, 0.25, 0.75, 0.25 };
      alpha = new DoubleData(0.25);
      parent1 = new DoubleArrayData(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      parent2 = new DoubleArrayData(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      expected = new DoubleArrayData(new double[] { 0.225, 0.1875, 0.3, 0.4625, 0.1875 });
      actual = BlendAlphaCrossover.Apply(random, parent1, parent2, alpha);
      Assert.IsTrue(Auxiliary.RealVectorIsAlmostEqualByPosition(actual, expected));
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.25, 0.75, 0.25, 0.75, 0.25 };
      alpha = new DoubleData(-0.25); // negative values for alpha are not allowed
      parent1 = new DoubleArrayData(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1 });
      parent2 = new DoubleArrayData(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      expected = new DoubleArrayData(new double[] { 0.225, 0.1875, 0.3, 0.4625, 0.1875 });
      exceptionFired = false;
      try {
        actual = BlendAlphaCrossover.Apply(random, parent1, parent2, alpha);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
      // The following test is not based on published examples
      random.Reset();
      random.DoubleNumbers = new double[] { 0.25, 0.75, 0.25, 0.75, 0.25, .75 };
      alpha = new DoubleData(0.25);
      parent1 = new DoubleArrayData(new double[] { 0.2, 0.2, 0.3, 0.5, 0.1, 0.9 }); // this parent is longer
      parent2 = new DoubleArrayData(new double[] { 0.4, 0.1, 0.3, 0.2, 0.8 });
      expected = new DoubleArrayData(new double[] { 0.225, 0.1875, 0.3, 0.4625, 0.1875 });
      exceptionFired = false;
      try {
        actual = BlendAlphaCrossover.Apply(random, parent1, parent2, alpha);
      } catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }

    /// <summary>
    ///A test for BlendAlphaCrossover Constructor
    ///</summary>
    [TestMethod()]
    public void BlendAlphaCrossoverConstructorTest() {
      BlendAlphaCrossover target = new BlendAlphaCrossover();
    }
  }
}
