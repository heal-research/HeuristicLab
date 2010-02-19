using System;
using System.Diagnostics;
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.SGA.UnitTest {
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class UnitTest {
    public UnitTest() {
      //
      // TODO: Add constructor logic here
      //
    }

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
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    private EventWaitHandle trigger = new AutoResetEvent(false);

    [TestMethod]
    [DeploymentItem(@"SGAEngine.hl")]
    public void SGAPerformanceTest() {
      IEngine engine = (IEngine)XmlParser.Deserialize("SGAEngine.hl");
      engine.Stopped += new EventHandler(engine_Stopped);
      engine.Prepare();
      engine.Start();
      trigger.WaitOne();
      TestContext.WriteLine("Runtime: {0}", engine.ExecutionTime.ToString());
    }

    private void engine_Stopped(object sender, EventArgs e) {
      trigger.Set();
    }
  }
}
